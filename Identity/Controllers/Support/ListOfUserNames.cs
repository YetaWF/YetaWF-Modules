/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.Components;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Core.Support;
using YetaWF.Core.Localize;
using System.Linq;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class ListOfUserNamesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfUserNamesDisplay_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<ListOfUserNamesDisplayComponent.Entry>(ListOfUserNamesDisplayComponent.GetGridModel(false, false, false), data, fieldPrefix, skip, take, sorts, filters);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfUserNamesEdit_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<ListOfUserNamesEditComponent.Entry>(ListOfUserNamesEditComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowListOfUserNamesAjax)]
        public async Task<ActionResult> ListOfUserNamesBrowse_GridData(string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync(ListOfUserNamesEditComponent.GetGridAllUsersModel(), fieldPrefix, skip, take, sorts, filters);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddUserName(string data, string fieldPrefix, string newUser) {
            using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemAsync(newUser);
                if (user == null)
                    throw new Error(this.__ResStr("noUser", "User {0} not found", newUser));
                List<ListOfUserNamesEditComponent.Entry> list = Utility.JsonDeserialize<List<ListOfUserNamesEditComponent.Entry>>(data);
                if ((from l in list where l.UserId == user.UserId select l).FirstOrDefault() != null)
                    throw new Error(this.__ResStr("dupUser", "User {0} has already been added", newUser));
                ListOfUserNamesEditComponent.Entry entry = new ListOfUserNamesEditComponent.Entry {
                    UserName = newUser,
                    UserId = user.UserId,
                };
                return await GridRecordViewAsync(await ListOfUserNamesEditComponent.GridRecordAsync(fieldPrefix, entry));
            }
        }
    }
}
