/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Messenger.DataProvider;
using YetaWF.Modules.Messenger.Hubs;

namespace YetaWF.Modules.Messenger.Modules;

public class SiteAnnouncementModuleDataProvider : ModuleDefinitionDataProvider<Guid, SiteAnnouncementModule>, IInstallableModel { }

[ModuleGuid("{bace50b3-7508-4df9-9e90-62cfd2a7a1a1}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SiteAnnouncementModule : ModuleDefinition {

    public SiteAnnouncementModule() {
        Title = this.__ResStr("modTitle", "New Site Announcement");
        Name = this.__ResStr("modName", "Site Announcement");
        Description = this.__ResStr("modSummary", "Sends a new site announcement to all users");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SiteAnnouncementModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Send(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Send Announcement"),
            MenuText = this.__ResStr("addText", "Send Announcement"),
            Tooltip = this.__ResStr("addTooltip", "Send a new site announcement to all users"),
            Legend = this.__ResStr("addLegend", "Sends a new site announcement to all users"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,

        };
    }

    [Trim]
    public class AddModel {

        [Caption("Title"), Description("Defines the title of the message to be sent")]
        [UIHint("Text80"), StringLength(DataProvider.SiteAnnouncement.MaxTitle), Trim, Required]
        public string? Title { get; set; } = null!;

        [TextAbove("Please enter the message to be sent to all users that are currently using the site (anonymous and logged on users).")]
        [Caption("Message"), Description("Defines the message to be sent")]
        [UIHint("TextArea"), AdditionalMetadata("PageBrowse", true), AdditionalMetadata("Encode", false), StringLength(DataProvider.SiteAnnouncement.MaxMessage), Trim, Required]
        public string? Message { get; set; } = null!;

        [Caption("Test Mode"), Description("Select to test sending a message to yourself (no other users will receive this message)")]
        [UIHint("Boolean"), SuppressIf(nameof(IsDemoMode), false), ReadOnly]
        public bool TestModeDemo { get { return TestMode; } set { TestMode = value; } }

        [Caption("Test Mode"), Description("Select to test sending a message to yourself (no other users will receive this message)")]
        [UIHint("Boolean"), SuppressIf(nameof(IsDemoMode), true)]
        public bool TestModeProd { get { return TestMode; } set { TestMode = value; } }

        public bool TestMode { get; set; }

        [Caption(" "), Description(" ")]
        [UIHint("String"), SuppressIf(nameof(IsDemoMode), false), ReadOnly]
        public string? Description { get; set; }

        public bool IsDemoMode { get { return YetaWFManager.IsDemo; } }

        public AddModel() {
            TestMode = IsDemoMode;
            Description = this.__ResStr("demo", "In Demo mode, the message is sent to the current user only - Other users do not receive the message.");
        }

        public SiteAnnouncement GetData() {
            SiteAnnouncement data = new SiteAnnouncement();
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void UpdateData() {
            if (IsDemoMode)
                TestMode = IsDemoMode;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        AddModel model = new AddModel { };
        ObjectSupport.CopyData(new SiteAnnouncement(), model);
        model.Title = this.__ResStr("anncTitle", "Site Announcement");
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(AddModel model) {
        model.UpdateData();
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        if (model.TestMode) {
            return await FormProcessedAsync(model, model.Message, model.Title, OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace, PopupOptions: "{encoded:true, canClose: true, autoClose: 0}");
        } else {
            using (SiteAnnouncementDataProvider siteAnnounceDP = new SiteAnnouncementDataProvider()) {

                await YetaWF_Messenger_SiteAnnouncementsHub.SendMessageAsync(model.Message!, model.Title!);

                if (await siteAnnounceDP.IsInstalledAsync()) {
                    if (!await siteAnnounceDP.AddItemAsync(model.GetData()))
                        throw new Error(this.__ResStr("noLog", "Message sent. New site announcement log record couldn't be added"));
                }
            }
            return await FormProcessedAsync(model);
        }
    }
}

