/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Scheduler.Controllers;
using YetaWF.Modules.Scheduler.DataProvider;
using YetaWF.Core.Components;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Scheduler.Modules {

    public class SchedulerBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, SchedulerBrowseModule>, IInstallableModel { }

    [ModuleGuid("{9C26B524-2934-44ae-B2AF-672B8F979A71}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SchedulerBrowseModule : ModuleDefinition {

        public SchedulerBrowseModule() : base() {
            Title = this.__ResStr("modTitle", "Scheduler");
            Name = this.__ResStr("modName", "Scheduler");
            Description = this.__ResStr("modSummary", "Displays and manages scheduler items");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SchedulerBrowseModuleDataProvider(); }

        [Category("General"), Caption("Add Url"), Description("The Url to add a new scheduler item - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }
        [Category("General"), Caption("Display Url"), Description("The Url to display a scheduler item - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }
        [Category("General"), Caption("Edit Url"), Description("The Url to edit a scheduler item - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        [Category("General"), Caption("Log Url"), Description("The Url to display the scheduler log - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string LogUrl { get; set; }

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

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            SchedulerAddModule mod = new SchedulerAddModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            LogBrowseModule logMod = new LogBrowseModule();
            menuList.NewIf(logMod.GetAction_Items(LogUrl), ModuleAction.ActionLocationEnum.ModuleLinks, location);
            return menuList;
        }

        public ModuleAction GetAction_SchedulerItems(string url) {
            return new ModuleAction(this) {
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
        public ModuleAction GetAction_RemoveItem(string name) {
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(SchedulerBrowseModuleController), "RemoveItem"),
                QueryArgs = new { Name = name },
                NeedsModuleContext = true,
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
        public async Task<ModuleAction> GetAction_RunItemAsync(string name) {
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(SchedulerBrowseModuleController), nameof(SchedulerBrowseModuleController.RunItem)),
                QueryArgs = new { Name = name },
                NeedsModuleContext = true,
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
        public async Task<ModuleAction> GetAction_SchedulerToggleAsync() {
            bool running;
            using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                running = dataProvider.GetRunning();
            }
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(SchedulerBrowseModuleController), nameof(SchedulerBrowseModuleController.SchedulerToggle)),
                QueryArgs = new { Start = !running },
                NeedsModuleContext = true,
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
    }
}