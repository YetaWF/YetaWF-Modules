using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class StringDisplayComponent : YetaWFComponent, IYetaWFComponent<string> {

        public const string TemplateName = "String";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {
            HtmlBuilder hb = new HtmlBuilder();

            if (model == null)
                model = "";
            string t = model.ToString();
            if (string.IsNullOrWhiteSpace(t))
                return Task.FromResult(new YHtmlString(""));
            t = YetaWFManager.HtmlEncode(t);

            return Task.FromResult(new YHtmlString(t));
        }
    }
}
