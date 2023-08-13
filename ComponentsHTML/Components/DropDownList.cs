/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
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

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }

        internal string TemplateName { get; set; }

        internal class Setup {
            public bool AdjustWidth { get; set; }
            public float DropDownWidthFactor { get; set; }
            public float DropDownHeightFactor { get; set; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        protected DropDownListEditComponentBase(string templateName) {
            TemplateName = templateName;
        }

        /// <summary>
        /// Adds all addons for the DropDownList component to the current page.
        /// </summary>
        public static async Task IncludeExplicitAsync() { // this component is reusable so we need to explicitly include all js/css
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, "DropDownList",  ComponentType.Edit);
            await Manager.AddOnManager.AddAddOnNamedAsync(YetaWF.Modules.ComponentsHTML.AreaRegistration.CurrentPackage.AreaName, "github.com.grsmto.simplebar");
        }

        /// <inheritdoc/>
        public Task<string> RenderAsync(TYPE model) {

            TryGetSiblingProperty($"{PropertyName}_List", out List<SelectionItem<TYPE>>? list);
            list ??= new List<SelectionItem<TYPE>>();
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
        public static async Task<string> RenderDropDownListAsync(YetaWFComponent component, TYPE model, List<SelectionItem<TYPE>> list, string? cssClass) {

            await IncludeExplicitAsync();

            string css = string.Empty;
            bool disabled = false;
            if (list.Count <= 1) {
                if (component.PropData.GetAdditionalAttributeValue("Disable1OrLess", true))
                    disabled = true;
            }
            if (disabled) {
                //$$$$$$ THIS DOESN'T WORK WITH TEMPLATES
                component.HtmlAttributes.Remove("disabled");
                component.HtmlAttributes.Add("disabled", "disabled");
                if (list.Count > 0)
                    css = CssManager.CombineCss(css, "disabled-submit");// submit disabled field
            }

            Setup setup = new Setup {
                AdjustWidth = component.PropData.GetAdditionalAttributeValue("AdjustWidth", true),
                DropDownWidthFactor = (float)component.PropData.GetAdditionalAttributeValue("DropDownWidthFactor", 1.0),
                DropDownHeightFactor = (float)component.PropData.GetAdditionalAttributeValue("DropDownHeightFactor", 1.0),
            };

            HtmlBuilder tagHtml = new HtmlBuilder();

            // find the selected value
            SelectionItem<TYPE>? selItem = null;
            foreach (SelectionItem<TYPE> item in list) {
                if (Equals(item.Value, model)) {
                    selItem = item;
                    break;
                }
            }
            if (selItem == null) {
                if (list.Count > 0) {
                    selItem = list[0];
                }
            }

            if (list.Count > 0) {
                foreach (var item in list) {

                    string? desc = null;
                    string? t = item.Tooltip?.ToString();
                    if (!string.IsNullOrWhiteSpace(t))
                        desc = $" {Basics.CssTooltip}='{HAE(t)}'";

                    string? selected = null;
                    if (item == selItem)
                        selected = " selected='selected'";

                    tagHtml.Append($"<option value='{item.Value?.ToString()}'{selected}{desc}>{HE(item.Text.ToString())}</option>");
                }
            }

            // &zwj; below is intended to work around sass failing to compile \200b. This should be in CSS.
            string tags = $@"
<div id='{component.ControlId}' class='yt_dropdownlist_base t_edit {cssClass}' {(disabled ? "aria-disabled='true'" : "tabindex='0' aria-disabled='false'")} role='listbox' aria-haspopup='true' aria-expanded='false' aria-owns='yDDPopup' aria-live='polite' aria-busy='false'
        aria-activedescendant='{Guid.NewGuid().ToString()}'>
    <div class='t_container {(disabled ? "t_disabled" : "")}' {(disabled ? "disabled='disabled'" : "")}>
        <div class='t_input'>{HAE(selItem != null ? selItem.Text : null)}&zwj;</div>
        <div class='t_select' aria-label='select'>
            {SkinSVGs.GetCaret(AreaRegistration.CurrentPackage, "down")}
        </div>
    </div>
    <select{component.FieldSetup(disabled ? FieldType.Normal : (component.Validation ? FieldType.Validated : FieldType.Normal))}{component.GetClassAttribute(css)}>{tagHtml.ToString()}</select>
</div>";

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DropDownListEditComponent('{component.ControlId}', {Utility.JsonSerialize(setup)});");

            return tags;
        }

        internal class AjaxData {
            public string OptionsHTML { get; set; } = null!;
            public string? ExtraData { get; set; }
        }

        /// <summary>
        /// Render a JSON object with data and tooltips for a DropDownList component.
        /// </summary>
        /// <param name="extraData">Optional data to be returned in JSON object as 'extra:' data.</param>
        /// <param name="list">A list of all items part of the DropDownList component.</param>
        /// <returns>A JSON object containing data and tooltips to update the contents of a DropDownList.</returns>
        public static string RenderDataSource(List<SelectionItem<TYPE>> list, string? extraData) {

            AjaxData data = new AjaxData {
                ExtraData = extraData,
                OptionsHTML = GetOptionsHTML(list),
            };

            ScriptBuilder sb = new ScriptBuilder();
            sb.Append(Basics.AjaxJSONReturn);
            sb.Append(Utility.JsonSerialize(data));

            return sb.ToString();
        }

        /// <summary>
        /// Returns HTML for a dropdownlist with the available &lt;option&gt;s.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetOptionsHTML(List<SelectionItem<TYPE>> list) {

            HtmlBuilder tagHtml = new HtmlBuilder();

            foreach (var item in list) {
                string? desc = null;
                string? t = item.Tooltip?.ToString();
                if (!string.IsNullOrWhiteSpace(t))
                    desc = $" {Basics.CssTooltip}='{HAE(t)}'";
                tagHtml.Append($"<option value='{item.Value?.ToString()}'{desc}>{HE(item.Text.ToString())}</option>");
            }
            return tagHtml.ToString();
        }
    }
}
