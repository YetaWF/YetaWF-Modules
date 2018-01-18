/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

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
            public string ProcessorName { get; set; }
            public bool TestMode { get; set; }
        }

        [AllowGet]
        public ActionResult SMSProcessorStatus() {
            if (!Manager.HasSuperUserRole)
                return new EmptyResult();
            int count;
            ISendSMS processor = SendSMS.GetSMSProcessorCond(out count);
            DisplayModel model = new DisplayModel() {
                Available = count,
                TestMode = processor != null ? processor.IsTestMode() : false,
                ProcessorName = processor != null ? processor.Name : null,
            };
            if (model.Available == 1 && !model.TestMode)
                return new EmptyResult();
            return View(model);
        }
    }
}
