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
    /// [UIHint("DropDown2List"), Required]
    /// public string Country { get; set; }
    /// public List&lt;SelectionItem&lt;string&gt;&gt; Country_List { get; set; }
    /// </example>
    [UsesSibling("_List", "List<YetaWF.Core.Components.SelectionItem<string>>", "Defines the list of values, displayed text and tooltips shown in the dropdown list.")]
    [UsesAdditional("Disable1OrLess", "bool", "true", "Defines whether the dropdown list is disabled when only 1 item or less is available in the list of values.")]
    public class DropDown2ListComponent : DropDown2ListEditComponentBase<string> {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DropDown2ListComponent() : base("DropDown2List") { }
    }
    /// <summary>
    /// Allows selection of a value from a list of int values using a dropdown list. The dropdown list supports optional tooltips.
    /// </summary>
    /// <example>
    /// [Caption("Entries"), Description("Select the number of entries per page")]
    /// [UIHint("DropDown2ListInt")]
    /// public int PageSelection { get; set; }
    /// public List&lt;SelectionItem&lt;int&gt;&gt; PageSelection_List { get; set; }
    /// </example>
    [UsesSibling("_List", "List<YetaWF.Core.Components.SelectionItem<int>>", "Defines the list of values, displayed text and tooltips shown in the dropdown list.")]
    [UsesAdditional("Disable1OrLess", "bool", "true", "Defines whether the dropdown list is disabled when only 1 item or less is available in the list of values.")]
    public class DropDown2ListIntComponent : DropDown2ListEditComponentBase<int> {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DropDown2ListIntComponent() : base("DropDown2ListInt") { }
    }
    /// <summary>
    /// Allows selection of a value from a list of nullable int values using a dropdown list. The dropdown list supports optional tooltips.
    /// </summary>
    /// <example>
    /// [Caption("Entries"), Description("Select the number of entries per page")]
    /// [UIHint("DropDown2ListIntNull")]
    /// public int? PageSelection { get; set; }
    /// public List&lt;SelectionItem&lt;int?&gt;&gt; PageSelection_List { get; set; }
    /// </example>
    [UsesSibling("_List", "List<YetaWF.Core.Components.SelectionItem<int?>>", "Defines the list of values, displayed text and tooltips shown in the dropdown list.")]
    [UsesAdditional("Disable1OrLess", "bool", "true", "Defines whether the dropdown list is disabled when only 1 item or less is available in the list of values.")]
    public class DropDown2ListIntNullComponent : DropDown2ListEditComponentBase<int?> {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DropDown2ListIntNullComponent() : base("DropDown2ListIntNull") { }
    }

    /// <summary>
    /// Base class for the DropDown2List component implementation.
    /// </summary>
    public abstract class DropDown2ListEditComponentBase<TYPE> : YetaWFComponent, IYetaWFComponent<TYPE> {

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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        public DropDown2ListEditComponentBase(string templateName) {
            TemplateName = templateName;
        }
        /// <summary>
        /// Adds all addons for the DropDown2List component to the current page.
        /// </summary>
        public static async Task IncludeExplicitAsync() { // this component is reusable so we need to explicitly include all js/css
            await Manager.AddOnManager.AddTemplateAsync(Controllers.AreaRegistration.CurrentPackage.AreaName, "DropDown2List");
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
            return RenderDropDown2ListAsync(this, model, list, "yt_dropdown2list");//$$$
        }

        /// <summary>
        /// Renders a DropDown2List component as HTML.
        /// </summary>
        /// <param name="component">The component to render.</param>
        /// <param name="model">The model rendered by the component.</param>
        /// <param name="list">A collection of items to render.</param>
        /// <param name="cssClass">A CSS class to add to the &lt;select&gt; tag. May be null.</param>
        /// <returns></returns>
        public static async Task<string> RenderDropDown2ListAsync(YetaWFComponent component, TYPE model, List<SelectionItem<TYPE>> list, string cssClass) {

            // Uses kendo styles (k-widget, k-dropdown , k-dropdown-wrap, k-input, k-select)

            await IncludeExplicitAsync();

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

            HtmlBuilder tagHtml = new HtmlBuilder();

            string text = null;
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
<div id='{component.ControlId}' class='k-widget k-dropdown yt_dropdown2list_base t_edit {cssClass}' tabindex='0' unselectable='on' role='listbox' aria-haspopup='true' aria-expanded='false' aria-owns='{component.ControlId}_list' aria-live='polite' aria-disabled='false' aria-busy='false'
        aria-activedescendant='{Guid.NewGuid().ToString()}'>
    <div unselectable='on' class='t_container k-dropdown-wrap k-state-default'>
        <div unselectable='on' class='t_input k-input'>{HAE(text ?? "TESTING$$" )}</div>
        <div unselectable='on' class='t_select' aria-label='select'>
            <span class='t_dd'>
                <div class='t_img'></div>
            </span>
        </div>
    </div>
    {tag.ToString(YTagRenderMode.Normal)}
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DropDown2ListEditComponent('{component.ControlId}');");

            return hb.ToString();
        }

        /// <summary>
        /// Render a JSON object with data and tooltips for a DropDown2List component.
        /// </summary>
        /// <param name="extraData">Optional data to be returned in JSON object as 'extra:' data.</param>
        /// <param name="list">A list of all items part of the DropDown2List component.</param>
        /// <returns>A JSON object containing data and tooltips to update the contents of a DropDown2List.</returns>
        public static string RenderDataSource(List<SelectionItem<TYPE>> list, string extraData) {
            ScriptBuilder sb = new ScriptBuilder();
            //sb.Append(Basics.AjaxJSONReturn);
            //sb.Append(@"{""data"":[");
            //foreach (SelectionItem<TYPE> item in list) {
            //    sb.Append(@"{{""t"":{0},""v"":{1}}},", Utility.JsonSerialize(item.Text.ToString()), Utility.JsonSerialize(item.Value != null ? item.Value.ToString() : ""));
            //}
            //if (list.Count > 0)
            //    sb.RemoveLast();
            //sb.Append(@"],""tooltips"":[");
            //if ((from i in list where i.Tooltip != null && !string.IsNullOrWhiteSpace(i.Tooltip.ToString()) select i).FirstOrDefault() != null) {
            //    foreach (SelectionItem<TYPE> item in list) {
            //        sb.Append("{0},", Utility.JsonSerialize(item.Tooltip == null ? "" : item.Tooltip.ToString()));
            //    }
            //    if (list.Count > 0)
            //        sb.RemoveLast();
            //}
            //if (!string.IsNullOrWhiteSpace(extraData)) {
            //    sb.Append(@"],""extra"":[");
            //    sb.Append("{0}", Utility.JsonSerialize(extraData));
            //}
            //sb.Append("]}");
            return sb.ToString();
        }

    }
}
