/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Languages.DataProvider;

namespace YetaWF.Modules.Languages.Modules {

    public class LocalizeConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, LocalizeConfigModule>, IInstallableModel { }

    [ModuleGuid("{ac486814-9c4b-4c53-986a-e2d02720e867}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class LocalizeConfigModule : ModuleDefinition {

        public LocalizeConfigModule() {
            Title = this.__ResStr("modTitle", "Localization Settings");
            Name = this.__ResStr("modName", "Localization Settings");
            Description = this.__ResStr("modSummary", "Used to edit site and system wide localization settings. It is accessible using Admin > Settings > Localization Settings (standard YetaWF site).");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LocalizeConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new LocalizeConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Localization Settings"),
                MenuText = this.__ResStr("editText", "Localization Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the localization settings"),
                Legend = this.__ResStr("editLegend", "Edits the localization settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}