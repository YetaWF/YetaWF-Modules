/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Modules.Search.DataProvider;
using System.Threading.Tasks;
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

            [UIHint("ModuleAction")]
            public ModuleAction On { get; set; }
            [UIHint("ModuleAction")]
            public ModuleAction Off { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> SearchControl() {
            if (Manager.EditMode)
                return new EmptyResult();
            if (!SearchDataProvider.IsUsable)
                return new EmptyResult();

            bool shown = Manager.SessionSettings.SiteSettings.GetValue<bool>("YetaWF_SearchControl_Highlight", true);

            Manager.ScriptManager.AddVolatileOption("YetaWF_Search", "HighLight", shown);// the default mode

            Model model = new Model() {
                HighlightSearch = shown,
                On = await Module.GetAction_OnAsync(),
                Off = await Module.GetAction_OffAsync(),
            };
            return View(model);
        }

        [AllowPost]
        public ActionResult Switch(bool value) {
            Manager.SessionSettings.SiteSettings.SetValue<bool>("YetaWF_SearchControl_Highlight", value);
            Manager.SessionSettings.SiteSettings.Save();
            return FormProcessed(null, OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.Nothing);
        }
    }
}