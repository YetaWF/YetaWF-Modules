/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.DevTests.Controllers;
using YetaWF.Modules.DevTests.Modules;

namespace YetaWF.Modules.DevTests.Views {

    public class SMSProcessorStatusView : YetaWFView, IYetaWFView<SMSProcessorStatusModule, SMSProcessorStatusModuleController.DisplayModel> {

        public const string ViewName = "SMSProcessorStatus";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(SMSProcessorStatusModule module, SMSProcessorStatusModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model.Available > 0) {
                if (model.Available > 1) {

                    hb.Append($@"
<div class='{Globals.CssDivAlert}'>
    {Utility.HtmlEncode(this.__ResStr("multiMode", "There are multiple active SMS processors - there should only be one."))}
</div>");

                }
                if (model.TestMode) {

                    hb.Append($@"
<div class='{Globals.CssDivAlert}'>
    {Utility.HtmlEncode(this.__ResStr("testMode", "The {0} SMS processor is in TEST/Sandbox mode.", model.ProcessorName))}
</div>");

                }
            } else {

                hb.Append($@"
<div class='{Globals.CssDivAlert}'>
    {Utility.HtmlEncode(this.__ResStr("noMode", "There is no active SMS processor - It is still possible to send a message to an email address, instead of a phone number."))}
</div>");
            }

            return Task.FromResult(hb.ToString());
        }
    }
}
