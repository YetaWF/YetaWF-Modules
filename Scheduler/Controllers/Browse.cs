/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Scheduler.DataProvider;
using YetaWF.Modules.Scheduler.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Scheduler.Controllers {

    public class SchedulerBrowseModuleController : ControllerImpl<YetaWF.Modules.Scheduler.Modules.SchedulerBrowseModule> {

        public class SchedulerItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    SchedulerDisplayModule dispMod = new SchedulerDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Name), ModuleAction.ActionLocationEnum.GridLinks);

                    SchedulerEditModule editMod = new SchedulerEditModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, Name), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(Module.GetAction_RunItem(Name), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_RemoveItem(Name), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [Caption("Running"), Description("Shows whether the scheduler item is currently running")]
            [UIHint("Boolean"), ReadOnly]
            public bool IsRunning { get; set; }

            [Caption("Name"), Description("The name of this scheduler item - the name is used to identify a scheduler item")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("The description of this scheduler item")]
            [UIHint("TextArea"), ReadOnly]
            public string Description { get; set; }

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
            public SchedulerFrequency Frequency { get; set; }

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
            public SchedulerEvent Event { get; set; }

            private SchedulerBrowseModule Module { get; set; }

            public SchedulerItem(SchedulerBrowseModule module, SchedulerItemData evnt) {
                Module = module;
                ObjectSupport.CopyData(evnt, this);
            }
        }

        public class SchedulerBrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [AllowGet]
        public ActionResult SchedulerBrowse() {
            SchedulerBrowseModel model = new SchedulerBrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("SchedulerBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(SchedulerItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [AllowPost]
        public ActionResult SchedulerBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                int total;
                List<SchedulerItemData> schedulerItems = dataProvider.GetItems(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in schedulerItems select new SchedulerItem(Module, s)).ToList<object>(),
                    Total = total
                });
            }
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public ActionResult RemoveItem(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new Error(this.__ResStr("noEvent", "No scheduler item name specified"));
            using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                dataProvider.RemoveItem(name);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }

        [AllowPost]
        [Permission("RunItems")]
        [ExcludeDemoMode]
        public ActionResult RunItem(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new Error(this.__ResStr("noEvent", "No scheduler item name specified"));
            SchedulerSupport.RunItem(name);
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }

        [AllowPost]
        [ExcludeDemoMode]
        public ActionResult SchedulerToggle(bool start) {
            using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                dataProvider.SetRunning(start);
            }
            return FormProcessed(null,
                start ?
                this.__ResStr("okStarting", "The scheduler has been started and the site is restarting") :
                    this.__ResStr("okStopping", "The scheduler has been stopped and the site is restarting"),
                OnClose: OnCloseEnum.GotoNewPage, OnPopupClose: OnPopupCloseEnum.GotoNewPage, NextPage: null );
        }
    }
}