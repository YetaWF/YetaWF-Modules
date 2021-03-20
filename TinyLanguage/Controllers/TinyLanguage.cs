/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyLanguage#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Language;
using YetaWF.Core.Models.Attributes;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.TinyLanguage.Controllers {

    public class TinyLanguageModuleController : ControllerImpl<YetaWF.Modules.TinyLanguage.Modules.TinyLanguageModule> {

        public TinyLanguageModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Language"), Description("Select the language to be used for the entire site")]
            [UIHint("LanguageId"), StringLength(LanguageData.MaxId), SubmitFormOnChange]
            public string? LanguageId { get; set; }

            public EditModel() { }
        }

        [AllowGet]
        public ActionResult TinyLanguage() {
            EditModel model = new EditModel { LanguageId = Manager.UserLanguage };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> TinyLanguage_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            await Manager.SetUserLanguageAsync(model.LanguageId!);
            return Redirect(Manager.ReturnToUrl, ForceRedirect: true);
        }
    }
}