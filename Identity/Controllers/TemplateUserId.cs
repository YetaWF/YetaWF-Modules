/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using YetaWF.Core.Support;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class TemplateUserIdModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.TemplateUserIdModule> {

        public TemplateUserIdModuleController() { }

        [Trim]
        [Header("Test case for the YetaWF_Identity_UserId template, which allows selection of 1 user. " +
            "If fewer than 50 users are available, a dropdown list is shown. " +
            "For more than 50, a scrollable grid (Ajax) is used instead to support thousands of users. " +
            "Some of these fields explicitly force a grid display even with fewer than 50 users.")]
        public class Model {

            [Caption("UserId (Required)"), Description("UserId (Required)")]
            [UIHint("YetaWF_Identity_UserId"), AdditionalMetadata("Force", "DropDown"), SelectionRequired, Trim]
            public int Prop1Req { get; set; }

            [Caption("UserId (Grid, Required)"), Description("UserId (Required)")]
            [UIHint("YetaWF_Identity_UserId"), AdditionalMetadata("Force", "Grid"), SelectionRequired, Trim]
            public int Prop1GridReq { get; set; }

            [Caption("UserId"), Description("UserId")]
            [UIHint("YetaWF_Identity_UserId"), AdditionalMetadata("Force", "DropDown"), Trim]
            public int Prop1 { get; set; }

            [Caption("UserId (Grid)"), Description("UserId")]
            [UIHint("YetaWF_Identity_UserId"), AdditionalMetadata("Force", "Grid"), Trim]
            public int Prop1Grid { get; set; }

            [Caption("UserId (Read/Only)"), Description("UserId (read/only)")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int Prop1RO { get; set; }

            public Model() { }
        }

        [AllowGet]
        public ActionResult TemplateUserId() {
            Model model = new Model {
                Prop1Req = Manager.UserId,
                Prop1GridReq = Manager.UserId,
                Prop1 = Manager.UserId,
                Prop1Grid = Manager.UserId,
                Prop1RO = Manager.UserId,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateUserId_Partial(Model model) {
            model.Prop1RO = Manager.UserId;
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
