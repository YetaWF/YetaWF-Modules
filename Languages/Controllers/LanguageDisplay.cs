/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Languages.DataProvider;

namespace YetaWF.Modules.Languages.Controllers {

    public class LanguageDisplayModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LanguageDisplayModule> {

        public LanguageDisplayModuleController() { }

        public class DisplayModel {

            [Caption("ID"), Description("The language id - this is the same as the culture name used throughout .NET")]
            [UIHint("String"), ReadOnly]
            public string Id { get; set; }

            [Caption("Name"), Description("The language's name, which is displayed in language selection controls so the user can select a language")]
            [UIHint("String"), ReadOnly]
            public string ShortName { get; set; }

            [Caption("Description"), Description("The description for the language - this is used for informational purposes only")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            public void SetData(LanguageData data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [HttpGet]
        public ActionResult LanguageDisplay(string id) {
            using (LanguageDataProvider dataProvider = new LanguageDataProvider()) {
                LanguageData data = dataProvider.GetItem(id);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Language \"{0}\" not found."), id);
                DisplayModel model = new DisplayModel();
                model.SetData(data);
                Module.Title = this.__ResStr("modTitle", "Language \"{0}\"", data.ShortName);
                return View(model);
            }
        }
    }
}