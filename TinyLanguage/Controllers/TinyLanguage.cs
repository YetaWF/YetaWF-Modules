/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/TinyLanguage#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Language;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.TinyLanguage.Controllers {

    public class TinyLanguageModuleController : ControllerImpl<YetaWF.Modules.TinyLanguage.Modules.TinyLanguageModule> {

        public TinyLanguageModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Language"), Description("Select the language to be used for the entire site")]
            [UIHint("LanguageId"), StringLength(LanguageData.MaxId), SubmitFormOnChange]
            public string LanguageId { get; set; }

            public EditModel() { }
        }

        [HttpGet]
        public ActionResult TinyLanguage() {
            EditModel model = new EditModel { LanguageId = Manager.UserLanguage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TinyLanguage_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            Manager.SetUserLanguage(model.LanguageId);
            return Redirect(Manager.ReturnToUrl);
        }
    }
}