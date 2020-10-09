/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Dashboard.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules {

    public class AuditConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuditConfigModule>, IInstallableModel { }

    [ModuleGuid("{3C45FB8E-123A-45f6-89BD-75596473F70B}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class AuditConfigModule : ModuleDefinition {

        public AuditConfigModule() {
            Title = this.__ResStr("modTitle", "Audit Settings");
            Name = this.__ResStr("modName", "Audit Settings");
            Description = this.__ResStr("modSummary", "Used to edit a site's audit settings. This can be accessed using Admin > Settings > Audit Settings (standard YetaWF site).");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AuditConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new AuditConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Audit Settings"),
                MenuText = this.__ResStr("editText", "Audit Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the audit settings"),
                Legend = this.__ResStr("editLegend", "Edits the audit settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
