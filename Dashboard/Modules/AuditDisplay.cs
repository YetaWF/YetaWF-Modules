/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules {

    public class AuditDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuditDisplayModule>, IInstallableModel { }

    [ModuleGuid("{d739ad4d-2208-485c-9b98-81bf2c960c0d}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class AuditDisplayModule : ModuleDefinition {

        public AuditDisplayModule() {
            Title = this.__ResStr("modTitle", "Audit Record");
            Name = this.__ResStr("modName", "Audit Record");
            Description = this.__ResStr("modSummary", "Displays an existing audit record. Used by the Audit Information Module.");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AuditDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, int id) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Id = id },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the audit record"),
                Legend = this.__ResStr("displayLegend", "Displays an existing audit record"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
