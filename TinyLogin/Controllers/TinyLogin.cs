/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyLogin#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.TinyLogin.Controllers {

    public class TinyLoginModuleController : ControllerImpl<YetaWF.Modules.TinyLogin.Modules.TinyLoginModule> {

        public TinyLoginModuleController() { }

        [Trim]
        public class TinyLoginModel {

            public string? UserName { get; set; }
            public bool LoggedOn { get; set; }

            public string LogonUrl { get; set; } = null!;
            public string LogoffUrl { get; set; } = null!;
            public string RegisterUrl { get; set; } = null!;
            public string UserUrl { get; set; } = null!;
            public string UserTooltip { get; set; } = null!;

            public TinyLoginModel() { }
        }

        [AllowGet]
        public ActionResult TinyLogin() {
            TinyLoginModel model = new TinyLoginModel {
                UserName = Manager.UserName,
                LoggedOn = Manager.HaveUser,
                LogonUrl = string.IsNullOrWhiteSpace(Module.LogonUrl) ? Manager.CurrentSite.LoginUrl : Module.LogonUrl,
                LogoffUrl = string.IsNullOrWhiteSpace(Module.LogoffUrl) ? Manager.CurrentSite.HomePageUrl : Module.LogoffUrl,
                RegisterUrl = string.IsNullOrWhiteSpace(Module.RegisterUrl) ? Manager.CurrentSite.LoginUrl : Module.RegisterUrl,
                UserUrl = string.IsNullOrWhiteSpace(Module.UserUrl) ? Manager.CurrentSite.HomePageUrl : Module.UserUrl,
                UserTooltip = Module.UserTooltip,
            };
            return View(model);
        }
    }
}