/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.Modules {

    public class MessagingModuleDataProvider : ModuleDefinitionDataProvider<Guid, MessagingModule>, IInstallableModel { }

    [ModuleGuid("{ec414123-0ee6-4804-90e6-6dfafb80240c}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class MessagingModule : ModuleDefinition {

        public MessagingModule() {
            Title = this.__ResStr("modTitle", "Messages");
            Name = this.__ResStr("modName", "Send Message");
            Description = this.__ResStr("modSummary", "Message history and message sending");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new MessagingModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return UserLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Send(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Messages"),
                MenuText = this.__ResStr("editText", "Messages"),
                Tooltip = this.__ResStr("editTooltip", "Display message history and send messages"),
                Legend = this.__ResStr("editLegend", "Displays message history and sends messages"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
