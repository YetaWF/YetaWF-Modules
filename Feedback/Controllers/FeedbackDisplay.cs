/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System.Threading.Tasks;
using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Feedback.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Feedback.Controllers {

    public class FeedbackDisplayModuleController : ControllerImpl<YetaWF.Modules.Feedback.Modules.FeedbackDisplayModule> {

        public FeedbackDisplayModuleController() { }

        public class DisplayModel {

            [Caption("Created"), Description("The date the message was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Name"), Description("The user's name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Email Address"), Description("The user's email address")]
            [UIHint("String"), ReadOnly]
            public string Email { get; set; }

            [Caption("Subject"), Description("The subject of the message")]
            [UIHint("String"), ReadOnly]
            public string Subject { get; set; }

            [Caption("IP Address"), Description("The IP address from which the feedback message was sent")]
            [UIHint("IPAddress"), ReadOnly]
            public string IPAddress { get; set; }

            [Caption("Message"), Description("The feedback message")]
            [UIHint("TextArea"), ReadOnly]
            public string Message { get; set; }

            public void SetData(FeedbackData data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public async Task<ActionResult> FeedbackDisplay(int key) {
            using (FeedbackDataProvider dataProvider = new FeedbackDataProvider()) {
                FeedbackData data = await dataProvider.GetItemAsync(key);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Feedback \"{0}\" not found."), key);
                DisplayModel model = new DisplayModel();
                model.SetData(data);
                return View(model);
            }
        }
    }
}