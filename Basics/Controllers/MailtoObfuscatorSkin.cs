/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.Basics.Controllers {

    public class MailtoObfuscatorSkinModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.MailtoObfuscatorSkinModule> {

        public MailtoObfuscatorSkinModuleController() { }

        [HttpGet]
        public ActionResult MailtoObfuscatorSkin(string mailtoObfuscator) {
            return new EmptyResult();
        }
    }
}