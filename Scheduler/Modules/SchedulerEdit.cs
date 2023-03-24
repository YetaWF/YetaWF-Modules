/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints.Support;
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

public class SchedulerEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, SchedulerEditModule>, IInstallableModel { }

[ModuleGuid("{09EB1AB0-2FA1-4d41-A853-91778FC86355}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SchedulerEditModule : ModuleDefinition {

    public SchedulerEditModule() : base() {
        Title = this.__ResStr("modTitle", "Scheduler Item");
        Name = this.__ResStr("modName", "Edit Scheduler Item");
        Description = this.__ResStr("modSummary", "Edits details for an existing scheduler item. Used by the Scheduler Module.");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SchedulerEditModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Edit(string? url, string name) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { EventName = name },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Edit"),
            MenuText = this.__ResStr("editText", "Edit"),
            Tooltip = this.__ResStr("editTooltip", "Edit an existing scheduler item"),
            Legend = this.__ResStr("editLegend", "Edits an existing scheduler item"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class SchedulerEditModel {

        [Caption("Name"), Description("The name of this scheduler item - the name is used to identify a scheduler item")]
        [UIHint("Text40"), StringLength(SchedulerItemData.MaxName), Required]
        public string? Name { get; set; }

        [Caption("Event"), Description("The event running at the scheduled time")]
        [UIHint("YetaWF_Scheduler_Event")]
        public SchedulerEvent Event { get; set; }

        [Caption("Description"), Description("The description of this scheduler item")]
        [UIHint("TextAreaSourceOnly"), StringLength(SchedulerItemData.MaxDescription), Required]
        public string? Description { get; set; }

        [Caption("Enabled"), Description("Defines whether the scheduler item is enabled")]
        [UIHint("Boolean")]
        public bool Enabled { get; set; }

        [Caption("Enable On Startup"), Description("Defines whether the scheduler item is enabled every time the website is restarted")]
        [UIHint("Boolean")]
        public bool EnableOnStartup { get; set; }

        [Caption("Run Once"), Description("Defines whether the scheduler item is run just once. Once it completes, it is disabled")]
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
        [UIHint("YetaWF_Scheduler_Frequency")]
        [ProcessIf(nameof(Enabled), true), RequiredIf(nameof(Enabled), true)]
        [ProcessIf(nameof(EnableOnStartup), true), RequiredIf(nameof(EnableOnStartup), true)]
        public SchedulerFrequency Frequency { get; set; }

        [UIHint("Hidden")]
        public string OriginalName { get; set; } = null!;

        public SchedulerEditModel() {
            Event = new SchedulerEvent();
            Frequency = new SchedulerFrequency();
        }

        public SchedulerItemData GetEvent() {
            SchedulerItemData evnt = new SchedulerItemData();
            ObjectSupport.CopyData(this, evnt);
            return evnt;
        }

        public void SetEvent(SchedulerItemData evnt) {
            ObjectSupport.CopyData(evnt, this);
            OriginalName = evnt.Name;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(string eventName) {
        using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
            SchedulerEditModel model = new SchedulerEditModel { };
            SchedulerItemData? evnt = await dataProvider.GetItemAsync(eventName);
            if (evnt == null)
                throw new Error(this.__ResStr("notFound", "Scheduler item \"{0}\" not found."), eventName);
            model.SetEvent(evnt);
            Title = this.__ResStr("title", "Scheduler Item \"{0}\"", evnt.Name);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(SchedulerEditModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
            switch (await dataProvider.UpdateItemAsync(model.OriginalName, model.GetEvent())) {
                default:
                case UpdateStatusEnum.RecordDeleted:
                    throw new Error(this.__ResStr("alreadyDeleted", "The scheduler item named \"{0}\" has been removed and can no longer be updated.", model.Name));
                case UpdateStatusEnum.NewKeyExists:
                    ModelState.AddModelError(nameof(model.Name), this.__ResStr("alreadyExists", "A scheduler item named \"{0}\" already exists.", model.Name));
                    return await PartialViewAsync(model);
                case UpdateStatusEnum.OK:
                    break;
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Scheduler item saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}