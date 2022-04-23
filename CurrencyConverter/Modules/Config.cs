/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.CurrencyConverter.DataProvider;

namespace YetaWF.Modules.CurrencyConverter.Modules {

    public class ConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, ConfigModule>, IInstallableModel { }

    [ModuleGuid("{03950959-d8d7-44bc-a6f7-3162b4db82ac}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class ConfigModule : ModuleDefinition {

        public ConfigModule() {
            Title = this.__ResStr("modTitle", "Currency Converter Settings");
            Name = this.__ResStr("modName", "Currency Converter Settings");
            Description = this.__ResStr("modSummary", "Used to edit the currency converter settings. It is accessible using Admin > Settings > Currency Converter Settings (standard YetaWF site).");
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
                LinkText = this.__ResStr("editLink", "Currency Converter Settings"),
                MenuText = this.__ResStr("editText", "Currency Converter Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the currency converter settings"),
                Legend = this.__ResStr("editLegend", "Edits the currency converter settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}