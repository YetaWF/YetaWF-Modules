/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Identity;
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

public class SelectTwoStepAuthModuleDataProvider : ModuleDefinitionDataProvider<Guid, SelectTwoStepAuthModule>, IInstallableModel { }

[ModuleGuid("{0ffd80d1-69ef-4187-8d2d-aa48e69aa3f8}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Two Step Authentication")]
public class SelectTwoStepAuthModule : ModuleDefinition {

    public SelectTwoStepAuthModule() {
        Title = this.__ResStr("modTitle", "Select Desired Two-Step Authentication");
        Name = this.__ResStr("modName", "Select Two-Step Authentication");
        Description = this.__ResStr("modSummary", "Displays a list of available two-step authentication providers that the user can select to use for two-step authentication. This is used during login processing to complete two-step authentication.");
        WantSearch = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SelectTwoStepAuthModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_SelectTwoStepAuth(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Style = ModuleAction.ActionStyleEnum.ForcePopup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }

    [Trim]
    [Header("Please click on one of the available authentication methods to complete logging in.")]
    public class EditModel {
        public List<ModuleAction> Actions { get; set; }

        public EditModel() {
            Actions = new List<ModuleAction>();
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        int userId = await Resource.ResourceAccess.GetTwoStepUserAsync();
        EditModel model = new EditModel();
        using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
            UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
            if (user == null)
                throw new InternalError("User with id {0} not found", userId);
            TwoStepAuth twoStep = new TwoStepAuth();
            List<ITwoStepAuth> list = await twoStep.GetTwoStepAuthProcessorsAsync();
            List<string> procs = (from p in list select p.Name).ToList();
            List<string> enabledTwoStepAuths = (from e in user.EnabledTwoStepAuthentications select e.Name).ToList();
            procs = procs.Intersect(enabledTwoStepAuths).ToList();
            foreach (string proc in procs) {
                ITwoStepAuth auth = await twoStep.GetTwoStepAuthProcessorByNameAsync(proc);
                if (auth != null) {
                    model.Actions.New(await auth.GetLoginActionAsync());
                }
            }
            if (model.Actions.Count == 0) {
                await Resource.ResourceAccess.ClearTwoStepUserAsync();
                throw new InternalError("There are no two-step authentication providers installed");
            }
        }
        return await RenderAsync(model);
    }
}
