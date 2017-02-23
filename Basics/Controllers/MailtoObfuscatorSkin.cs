/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

using YetaWF.Core.Controllers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Basics.Controllers {

    public class MailtoObfuscatorSkinModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.MailtoObfuscatorSkinModule> {

        public MailtoObfuscatorSkinModuleController() { }

        [HttpGet]
        public ActionResult MailtoObfuscatorSkin(string mailtoObfuscator) {
            YetaWF.Core.Packages.Package package = YetaWF.Modules.Basics.Controllers.AreaRegistration.CurrentPackage;
            Manager.AddOnManager.AddAddOn(package.Domain, package.Product, "MailToObfuscator");
            return new EmptyResult();
        }
    }
}