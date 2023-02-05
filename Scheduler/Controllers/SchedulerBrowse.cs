/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;
using YetaWF.Modules.Scheduler.Modules;
using YetaWF.Core.Components;
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Endpoints;
using YetaWF.Modules.Scheduler.Endpoints;

namespace YetaWF.Modules.Scheduler.Controllers {

    public class SchedulerBrowseModuleController : ControllerImpl<YetaWF.Modules.Scheduler.Modules.SchedulerBrowseModule> {

        public class SchedulerItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands { get; set; } = null!;

            public async Task<List<ModuleAction>> __GetCommandsAsync() {
                List<ModuleAction> actions = new List<ModuleAction>();

                SchedulerDisplayModule dispMod = new SchedulerDisplayModule();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Name), ModuleAction.ActionLocationEnum.GridLinks);

                SchedulerEditModule editMod = new SchedulerEditModule();
                actions.New(editMod.GetAction_Edit(Module.EditUrl, Name), ModuleAction.ActionLocationEnum.GridLinks);

                actions.New(await Module.GetAction_RunItemAsync(Name), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(Module.GetAction_RemoveItem(Name), ModuleAction.ActionLocationEnum.GridLinks);

                return actions;
            }

            [Caption("Running"), Description("Shows whether the scheduler item is currently running")]
            [UIHint("Boolean"), ReadOnly]
            public bool IsRunning { get; set; }

            [Caption("Name"), Description("The name of this scheduler item - the name is used to identify a scheduler item")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; } = null!;

            [Caption("Description"), Description("The description of this scheduler item")]
            [UIHint("TextArea"), ReadOnly]
            public string? Description { get; set; }

            [Caption("Enabled"), Description("The status of the scheduler item")]
            [UIHint("Boolean"), ReadOnly]
            public bool Enabled { get; set; }

            [Caption("Enable On Startup"), Description("Shows whether the scheduler item is enabled every time the website is restarted")]
            [UIHint("Boolean"), ReadOnly]
            public bool EnableOnStartup { get; set; }

            [Caption("Run Once"), Description("Shows whether the scheduler item is run just once - once it completes, it is disabled")]
            [UIHint("Boolean"), ReadOnly]
            public bool RunOnce { get; set; }

            [Caption("Startup"), Description("Shows whether the scheduler item runs at website startup")]
            [UIHint("Boolean"), ReadOnly]
            public bool Startup { get; set; }

            [Caption("Site Specific"), Description("Shows whether the scheduler item runs for each site")]
            [UIHint("Boolean"), ReadOnly]
            public bool SiteSpecific { get; set; }

            [Caption("Interval"), Description("The scheduler item's frequency")]
            [UIHint("YetaWF_Scheduler_Frequency"), AdditionalMetadata("ShowEnumValue", false), ReadOnly]
            public SchedulerFrequency Frequency { get; set; } = null!;

            [Caption("Last"), Description("The last time this item ran")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Last { get; set; }

            [Caption("Next"), Description("The time this item is scheduled to run next")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Next { get; set; }

            [Caption("RunTime"), Description("The duration of the last run of this scheduler item (hh:mm:ss)")]
            [UIHint("TimeSpan"), ReadOnly]
            public TimeSpan RunTime { get; set; }

            [Caption("Event"), Description("The event name running at the scheduled time")]
            [UIHint("YetaWF_Scheduler_Event"), ReadOnly]
            public SchedulerEvent Event { get; set; } = null!;

            public bool __highlight { get { return IsRunning; } }

            private SchedulerBrowseModule Module { get; set; }

            public SchedulerItem(SchedulerBrowseModule module, SchedulerItemData evnt) {
                Module = module;
                ObjectSupport.CopyData(evnt, this);
            }
        }

        public class SchedulerBrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }

        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(SchedulerItem),
                AjaxUrl = Utility.UrlFor<SchedulerBrowseModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                        DataProviderGetRecords<SchedulerItemData> schedulerItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in schedulerItems.Data select new SchedulerItem((SchedulerBrowseModule)module, s)).ToList<object>(),
                            Total = schedulerItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult SchedulerBrowse() {
            SchedulerBrowseModel model = new SchedulerBrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}