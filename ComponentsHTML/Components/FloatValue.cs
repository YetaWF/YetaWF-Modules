using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class FloatValueComponentBase : YetaWFComponent {

        public const string TemplateName = "FloatValue";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class FloatValueDisplayComponent : FloatValueComponentBase, IYetaWFComponent<Single?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(Single? model) {

            HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                if ((Single)model != 0.0 || !PropData.GetAdditionalAttributeValue("EmptyIf0", false)) {
                    hb.Append(model.ToString());
                }
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
