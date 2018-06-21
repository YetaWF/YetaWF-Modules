/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class ModuleActionComponent : YetaWFComponent, IYetaWFComponent<ModuleAction> {

        public const string TemplateName = "ModuleAction";

        public override ComponentType GetComponentType() { return ComponentType.Display; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(ModuleAction model) {

            using (Manager.StartNestedComponent(FieldName)) {

                HtmlBuilder hb = new HtmlBuilder();

                ModuleAction.RenderModeEnum render = PropData.GetAdditionalAttributeValue("RenderAs", ModuleAction.RenderModeEnum.Button);
                if (model != null) {
                    hb.Append($@"
<div class='yt_moduleaction t_display'>
    {await model.RenderAsync(render)}
</div>");
                }
                return hb.ToYHtmlString();
            }
        }
    }
}
