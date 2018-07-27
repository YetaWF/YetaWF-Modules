/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Modules.SyntaxHighlighter.DataProvider;
using YetaWF.Core.Packages;
using YetaWF.Core.Addons;
using YetaWF.Modules.SyntaxHighlighter.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.SyntaxHighlighter.Controllers {

    public class SkinSyntaxHighlighterModuleController : ControllerImpl<YetaWF.Modules.SyntaxHighlighter.Modules.SkinSyntaxHighlighterModule> {

        public SkinSyntaxHighlighterModuleController() { }

        [AllowGet]
        public async Task<ActionResult> SkinSyntaxHighlighter() {
            ConfigData config = await ConfigDataProvider.GetConfigAsync();

            // find theme specific skin
            Package package = AreaRegistration.CurrentPackage;
            SkinAccess skinAccess = new SkinAccess();
            string theme = skinAccess.FindSyntaxHighlighterSkin(config.SyntaxHighlighterSkin);
            await Manager.AddOnManager.AddAddOnNamedAsync(package.AreaName, "SkinSyntaxHighlighter", theme);

            // add client-side init
            string url = VersionManager.GetAddOnNamedUrl(package.AreaName, "SkinSyntaxHighlighter");
            Manager.ScriptManager.AddLast("AlexGorbatchevCom_SyntaxHighlighter", "YetaWF_SyntaxHighlighter.AlexGorbatchevCom.init('" + url + "');");

            return new EmptyResult();
        }
    }
}