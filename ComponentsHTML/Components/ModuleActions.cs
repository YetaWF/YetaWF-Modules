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

        internal class ModuleActionsSetup {
            public string MenuId { get; set; } = null!;
        }

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(DropDownButtonComponent.TemplateName, YetaWFComponentBase.ComponentType.Display);
            await base.IncludeAsync();
        }

        /// <inheritdoc/>
        public async Task<string> RenderAsync(List<ModuleAction> model) {

            ModuleAction.RenderModeEnum renderMode = PropData.GetAdditionalAttributeValue<ModuleAction.RenderModeEnum>("RenderAs", ModuleAction.RenderModeEnum.Button);

            return await RenderAsync(this, model, renderMode);
        }

        internal static async Task<string> RenderAsync(YetaWFComponent component, List<ModuleAction> model, ModuleAction.RenderModeEnum renderMode) {

            if (model != null && model.Count > 0) {
                switch (renderMode) {
                    case ModuleAction.RenderModeEnum.ButtonDropDown:
                        return await RenderDropDownAsync(component, model, ModuleAction.RenderModeEnum.NormalMenu, false);
                    case ModuleAction.RenderModeEnum.ButtonMiniDropDown:
                        return await RenderDropDownAsync(component, model, ModuleAction.RenderModeEnum.NormalMenu, true);
                    default:
                        return await RenderModuleActionsAsync(model, renderMode);
                }
            }
            return string.Empty;
        }

        private static async Task<string> RenderDropDownAsync(YetaWFComponent component, List<ModuleAction> model, ModuleAction.RenderModeEnum renderMode, bool mini) {

            HtmlBuilder hb = new HtmlBuilder();

            MenuList menu = new MenuList(model) {
                RenderMode = renderMode
            };
            string buttonId = component.ControlId + "_btn";
            ModuleActionsSetup setup = new ModuleActionsSetup {
                MenuId = component.ControlId + "_menu",
            };
            string menuHTML = await MenuDisplayComponent.RenderMenuAsync(menu, setup.MenuId, Globals.CssGridActionMenu, HtmlHelper: component.HtmlHelper, Hidden: true);

            if (!string.IsNullOrWhiteSpace(menuHTML)) {

                DropDownButtonComponent.Model ddModel = new DropDownButtonComponent.Model {
                    Text = __ResStr("dropdownText", "Manage"),
                    Tooltip = null,
                    ButtonId = buttonId,
                    MenuHTML = menuHTML,
                    Mini = mini,
                };

                hb.Append($@"
<div class='yt_moduleactions{(mini ? " t_mini" : "")}' id='{component.ControlId}'>
    {await component.HtmlHelper.ForDisplayContainerAsync(ddModel, DropDownButtonComponent.TemplateName)}
</div>");

                Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.ModuleActionsComponent('{component.ControlId}', {Utility.JsonSerialize(setup)});");
            }

            return hb.ToString();
        }

        internal static async Task<string> RenderModuleActionsAsync(List<ModuleAction> model, ModuleAction.RenderModeEnum renderMode) {

            HtmlBuilder hb = new HtmlBuilder();
            if (model.Count > 0) {

                hb.Append($@"
<div class='yt_moduleactions t_display'>");

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

                hb.Append($@"
</div>");
            }
            return hb.ToString();
        }
    }
}
