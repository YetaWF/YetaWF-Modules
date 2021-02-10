/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
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

        internal const string TemplateName = "ModuleActions";

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateFromUIHintAsync("ActionIcons", YetaWFComponentBase.ComponentType.Display);// this is needed because we're not used by templates
            await base.IncludeAsync();
        }

        /// <inheritdoc/>
        public async Task<string> RenderAsync(List<ModuleAction> model) {

            using (Manager.StartNestedComponent(FieldName)) {

                HtmlBuilder hb = new HtmlBuilder();

                ModuleAction.RenderModeEnum render = PropData.GetAdditionalAttributeValue<ModuleAction.RenderModeEnum>("RenderAs", ModuleAction.RenderModeEnum.Button);

                if (model != null && model.Count > 0) {

                    hb.Append($@"
<div class='yt_moduleactions t_display'>");

                    foreach (ModuleAction a in model) {
                        hb.Append($@"
    { await a.RenderAsync(render) }");
                    }
                    hb.Append($@"
</div>");
                }
                return hb.ToString();
            }
        }
    }
}
