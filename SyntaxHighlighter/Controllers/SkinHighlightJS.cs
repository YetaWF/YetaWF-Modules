/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Threading.Tasks;
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
        public async Task<ActionResult> SkinHighlightJS() {
            ConfigData config = await ConfigDataProvider.GetConfigAsync();

            // find theme specific skin
            Package package = AreaRegistration.CurrentPackage;
            SkinAccess skinAccess = new SkinAccess();
            string theme = skinAccess.FindHighlightJSSkin(config.HighlightJSSkin);
            await Manager.AddOnManager.AddAddOnNamedAsync(package.AreaName, "SkinHighlightJS", theme);

            // add client-side init
            Manager.ScriptManager.AddLast("YetaWF_SyntaxHighlighter_HighlightJS", "YetaWF_SyntaxHighlighter.HighlightJS.init();");

            return new EmptyResult();
        }
    }
}