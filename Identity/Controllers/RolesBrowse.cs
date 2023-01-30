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
using YetaWF.Modules.Identity.Endpoints;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Controllers {

    public class RolesBrowseModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.RolesBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    RolesDisplayModule dispMod = new RolesDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Name), ModuleAction.ActionLocationEnum.GridLinks);

                    RolesEditModule editMod = new RolesEditModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, Name), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(Module.GetAction_RemoveLink(Name), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [Caption("Name"), Description("The role name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("The intended use of the role")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [Caption("Post Login URL"), Description("The URL where a user with this role is redirected after logging on")]
            [UIHint("Url"), ReadOnly]
            public string PostLoginUrl { get; set; }

            [Caption("Role Id"), Description("The internal id of the role")]
            [UIHint("IntValue"), ReadOnly]
            public int RoleId { get; set; }

            private RolesBrowseModule Module { get; set; }

            public BrowseItem(RolesBrowseModule module, RoleDefinition data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
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
                AjaxUrl = Utility.UrlFor<RolesBrowseModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                        DataProviderGetRecords<RoleDefinition> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem((RolesBrowseModule)module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }
        [AllowGet]
        public ActionResult RolesBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}
