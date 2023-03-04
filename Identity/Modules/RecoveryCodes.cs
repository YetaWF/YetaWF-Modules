/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
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
using YetaWF.Modules.Identity.Models;

namespace YetaWF.Modules.Identity.Modules;

public class RecoveryCodesModuleDataProvider : ModuleDefinitionDataProvider<Guid, RecoveryCodesModule>, IInstallableModel { }

[ModuleGuid("{e26230b2-a603-4a54-97ca-1e1b0b400d19}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Login & Registration")]
public class RecoveryCodesModule : ModuleDefinition2 {

    public RecoveryCodesModule() {
        Title = this.__ResStr("modTitle", "Recovery Codes");
        Name = this.__ResStr("modName", "Recovery Codes");
        Description = this.__ResStr("modSummary", "Allows users to review and generate two step authentication recovery codes.");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new RecoveryCodesModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return UserLevel_DefaultAllowedRoles; } }

    public const string IDENTITY_RECOVERY_PROGRESS = "YetaWF_Identity-Recovery-Progress";

    [Trim]
    [Header("Recovery codes are used to access your account should you be unable to authenticate using the currently defined two step authentication.")]
    public class EditModel {

        public enum ModelProgressEnum {
            New = 0,
            ShowLogin = 1,
            ShowCodes = 2,
        }

        [TextAbove("Enter your password to display your recovery code.")]
        [Caption("Password"), Description("Enter your password to access your recovery code")]
        [UIHint("Password20"), StringLength(Globals.MaxPswd), Trim]
        [RequiredIf(nameof(ModelProgress), ModelProgressEnum.ShowLogin)]
        [SuppressIfNot(nameof(ModelProgress), ModelProgressEnum.ShowLogin)]
        public string Password { get; set; }

        [Caption(""), Description("")]
        [UIHint("FormButton"), ReadOnly]
        [SuppressIfNot(nameof(ModelProgress), ModelProgressEnum.New)]
        public FormButton ShowRecoveryCodes { get; set; }

        [Caption(""), Description("")]
        [UIHint("FormButton"), ReadOnly]
        [SuppressIfNot(nameof(ModelProgress), ModelProgressEnum.ShowLogin)]
        public FormButton ShowRecoveryCodesLogin { get; set; }

        [TextBelow("Just like your account password, keep this recovery code a secret.")]
        [Caption("Your Recovery Code"), Description("Displays the available recovery code")]
        [UIHint("Text20"), AdditionalMetadata("ReadOnly", true), ReadOnly]
        [SuppressIfNot(nameof(ModelProgress), ModelProgressEnum.ShowCodes)]
        public string RecoveryCode { get; set; }

        [Caption(" "), Description(" ")]
        [UIHint("FormButton"), ReadOnly]
        [SuppressIfNot(nameof(ModelProgress), ModelProgressEnum.ShowCodes)]
        public FormButton GenerateRecoveryCode { get; set; }

        [UIHint("Hidden"), ReadOnly]
        public ModelProgressEnum ModelProgress { get; set; }

        public bool __submitShown => false;
        public bool __cancelShown => false;

        public EditModel() { }

        public void UpdateData(UserDefinition user) {
            switch (ModelProgress) {
                case EditModel.ModelProgressEnum.New:
                    ShowRecoveryCodes = new FormButton {
                        Text = this.__ResStr("showCode", "Show Recovery Code"),
                        ButtonType = ButtonTypeEnum.Apply,
                    };
                    break;
                case EditModel.ModelProgressEnum.ShowLogin:
                    ShowRecoveryCodesLogin = new FormButton {
                        Text = this.__ResStr("showCode", "Show Recovery Code"),
                        ButtonType = ButtonTypeEnum.Apply,
                    };
                    break;
                case EditModel.ModelProgressEnum.ShowCodes:
                    GenerateRecoveryCode = new FormButton {
                        Text = this.__ResStr("genCode", "Generate New Recovery Code"),
                        ButtonType = ButtonTypeEnum.Apply,
                    };
                    RecoveryCode = user.RecoveryCode;
                    break;
            }
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {

        using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
            UserDefinition user = await userDP.GetItemByUserIdAsync(Manager.UserId);
            if (user == null)
                throw new InternalError("User with id {0} not found", Manager.UserId);
            // Make sure this user is not using an external account
            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                if (await logInfoDP.IsExternalUserAsync(Manager.UserId))
                    return ActionInfo.Empty;
            }
            // Make sure there are any 2fa processors
            TwoStepAuth twoStep = new TwoStepAuth();
            List<ITwoStepAuth> list = await twoStep.GetTwoStepAuthProcessorsAsync();
            if (list.Count == 0)
                return ActionInfo.Empty;

            // If there is no recovery code, generate one (upgraded system)
            if (user.RecoveryCode == null)
                await GenerateRecoveryCodeAsync(userDP, user);

            EditModel.ModelProgressEnum progress = (EditModel.ModelProgressEnum)Manager.SessionSettings.SiteSettings.GetValue<int>(IDENTITY_RECOVERY_PROGRESS, (int)EditModel.ModelProgressEnum.New);

            EditModel model = new EditModel() {
                ModelProgress = progress,
            };
            model.UpdateData(user);

            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_ComponentsHTML", "clipboardjs.com.clipboard");// add clipboard support which is needed later (after partial form update)

            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {

        using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
            UserDefinition user = await userDP.GetItemByUserIdAsync(Manager.UserId);
            if (user == null)
                throw new InternalError("User with id {0} not found", Manager.UserId);

            EditModel.ModelProgressEnum progress = (EditModel.ModelProgressEnum)Manager.SessionSettings.SiteSettings.GetValue<int>(IDENTITY_RECOVERY_PROGRESS, (int)EditModel.ModelProgressEnum.New);

            model.ModelProgress = progress;
            model.UpdateData(user);

            if (!ModelState.IsValid)
                return await PartialViewAsync(model);

            string msg = null;
            switch (progress) {
                case EditModel.ModelProgressEnum.New:
                    progress = EditModel.ModelProgressEnum.ShowLogin;
                    break;
                case EditModel.ModelProgressEnum.ShowLogin:
                    if (!await Managers.GetUserManager().CheckPasswordAsync(user, model.Password))
                        ModelState.AddModelError(nameof(model.Password), this.__ResStr("badPassword", "The password is invalid"));

                    if (!ModelState.IsValid)
                        return await PartialViewAsync(model);

                    progress = EditModel.ModelProgressEnum.ShowCodes;
                    break;
                case EditModel.ModelProgressEnum.ShowCodes:
                    await GenerateRecoveryCodeAsync(userDP, user);
                    msg = this.__ResStr("newCode", "A new recovery code has been generated");
                    break;
            }
            Manager.SessionSettings.SiteSettings.SetValue<int>(IDENTITY_RECOVERY_PROGRESS, (int)progress);
            Manager.SessionSettings.SiteSettings.Save();

            model.ModelProgress = progress;
            model.UpdateData(user);

            return await FormProcessedAsync(model, popupText: msg, ForceApply: true);
        }
    }

    private async Task GenerateRecoveryCodeAsync(UserDefinitionDataProvider userDP, UserDefinition user) {
        user.RecoveryCode = Guid.NewGuid().ToString().Substring(0, 12);
        UpdateStatusEnum status = await userDP.UpdateItemAsync(user);
        if (status != UpdateStatusEnum.OK)
            throw new Error(this.__ResStr("cantUpdate", "Updating user information failed - {0}", status));
    }
}
