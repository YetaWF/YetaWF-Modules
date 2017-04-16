/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules {

    public class AddonDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddonDisplayModule>, IInstallableModel { }

    [ModuleGuid("{df3df0e1-f88b-45e1-a04e-864748166a21}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class AddonDisplayModule : ModuleDefinition {

        public AddonDisplayModule() {
            Title = this.__ResStr("modTitle", "AddOn Info");
            Name = this.__ResStr("modName", "AddOn Info");
            Description = this.__ResStr("modSummary", "Displays information for an AddOn");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AddonDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, string addonKey) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Key = addonKey },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display AddOn information"),
                Legend = this.__ResStr("displayLegend", "Displays AddOn information"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}