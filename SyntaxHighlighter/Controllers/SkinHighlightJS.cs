/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Packages;
using YetaWF.Modules.SyntaxHighlighter.DataProvider;

namespace YetaWF.Modules.SyntaxHighlighter.Controllers {

    public class SkinHighlightJSModuleController : ControllerImpl<YetaWF.Modules.SyntaxHighlighter.Modules.SkinHighlightJSModule> {

        public SkinHighlightJSModuleController() { }

        [AllowGet]
        public async Task<ActionResult> SkinHighlightJS() {
            ConfigData config = await ConfigDataProvider.GetConfigAsync();

            Package package = AreaRegistration.CurrentPackage;
            await Manager.AddOnManager.AddAddOnNamedAsync(package.AreaName, "SkinHighlightJS", config.HighlightJSSkin);

            return new EmptyResult();
        }
    }
}