/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Support.TwoStepAuthorization;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules;

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


    public async Task<ModuleAction> GetAction_SelectTwoStepSetupAsync(string url, string returnUrl) {
        return await GetAction_ForceTwoStepSetupAsync(url, returnUrl);
    }
    public async Task<ModuleAction> GetAction_ForceTwoStepSetupAsync(string url, string returnUrl) {
        if (string.IsNullOrWhiteSpace(url)) {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            url = config.TwoStepAuthUrl;
        }
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { ReturnUrl = returnUrl },
            Image = "#Edit",
            LinkText = this.__ResStr("setupLink", "Two-Step Authentication Setup"),
            MenuText = this.__ResStr("setupText", "Two-Step Authentication Setup"),
            Tooltip = this.__ResStr("setupTooltip", "Setup Two-Step Authentication"),
            Legend = this.__ResStr("setupLegend", "Setup Two-Step Authentication"),
            Style = ModuleAction.ActionStyleEnum.ForcePopup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,

        };
    }

    [Trim]
    [Header("Please set up a two-step authentication method to protect your account from being hijacked.")]
    public class EditModel {
        public class AuthMethod {

            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.LinksOnly), ReadOnly]
            public ModuleAction Action { get; set; }

            [UIHint("Switch"), ReadOnly]
            public bool Enabled { get; set; }
            public string Enabled_On { get; set; } = "Enabled";
            public string Enabled_Off { get; set; } = "Disabled";
            public string Enabled_Size { get; set; } = "md";

            public string Description { get; set; }
        }
        public List<AuthMethod> AuthMethods { get; set; }

        public EditModel() {
            AuthMethods = new List<AuthMethod>();
        }
    }

    [ExcludeDemoMode] //$$$$ verify
    public async Task<ActionInfo> RenderModuleAsync() {
        EditModel model = new EditModel();
        Manager.NeedUser();
        using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
            UserDefinition user = await userDP.GetItemByUserIdAsync(Manager.UserId);
            if (user == null)
                throw new InternalError("User with id {0} not found", Manager.UserId);
            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                string ext = await logInfoDP.GetExternalLoginProviderAsync(Manager.UserId);
                if (ext != null)
                    return await RenderAsync(this.__ResStr("extUser", "Your account uses a {0} account - Two-step authentication must be set up using your {0} account.", ext), ViewName: "ShowMessage", UseAreaViewName: false);
            }
            TwoStepAuth twoStep = new TwoStepAuth();
            List<ITwoStepAuth> list = await twoStep.GetTwoStepAuthProcessorsAsync();
            List<string> procs = (from p in list select p.Name).ToList();
            List<string> enabledTwoStepAuths = (from e in user.EnabledTwoStepAuthentications select e.Name).ToList();
            foreach (string proc in procs) {
                ITwoStepAuth auth = await twoStep.GetTwoStepAuthProcessorByNameAsync(proc);
                if (auth != null) {
                    ModuleAction action = await auth.GetSetupActionAsync();
                    if (action != null && await action.IsAuthorizedAsync()) {
                        bool enabled = enabledTwoStepAuths.Contains(auth.Name);
                        model.AuthMethods.Add(new EditModel.AuthMethod {
                            Action = action,
                            Enabled = enabled,
                            Description = auth.GetDescription()
                        });
                    }
                }
            }
            model.AuthMethods = (from a in model.AuthMethods orderby a.Action.LinkText select a).ToList();
        }
        return await RenderAsync(model);
    }
}
