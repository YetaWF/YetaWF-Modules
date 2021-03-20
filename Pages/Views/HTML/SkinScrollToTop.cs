/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Pages.Controllers;
using YetaWF.Modules.Pages.Modules;

namespace YetaWF.Modules.Pages.Views {

    public class SkinScrollToTopView : YetaWFView, IYetaWFView<SkinScrollToTopModule, SkinScrollToTopModuleController.DisplayModel> {

        public const string ViewName = "SkinScrollToTop";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(SkinScrollToTopModule module, SkinScrollToTopModuleController.DisplayModel model) {

            Manager.ScriptManager.AddLast($@"YetaWF_Pages.ScrollUp.init();");

            return Task.FromResult<string>(string.Empty);
        }
    }
}
