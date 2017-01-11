/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Controllers {

    public class AuthorizationBrowseModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.AuthorizationBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

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
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [HttpGet]
        public ActionResult AuthorizationBrowse() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("AuthorizationBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuthorizationBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (AuthorizationDataProvider dataProvider = new AuthorizationDataProvider()) {
                int total;
                List<Authorization> browseItems = dataProvider.GetItems(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(
                    new DataSourceResult {
                        Data = (from s in browseItems select new BrowseItem(Module, s)).ToList<object>(),
                        Total = total
                    }
                );
            }
        }

        [HttpPost]
        [Permission("RemoveResources")]
        [ExcludeDemoMode]
        public ActionResult Remove(string resourceName) {
            using (AuthorizationDataProvider authDP = new AuthorizationDataProvider()) {
                if (authDP.GetItem(resourceName) == null)
                    throw new Error(this.__ResStr("cantDel", "Resource {0} not found", resourceName));
                authDP.RemoveItem(resourceName);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }

    }
}