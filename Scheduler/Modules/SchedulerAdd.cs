/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
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

namespace YetaWF.Modules.Scheduler.Modules;

public class SchedulerAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, SchedulerAddModule>, IInstallableModel { }

[ModuleGuid("{F541B60F-4468-40ed-A59D-707463B1FBAA}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SchedulerAddModule : ModuleDefinition2 {

    public SchedulerAddModule() : base() {
        Title = this.__ResStr("modTitle", "New Scheduler Item");
        Name = this.__ResStr("modName", "New Scheduler Item");
        Description = this.__ResStr("modSummary", "Creates a new scheduler item. Used by the Scheduler Module.");
        DefaultViewName = StandardViews.Add;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SchedulerAddModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Add(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add"),
            MenuText = this.__ResStr("addText", "Add"),
            Tooltip = this.__ResStr("addTooltip", "Create a new scheduler item"),
            Legend = this.__ResStr("addLegend", "Creates a new scheduler item"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class SchedulerAddModel {

        [Caption("Name"), Description("The name of this scheduler item - the name is used to identify a scheduler item")]
        [UIHint("Text40"), Required, StringLength(SchedulerItemData.MaxName)]
        public string? Name { get; set; }

        [Caption("Event"), Description("The event type of the scheduler item")]
        [UIHint("YetaWF_Scheduler_Event")]
        public SchedulerEvent Event { get; set; }

        [Caption("Description"), Description("The description of this scheduler item")]
        [UIHint("TextAreaSourceOnly"), StringLength(SchedulerItemData.MaxDescription), Required]
        public string? Description { get; set; }

        [Caption("Enabled"), Description("The status of the scheduler item")]
        [UIHint("Boolean")]
        public bool Enabled { get; set; }

        [Caption("Enable On Startup"), Description("Defines whether the scheduler item is enabled every time the website is restarted")]
        [UIHint("Boolean")]
        public bool EnableOnStartup { get; set; }

        [Caption("Run Once"), Description("Defines whether the scheduler item is run just once - once it completes, it is disabled")]
        [UIHint("Boolean")]
        [ProcessIf(nameof(Enabled), true)]
        [ProcessIf(nameof(EnableOnStartup), true)]
        public bool RunOnce { get; set; }

        [Caption("Startup"), Description("Defines whether the scheduler item runs at website startup")]
        [UIHint("Boolean")]
        [ProcessIf(nameof(Enabled), true)]
        [ProcessIf(nameof(EnableOnStartup), true)]
        public bool Startup { get; set; }

        [Caption("Site Specific"), Description("Defines whether the scheduler item runs for each site")]
        [UIHint("Boolean")]
        [ProcessIf(nameof(Enabled), true)]
        [ProcessIf(nameof(EnableOnStartup), true)]
        public bool SiteSpecific { get; set; }

        [Caption("Interval"), Description("The scheduler item's frequency")]
        [UIHint("YetaWF_Scheduler_Frequency"), Required]
        [ProcessIf(nameof(Enabled), true)]
        [ProcessIf(nameof(EnableOnStartup), true)]
        public SchedulerFrequency Frequency { get; set; }

        public SchedulerAddModel() {
            Frequency = new SchedulerFrequency() { TimeUnits = SchedulerFrequency.TimeUnitEnum.Hours, Value = 1 };
            Event = new SchedulerEvent();
        }
        public SchedulerItemData GetEvent() {
            SchedulerItemData evnt = new SchedulerItemData();
            ObjectSupport.CopyData(this, evnt);
            return evnt;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        SchedulerAddModel model = new SchedulerAddModel { };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(SchedulerAddModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
            if (!await dataProvider.AddItemAsync(model.GetEvent()))
                throw new Error(this.__ResStr("alreadyExists", "A scheduler item named \"{0}\" already exists."), model.Name);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New scheduler item saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}