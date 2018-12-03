﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public partial class PropertyListDisplayComponent : PropertyListComponentBase, IYetaWFContainer<object>, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderContainerAsync(object model) {

            return await RenderPropertyListTabbedAsync(model, true);
        }
        public async Task<YHtmlString> RenderAsync(object model) {
            using (Manager.StartNestedComponent($"{FieldName}")) {
                return await RenderPropertyListTabbedAsync(model, true);
            }
        }
    }
    public partial class PropertyListEditComponent : PropertyListComponentBase, IYetaWFContainer<object>, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderContainerAsync(object model) {

            return await RenderPropertyListTabbedAsync(model, false);
        }
        public async Task<YHtmlString> RenderAsync(object model) {
            using (Manager.StartNestedComponent($"{FieldName}")) {
                return await RenderPropertyListTabbedAsync(model, false);
            }
        }
    }
    public abstract partial class PropertyListComponentBase : YetaWFComponent {

        public const string TemplateName = "PropertyList";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderPropertyListTabbedAsync(object model, bool readOnly) {

            List<string> categories = GetCategories(model);
            if (categories.Count <= 1) // if there is only one tab, show as regular property list
                return await RenderPropertyListAsync(model, readOnly);

            HtmlBuilder hb = new HtmlBuilder();
            Type modelType = model.GetType();

            ClassData classData = ObjectSupport.GetClassData(modelType);
            RenderHeader(hb, classData);

            string divId = Manager.UniqueId();
            hb.Append($@"
<div id='{divId}' class='yt_propertylisttabbed {(readOnly ? "t_display" : "t_edit")}'>");

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

            hb.Append($@"
</div>
{await RenderTabInitAsync(divId, model)}");

            RenderFooter(hb, classData);

            if (!readOnly) {
                ControlData cd = GetControlSets(model, divId);
                if (cd != null) {
                    hb.Append($@"
<script>
    new YetaWF_ComponentsHTML.PropertyListComponent('{divId}', {YetaWFManager.JsonSerialize(cd)});
</script>");
                }
            }
            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPropertyListAsync(object model, bool ReadOnly) {

            HtmlBuilder hb = new HtmlBuilder();
            Type modelType = model.GetType();
            ClassData classData = ObjectSupport.GetClassData(modelType);
            RenderHeader(hb, classData);

            hb.Append(await RenderHiddenAsync(model));

            bool showVariables = YetaWF.Core.Localize.UserSettings.GetProperty<bool>("ShowVariables");

            // property table
            HtmlBuilder hbProps = new HtmlBuilder();
            string divId = Manager.UniqueId();
            hbProps.Append($@"
<div id='{divId}' class='yt_propertylist t_table {(ReadOnly ? "t_display" : "t_edit")}'>
   {await RenderListAsync(model, null, showVariables, ReadOnly)}
</div>");

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
                ControlData cd = GetControlSets(model, divId);
                if (cd != null) {
                    hb.Append($@"
<script>
    new YetaWF_ComponentsHTML.PropertyListComponent('{divId}', {YetaWFManager.JsonSerialize(cd)});
</script>");
                }
            }

            return hb.ToYHtmlString();
        }
    }
}
