/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Logging.DataProvider;

namespace YetaWF.Modules.Logging.Modules {

    public class LoggingConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, LoggingConfigModule>, IInstallableModel { }

    [ModuleGuid("{5F8435C9-9896-460b-A0CC-26F5C3693B39}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class LoggingConfigModule : ModuleDefinition {

        public LoggingConfigModule() {
            Title = this.__ResStr("modTitle", "Logging Settings");
            Name = this.__ResStr("modName", "Logging Settings");
            Description = this.__ResStr("modSummary", "Used to edit a site's logging settings. This can be accessed using Admin > Settings > Logging Settings (standard YetaWF site).");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LoggingConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new LoggingConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Logging Settings"),
                MenuText = this.__ResStr("editText", "Logging Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the logging settings"),
                Legend = this.__ResStr("editLegend", "Edits the logging settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}