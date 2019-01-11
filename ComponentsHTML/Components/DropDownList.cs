/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class DropDownListComponent : DropDownListEditComponentBase<string> { public DropDownListComponent() : base("DropDownList") { } }
    public class DropDownListIntComponent : DropDownListEditComponentBase<int> { public DropDownListIntComponent() : base("DropDownListInt") { } }

    public abstract class DropDownListEditComponentBase<TYPE> : YetaWFComponent, IYetaWFComponent<TYPE> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public string TemplateName { get; set; }

        public DropDownListEditComponentBase(string templateName) {
            TemplateName = templateName;
        }
        public static async Task IncludeExplicitAsync() { // this component is reusable so we need to explicitly include all js/css
            bool useKendo = true;
            if (useKendo) {
                await KendoUICore.AddFileAsync("kendo.data.min.js");
                // await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
                await KendoUICore.AddFileAsync("kendo.list.min.js");
                await KendoUICore.AddFileAsync("kendo.fx.min.js");
                await KendoUICore.AddFileAsync("kendo.userevents.min.js");
                await KendoUICore.AddFileAsync("kendo.draganddrop.min.js");
                await KendoUICore.AddFileAsync("kendo.mobile.scroller.min.js");
                await KendoUICore.AddFileAsync("kendo.virtuallist.min.js");
                await KendoUICore.AddFileAsync("kendo.dropdownlist.min.js");
            }
            await Manager.AddOnManager.AddTemplateAsync(Controllers.AreaRegistration.CurrentPackage.AreaName, "DropDownList");
        }
        public Task<YHtmlString> RenderAsync(TYPE model) {

            List<SelectionItem<TYPE>> list;
            if (!TryGetSiblingProperty($"{PropertyName}_List", out list))
                list = new List<SelectionItem<TYPE>>();
            return RenderDropDownListAsync(this, model, list, "yt_dropdownlist");
        }

        public static async Task<YHtmlString> RenderDropDownListAsync(YetaWFComponent component, TYPE model, List<SelectionItem<TYPE>> list, string cssClass) {

            await IncludeExplicitAsync();

            HtmlBuilder hb = new HtmlBuilder();

            bool useKendo = true;
            bool adjustWidth = false;

            YTagBuilder tag = new YTagBuilder("select");
            if (!string.IsNullOrWhiteSpace(cssClass))
                tag.AddCssClass(cssClass);
            tag.AddCssClass("t_edit");
            tag.AddCssClass("yt_dropdownlist_base");

            bool disabled = false;
            if (list.Count <= 1) {
                if (component.PropData.GetAdditionalAttributeValue("Disable1OrLess", true))
                    disabled = true;
            }
            if (disabled) {
                component.FieldSetup(tag, FieldType.Anonymous);
                tag.Attributes.Remove("disabled");
                tag.Attributes.Add("disabled", "disabled");

                if (list.Count > 0) {
                    // disabled fields are not submitted so we dummy up a hidden field with the value
                    YTagBuilder tagHidden = new YTagBuilder("input");
                    tagHidden.Attributes.Add("type", "hidden");
                    tagHidden.MergeAttribute("name", component.FieldName, false);
                    SelectionItem<TYPE> sel = list.First();
                    tagHidden.Attributes.Add("value", sel.Value?.ToString());
                    tagHidden.SetInnerText(sel.Text.ToString());
                    hb.Append(tagHidden.ToString(YTagRenderMode.StartTag));
                }
            } else {
                component.FieldSetup(tag, component.Validation ? FieldType.Validated : FieldType.Normal);
            }

            string id = null;
            if (useKendo) {
                id = component.MakeId(tag);
                tag.Attributes.Add("data-charavgw", Manager.CharWidthAvg.ToString());
                tag.AddCssClass("t_kendo");
                adjustWidth = component.PropData.GetAdditionalAttributeValue("AdjustWidth", true);
            } else
                tag.AddCssClass("t_native");

            HtmlBuilder tagHtml = new HtmlBuilder();
            ScriptBuilder sb = new ScriptBuilder();

            bool haveDesc = false;
            int empty = 0;// count empty tooltips so we don't generate them (and just drop if all are trailing entries)
            foreach (var item in list) {
                YTagBuilder tagOpt = new YTagBuilder("option");
                tagOpt.SetInnerText(item.Text.ToString());
                if (item.Value != null)
                    tagOpt.Attributes["value"] = item.Value.ToString();
                else
                    tagOpt.Attributes["value"] = "";
                if (Equals(item.Value, model))
                    tagOpt.Attributes["selected"] = "selected";
                string desc = (item.Tooltip != null) ? item.Tooltip.ToString() : null;
                if (!useKendo) {
                    if (!string.IsNullOrWhiteSpace(desc))
                        tagOpt.Attributes["title"] = desc;
                } else {
                    if (string.IsNullOrWhiteSpace(desc)) {
                        desc = "";
                        empty++;
                    } else {
                        while (empty-- > 0)
                            sb.Append("\"\",");
                        empty = 0;
                        haveDesc = true;
                        sb.Append("{0},", YetaWFManager.JsonSerialize(desc));
                    }
                }
                tagHtml.Append(tagOpt.ToString(YTagRenderMode.Normal));
            }

            if (useKendo) {
                if (!haveDesc) // if we don't have any descriptions, clear the tooltip array
                    sb = new ScriptBuilder();
                ScriptBuilder newSb = new ScriptBuilder();
                newSb.Append($@"new YetaWF_ComponentsHTML.DropDownListEditComponent('{id}', {{
                    ToolTips: [{sb.ToString()}],
                    AdjustWidth: {JE(adjustWidth)}
                }});");
                sb = newSb;
            }

            tag.InnerHtml = tagHtml.ToString();

            hb.Append($@"
{tag.ToString(YTagRenderMode.Normal)}");

            if (sb.Length > 0) {
                hb.Append($@"
<script>
    {sb.ToString()}
</script>");
            }
            return hb.ToYHtmlString();
        }

        /// <summary>
        /// Render a JSON object with data and tooltips for a dropdownlist.
        /// </summary>
        /// <typeparam name="TYPE">The Type of the property.</typeparam>
        /// <param name="extraData">Optional data to be returned in JSON object as 'extra:' data.</param>
        /// <param name="list">A list of all items part of the dropdownlist.</param>
        /// <returns>A JSON object containing data and tooltips to update the contents of a dropdownlist.</returns>
        public static YHtmlString RenderDataSource(List<SelectionItem<TYPE>> list, string extraData) {
            ScriptBuilder sb = new ScriptBuilder();
            sb.Append(Basics.AjaxJSONReturn);
            sb.Append(@"{""data"":[");
            foreach (SelectionItem<TYPE> item in list) {
                sb.Append(@"{{""t"":{0},""v"":{1}}},", YetaWFManager.JsonSerialize(item.Text.ToString()), YetaWFManager.JsonSerialize(item.Value != null ? item.Value.ToString() : ""));
            }
            if (list.Count > 0)
                sb.RemoveLast();
            sb.Append(@"],""tooltips"":[");
            if ((from i in list where i.Tooltip != null && !string.IsNullOrWhiteSpace(i.Tooltip.ToString()) select i).FirstOrDefault() != null) {
                foreach (SelectionItem<TYPE> item in list) {
                    sb.Append("{0},", YetaWFManager.JsonSerialize(item.Tooltip == null ? "" : item.Tooltip.ToString()));
                }
                if (list.Count > 0)
                    sb.RemoveLast();
            }
            if (!string.IsNullOrWhiteSpace(extraData)) {
                sb.Append(@"],""extra"":[");
                sb.Append("{0}", YetaWFManager.JsonSerialize(extraData));
            }
            sb.Append("]}");
            return sb.ToYHtmlString();
        }

    }
}
