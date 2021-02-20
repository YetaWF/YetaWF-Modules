/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Packages;
using YetaWF.Modules.SyntaxHighlighter.DataProvider;
using YetaWF.Modules.SyntaxHighlighter.Support;

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
            string url = Package.GetAddOnNamedUrl(package.AreaName, "SkinSyntaxHighlighter");
            Manager.ScriptManager.AddLast("AlexGorbatchevCom_SyntaxHighlighter", "YetaWF_SyntaxHighlighter.AlexGorbatchevCom.init('" + url + "');");

            return new EmptyResult();
        }
    }
}