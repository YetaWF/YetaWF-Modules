/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Search.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Search.Controllers {

    public class SearchEditModuleController : ControllerImpl<YetaWF.Modules.Search.Modules.SearchEditModule> {

        public SearchEditModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Search Keyword"), Description("Search Keyword")]
            [UIHint("Text40"), StringLength(SearchData.MaxSearchTerm), Required]
            public string SearchTerm { get; set; }

            [Caption("Added"), Description("The date/time this keyword was added")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateAdded { get; set; }

            [Caption("Url"), Description("The page where this keyword was found")]
            [UIHint("Url"), ReadOnly]
            public string PageUrl { get; set; }

            [Caption("Language"), Description("The page language where this keyword was found")]
            [UIHint("LanguageId")]
            public string Language { get; set; }

            [Caption("Count"), Description("The number of times this keyword was found on the page")]
            [UIHint("IntValue"), Range(1,999999)]
            public int Count { get; set; }

            [Caption("Anonymous Users"), Description("Whether anonymous users can view the page")]
            [UIHint("Boolean")]
            public bool AllowAnonymous { get; set; }
            [Caption("Any Users"), Description("Whether any logged on user can view the page")]
            [UIHint("Boolean")]
            public bool AllowAnyUser { get; set; }

            [Caption("Id"), Description("The internal id this keyword")]
            [UIHint("IntValue"), ReadOnly]
            public int DisplaySearchDataId { get; set; }

            [UIHint("Hidden")]
            public int SearchDataId { get; set; }

            public SearchData GetData(SearchData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(SearchData data) {
                ObjectSupport.CopyData(data, this);
                DisplaySearchDataId = data.SearchDataId;
            }
            public EditModel() { }
        }

        [AllowGet]
        public async Task<ActionResult> SearchEdit(int searchDataId) {
            if (!SearchDataProvider.IsUsable) return View("SearchUnavailable_Edit");
            using (SearchDataProvider dataProvider = new SearchDataProvider()) {
                EditModel model = new EditModel { };
                SearchData data = await dataProvider.GetItemWithUrlAsync(searchDataId);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Search keyword with id {0} not found."), searchDataId);
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SearchEdit_Partial(EditModel model) {

            using (SearchDataProvider dataProvider = new SearchDataProvider()) {
                SearchData data = await dataProvider.GetItemWithUrlAsync(model.SearchDataId);
                if (data == null)
                    throw new Error(this.__ResStr("alreadyDeleted", "The search keyword with id {0} has been removed and can no longer be updated.", model.SearchDataId));

                if (!ModelState.IsValid)
                    return PartialView(model);

                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                switch (await dataProvider.UpdateItemAsync(data)) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("alreadyDeleted", "The search keyword with id {0} has been removed and can no longer be updated.", model.SearchDataId));
                    case UpdateStatusEnum.NewKeyExists:
                        throw new Error(this.__ResStr("alreadyExists", "A search keyword with id {0} already exists.", model.SearchDataId));
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Search keyword saved"), OnClose: OnCloseEnum.Return, OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}