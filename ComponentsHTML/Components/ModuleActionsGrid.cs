/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Renders the model as module actions (buttons, icons or links) for display within a grid. If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Archive"), Description("Monthly blog entries")]
    /// [UIHint("ModuleActionsGrid"), ReadOnly]
    /// public List&lt;ModuleAction&gt; Actions { get; set; }
    /// </example>
    [UsesAdditional("RenderAs", "YetaWF.Core.Modules.ModuleAction.RenderModeEnum", "ModuleAction.RenderModeEnum.Button", "Defines how the module actions are rendered.")]
    public class ModuleActionsGridComponent : YetaWFComponent, IYetaWFComponent<List<ModuleAction>?> {

        internal const string TemplateName = "ModuleActionsGrid";

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }
        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, ModuleActionsComponent.TemplateName, YetaWFComponentBase.ComponentType.Display);
            await base.IncludeAsync();
        }

        /// <inheritdoc/>
        public async Task<string> RenderAsync(List<ModuleAction>? model) {

            if (model == null || model.Count == 0)
                return string.Empty;

            Grid.GridActionsEnum actionStyle = PropData.GetAdditionalAttributeValue<Grid.GridActionsEnum>("GridActionsEnum", UserSettings.GetProperty<Grid.GridActionsEnum>("GridActions"));

            if (model.Count == 1 && actionStyle == Grid.GridActionsEnum.DropdownMenu)
                actionStyle = Grid.GridActionsEnum.Icons;

            ModuleAction.RenderModeEnum renderMode;
            switch (actionStyle) {
                default:
                case Grid.GridActionsEnum.Icons:
                    renderMode = ModuleAction.RenderModeEnum.IconsOnly;
                    break;
                case Grid.GridActionsEnum.DropdownMenu:
                    renderMode = ModuleAction.RenderModeEnum.ButtonDropDown;
                    break;
                case Grid.GridActionsEnum.Mini:
                    renderMode = ModuleAction.RenderModeEnum.ButtonMiniDropDown;
                    break;
                case Grid.GridActionsEnum.ButtonBar:
                    renderMode = ModuleAction.RenderModeEnum.ButtonBar;
                    break;
            }

            return await ModuleActionsComponent.RenderAsync(this, model, renderMode);
        }
    }
}
