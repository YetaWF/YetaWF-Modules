using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class PropertyListComponent : YetaWFComponent {

        public const string TemplateName = "PropertyList";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public partial class PropertyListDisplayComponent : PropertyListComponentBase, IYetaWFContainer<object> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderContainerAsync(object model) {

            return await RenderPropertyListTabbedAsync(model, false);
        }
    }
    public partial class PropertyListEditComponent : PropertyListComponentBase, IYetaWFContainer<object> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderContainerAsync(object model) {

            return await RenderPropertyListTabbedAsync(model, false);
        }
    }
    public abstract partial class PropertyListComponentBase : PropertyListComponent {

        protected async Task<YHtmlString> RenderPropertyListTabbedAsync(object model, bool readOnly) {

            await Manager.AddOnManager.AddTemplateAsync("PropertyList"); /*we're using the same javascript as the regular propertylist template */

            List<string> categories = GetCategories(model);
            if (categories.Count <= 1) // if there is only one tab, show as regular property list
                return await RenderPropertyListAsync(model, null, readOnly);

            HtmlBuilder hb = new HtmlBuilder();
            Type modelType = model.GetType();

            ClassData classData = ObjectSupport.GetClassData(modelType);
            RenderHeader(hb, classData);

            string divId = Manager.UniqueId(); //$$$ string.IsNullOrWhiteSpace(id) ? Manager.UniqueId() : id;
            hb.Append("<div id='{0}' class='yt_propertylisttabbed {1}'>", divId, readOnly ? "t_display" : "t_edit");

            hb.Append(await RenderHiddenAsync(model));
            bool showVariables = YetaWF.Core.Localize.UserSettings.GetProperty<bool>("ShowVariables");

            // tabstrip
            hb.Append(RenderTabStripStart(divId));
            int tabEntry = 0;
            foreach (string category in categories) {
                string cat = category;
                if (classData.Categories.ContainsKey(cat))
                    cat = classData.Categories[cat];
                hb.Append(RenderTabEntry(divId, cat, "", tabEntry));
                ++tabEntry;
            }
            hb.Append(RenderTabStripEnd(divId));

            // panels
            int panel = 0;
            foreach (string category in categories) {
                hb.Append(RenderTabPaneStart(divId, panel));
                hb.Append(await RenderListAsync(model, category, showVariables, readOnly));
                hb.Append(RenderTabPaneEnd(divId, panel));
                ++panel;
            }

            hb.Append("</div>");
            hb.Append(await RenderTabInitAsync(divId, model));

            RenderFooter(hb, classData);

            if (!readOnly) {
                string script = GetControlSets(model, divId);
                if (!string.IsNullOrWhiteSpace(script)) {
                    ScriptBuilder sb = new ScriptBuilder();
                    sb.Append("YetaWF_PropertyList.init('{0}', {1}, {2});", divId, script, Manager.InPartialView ? 1 : 0);
                    Manager.ScriptManager.AddLastDocumentReady(sb);
                }
            }
            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPropertyListAsync(object model, string id, bool ReadOnly) {//$$$ id
             
            HtmlBuilder hb = new HtmlBuilder();
            Type modelType = model.GetType();
            ClassData classData = ObjectSupport.GetClassData(modelType);
            RenderHeader(hb, classData);

            hb.Append(await RenderHiddenAsync(model));
            bool showVariables = YetaWF.Core.Localize.UserSettings.GetProperty<bool>("ShowVariables");

            // property table
            HtmlBuilder hbProps = new HtmlBuilder();
            string divId = string.IsNullOrWhiteSpace(id) ? Manager.UniqueId() : id;
            hbProps.Append("<div id='{0}' class='yt_propertylist t_table {1}'>", divId, ReadOnly ? "t_display" : "t_edit");
            hbProps.Append(await RenderListAsync(model, null, showVariables, ReadOnly));
            hbProps.Append("</div>");

            if (!string.IsNullOrWhiteSpace(classData.Legend)) {
                YTagBuilder tagFieldSet = new YTagBuilder("fieldset");
                YTagBuilder tagLegend = new YTagBuilder("legend");
                tagLegend.SetInnerText(classData.Legend);
                tagFieldSet.InnerHtml = tagLegend.ToString(YTagRenderMode.Normal) + hbProps.ToString();
                hb.Append(tagFieldSet.ToString(YTagRenderMode.Normal));
            } else {
                hb.Append(hbProps.ToHtmlString());
            }
            RenderFooter(hb, classData);

            if (!ReadOnly) {
                string script = GetControlSets(model, divId);
                if (!string.IsNullOrWhiteSpace(script)) {
                    ScriptBuilder sb = new ScriptBuilder();
                    sb.Append("YetaWF_PropertyList.init('{0}', {1}, {2});", divId, script, Manager.InPartialView ? 1 : 0);
                    Manager.ScriptManager.AddLastDocumentReady(sb);
                }
            }

            return hb.ToYHtmlString();
        }
    }
}
