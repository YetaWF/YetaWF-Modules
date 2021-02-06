/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Enum component implementation.
    /// </summary>
    public abstract class EnumComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(EnumComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "Enum";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Displays the model formatted using the descriptive text found in the EnumDescription attribute (if present). The model is an enumeration (enum).
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <example>
    /// public enum LevelEnum {
    ///     [EnumDescription("Info", "Informational")]
    ///     Info = 0,
    ///     [EnumDescription("Error", "Error")]
    ///     Error = 99,
    /// }
    ///
    /// [Caption("Level"), Description("The error level of this log record")]
    /// [UIHint("Enum"), AdditionalMetadata("ShowEnumValue", false), ReadOnly]
    /// public LevelEnum Level { get; set; }
    /// </example>
    [UsesAdditionalAttribute("ShowEnumValue", "bool", "true", "If true, the enum value is displayed along with the descriptive text for the enum. Otherwise, the value of the enum is not displayed. In either case, enum values are only shown if the user's User Settings (see User > Settings, standard YetaWF site) has the ShowEnumValue property set to true. AdditionalMetadata(\"ShowEnumValue\", false) is normally used in grids to explicitly suppress the enum value so only the descriptive text is shown.")]
    public class EnumDisplayComponent : EnumComponentBase, IYetaWFComponent<object> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(object model) {

            bool showValues = UserSettings.GetProperty<bool>("ShowEnumValue");
            showValues = showValues && PropData.GetAdditionalAttributeValue("ShowEnumValue", true);

            string desc;
            string caption = ObjectSupport.GetEnumDisplayInfo(model, out desc, ShowValue: showValues);

            if (HtmlAttributes.Count > 0 || !string.IsNullOrWhiteSpace(desc)) {
                return Task.FromResult($"<span{FieldSetup(FieldType.Anonymous)} class='yt_enum t_display{GetClasses()}' {Basics.CssTooltipSpan}='{HAE(desc)}'>{HE(caption)}</span>");
            } else {
                return Task.FromResult(caption);
            }
        }
    }

    /// <summary>
    /// Allows selection of an enum value using a dropdown list which is created based on the enum type and the EnumDescription attributes (if present).
    /// </summary>
    /// <remarks>
    /// Enum values are only shown if the user's User Settings (see User > Settings, standard YetaWF site) has the ShowEnumValue property set to true.
    /// </remarks>
    [UsesAdditionalAttribute("ShowSelect", "bool", "false", "If true, an entry showing \"(select)\" with a value of 0 is inserted as the first entry, in addition to all enum values. Otherwise, only the enum values are shown.")]
    public class EnumEditComponent : EnumComponentBase, IYetaWFComponent<object> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(object model) {
            bool showSelect = PropData.GetAdditionalAttributeValue("ShowSelect", false);
            List<SelectionItem<int>> list = GetEnumSelectionList(model.GetType(), showSelect: showSelect);
            return await DropDownListIntComponent.RenderDropDownListAsync(this, (int)model, list, "yt_enum");
        }

        /// <summary>
        /// Given an enum type, returns a collection suitable for use in a DropDownList component.
        /// </summary>
        /// <param name="enumType">The type of the enum.</param>
        /// <param name="showSelect">Defines whether the first entry "(select)" should be generated.</param>
        /// <returns>Returns a collection suitable for use in a DropDownList component.</returns>
        public static List<SelectionItem<int>> GetEnumSelectionList(Type enumType, bool showSelect = false) {
            List<SelectionItem<int>> list = new List<SelectionItem<int>>();

            EnumData enumData = ObjectSupport.GetEnumData(enumType);
            bool showValues = UserSettings.GetProperty<bool>("ShowEnumValue");

            if (showSelect) {
                list.Add(new SelectionItem<int> {
                    Text = __ResStr("enumSelect", "(select)"),
                    Value = 0,
                    Tooltip = __ResStr("enumPlsSelect", "Please select one of the available options"),
                });
            }
            foreach (EnumDataEntry entry in enumData.Entries) {

                int enumVal = Convert.ToInt32(entry.Value);
                if (enumVal == 0 && showSelect) continue;

                string caption = entry.Caption;
                if (showValues)
                    caption = __ResStr("enumFmt", "{0} - {1}", enumVal, caption);

                list.Add(new SelectionItem<int> {
                    Text = caption,
                    Value = enumVal,
                    Tooltip = entry.Description,
                });
            }
            return list;
        }
    }
}
