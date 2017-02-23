/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Languages.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Languages.Controllers {

    public class LanguageEditModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LanguageEditModule> {

        public LanguageEditModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("ID"), Description("The language id - this is the same as the culture name used throughout .NET")]
            [UIHint("String"), ReadOnly]
            public string DisplayId { get; set; }

            [Caption("Name"), Description("The language's name, which is displayed in language selection controls so the user can select a language")]
            [UIHint("Text40"), StringLength(LanguageData.MaxShortName), Required, Trim]
            public string ShortName { get; set; }

            [Caption("Description"), Description("The description for the language - this is used for informational purposes only")]
            [UIHint("Text80"), StringLength(LanguageData.MaxDescription), Required, Trim]
            public string Description { get; set; }

            [UIHint("Hidden")]
            public string Id { get; set; }

            public LanguageData GetData(LanguageData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(LanguageData data) {
                ObjectSupport.CopyData(data, this);
                DisplayId = Id;
            }
            public EditModel() { }
        }

        [HttpGet]
        public ActionResult LanguageEdit(string id) {
            using (LanguageDataProvider dataProvider = new LanguageDataProvider()) {
                EditModel model = new EditModel { };
                LanguageData data = dataProvider.GetItem(id);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Language \"{0}\" not found."), id);
                model.SetData(data);
                Module.Title = this.__ResStr("modTitle", "Language \"{0}\"", data.ShortName);
                return View(model);
            }
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult LanguageEdit_Partial(EditModel model) {
            string originalId = model.Id;

            // get the original item
            using (LanguageDataProvider dataProvider = new LanguageDataProvider()) {
                LanguageData data = dataProvider.GetItem(originalId);
                if (data == null)
                    ModelState.AddModelError("Key", this.__ResStr("alreadyDeleted", "The language with id {0} has been removed and can no longer be updated.", originalId));

                if (!ModelState.IsValid)
                    return PartialView(model);

                // save updated item
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                switch (dataProvider.UpdateItem(originalId, data)) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyDeleted", "The language with id {0} has been removed and can no longer be updated.", originalId));
                        return PartialView(model);
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "An language with id {0} already exists.", model.Id));
                        return PartialView(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Language updated and saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}