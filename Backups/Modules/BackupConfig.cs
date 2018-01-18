/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Backups.DataProvider;

namespace YetaWF.Modules.Backups.Modules {

    public class BackupConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, BackupConfigModule>, IInstallableModel { }

    [ModuleGuid("{84b5bc7e-e5d9-4ab1-9535-8ba729d66649}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class BackupConfigModule : ModuleDefinition {

        public BackupConfigModule() {
            Title = this.__ResStr("modTitle", "Backup Settings");
            Name = this.__ResStr("modName", "Backup Settings");
            Description = this.__ResStr("modSummary", "Edits a site's backup settings");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BackupConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new ConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Backup Settings"),
                MenuText = this.__ResStr("editText", "Backup Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the backup settings"),
                Legend = this.__ResStr("editLegend", "Edits the backup settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}