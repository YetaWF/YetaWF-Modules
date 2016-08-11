/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Support.SendSMS;

namespace YetaWF.Modules.DevTests.Controllers {

    public class SMSProcessorStatusModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.SMSProcessorStatusModule> {

        public SMSProcessorStatusModuleController() { }

        public class DisplayModel {
            public int Available { get; set; }
            public string ProcessorName { get; set; }
            public bool TestMode { get; set; }
        }

        [HttpGet]
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
            return View(model);
        }
    }
}
