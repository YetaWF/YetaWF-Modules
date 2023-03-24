/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Scheduler.DataProvider;
using YetaWF.Modules.Scheduler.Endpoints;

namespace YetaWF.Modules.Scheduler.Modules;

public class SchedulerBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, SchedulerBrowseModule>, IInstallableModel { }

[ModuleGuid("{9C26B524-2934-44ae-B2AF-672B8F979A71}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SchedulerBrowseModule : ModuleDefinition {

    public SchedulerBrowseModule() : base() {
        Title = this.__ResStr("modTitle", "Scheduler");
        Name = this.__ResStr("modName", "Scheduler");
        Description = this.__ResStr("modSummary", "Displays all defined scheduler items and supports displaying, editing and removing scheduler items. The scheduler can also be started and stopped. It can be accessed at Admin > Panel > Scheduler (standard YetaWF site).");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SchedulerBrowseModuleDataProvider(); }

    [Category("General"), Caption("Add Url"), Description("The Url to add a new scheduler item - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? AddUrl { get; set; }
    [Category("General"), Caption("Display Url"), Description("The Url to display a scheduler item - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }
    [Category("General"), Caption("Edit Url"), Description("The Url to edit a scheduler item - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? EditUrl { get; set; }

    [Category("General"), Caption("Log Url"), Description("The Url to display the scheduler log - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? LogUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveItems",
                    this.__ResStr("roleRemItemsC", "Remove Scheduler Items"), this.__ResStr("roleRemItems", "The role has permission to remove individual scheduler items"),
                    this.__ResStr("userRemItemsC", "Remove Scheduler Items"), this.__ResStr("userRemItems", "The user has permission to remove individual scheduler items")),
                new RoleDefinition("RunItems",
                    this.__ResStr("roleRunItemsC", "Run Scheduler Items"), this.__ResStr("roleRunItems", "The role has permission to run individual scheduler items"),
                    this.__ResStr("userRunItemsC", "Run Scheduler Items"), this.__ResStr("userRunItems", "The user has permission to run individual scheduler items")),
            };
        }
    }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        SchedulerAddModule mod = new SchedulerAddModule();
        menuList.New(mod.GetAction_Add(AddUrl), location);
        LogBrowseModule logMod = new LogBrowseModule();
        menuList.NewIf(logMod.GetAction_Items(LogUrl), ModuleAction.ActionLocationEnum.ModuleLinks, location);
        return menuList;
    }

    public ModuleAction? GetAction_SchedulerItems(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Scheduler"),
            MenuText = this.__ResStr("browseText", "Scheduler"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage scheduler items"),
            Legend = this.__ResStr("browseLegend", "Displays and manages scheduler items"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_RemoveItem(string? name) {
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(SchedulerBrowseModuleEndpoints), SchedulerBrowseModuleEndpoints.RemoveItem),
            QueryArgs = new { Name = name },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove Item"),
            MenuText = this.__ResStr("removeMenu", "Remove Item"),
            Legend = this.__ResStr("removeLegend", "Remove the scheduler item"),
            Tooltip = this.__ResStr("removeTT", "Remove the scheduler item"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove scheduler item \"{0}\"?", name),
        };
    }
    public async Task<ModuleAction?> GetAction_RunItemAsync(string? name) {
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(SchedulerBrowseModuleEndpoints), SchedulerBrowseModuleEndpoints.RunItem),
            QueryArgs = new { Name = name },
            Image = await CustomIconAsync("RunItem.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("runLink", "Run Item"),
            MenuText = this.__ResStr("runMenu", "Run Item"),
            Legend = this.__ResStr("runLegend", "Run the scheduler item"),
            Tooltip = this.__ResStr("runTT", "Run the scheduler item"),
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("runConfirm", "Are you sure you want to run scheduler item \"{0}\"?", name),
        };
    }
    public async Task<ModuleAction?> GetAction_SchedulerToggleAsync() {
        bool running;
        using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
            running = dataProvider.GetRunning();
        }
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(SchedulerBrowseModuleEndpoints), SchedulerBrowseModuleEndpoints.SchedulerToggle),
            QueryArgs = new { Start = !running },
            Image = await CustomIconAsync("SchedulerToggle.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = running ? this.__ResStr("stopLink", "Stop Scheduler") : this.__ResStr("startLink", "Start Scheduler"),
            MenuText = running ? this.__ResStr("stopMenu", "Stop Scheduler") : this.__ResStr("startMenu", "Start Scheduler"),
            Legend = running ? this.__ResStr("stopLegend", "Stops the scheduler") : this.__ResStr("startLegend", "Starts the scheduler"),
            Tooltip = running ? this.__ResStr("stopTT", "Stop the scheduler") : this.__ResStr("startTT", "Start the scheduler"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = running ?
                this.__ResStr("toggleConfirmStart", "Are you sure you want to stop the scheduler?") :
                this.__ResStr("toggleConfirmStop", "Are you sure you want to start the scheduler?"),
        };
    }

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

    public GridDefinition GetGridModel() {
        return new GridDefinition {
            SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(SchedulerItem),
            AjaxUrl = Utility.UrlFor<SchedulerBrowseModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                    DataProviderGetRecords<SchedulerItemData> schedulerItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in schedulerItems.Data select new SchedulerItem(this, s)).ToList<object>(),
                        Total = schedulerItems.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        SchedulerBrowseModel model = new SchedulerBrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}