/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

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

            public void SetData(LanguageEntryElement data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public ActionResult LanguageDisplay(string id) {
            LanguageEntryElement language = (from l in LanguageSection.Languages where id == l.Id select l).FirstOrDefault();
            if (language == null)
                throw new Error(this.__ResStr("notFound", "Language \"{0}\" not found"), id);
            DisplayModel model = new DisplayModel();
            model.SetData(language);
            Module.Title = this.__ResStr("modTitle", "Language \"{0}\"", language.ShortName);
            return View(model);
        }
    }
}