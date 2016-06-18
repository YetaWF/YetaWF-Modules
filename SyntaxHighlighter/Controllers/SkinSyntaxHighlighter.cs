/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.SyntaxHighlighter.Controllers {

    public class SkinSyntaxHighlighterModuleController : ControllerImpl<YetaWF.Modules.SyntaxHighlighter.Modules.SkinSyntaxHighlighterModule> {

        public SkinSyntaxHighlighterModuleController() { }

        public class Model { }

        //[HttpGet, HttpPost] used during views and partial views
        public ActionResult SkinSyntaxHighlighter() {
            Model model = new Model();
            return View(model);
        }
    }
}