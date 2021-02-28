/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Renders the model as module actions (buttons, icons or links). If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Archive"), Description("Monthly blog entries")]
    /// [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks), ReadOnly]
    /// public List&lt;ModuleAction&gt; Actions { get; set; }
    /// </example>
    [UsesAdditional("RenderAs", "YetaWF.Core.Modules.ModuleAction.RenderModeEnum", "ModuleAction.RenderModeEnum.Button", "Defines how the module actions are rendered.")]
    public class ModuleActionsComponent : YetaWFComponent, IYetaWFComponent<List<ModuleAction>> {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ModuleActionsComponent), name, defaultValue, parms); }

        internal const string TemplateName = "ModuleActions";

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(ActionIconsComponent.TemplateName, YetaWFComponentBase.ComponentType.Display);// this is needed because we're not used by templates
            await base.IncludeAsync();
        }

        /// <inheritdoc/>
        public async Task<string> RenderAsync(List<ModuleAction> model) {

            using (Manager.StartNestedComponent(FieldName)) {

                HtmlBuilder hb = new HtmlBuilder();

                ModuleAction.RenderModeEnum renderMode = PropData.GetAdditionalAttributeValue<ModuleAction.RenderModeEnum>("RenderAs", ModuleAction.RenderModeEnum.Button);

                if (model != null && model.Count > 0) {

                    hb.Append($@"
<div class='yt_moduleactions t_display'>");

                    switch (renderMode) {
                        case ModuleAction.RenderModeEnum.ButtonDropDown:
                            await RenderDropDownAsync(hb, model, ModuleAction.RenderModeEnum.NormalMenu, false);
                            break;
                        case ModuleAction.RenderModeEnum.ButtonMiniDropDown:
                            await RenderDropDownAsync(hb, model, ModuleAction.RenderModeEnum.NormalMenu, true);
                            break;
                        default:
                            await RenderDefaultsAsync(hb, model, renderMode);
                            break;
                    }

                    hb.Append($@"
</div>");
                }
                return hb.ToString();
            }
        }

        private async Task RenderDropDownAsync(HtmlBuilder hb, List<ModuleAction> model, ModuleAction.RenderModeEnum renderMode, bool mini) {
            MenuList menu = new MenuList(model) {
                RenderMode = renderMode
            };
            string buttonId = ControlId + "_btn";
            ActionIconsComponent.ActionIconsSetup setup = new ActionIconsComponent.ActionIconsSetup {
                MenuId = ControlId + "_menu",
            };
            string menuHTML = await MenuDisplayComponent.RenderMenuAsync(menu, setup.MenuId, Globals.CssGridActionMenu, HtmlHelper: HtmlHelper, Hidden: true);

            if (!string.IsNullOrWhiteSpace(menuHTML)) {

                DropDownButtonComponent.Model ddModel = new DropDownButtonComponent.Model {
                    Text = __ResStr("dropdownText", "Manage"),
                    Tooltip = null,
                    ButtonId = buttonId,
                    MenuHTML = menuHTML,
                    Mini = mini,
                };

                hb.Append($@"
<div class='yt_actionicons{(mini ? " t_mini" : "")}' id='{ControlId}'>
    {await HtmlHelper.ForDisplayContainerAsync(ddModel, DropDownButtonComponent.TemplateName)}
</div>");

                Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.ActionIconsComponent('{ControlId}', {Utility.JsonSerialize(setup)});");
            }
        }

        private async Task RenderDefaultsAsync(HtmlBuilder hb, List<ModuleAction> model, ModuleAction.RenderModeEnum renderMode) {
            int firstIndex = 0;
            int lastIndex = model.Count - 1;

            int index = 0;
            foreach (ModuleAction a in model) {
                string css = "t_middle";
                if (index == firstIndex && index == lastIndex) css = "t_firstlast";
                else if (index == firstIndex) css = "t_first";
                else if (index == lastIndex) css = "t_last";
                hb.Append(await a.RenderAsync(renderMode, Css: css));
                ++index;
            }
        }
    }
}
