/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Pages.Controllers;
using YetaWF.Modules.Pages.Modules;

namespace YetaWF.Modules.Pages.Views {

    public class SkinScrollToTopView : YetaWFView, IYetaWFView<SkinScrollToTopModule, SkinScrollToTopModuleController.DisplayModel> {

        public const string ViewName = "SkinScrollToTop";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(SkinScrollToTopModule module, SkinScrollToTopModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, ViewName);

            hb.Append($@"
<script>
    YetaWF_Pages_ScrollUp.init();
</script>");
            return hb.ToYHtmlString();
        }
    }
}
