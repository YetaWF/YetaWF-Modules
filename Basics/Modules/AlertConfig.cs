/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Basics.DataProvider;

namespace YetaWF.Modules.Basics.Modules {

    public class AlertConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, AlertConfigModule>, IInstallableModel { }

    [ModuleGuid("{d2c029c4-6b03-45e4-9b88-1cbda8972738}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class AlertConfigModule : ModuleDefinition {

        public AlertConfigModule() {
            Title = this.__ResStr("modTitle", "Alert Settings");
            Name = this.__ResStr("modName", "Alert Settings");
            Description = this.__ResStr("modSummary", "Edits a site's Alert settings");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AlertConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new AlertConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Alert Settings"),
                MenuText = this.__ResStr("editText", "Alert Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the site's Alert settings"),
                Legend = this.__ResStr("editLegend", "Edits the site's Alert settings"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}