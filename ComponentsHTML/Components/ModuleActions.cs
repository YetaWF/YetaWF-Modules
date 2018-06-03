using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class ModuleActionsComponent : YetaWFComponent, IYetaWFComponent<List<ModuleAction>> {

        public const string TemplateName = "ModuleActions";

        public override ComponentType GetComponentType() { return ComponentType.Display; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(List<ModuleAction> model) {

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
                return hb.ToYHtmlString();
            }
        }
    }
}
