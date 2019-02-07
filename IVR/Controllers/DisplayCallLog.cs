/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class DisplayCallLogModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.DisplayCallLogModule> {

        public DisplayCallLogModuleController() { }

        public class DisplayModel {

            [Caption("Created"), Description("The date/time the call was received")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("From"), Description("The caller's phone number")]
            [UIHint("Softelvdm_IVR_PhoneNumber"), ReadOnly]
            public string Caller { get; set; }
            [Caption("From City"), Description("The caller's city (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerCity { get; set; }
            [Caption("From State"), Description("The caller's state (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerState { get; set; }
            [Caption("From Zip Code"), Description("The caller's ZIP code (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerZip { get; set; }
            [Caption("From Country"), Description("The caller's country (if available)")]
            [UIHint("String"), ReadOnly]
            public string CallerCountry { get; set; }

            [Caption("Phone Number"), Description("The phone number called")]
            [UIHint("Softelvdm_IVR_PhoneNumber"), ReadOnly]
            public string To { get; set; }

            [Caption("Id"), Description("The internal id")]
            [UIHint("IntValue"), ReadOnly]
            public int Id { get; set; }

            public void SetData(CallLogEntry data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public async Task<ActionResult> DisplayCallLog(int id) {
            using (CallLogDataProvider dataProvider = new CallLogDataProvider()) {
                CallLogEntry data = await dataProvider.GetItemByIdentityAsync(id);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Call Log Entry with id {0} not found"), id);
                DisplayModel model = new DisplayModel();
                model.SetData(data);
                return View(model);
            }
        }
    }
}
