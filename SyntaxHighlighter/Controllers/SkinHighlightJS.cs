/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Packages;
using YetaWF.Modules.SyntaxHighlighter.DataProvider;
using YetaWF.Modules.SyntaxHighlighter.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.SyntaxHighlighter.Controllers {

    public class SkinHighlightJSModuleController : ControllerImpl<YetaWF.Modules.SyntaxHighlighter.Modules.SkinHighlightJSModule> {

        public SkinHighlightJSModuleController() { }

        [AllowGet]
        public ActionResult SkinHighlightJS() {
            ConfigData config = ConfigDataProvider.GetConfig();

            // find theme specific skin
            Package package = AreaRegistration.CurrentPackage;
            SkinAccess skinAccess = new SkinAccess();
            string theme = skinAccess.FindHighlightJSSkin(config.HighlightJSSkin);
            Manager.AddOnManager.AddAddOnNamed(package.Domain, package.Product, "SkinHighlightJS", theme);

            // add client-side init
            Manager.ScriptManager.AddLast("YetaWF_SyntaxHighlighter_HighlightJS", "YetaWF_SyntaxHighlighter_HighlightJS.Init();");

            return new EmptyResult();
        }
    }
}