/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
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
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
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
    /// Implementation of the Enum display component.
    /// </summary>
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
                YTagBuilder tag = new YTagBuilder("span");
                tag.AddCssClass("yt_enum");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);
                tag.Attributes.Add(Basics.CssTooltipSpan, desc);
                tag.SetInnerText(caption);
                return Task.FromResult(tag.ToString(YTagRenderMode.Normal));
            } else {
                return Task.FromResult(caption);
            }
        }
    }

    /// <summary>
    /// Implementation of the Enum edit component.
    /// </summary>
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
