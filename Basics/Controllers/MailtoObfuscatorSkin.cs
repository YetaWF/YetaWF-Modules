/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using YetaWF.Core.Controllers;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Basics.Controllers {

    public class MailtoObfuscatorSkinModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.MailtoObfuscatorSkinModule> {

        public MailtoObfuscatorSkinModuleController() { }

        [AllowGet]
        public async Task<ActionResult> MailtoObfuscatorSkin(string mailtoObfuscator) {
            YetaWF.Core.Packages.Package package = YetaWF.Modules.Basics.AreaRegistration.CurrentPackage;
            await Manager.AddOnManager.AddAddOnNamedAsync(package.AreaName, "MailToObfuscator");
            return new EmptyResult();
        }
    }
}