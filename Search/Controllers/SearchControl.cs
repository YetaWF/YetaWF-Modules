/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Search.Addons;
using YetaWF.Modules.Search.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

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
            if (!SearchDataProvider.IsUsable)
                return new EmptyResult();
            string searchTerms;
#if MVC6
            searchTerms = Manager.CurrentRequest.Query[Info.UrlArg];
#else
            searchTerms = Manager.CurrentRequest[Info.UrlArg];
#endif
            if (searchTerms == null)
                return new EmptyResult();
            List<string> kwds = searchTerms.Split(new char[] { ' ', ',' }).ToList();
            using (SearchResultDataProvider searchResDP = new SearchResultDataProvider()) {
                string searchKwdOr = searchResDP.GetKeyWordOr();
                string searchKwdAnd = searchResDP.GetKeyWordAnd();
                kwds.RemoveAll(m => { return m == searchKwdOr; });
                kwds.RemoveAll(m => { return m == searchKwdAnd; });
            }
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