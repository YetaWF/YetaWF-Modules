/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Search.Addons;

namespace YetaWF.Modules.Search.Controllers {

    public class SearchControlModuleController : ControllerImpl<YetaWF.Modules.Search.Modules.SearchControlModule> {

        public SearchControlModuleController() { }

        public class Model {

            [UIHint("Boolean")]
            public bool HighlightSearch { get; set; }

            [UIHint("Hidden")]
            public string SearchTerms { get; set; }

            [UIHint("ModuleAction")]
            public ModuleAction On { get; set; }
            [UIHint("ModuleAction")]
            public ModuleAction Off { get; set; }
        }

        [HttpGet]
        public ActionResult SearchControl() {
            if (Manager.EditMode)
                return new EmptyResult();
            string searchTerms = HttpContext.Request[Info.UrlArg];
            if (searchTerms == null)
                return new EmptyResult();
            List<string> kwds = searchTerms.Split(new char[] { ' ', ',' }).ToList();
            kwds.RemoveAll(m => { return m == "OR"; });//TODO: Localize
            kwds.RemoveAll(m => { return m == "AND"; });
            kwds = (from k in kwds select k.Trim(new char[] { '(', ')', '*' })).ToList();
            if (kwds.Count == 0)
                return new EmptyResult();

            bool shown = Manager.SessionSettings.SiteSettings.GetValue<bool>("YetaWF_SearchControl_Highlight", true);
            Model model = new Model() {
                HighlightSearch = shown,
                SearchTerms = YetaWFManager.Jser.Serialize(kwds),
                On = Module.GetAction_On(!shown),
                Off = Module.GetAction_Off(shown),
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Switch(bool value) {
            Manager.SessionSettings.SiteSettings.SetValue<bool>("YetaWF_SearchControl_Highlight", value);
            Manager.SessionSettings.SiteSettings.Save();
            return FormProcessed(null, OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.Nothing);
        }
    }
}