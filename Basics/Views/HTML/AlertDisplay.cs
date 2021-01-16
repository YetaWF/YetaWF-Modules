/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Basics.Controllers;
using YetaWF.Modules.Basics.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Basics.Views {

    public class AlertDisplayView : YetaWFView, IYetaWFView<AlertDisplayModule, AlertDisplayModuleController.DisplayModel> {

        public const string ViewName = "AlertDisplay";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(AlertDisplayModule module, AlertDisplayModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            string rootUrl = VersionManager.GetAddOnPackageUrl(Package.AreaName);
            string closeUrl = Manager.GetCDNUrl(System.IO.Path.Combine(rootUrl, "Icons", "Close.png"));

            if (model.MessageHandling == DataProvider.AlertConfig.MessageHandlingEnum.DisplayUntilOff) {

                string ajaxUrl = Utility.UrlFor(typeof(AlertDisplayModuleController), nameof(AlertDisplayModuleController.Off), new { __ModuleGuid = module.ModuleGuid });

                // icon used fas-multiply
                hb.Append($@"
    <div class='t_close' data-ajaxurl='{Utility.HAE(ajaxUrl)}'>
        <a class='y_button_outline y_button' {YetaWF.Core.Addons.Basics.CssTooltip}='{HAE(this.__ResStr("clsButtonTT", "Click to close alert"))}' href='#' rel='nofollow' data-button='' data-save-return=''>
            <svg aria-hidden='true' focusable='false' role='img' xmlns='http://www.w3.org/2000/svg' viewBox='0 0 352 512'>
                <path fill='currentColor' d='M242.72 256l100.07-100.07c12.28-12.28 12.28-32.19 0-44.48l-22.24-22.24c-12.28-12.28-32.19-12.28-44.48 0L176 189.28 75.93 89.21c-12.28-12.28-32.19-12.28-44.48 0L9.21 111.45c-12.28 12.28-12.28 32.19 0 44.48L109.28 256 9.21 356.07c-12.28 12.28-12.28 32.19 0 44.48l22.24 22.24c12.28 12.28 32.2 12.28 44.48 0L176 322.72l100.07 100.07c12.28 12.28 32.2 12.28 44.48 0l22.24-22.24c12.28-12.28 12.28-32.19 0-44.48L242.72 256z'></path>
            </svg>
        </a>
    </div>");

            }
            hb.Append($@"
<div class='t_message'>
    {model.Message}
</div>");

            return Task.FromResult(hb.ToString());
        }
    }
}
