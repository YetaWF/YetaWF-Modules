/* Copyright � 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections;
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
    /// Base class for the EnumSupported component implementation.
    /// </summary>
    public abstract class EnumSupportedComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(EnumSupportedComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "EnumSupported";

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
    public class EnumSupportedDisplayComponent : EnumSupportedComponentBase, IYetaWFComponent<object> {

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

            string? desc;
            string? caption = ObjectSupport.GetEnumDisplayInfo(model, out desc, ShowValue: showValues);

            if (HtmlAttributes.Count > 0 || !string.IsNullOrWhiteSpace(desc)) {
                return Task.FromResult($"<span{FieldSetup(FieldType.Anonymous)} class='yt_enum t_display{GetClasses()}' {Basics.CssTooltipSpan}='{HAE(desc)}'>{HE(caption)}</span>");
            } else {
                return Task.FromResult(caption);
            }
        }
    }

    /// <summary>
    /// Allows selection of an enum value using a dropdown list which is created based on the enum type and the EnumDescription attributes (if present).
    /// The presented list of enum values only includes values that are listed in the Supported sibling property.
    /// </summary>
    /// <remarks>
    /// Enum values are only shown if the user's User Settings (see User > Settings, standard YetaWF site) has the ShowEnumValue property set to true.
    /// </remarks>
    [UsesAdditional("NoDefault", "bool", "false", "Defines whether a \"(select)\" entry is automatically added as the first entry, with a value of null")]
    [UsesSibling("_Supported", "List<object>", "Lists supported enum values. Enum values that are not in this list are suppressed.")]
    public class EnumSupportedEditComponent : EnumSupportedComponentBase, IYetaWFComponent<object> {

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

            Type enumType = model.GetType();

            // get all enum values in supported list
            List<int> supported = new List<int>();
            object? obj = GetSiblingProperty<object>($"{PropertyName}_Supported");
            if (obj == null) throw new InternalError($"{PropertyName}_Supported value is null");
            IEnumerable ienumerable = (obj as IEnumerable)!;
            IEnumerator ienum = ienumerable.GetEnumerator();
            while (ienum.MoveNext()) {
                supported.Add(Convert.ToInt32(ienum.Current));
            }

            List <SelectionItem<int>> list = new List<SelectionItem<int>>();

            EnumData enumData = ObjectSupport.GetEnumData(enumType);
            bool showValues = UserSettings.GetProperty<bool>("ShowEnumValue");

            bool showSelect = PropData.GetAdditionalAttributeValue("ShowSelect", false);
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

                if (supported.Contains(enumVal)) {

                    string caption = entry.Caption;
                    if (showValues)
                        caption = __ResStr("enumFmt", "{0} - {1}", enumVal, caption);

                    list.Add(new SelectionItem<int> {
                        Text = caption,
                        Value = enumVal,
                        Tooltip = entry.Description,
                    });
                }
            }
            return await DropDownListIntComponent.RenderDropDownListAsync(this, (int)model, list, "yt_enumsupported");
        }
    }
}

