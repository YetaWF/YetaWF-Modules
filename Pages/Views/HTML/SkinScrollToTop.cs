/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

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

        public Task<YHtmlString> RenderViewAsync(SkinScrollToTopModule module, SkinScrollToTopModuleController.DisplayModel model) {

            Manager.ScriptManager.AddLast($@"
YetaWF_Pages_ScrollUp.init();");

            return Task.FromResult(new YHtmlString());
        }
    }
}
