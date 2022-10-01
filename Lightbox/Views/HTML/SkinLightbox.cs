/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Lightbox#License */

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

        public Task<string> RenderViewAsync(SkinLightboxModule module, SkinLightboxModuleController.Model model) {

            HtmlBuilder hb = new();
            hb.Append("<!-- A comment so we generate something, otherwise js/css is not included -->");
            return Task.FromResult(hb.ToString());
        }
    }
}
