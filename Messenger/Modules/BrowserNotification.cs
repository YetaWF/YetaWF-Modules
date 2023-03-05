/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Messenger.Hubs;

namespace YetaWF.Modules.Messenger.Modules;

public class BrowserNotificationsModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowserNotificationsModule>, IInstallableModel { }

[ModuleGuid("{7F60ABC1-07A1-49f1-8381-BD4276977FF0}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class BrowserNotificationsModule : ModuleDefinition2 {

    public BrowserNotificationsModule() {
        Title = this.__ResStr("modTitle", "Web Browser Notification Test");
        Name = this.__ResStr("modName", "Test - Web Browser Notification");
        Description = this.__ResStr("modSummary", "Web browser notification test");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BrowserNotificationsModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Browser Notification"),
            MenuText = this.__ResStr("displayText", "Browser Notification"),
            Tooltip = this.__ResStr("displayTooltip", "Display the web browser notification test"),
            Legend = this.__ResStr("displayLegend", "Displays the web browser notification test"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    [Header("The module Web Browser Notifications (Skin) must be defined as a site-wide reference in order for notifications to be sent to the desktop.")]
    public class Model {

        [Caption("Title"), Description("Defines the title of the message to be sent")]
        [UIHint("Text80"), StringLength(80), Trim, Required]
        public string Title { get; set; } = null!;

        [TextAbove("Please enter the message to be sent as a desktop notification.")]
        [Caption("Message"), Description("Defines the message to be sent")]
        [UIHint("TextAreaSourceOnly"), StringLength(120), Trim, Required]
        public string Message { get; set; } = null!;

        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Model model = new Model { };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        // Send the message. This can be customized with icons, url and timeout value.
        await YetaWF_Messenger_BrowserNotificationsHub.SendMessageAsync(model.Title, model.Message, "", 5000, "https://YetaWF.com/");

        return await FormProcessedAsync(model);
    }
}