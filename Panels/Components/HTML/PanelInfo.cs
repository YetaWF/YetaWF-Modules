/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Components {

    public abstract class PanelInfoComponentBase : YetaWFComponent {

        public const string TemplateName = "PanelInfo";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.Panels package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class ModulePanelInfoDisplayComponent : PanelInfoComponentBase, IYetaWFComponent<PanelInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        internal class UI {
            [UIHint("Tabs")]
            public TabsDefinition TabsDef { get; set; }
        }
        internal class Setup {
            public PanelInfo.PanelStyleEnum Style { get; set; }
            public int ActiveIndex { get; set; }
            public Setup() { }
        }

        public async Task<string> RenderAsync(PanelInfo model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_panels_panelinfo t_display' id='{ControlId}'>");

            if (model.Style == PanelInfo.PanelStyleEnum.Tabs) {

                UI ui = new UI {
                    TabsDef = new TabsDefinition {
                        ActiveTabIndex = model._ActiveTab,
                    }
                };

                for (int panelIndex = 0; panelIndex < model.Panels.Count; ++panelIndex) {
                    if (await model.Panels[panelIndex].IsAuthorizedAsync()) {
                        string caption = model.Panels[panelIndex].Caption;
                        if (string.IsNullOrWhiteSpace(caption)) { caption = this.__ResStr("noCaption", "(no caption)"); }
                        string toolTip = model.Panels[panelIndex].ToolTip;
                        if (string.IsNullOrWhiteSpace(toolTip)) { toolTip = null; }
                        ui.TabsDef.Tabs.Add(new TabEntry {
                            Caption = caption,
                            ToolTip = toolTip,
                            PaneCssClasses = "t_panel",
                            RenderPaneAsync = async (int tabIndex) => {
                                HtmlBuilder hbt = new HtmlBuilder();
                                if (await model.Panels[tabIndex].IsAuthorizedAsync()) {
                                    ModuleDefinition mod = await model.Panels[tabIndex].GetModuleAsync();
                                    if (mod != null) {
                                        mod.ShowTitle = false;
                                        mod.UsePartialFormCss = false;
                                        hbt.Append(await mod.RenderModuleAsync(HtmlHelper));
                                    } else {
                                        hbt.Append($@"<div>{this.__ResStr("noModule", "(no module defined)")}</div>");
                                    }
                                }
                                return hbt.ToString();
                            },
                        });
                    }
                }
                hb.Append($@"
    <div class='t_panels t_acctabs' id='{DivId}'>
        {await HtmlHelper.ForDisplayAsync(ui, nameof(ui.TabsDef), HtmlAttributes: new { __NoTemplate = true })}
    </div>");

            } else if (model.Style == PanelInfo.PanelStyleEnum.Accordion) {

                hb.Append($@"
    <div class='t_panels t_accordion' id='{DivId}' role='tablist'>");

                for (int panelIndex = 0; panelIndex < model.Panels.Count; ++panelIndex) {

                    bool active = panelIndex == model._ActiveTab;

                    if (await model.Panels[panelIndex].IsAuthorizedAsync()) {
                        string caption = model.Panels[panelIndex].Caption;
                        if (string.IsNullOrWhiteSpace(caption)) { caption = this.__ResStr("noCaption", "(no caption)"); }

                        // svg icon used: fas-caret-down
                        // svg icon used: fas-caret-up
                        hb.Append($@"
        <h2 class='t_accheader {(active ? "t_active" : "t_collapsed")}' role='tab'
            id='{DivId}_{panelIndex}_lb' aria-controls='{DivId}_{panelIndex}_pane' aria-selected='{(active ? "true" : "false")}' aria-expanded='{(active ? "true" : "false")}' tabindex='{(active ? "0" : "-1")}'>
            <div class='t_accicon'>
                <svg class='t_down' aria-hidden='true' focusable='false' role='img' width='16' height='16' viewBox='0 0 320 512'>
                    <path fill='currentColor' d='M31.3 192h257.3c17.8 0 26.7 21.5 14.1 34.1L174.1 354.8c-7.8 7.8-20.5 7.8-28.3 0L17.2 226.1C4.6 213.5 13.5 192 31.3 192z'></path>
                </svg>
                <svg class='t_up' aria-hidden='true' focusable='false' role='img' width='16' height='16' viewBox='0 0 320 512'>
                    <path fill='currentColor' d='M288.662 352H31.338c-17.818 0-26.741-21.543-14.142-34.142l128.662-128.662c7.81-7.81 20.474-7.81 28.284 0l128.662 128.662c12.6 12.599 3.676 34.142-14.142 34.142z'></path>
                </svg>
            </div>
            {Utility.HE(caption)}
        </h2>
        <div class='t_panel t_acccontent {(active ? "t_active" : "")}' id='{DivId}_{panelIndex}_pane'
            aria-labelledby='{DivId}_{panelIndex}_lb' role='tabpanel' aria-hidden='{(active ? "false" : "true")}' style='overflow: hidden;{(active ? "" : "display:none")}'>");

                        ModuleDefinition mod = await model.Panels[panelIndex].GetModuleAsync();
                        if (mod != null) {
                            mod.ShowTitle = false;
                            mod.UsePartialFormCss = false;
                            hb.Append(await mod.RenderModuleAsync(HtmlHelper));
                        } else {
                            hb.Append($@"<div>{this.__ResStr("noModule", "(no module defined)")}</div>");
                        }
                        hb.Append($@"
        </div>");
                    }
                }
                hb.Append($@"
    </div>");

            }
            hb.Append($@"
</div>");

            Setup setup = new Setup {
                Style = model.Style,
                ActiveIndex = model._ActiveTab,
            };
            Manager.ScriptManager.AddLast($@"new YetaWF_Panels.PanelInfoComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }

    /// <summary>
    /// This component is used by the YetaWF.Panels package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class ModulePanelInfoEditComponent : PanelInfoComponentBase, IYetaWFComponent<PanelInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class UI {
            [UIHint("Tabs")]
            public TabsDefinition TabsDef { get; set; }
        }

        public async Task<string> RenderAsync(PanelInfo model) {

            UI ui = new UI {
                TabsDef = new TabsDefinition {
                    ActiveTabIndex = model._ActiveTab,
                }
            };

            for (int panelIndex = 0; panelIndex < model.Panels.Count; ++panelIndex) {
                string caption = model.Panels[panelIndex].Caption;
                if (string.IsNullOrWhiteSpace(caption)) { caption = this.__ResStr("noCaption", "(no caption)"); }
                ui.TabsDef.Tabs.Add(new TabEntry {
                    Caption = caption,
                    PaneCssClasses = "t_panel",
                    RenderPaneAsync = async (int tabIndex) => {
                        HtmlBuilder hbt = new HtmlBuilder();
                        using (Manager.StartNestedComponent($"{FieldNamePrefix}.Panels[{tabIndex}]")) {
                            hbt.Append(await HtmlHelper.ForEditContainerAsync(model.Panels[tabIndex], "PropertyList"));
                        }
                        return hbt.ToString();
                    },
                });
            }


            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{ControlId}' class='yt_panels_panelinfo t_edit'>
    {await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}
    <div class='t_panels' id='{DivId}'>
        {await HtmlHelper.ForDisplayAsync(ui, nameof(ui.TabsDef), HtmlAttributes: new { __NoTemplate = true })}
    </div>
    <div class='t_buttons'>
        <input type='button' class='y_button t_apply' value='{this.__ResStr("btnApply", "Apply")}' title='{this.__ResStr("txtApply", "Click to apply the current changes")}' />
        <input type='button' class='y_button t_up' value='{this.__ResStr("btnUp", "<<")}' title='{this.__ResStr("txtUp", "Click to move the current panel")}' />
        <input type='button' class='y_button t_down' value='{this.__ResStr("btnDown", ">>")}' title='{this.__ResStr("txtDown", "Click to move the current panel")}' />
        <input type='button' class='y_button t_ins' value='{this.__ResStr("btnIns", "Insert")}' title='{this.__ResStr("txtIns", "Click to insert a new panel before the current panel")}' />
        <input type='button' class='y_button t_add' value='{this.__ResStr("btnAdd", "Add")}' title='{this.__ResStr("txtAdd", "Click to add a new panel after the current panel")}' />
        <input type='button' class='y_button t_delete' value='{this.__ResStr("btnDelete", "Remove")}' title='{this.__ResStr("txtDelete", "Click to remove the current panel")}' />
    </div>
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_Panels.PanelInfoEditComponent('{ControlId}');");

            return hb.ToString();
        }
    }
}

