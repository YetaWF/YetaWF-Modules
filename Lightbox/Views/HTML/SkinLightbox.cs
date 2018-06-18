using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Lightbox.Controllers;
using YetaWF.Modules.Lightbox.Modules;

namespace YetaWF.Modules.Lightbox.Views {

    public class SkinLightboxView : YetaWFView, IYetaWFView<SkinLightboxModule, SkinLightboxModuleController.Model> {

        public const string ViewName = "SkinLightbox";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<YHtmlString> RenderViewAsync(SkinLightboxModule module, SkinLightboxModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
