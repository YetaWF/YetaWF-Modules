/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEar#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.PageEar.Controllers;
using YetaWF.Modules.PageEar.Modules;

namespace YetaWF.Modules.PageEar.Views {

    public class EditView : YetaWFView, IYetaWFView<PageEarModule, PageEarModuleController.Model> {

        public const string ViewName = "PageEar";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public class Setup {
            public string AdImage { get; set; } = null!;
            public string PeelImage { get; set; } = null!;
            public string ClickURL { get; set; } = null!;
            public int SmallSize { get; set; }
            public int LargeSize { get; set; }
            public bool AutoAnimate { get; set; }
            public bool Debug { get; set; }
        }

        public async Task<string> RenderViewAsync(PageEarModule module, PageEarModuleController.Model model) {

            await Manager.AddOnManager.AddAddOnNamedAsync(module.AreaName, ViewName);

            HtmlBuilder hb = new HtmlBuilder();

            if (Manager.EditMode) {
                hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {HE(this.__ResStr("changeMod", "Change \"Module Settings\" to configure the page ear displayed"))}
</div>");
            } else {
                hb.Append("&nbsp;");// generate something otherwise module suppressed
            }

            if (module.AdImage_Data.Length > 0 && module.CoverImage_Data.Length > 0 && !string.IsNullOrWhiteSpace(module.ClickUrl)) {

                Setup setup = new Setup {
                    AdImage = ImageHTML.FormatUrl(YetaWF.Core.Modules.ModuleImageSupport.ImageType, null, module.AdImage),
                    PeelImage = ImageHTML.FormatUrl(YetaWF.Core.Modules.ModuleImageSupport.ImageType, null, module.CoverImage),
                    ClickURL = module.ClickUrl,
                    SmallSize = module.SmallSize,
                    LargeSize = module.LargeSize,
                    AutoAnimate = module.Animate,
                    Debug = false,
                };
                Manager.ScriptManager.AddLast($@"new YetaWF_PageEar.PageEarModule('{module.ModuleHtmlId}', {Utility.JsonSerialize(setup)});");
            }
            return hb.ToString();
        }
    }
}
