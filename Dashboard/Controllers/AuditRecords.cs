/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Extensions;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Dashboard.DataProvider;
using YetaWF.Modules.Dashboard.Modules;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Dashboard.Controllers {

    public class AuditRecordsModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.AuditRecordsModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
                    AuditDisplayModule dispMod = new AuditDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Id), ModuleAction.ActionLocationEnum.GridLinks);
                    //actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Id"), Description("The internal id")]
            [UIHint("IntValue"), ReadOnly]
            public int Id { get; set; }
            [Caption("Created"), Description("The date/time this record was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Identifier/String"), Description("The identifying string of the record - Identifier String and Type both identify the source of this record")]
            [UIHint("String"), ReadOnly]
            public string IdentifyString { get; set; }
            [Caption("Identifier/Type"), Description("The type of the record - Identifier String and Type both identify the source of this record")]
            [UIHint("Guid"), ReadOnly]
            public Guid IdentifyGuid { get; set; }

            [Caption("Action"), Description("The action that created this record")]
            [UIHint("String"), ReadOnly]
            public string Action { get; set; }
            [Caption("Description"), Description("The description for this record")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }
            [Caption("Changes"), Description("The properties that were changed")]
            [UIHint("String"), ReadOnly]
            public string Changes { get; set; }

            [Caption("Site"), Description("The site that was changed")]
            [UIHint("SiteId"), ReadOnly]
            public int SiteIdentity { get; set; }
            [Caption("User"), Description("The user that made the change")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserId { get; set; }

            [Caption("Restart Pending"), Description("Defines whether this action requires a restart to take effect")]
            [UIHint("Boolean"), ReadOnly]
            public bool RequiresRestart { get; set; }
            [Caption("Expensive Action"), Description("Defines whether this action is an expensive action in a multi-instance site")]
            [UIHint("Boolean"), ReadOnly]
            public bool ExpensiveMultiInstance { get; set; }

            public bool __highlight { get { return RequiresRestart && YetaWF.Core.Support.Startup.MultiInstanceStartTime < Created; ; } }
            public bool __lowlight { get { return YetaWF.Core.Support.Startup.MultiInstanceStartTime > Created; } }

            private AuditRecordsModule Module { get; set; }

            public BrowseItem(AuditRecordsModule module, AuditInfo data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
                Description = Description?.Truncate(100);
                Changes = Changes?.Replace(",", ", ");
                Changes = Changes.TruncateWithEllipse(100);
            }
            public BrowseItem() { }
        }

        public class BrowseModel {
            [Caption("Restart Pending"), Description("Defines whether a site restart (all instances) is pending to active all pending changes")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Boolean"), ReadOnly]
            public bool RestartPending { get; set; }

            [Caption("Last Restart"), Description("The date and time the site (all instances) was last restarted")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime LastRestart { get; set; }

            [Caption("Auditing Active"), Description("Defines whether auditing is active (enabled using appsettings.json)")]
            [UIHint("Boolean"), ReadOnly]
            [SuppressIf("AuditingActive", true)]
            public bool AuditingActive { get { return YetaWF.Core.Audit.Auditing.Active; } }

            [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Grid"), ReadOnly]
            [SuppressIf("AuditingActive", false)]
            public GridDefinition GridDef { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(AuditRecords_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (AuditInfoDataProvider dataProvider = new AuditInfoDataProvider()) {
                        DataProviderGetRecords<AuditInfo> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public async Task<ActionResult> AuditRecords() {
            using (AuditInfoDataProvider dataProvider = new AuditInfoDataProvider()) {
                BrowseModel model = new BrowseModel {
                    RestartPending = YetaWF.Core.Support.Startup.RestartPending || (YetaWF.Core.Audit.Auditing.Active ? await YetaWF.Core.Audit.Auditing.AuditProvider.HasPendingRestartAsync() : false),
                    LastRestart = YetaWF.Core.Support.Startup.MultiInstanceStartTime,
                    GridDef = GetGridModel()
                };
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> AuditRecords_GridData(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync(GetGridModel(), fieldPrefix, skip, take, sorts, filters);
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(int id) {
            using (AuditInfoDataProvider dataProvider = new AuditInfoDataProvider()) {
                if (!await dataProvider.RemoveItemAsync(id))
                    throw new Error(this.__ResStr("cantRemove", "Couldn't remove {0}", id));
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}
