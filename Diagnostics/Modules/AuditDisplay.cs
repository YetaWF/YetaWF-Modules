using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Diagnostics.Modules {

    public class AuditDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuditDisplayModule>, IInstallableModel { }

    [ModuleGuid("{d739ad4d-2208-485c-9b98-81bf2c960c0d}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class AuditDisplayModule : ModuleDefinition {

        public AuditDisplayModule() {
            Title = this.__ResStr("modTitle", "Audit Info");
            Name = this.__ResStr("modName", "Audit Info");
            Description = this.__ResStr("modSummary", "Displays an existing audit info");
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
                Tooltip = this.__ResStr("displayTooltip", "Display the audit info"),
                Legend = this.__ResStr("displayLegend", "Displays an existing audit info"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
