/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

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
            Title = this.__ResStr("modTitle", "AddOn Details");
            Name = this.__ResStr("modName", "AddOn Details");
            Description = this.__ResStr("modSummary", "Displays detailed information about one AddOn. Used by the AddOn Info Module.");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AddonDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string? url, string addonKey) {
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
