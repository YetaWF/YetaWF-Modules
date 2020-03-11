/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Components {

    public abstract class StepInfoComponentBase : YetaWFComponent {

        public const string TemplateName = "StepInfo";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ModuleStepInfoDisplayComponent : StepInfoComponentBase, IYetaWFComponent<StepInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<string> RenderAsync(StepInfo model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_panels_stepinfo t_display' id='{ControlId}'>");

            int stepIndex = 0;

            foreach (StepInfo.StepEntry step in model.Steps) {

                string caption = model.Steps[stepIndex].Caption;
                if (string.IsNullOrWhiteSpace(caption)) { caption = this.__ResStr("noCaption", "(no caption)"); }
                string description = model.Steps[stepIndex].Description;
                if (string.IsNullOrWhiteSpace(description)) { description = this.__ResStr("noDesc", "(no description)"); }
                string name = model.Steps[stepIndex].Name;
                if (string.IsNullOrWhiteSpace(name)) { name = "unnamed"; }

                hb.Append($@"
    <a class='t_step yaction-link' data-name='{HAE(name)}'>
        <span class='t_caption'>{HE(caption)}</span>
        <span class='t_description'>{HE(description)}</span>
    </a>");

                ++stepIndex;
            }

            hb.Append($@"
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_Panels.StepInfoComponent('{ControlId}');");

            return Task.FromResult(hb.ToString());
        }
    }
    public class ModuleStepInfoEditComponent : StepInfoComponentBase, IYetaWFComponent<StepInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class UI {
            [UIHint("Tabs")]
            public TabsDefinition TabsDef { get; set; }
        }

        public async Task<string> RenderAsync(StepInfo model) {

            UI ui = new UI {
                TabsDef = new TabsDefinition {
                    ActiveTabIndex = model._ActiveTab,
                }
            };
            for (int i = 0; i < model.Steps.Count; ++i) {
                string caption = model.Steps[i].Caption.ToString();
                if (string.IsNullOrWhiteSpace(caption)) { caption = this.__ResStr("noCaption", "(no caption)"); }
                ui.TabsDef.Tabs.Add(new TabEntry {
                    Caption = caption,
                    PaneCssClasses = "t_steps",
                    RenderPaneAsync = async (int tabIndex) => {
                        HtmlBuilder hbt = new HtmlBuilder();
                        using (Manager.StartNestedComponent($"{FieldNamePrefix}.Steps[{tabIndex}]")) {
                            hbt.Append(await HtmlHelper.ForEditContainerAsync(model.Steps[tabIndex], "PropertyList"));
                        }
                        return hbt.ToString();
                    },
                });
            }

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{ControlId}' class='yt_panels_stepinfo t_edit'>
    {await HtmlHelper.ForEditContainerAsync(model, "PropertyList")}
    <div class='t_steps' id='{DivId}'>
        {await HtmlHelper.ForDisplayAsync(ui, nameof(ui.TabsDef), HtmlAttributes: new { __NoTemplate = true })}
    </div>
    <div class='t_buttons'>
        <input type='button' class='t_apply' value='{this.__ResStr("btnApply", "Apply")}' title='{this.__ResStr("txtApply", "Click to apply the current changes")}' />
        <input type='button' class='t_up' value='{this.__ResStr("btnUp", "<<")}' title='{this.__ResStr("txtUp", "Click to move the current step")}' />
        <input type='button' class='t_down' value='{this.__ResStr("btnDown", ">>")}' title='{this.__ResStr("txtDown", "Click to move the current step")}' />
        <input type='button' class='t_ins' value='{this.__ResStr("btnIns", "Insert")}' title='{this.__ResStr("txtIns", "Click to insert a new step before the current step")}' />
        <input type='button' class='t_add' value='{this.__ResStr("btnAdd", "Add")}' title='{this.__ResStr("txtAdd", "Click to add a new step after the current step")}' />
        <input type='button' class='t_delete' value='{this.__ResStr("btnDelete", "Remove")}' title='{this.__ResStr("txtDelete", "Click to remove the current step")}' />
    </div>
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_Panels.StepInfoEditComponent('{ControlId}');");

            return hb.ToString();
        }
    }
}

