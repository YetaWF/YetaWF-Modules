/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Messenger.DataProvider;

namespace YetaWF.Modules.Messenger.Modules {

    public class ConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, ConfigModule>, IInstallableModel { }

    [ModuleGuid("{952f1ec7-bad2-4a71-81eb-8934ea395f13}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class ConfigModule : ModuleDefinition {

        public ConfigModule() {
            Title = this.__ResStr("modTitle", "Messenger Settings");
            Name = this.__ResStr("modName", "Messenger Settings");
            Description = this.__ResStr("modSummary", "Edits a site's messenger settings");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new ConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Messenger Settings"),
                MenuText = this.__ResStr("editText", "Messenger Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the messenger settings"),
                Legend = this.__ResStr("editLegend", "Edits the messenger settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
