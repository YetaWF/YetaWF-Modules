/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.ModuleEdit.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ModuleEdit.Controllers {

    public class AllowedUsersController : ControllerImpl<YetaWF.Core.Modules.ModuleDefinition> {

        public AllowedUsersController() { }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddUserToModule(string prefix, int newRecNumber, string newValue, Guid editGuid) {

            if (string.IsNullOrWhiteSpace(newValue))
                throw new Error(this.__ResStr("noParm", "No user name specified"));
            if (editGuid == Guid.Empty)
                throw new InternalError("No module being edited");

            int userId = await Resource.ResourceAccess.GetUserIdAsync(newValue);
            if (userId == 0)
                throw new Error(this.__ResStr("noUser", "User {0} doesn't exist", newValue));

            // get the type of the grid record from the module being edited by checking its
            // AllowedUsers property [AdditionalMetadata("GridEntry", typeof(GridAllowedUserEntry))]
            ModuleDefinition editMod = await ModuleDefinition.LoadAsync(editGuid);
            PropertyData propData = ObjectSupport.GetPropertyData(editMod.GetType(), nameof(ModuleDefinition.AllowedUsers));
            Type gridEntryType = propData.GetAdditionalAttributeValue<Type>("GridEntry");

            // add new grid record
            ModuleDefinition.GridAllowedUser userEntry = (ModuleDefinition.GridAllowedUser)Activator.CreateInstance(gridEntryType);
            await userEntry.SetUserAsync(userId);
            GridDefinition.GridEntryDefinition gridEntryDef = new GridDefinition.GridEntryDefinition(prefix, newRecNumber, userEntry);
            return await GridPartialViewAsync(gridEntryDef);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(YetaWF.Modules.Identity.Addons.Info.Resource_AllowListOfUserNamesAjax)]
        public async Task<ActionResult> AllowedUsersBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters /*, Guid settingsModuleGuid - not available in templates */) {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                DataProviderGetRecords<UserDefinition> browseItems = await userDP.GetItemsAsync(skip, take, sort, filters);
                //Grid.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return await GridPartialViewAsync(new DataSourceResult {
                    Data = (from s in browseItems.Data select new AllowedUsersEditComponent.GridAllEntry(s)).ToList<object>(),
                    Total = browseItems.Total
                });
            }
        }
    }
}