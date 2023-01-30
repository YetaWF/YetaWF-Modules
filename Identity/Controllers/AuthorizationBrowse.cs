/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Controllers {

    public class AuthorizationBrowseModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.AuthorizationBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    AuthorizationEditModule editMod = new AuthorizationEditModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, ResourceName), ModuleAction.ActionLocationEnum.GridLinks);

                    if (CanDelete)
                        actions.New(Module.GetAction_Remove(ResourceName), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Resource Name"), Description("The name of this resource")]
            [UIHint("String"), ReadOnly]
            public string ResourceName { get; set; }

            [Caption("Resource Description"), Description("The permissions granted if a user or role has access to this resource")]
            [UIHint("TextArea"), ReadOnly]
            public string ResourceDescription { get; set; }

            public bool CanDelete { get; set; }

            private AuthorizationBrowseModule Module { get; set; }

            public BrowseItem(AuthorizationBrowseModule module, Authorization auth) {
                Module = module;
                ObjectSupport.CopyData(auth, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl =  Utility.UrlFor<Endpoints.AuthorizationBrowseModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (AuthorizationDataProvider dataProvider = new AuthorizationDataProvider()) {
                        DataProviderGetRecords<Authorization> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem((AuthorizationBrowseModule)module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult AuthorizationBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}
