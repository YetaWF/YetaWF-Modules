/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Components {

    public abstract class PanelInfoComponentBase : YetaWFComponent {

        public const string TemplateName = "PanelInfo";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class ModulePanelInfoDisplayComponent : PanelInfoComponentBase, IYetaWFComponent<PanelInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(PanelInfo model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_panels_panelinfo t_display' id='{ControlId}'>");

            if (model.Style == PanelInfo.PanelStyleEnum.Tabs) {

                hb.Append($@"
    <div class='t_panels t_acctabs' id='{DivId}'>");

                int panelIndex = 0;
                int tabIndex = 0;

                hb.Append(PropertyListComponentBase.RenderTabStripStart(DivId));
                for (panelIndex = 0; panelIndex < model.Panels.Count; ++panelIndex) {
                    if (model.Panels[panelIndex].IsAuthorizedAsync().Result) {
                        string caption = model.Panels[panelIndex].Caption;
                        if (string.IsNullOrWhiteSpace(caption)) { caption = this.__ResStr("noCaption", "(no caption)"); }
                        string toolTip = model.Panels[panelIndex].ToolTip;
                        if (string.IsNullOrWhiteSpace(toolTip)) { toolTip = null; }
                        hb.Append(PropertyListComponentBase.RenderTabEntry(DivId, caption, toolTip, tabIndex));
                        ++tabIndex;
                    }
                }
                hb.Append(PropertyListComponentBase.RenderTabStripEnd(DivId));

                panelIndex = 0;
                tabIndex = 0;

                foreach (PanelInfo.PanelEntry panel in model.Panels) {
                    if (model.Panels[panelIndex].IsAuthorizedAsync().Result) {
                        hb.Append(PropertyListComponentBase.RenderTabPaneStart(DivId, tabIndex, "t_panel"));
                        //TODO: is this needed? hb.Append(PropertyListComponentBase.RenderHidden("Index", tabIndex));
                        ModuleDefinition mod = await model.Panels[panelIndex].GetModuleAsync();
                        if (mod != null) {
                            mod.ShowTitle = false;
                            mod.UsePartialFormCss = false;
                            hb.Append(await mod.RenderModuleAsync(HtmlHelper));
                        } else {
                            hb.Append($@"<div>{this.__ResStr("noModule", "(no module defined)")}</div>");
                        }
                        hb.Append(PropertyListComponentBase.RenderTabPaneEnd(DivId, tabIndex));
                        tabIndex++;
                    }
                    panelIndex++;
                }
                hb.Append($@"
    </div>");
                hb.Append(await PropertyListComponentBase.RenderTabInitAsync(DivId, model));

            } else if (model.Style == PanelInfo.PanelStyleEnum.AccordionjQuery) {

                hb.Append($@"
    <div class='t_panels t_accjquery' id='{DivId}'>");

                int panelIndex = 0;

                for (panelIndex = 0; panelIndex < model.Panels.Count; ++panelIndex) {
                    if (await model.Panels[panelIndex].IsAuthorizedAsync()) {
                        string caption = model.Panels[panelIndex].Caption;
                        if (string.IsNullOrWhiteSpace(caption)) { caption = this.__ResStr("noCaption", "(no caption)"); }
                        hb.Append($@"<h3>{YetaWFManager.HtmlEncode(caption)}</h3>");
                        ModuleDefinition mod = model.Panels[panelIndex].GetModuleAsync().Result;
                        if (mod != null) {
                            mod.ShowTitle = false;
                            mod.UsePartialFormCss = false;
                            hb.Append(await mod.RenderModuleAsync(HtmlHelper));
                        } else {
                            hb.Append($@"<div>{this.__ResStr("noModule", "(no module defined)")}</div>");
                        }
                    }
                }
                hb.Append($@"
    </div>
    <script>
        $('#{DivId}').accordion({{
            collapsible: true,
            heightStyle: 'content',
            activate: function (ev, ui) {{
                if (ui.newPanel[0])
                    $YetaWF.processActivateDivs([ui.newPanel[0]]);
            }}
        }});
    </script>");

            } else if (model.Style == PanelInfo.PanelStyleEnum.AccordionKendo) {

                hb.Append($@"
    <ul class='t_panels t_acckendo' id='{DivId}'>");

                int panelIndex = 0;
                int tabIndex = 0;

                for (panelIndex = 0; panelIndex < model.Panels.Count; ++panelIndex) {
                    if (model.Panels[panelIndex].IsAuthorizedAsync().Result) {
                        string caption = model.Panels[panelIndex].Caption;
                        if (string.IsNullOrWhiteSpace(caption)) { caption = this.__ResStr("noCaption", "(no caption)"); }

                        hb.Append($@"
        <li class='{(model._ActiveTab == tabIndex ? "k-state-active" : "")}'>
            <span class='{(model._ActiveTab == tabIndex ? "k-link k-state-selected" : "")}'>{YetaWFManager.HtmlEncode(caption)}</span>
            <div class='t_panel-kendo' style='display:none'>");

                        ModuleDefinition mod = await model.Panels[panelIndex].GetModuleAsync();
                        if (mod != null) {
                            mod.ShowTitle = false;
                            mod.UsePartialFormCss = false;
                            hb.Append(await mod.RenderModuleAsync(HtmlHelper));
                        } else {
                            hb.Append(this.__ResStr("noModule", "(no module defined)"));
                        }
                        hb.Append($@"
            </div>
        </li>");

                        ++tabIndex;
                    }
                }
                hb.Append($@"
    </ul>");

                await KendoUICore.AddFileAsync("kendo.data.min.js");
                await KendoUICore.AddFileAsync("kendo.panelbar.min.js");
                hb.Append($@"
    <script>
        $('#{DivId}').kendoPanelBar({{
            expandMode: 'single',
            activate: function(ev) {{
                $YetaWF.processActivateDivs([ev.item[0]]);
            }}
        }});
        var $panelBar = $('#{DivId}').kendoPanelBar().data('kendoPanelBar');
        $panelBar.select();
    </script>");
            }
            hb.Append($@"
    <script>");
            using (DocumentReady(hb, ControlId)) {
                hb.Append($@"$YetaWF.processActivateDivs([$YetaWF.getElementById('{ControlId}')]);");
            }
            hb.Append($@"
    </script>
</div>
");

            return hb.ToYHtmlString();
        }
    }
    public class ModulePanelInfoEditComponent : PanelInfoComponentBase, IYetaWFComponent<PanelInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(PanelInfo model) {

            HtmlBuilder hb = new HtmlBuilder();

            int tabEntry = 0;

            hb.Append($@"
<div id='{ControlId}' class='yt_panels_panelinfo t_edit'>
    {await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}
    <div class='t_panels' id='{DivId}'>
        {PropertyListComponentBase.RenderTabStripStart(DivId)}");

            for (int i = 0; i < model.Panels.Count; ++i) {
                string caption = model.Panels[i].Caption.ToString();
                if (string.IsNullOrWhiteSpace(caption)) { caption = this.__ResStr("noCaption", "(no caption)"); }
                hb.Append(PropertyListComponentBase.RenderTabEntry(DivId, caption, null, i));
            }
            hb.Append(PropertyListComponentBase.RenderTabStripEnd(DivId));

            foreach (var panel in model.Panels) {
                hb.Append(PropertyListComponentBase.RenderTabPaneStart(DivId, tabEntry, "t_panel"));
                using (Manager.StartNestedComponent($"{FieldNamePrefix}.Panels[{tabEntry}]")) {
                    hb.Append(await HtmlHelper.ForEditContainerAsync(panel, "PropertyList"));
                }
                hb.Append(PropertyListComponentBase.RenderTabPaneEnd(DivId, tabEntry));
                ++tabEntry;
            }
            hb.Append($@"
    </div>
    {await PropertyListComponentBase.RenderTabInitAsync(DivId, model)}
    <div class='t_buttons'>
        <input type='button' class='t_apply' value='{this.__ResStr("btnApply", "Apply")}' title='{this.__ResStr("txtApply", "Click to apply the current changes")}' />
        <input type='button' class='t_up' value='{this.__ResStr("btnUp", "<<")}' title='{this.__ResStr("txtUp", "Click to move the current panel")}' />
        <input type='button' class='t_down' value='{this.__ResStr("btnDown", ">>")}' title='{this.__ResStr("txtDown", "Click to move the current panel")}' />
        <input type='button' class='t_ins' value='{this.__ResStr("btnIns", "Insert")}' title='{this.__ResStr("txtIns", "Click to insert a new panel before the current panel")}' />
        <input type='button' class='t_add' value='{this.__ResStr("btnAdd", "Add")}' title='{this.__ResStr("txtAdd", "Click to add a new panel after the current panel")}' />
        <input type='button' class='t_delete' value='{this.__ResStr("btnDelete", "Remove")}' title='{this.__ResStr("txtDelete", "Click to remove the current panel")}' />
    </div>
</div>
<script>
    new YetaWF_Panels.PanelInfoEditComponent('{ControlId}');
</script>");

            return hb.ToYHtmlString();
        }
    }
}

