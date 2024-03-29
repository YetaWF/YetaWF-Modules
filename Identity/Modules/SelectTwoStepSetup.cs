/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class SelectTwoStepSetupModuleDataProvider : ModuleDefinitionDataProvider<Guid, SelectTwoStepSetupModule>, IInstallableModel { }

    [ModuleGuid("{E70CB5F5-1FAE-4D69-9254-698E8C7C3CC6}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    [ModuleCategory("Two Step Authentication")]
    public class SelectTwoStepSetupModule : ModuleDefinition {

        public SelectTwoStepSetupModule() {
            Title = this.__ResStr("modTitle", "Setup Desired Two-Step Authentication");
            Name = this.__ResStr("modName", "Setup Two-Step Authentication - Selection");
            Description = "Displays a list of available two-step authentication providers that the user can define to use for two-step authentication. " +
                "This can be accessed using User > Two-Step Authentication (standard YetaWF site).";
            WantSearch = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SelectTwoStepSetupModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction> GetAction_SelectTwoStepSetupAsync(string url) {
            return await GetAction_ForceTwoStepSetupAsync(url);
        }
        public async Task<ModuleAction> GetAction_ForceTwoStepSetupAsync(string url) {
            if (string.IsNullOrWhiteSpace(url)) {
                LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
                url = config.TwoStepAuthUrl;
            }
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("setupLink", "Two-Step Authentication Setup"),
                MenuText = this.__ResStr("setupText", "Two-Step Authentication Setup"),
                Tooltip = this.__ResStr("setupTooltip", "Setup Two-Step Authentication"),
                Legend = this.__ResStr("setupLegend", "Setup Two-Step Authentication"),
                Style = ModuleAction.ActionStyleEnum.ForcePopup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
                AddToOriginList = true,
            };
        }
    }
}
