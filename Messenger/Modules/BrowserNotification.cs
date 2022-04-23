/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.Modules {

    public class BrowserNotificationsModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowserNotificationsModule>, IInstallableModel { }

    [ModuleGuid("{7F60ABC1-07A1-49f1-8381-BD4276977FF0}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BrowserNotificationsModule : ModuleDefinition {

        public BrowserNotificationsModule() {
            Title = this.__ResStr("modTitle", "Web Browser Notification Test");
            Name = this.__ResStr("modName", "Test - Web Browser Notification");
            Description = this.__ResStr("modSummary", "Web browser notification test");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BrowserNotificationsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string? url) {
            return new ModuleAction(this) {
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
    }
}