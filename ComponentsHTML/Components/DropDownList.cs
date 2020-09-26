/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Allows selection of a value from a list of string values using a dropdown list. The dropdown list supports optional tooltips.
    /// </summary>
    /// <example>
    /// [Caption("Country"), Description("Defines the country where your new fax number is located")]
    /// [UIHint("DropDownList"), Required]
    /// public string Country { get; set; }
    /// public List&lt;SelectionItem&lt;string&gt;&gt; Country_List { get; set; }
    /// </example>
    [UsesSibling("_List", "List<YetaWF.Core.Components.SelectionItem<string>>", "Defines the list of values, displayed text and tooltips shown in the dropdown list.")]
    [UsesAdditional("Disable1OrLess", "bool", "true", "Defines whether the dropdown list is disabled when only 1 item or less is available in the list of values.")]
    [UsesAdditional("AdjustWidth", "bool", "true", "Defines whether the dropdown list width is adjusted to optimally fit the content (horizontally).")]
    [UsesAdditional("DropDownWidthFactor", "float", "1.0", "Defines the width of the dropdown portion relative to the control width. The control width is multiplied by the factor DropDownWidthFactor to calculate the width. If 1.0 is specified, the dropdown portion is the same size as the control.  If 2.0 is specified, the dropdown portion is the twice the size of the control.")]
    [UsesAdditional("DropDownHeightFactor", "float", "1.0", "Defines the height of the dropdown portion relative to 200 pixels. The control width is multiplied by the factor DropDownHeightFactor to calculate the height. If 1.0 is specified, the dropdown portion is 200 pixels.  If 2.0 is specified, the dropdown portion is the 400 pixels in height.")]
    public class DropDownListComponent : DropDownListEditComponentBase<string> {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DropDownListComponent() : base("DropDownList") { }
    }
    /// <summary>
    /// Allows selection of a value from a list of int values using a dropdown list. The dropdown list supports optional tooltips.
    /// </summary>
    /// <example>
    /// [Caption("Entries"), Description("Select the number of entries per page")]
    /// [UIHint("DropDownListInt")]
    /// public int PageSelection { get; set; }
    /// public List&lt;SelectionItem&lt;int&gt;&gt; PageSelection_List { get; set; }
    /// </example>
    [UsesSibling("_List", "List<YetaWF.Core.Components.SelectionItem<int>>", "Defines the list of values, displayed text and tooltips shown in the dropdown list.")]
    [UsesAdditional("Disable1OrLess", "bool", "true", "Defines whether the dropdown list is disabled when only 1 item or less is available in the list of values.")]
    [UsesAdditional("AdjustWidth", "bool", "true", "Defines whether the dropdown list width is adjusted to optimally fit the content (horizontally).")]
    [UsesAdditional("DropDownWidthFactor", "float", "1.0", "Defines the width of the dropdown portion relative to the control width. The control width is multiplied by the factor DropDownWidthFactor to calculate the width. If 1.0 is specified, the dropdown portion is the same size as the control.  If 2.0 is specified, the dropdown portion is the twice the size of the control.")]
    [UsesAdditional("DropDownHeightFactor", "float", "1.0", "Defines the height of the dropdown portion relative to 200 pixels. The control width is multiplied by the factor DropDownHeightFactor to calculate the height. If 1.0 is specified, the dropdown portion is 200 pixels.  If 2.0 is specified, the dropdown portion is the 400 pixels in height.")]
    public class DropDownListIntComponent : DropDownListEditComponentBase<int> {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DropDownListIntComponent() : base("DropDownListInt") { }
    }
    /// <summary>
    /// Allows selection of a value from a list of nullable int values using a dropdown list. The dropdown list supports optional tooltips.
    /// </summary>
    /// <example>
    /// [Caption("Entries"), Description("Select the number of entries per page")]
    /// [UIHint("DropDownListIntNull")]
    /// public int? PageSelection { get; set; }
    /// public List&lt;SelectionItem&lt;int?&gt;&gt; PageSelection_List { get; set; }
    /// </example>
    [UsesSibling("_List", "List<YetaWF.Core.Components.SelectionItem<int?>>", "Defines the list of values, displayed text and tooltips shown in the dropdown list.")]
    [UsesAdditional("Disable1OrLess", "bool", "true", "Defines whether the dropdown list is disabled when only 1 item or less is available in the list of values.")]
    [UsesAdditional("AdjustWidth", "bool", "true", "Defines whether the dropdown list width is adjusted to optimally fit the content (horizontally).")]
    [UsesAdditional("DropDownWidthFactor", "float", "1.0", "Defines the width of the dropdown portion relative to the control width. The control width is multiplied by the factor DropDownWidthFactor to calculate the width. If 1.0 is specified, the dropdown portion is the same size as the control.  If 2.0 is specified, the dropdown portion is the twice the size of the control.")]
    [UsesAdditional("DropDownHeightFactor", "float", "1.0", "Defines the height of the dropdown portion relative to 200 pixels. The control width is multiplied by the factor DropDownHeightFactor to calculate the height. If 1.0 is specified, the dropdown portion is 200 pixels.  If 2.0 is specified, the dropdown portion is the 400 pixels in height.")]
    public class DropDownListIntNullComponent : DropDownListEditComponentBase<int?> {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DropDownListIntNullComponent() : base("DropDownListIntNull") { }
    }

    /// <summary>
    /// Base class for the DropDownList component implementation.
    /// </summary>
    public abstract class DropDownListEditComponentBase<TYPE> : YetaWFComponent, IYetaWFComponent<TYPE> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

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

        internal string TemplateName { get; set; }

        public class Setup {
            public bool AdjustWidth { get; set; }
            public float DropDownWidthFactor { get; set; }
            public float DropDownHeightFactor { get; set; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        public DropDownListEditComponentBase(string templateName) {
            TemplateName = templateName;
        }
        /// <summary>
        /// Adds all addons for the DropDownList component to the current page.
        /// </summary>
        public static async Task IncludeExplicitAsync() { // this component is reusable so we need to explicitly include all js/css
            await KendoUICore.UseAsync();// needed for css
            await Manager.AddOnManager.AddTemplateAsync(Controllers.AreaRegistration.CurrentPackage.AreaName, "DropDownList");
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(TYPE model) {

            List<SelectionItem<TYPE>> list;
            if (!TryGetSiblingProperty($"{PropertyName}_List", out list))
                list = new List<SelectionItem<TYPE>>();
            return RenderDropDownListAsync(this, model, list, "yt_dropdownlist");
        }

        /// <summary>
        /// Renders a DropDownList component as HTML.
        /// </summary>
        /// <param name="component">The component to render.</param>
        /// <param name="model">The model rendered by the component.</param>
        /// <param name="list">A collection of items to render.</param>
        /// <param name="cssClass">A CSS class to add to the &lt;select&gt; tag. May be null.</param>
        /// <returns></returns>
        public static async Task<string> RenderDropDownListAsync(YetaWFComponent component, TYPE model, List<SelectionItem<TYPE>> list, string cssClass) {

            // Uses kendo styles (k-widget, k-dropdown , k-dropdown-wrap, k-input, k-select)

            await IncludeExplicitAsync();

            component.UseSuppliedIdAsControlId();

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("select");

            bool disabled = false;
            if (list.Count <= 1) {
                if (component.PropData.GetAdditionalAttributeValue("Disable1OrLess", true))
                    disabled = true;
            }
            if (disabled) {
                //$$$$$$ THIS DOESN'T WORK WITH TEMPLATES
                component.FieldSetup(tag, FieldType.Normal);
                tag.Attributes.Remove("disabled");
                tag.Attributes.Add("disabled", "disabled");
                if (list.Count > 0)
                    tag.AddCssClass("disabled-submit");// submit disabled field

            } else {
                component.FieldSetup(tag, component.Validation ? FieldType.Validated : FieldType.Normal);
            }

            Setup setup = new Setup {
                AdjustWidth = component.PropData.GetAdditionalAttributeValue("AdjustWidth", true),
                DropDownWidthFactor = (float)component.PropData.GetAdditionalAttributeValue("DropDownWidthFactor", 1.0),
                DropDownHeightFactor = (float)component.PropData.GetAdditionalAttributeValue("DropDownHeightFactor", 1.0),
            };

            HtmlBuilder tagHtml = new HtmlBuilder();

            string text = list.Count > 0 ? list[0].Text : null;
            foreach (var item in list) {
                YTagBuilder tagOpt = new YTagBuilder("option");
                tagOpt.SetInnerText(item.Text.ToString());
                tagOpt.Attributes["value"] = item.Value?.ToString();
                if (Equals(item.Value, model)) {
                    tagOpt.Attributes["selected"] = "selected";
                    text = item.Text;
                }
                string desc = item.Tooltip?.ToString();
                if (!string.IsNullOrWhiteSpace(desc))
                    tagOpt.Attributes[Basics.CssTooltip] = desc;
                tagHtml.Append(tagOpt.ToString(YTagRenderMode.Normal));
            }

            tag.InnerHtml = tagHtml.ToString();

            hb.Append($@"
<div id='{component.ControlId}' class='k-widget k-dropdown yt_dropdownlist_base t_edit {cssClass}' {(disabled ? "aria-disabled='true'" : "tabindex='0' aria-disabled='false'")} unselectable='on' role='listbox' aria-haspopup='true' aria-expanded='false' aria-owns='yDDPopup' aria-live='polite' aria-busy='false'
        aria-activedescendant='{Guid.NewGuid().ToString()}'>
    <div unselectable='on' class='t_container k-dropdown-wrap k-state-default {(disabled ? "k-state-disabled" : "")}' {(disabled ? "disabled='disabled'" : "")}>
        <div unselectable='on' class='t_input k-input'>{HAE(text)}</div>
        <div unselectable='on' class='t_select' aria-label='select'>
            <span class='t_dd'>
                <div class='t_img'></div>
            </span>
        </div>
    </div>
    {tag.ToString(YTagRenderMode.Normal)}
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DropDownListEditComponent('{component.ControlId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }

        internal class AjaxData {
            public string OptionsHTML { get; set; }
            public string ExtraData { get; set; }
        }

        /// <summary>
        /// Render a JSON object with data and tooltips for a DropDownList component.
        /// </summary>
        /// <param name="extraData">Optional data to be returned in JSON object as 'extra:' data.</param>
        /// <param name="list">A list of all items part of the DropDownList component.</param>
        /// <returns>A JSON object containing data and tooltips to update the contents of a DropDownList.</returns>
        public static string RenderDataSource(List<SelectionItem<TYPE>> list, string extraData) {

            AjaxData data = new AjaxData {
                ExtraData = extraData,
                OptionsHTML = GetOptionsHTML(list),
            };

            ScriptBuilder sb = new ScriptBuilder();
            sb.Append(Basics.AjaxJSONReturn);
            sb.Append(Utility.JsonSerialize(data));

            return sb.ToString();
        }

        private static string GetOptionsHTML(List<SelectionItem<TYPE>> list) {

            HtmlBuilder tagHtml = new HtmlBuilder();

            foreach (var item in list) {
                YTagBuilder tagOpt = new YTagBuilder("option");
                tagOpt.SetInnerText(item.Text.ToString());
                tagOpt.Attributes["value"] = item.Value?.ToString();
                string desc = item.Tooltip?.ToString();
                if (!string.IsNullOrWhiteSpace(desc))
                    tagOpt.Attributes[Basics.CssTooltip] = desc;
                tagHtml.Append(tagOpt.ToString(YTagRenderMode.Normal));
            }
            return tagHtml.ToString();
        }
    }
}
