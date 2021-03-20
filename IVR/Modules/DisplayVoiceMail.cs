/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules {

    public class DisplayVoiceMailModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisplayVoiceMailModule>, IInstallableModel { }

    [ModuleGuid("{e8ec7d7d-6c13-4821-aaad-bbc162879ac5}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class DisplayVoiceMailModule : ModuleDefinition {

        public DisplayVoiceMailModule() {
            Title = this.__ResStr("modTitle", "Voice Mail Entry");
            Name = this.__ResStr("modName", "Voice Mail Entry");
            Description = this.__ResStr("modSummary", "Displays an existing voice mail entry.");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new DisplayVoiceMailModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction?> GetAction_DisplayAsync(string? url, int id) {
            if (url == null) {
                IVRConfig config = await IVRConfigDataProvider.GetConfigAsync();
                url = config.DisplayVoiceMailUrl;
            }
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Id = id },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the voice mail entry"),
                Legend = this.__ResStr("displayLegend", "Displays an existing voice mail entry"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
        public async Task<ModuleAction> GetAction_ListenAsync(string recordingUrl) {
            return new ModuleAction(this) {
                Url = recordingUrl,
                Image = await CustomIconAsync("Listen.png"),
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("listenLink", "Listen"),
                MenuText = this.__ResStr("listenMenu", "Listen"),
                Tooltip = this.__ResStr("listenTT", "Listen to the voice mail"),
                Legend = this.__ResStr("listenLegend", "Used to listen to the voice mail"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
    }
}
