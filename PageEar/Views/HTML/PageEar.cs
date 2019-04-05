/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEar#License */

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

        public Task<string> RenderViewAsync(PageEarModule module, PageEarModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (Manager.EditMode) {
                hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {HE(this.__ResStr("changeMod", "Change \"Module Settings\" to configure the page ear displayed"))}
</div>");
            }

            if (module.AdImage_Data.Length > 0 && module.CoverImage_Data.Length > 0 && !string.IsNullOrWhiteSpace(module.ClickUrl)) {

                Manager.ScriptManager.AddLast($@"
$('body').peelback({{
    adImage: '{JE(ImageHTML.FormatUrl(YetaWF.Core.Modules.ModuleImageSupport.ImageType, null, module.AdImage))}',
    peelImage: '{JE(ImageHTML.FormatUrl(YetaWF.Core.Modules.ModuleImageSupport.ImageType, null, module.CoverImage))}',
    clickURL: '{JE(module.ClickUrl)}',
    smallSize: {module.SmallSize},
    bigSize: {module.LargeSize},
    autoAnimate: {(module.Animate ? "true" : "false")},
    //gaTrack: true, //RFFU
    //gaLabel: '#1 Stegosaurus',
    debug: false
}});

// Listen for events that the page is changing
$YetaWF.registerPageChange(function () {{
    // when the page is removed, we need to clean up
    $('#peelback').remove();
}});");

            }
            return Task.FromResult(hb.ToString());
        }
    }
}
