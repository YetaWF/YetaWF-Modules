/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

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

                string ajaxUrl = YetaWFManager.UrlFor(typeof(AlertDisplayModuleController), nameof(AlertDisplayModuleController.Off), new { __ModuleGuid = module.ModuleGuid });

                hb.Append($@"
    <div class='t_close' data-ajaxurl='{YetaWFManager.HtmlAttributeEncode(ajaxUrl)}'>
        <img src='{YetaWFManager.HtmlAttributeEncode(closeUrl)}' alt='{HAE(this.__ResStr("clsButtonAlt", "Close Button"))}' title='{HAE(this.__ResStr("clsButtonTT", "Click to close alert"))}' />
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
