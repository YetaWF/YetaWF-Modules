using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

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
            bool useKendo = !Manager.IsRenderingGrid;
            if (useKendo) {
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.data.min.js");
                // await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.list.min.js");
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.fx.min.js");
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.userevents.min.js");
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.draganddrop.min.js");
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.mobile.scroller.min.js");
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.virtuallist.min.js");
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.dropdownlist.min.js");
            }
            await Manager.AddOnManager.AddTemplateAsync(Controllers.AreaRegistration.CurrentPackage.Domain, Controllers.AreaRegistration.CurrentPackage.Product, "DropDownList");
        }
        public Task<YHtmlString> RenderAsync(TYPE model) {

            List<SelectionItem<TYPE>> list;
            if (!TryGetSiblingProperty($"{PropertyName}_List", out list))
                list = new List<SelectionItem<TYPE>>();
            return RenderDropDownListAsync(model, list, this, "yt_dropdownlist");
        }

        public static async Task<YHtmlString> RenderDropDownListAsync(TYPE model, List<SelectionItem<TYPE>> list, YetaWFComponent component, string cssClass) {

            await IncludeExplicitAsync();

            bool useKendo = !Manager.IsRenderingGrid;

            YTagBuilder tag = new YTagBuilder("select");
            tag.AddCssClass(cssClass);
            tag.AddCssClass("t_edit");
            tag.AddCssClass("yt_dropdownlist_base");
            component.FieldSetup(tag, component.Validation ? FieldType.Validated : FieldType.Normal);
            string id = null;
            if (useKendo) {
                id = component.MakeId(tag);
                tag.Attributes.Add("data-needinit", "");
                tag.Attributes.Add("data-charavgw", Manager.CharWidthAvg.ToString());
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
                newSb.Append("$('#{0}').data('tooltips', [{1}]);", id, sb.ToString());
                newSb.Append("YetaWF_TemplateDropDownList.initOne($('#{0}'));", id);
                sb = newSb;
            }

            tag.InnerHtml = tagHtml.ToString();

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(tag.ToString(YTagRenderMode.Normal));
            Manager.ScriptManager.AddLast(sb.ToString());
            return hb.ToYHtmlString();
        }
    }
}
