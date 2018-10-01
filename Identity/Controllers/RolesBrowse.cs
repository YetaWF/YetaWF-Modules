/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class RolesBrowseModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.RolesBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

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
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(RolesBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                        DataProviderGetRecords<RoleDefinition> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }
        [AllowGet]
        public ActionResult RolesBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> RolesBrowse_GridData(string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync(GetGridModel(), fieldPrefix, skip, take, sorts, filters);
        }

        [AllowPost]
        [Permission("RemoveRoles")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new Error(this.__ResStr("noItem", "No role name specified"));
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                await dataProvider.RemoveItemAsync(name);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}