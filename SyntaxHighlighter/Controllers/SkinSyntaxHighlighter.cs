/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using YetaWF.Core.Controllers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.SyntaxHighlighter.Controllers {

    public class SkinSyntaxHighlighterModuleController : ControllerImpl<YetaWF.Modules.SyntaxHighlighter.Modules.SkinSyntaxHighlighterModule> {

        public SkinSyntaxHighlighterModuleController() { }

        public class Model { }

        [AllowGet]
        public ActionResult SkinSyntaxHighlighter() {
            Model model = new Model();
            return View(model);
        }
    }
}