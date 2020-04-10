/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.TawkTo.DataProvider;

namespace YetaWF.Modules.TawkTo.Modules {

    public class ConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, ConfigModule>, IInstallableModel { }

    [ModuleGuid("{fdfa95b0-1dfb-4f62-ab07-7328c9d3aff2}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class ConfigModule : ModuleDefinition {

        public ConfigModule() {
            Title = this.__ResStr("modTitle", "TawkTo Settings");
            Name = this.__ResStr("modName", "TawkTo Settings");
            Description = this.__ResStr("modSummary", "Implements the TawkTo configuration. It can be accessed using Admin > Settings > TawkTo Settings (standard YetaWF site).");
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
                LinkText = this.__ResStr("editLink", "TawkTo Settings"),
                MenuText = this.__ResStr("editText", "TawkTo Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the tawkto settings"),
                Legend = this.__ResStr("editLegend", "Edits the tawkto settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
