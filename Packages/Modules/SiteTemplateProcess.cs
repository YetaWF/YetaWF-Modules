/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Packages.Modules {

    public class SiteTemplateProcessModuleDataProvider : ModuleDefinitionDataProvider<Guid, SiteTemplateProcessModule>, IInstallableModel { }

    [ModuleGuid("{8a375e4b-f0ee-4d95-8c18-4e51e83c06e5}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SiteTemplateProcessModule : ModuleDefinition {

        public SiteTemplateProcessModule() {
            Title = this.__ResStr("modTitle", "Process Site Template");
            Name = this.__ResStr("modName", "Process Site Template");
            Description = this.__ResStr("modSummary", "Processes a Site Template and adds all defined pages to the current site. The Process Site Template Module is accessible using Admin > Panel > Packages (standard YetaWF site).");
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SiteTemplateProcessModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction> GetAction_ProcessSiteTemplateAsync(string url, string fileName) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { FileName = fileName },
                Image = await CustomIconAsync("SiteTemplateProcess.png"),
                LinkText = this.__ResStr("processLink", "Site Template"),
                MenuText = this.__ResStr("processText", "Site Template"),
                Tooltip = this.__ResStr("processTooltip", "Process a site template and add all defined pages to the current site"),
                Legend = this.__ResStr("processLegend", "Processes a site template and adds all defined pages to the current site"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}