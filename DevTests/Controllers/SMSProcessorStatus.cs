/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Support.SendSMS;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class SMSProcessorStatusModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.SMSProcessorStatusModule> {

        public SMSProcessorStatusModuleController() { }

        public class DisplayModel {
            public int Available { get; set; }
            public string ProcessorName { get; set; } = null!;
            public bool TestMode { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> SMSProcessorStatus() {
            if (!Manager.HasSuperUserRole)
                return new EmptyResult();
            SendSMS.GetSMSProcessorCondInfo info = await SendSMS.GetSMSProcessorCondAsync();
            DisplayModel model = new DisplayModel() {
                Available = info.Count,
                TestMode = info.Processor != null ? await info.Processor.IsTestModeAsync() : false,
                ProcessorName = info.Processor?.Name ?? string.Empty,
            };
            if (model.Available == 1 && !model.TestMode)
                return new EmptyResult();
            return View(model);
        }
    }
}
