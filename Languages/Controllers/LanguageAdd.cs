/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Languages.DataProvider;

namespace YetaWF.Modules.Languages.Controllers {
    public class LanguageAddModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LanguageAddModule> {

        public LanguageAddModuleController() { }

        [Trim]
        public class AddModel {
            [Caption("Id"), Description("The language id - this is the same as the culture name used throughout .NET")]
            [UIHint("LanguageId"), StringLength(LanguageData.MaxId), AdditionalMetadata("NoDefault", true), AdditionalMetadata("AllLanguages", true), Required, Trim]
            public string Id { get; set; }

            [Caption("Name"), Description("The language's name, which is displayed in language selection controls so the user can select a language")]
            [UIHint("Text40"), StringLength(LanguageData.MaxShortName), Required, Trim]
            public string ShortName { get; set; }

            [Caption("Description"), Description("The description for the language - this is used for informational purposes only")]
            [UIHint("Text80"), StringLength(LanguageData.MaxDescription), Required, Trim]
            public string Description { get; set; }

            public AddModel() { }

            public LanguageData GetData() {
                LanguageData data = new LanguageData();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [HttpGet]
        public ActionResult LanguageAdd() {
            AddModel model = new AddModel { };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult LanguageAdd_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            using (LanguageDataProvider dataProvider = new LanguageDataProvider()) {
                if (!dataProvider.AddItem(model.GetData()))
                    throw new Error(this.__ResStr("alreadyExists", "A language with id {0} already exists."), model.Id);
                return FormProcessed(model, this.__ResStr("okSaved", "New language \"{0}\" saved", model.ShortName), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
