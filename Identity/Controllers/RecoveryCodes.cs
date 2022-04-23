/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Core;
using YetaWF.Core.Components;
using System;
using YetaWF.Core.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Core.Support.TwoStepAuthorization;
using System.Collections.Generic;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class RecoveryCodesModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.RecoveryCodesModule> {

        public const string IDENTITY_RECOVERY_PROGRESS = "YetaWF_Identity-Recovery-Progress";

        public RecoveryCodesModuleController() { }

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

        [AllowGet]
        public async Task<ActionResult> RecoveryCodes() {

            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(Manager.UserId);
                if (user == null)
                    throw new InternalError("User with id {0} not found", Manager.UserId);
                // Make sure this user is not using an external account
                using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                    if (await logInfoDP.IsExternalUserAsync(Manager.UserId))
                        return new EmptyResult();
                }
                // Make sure there are any 2fa processors
                TwoStepAuth twoStep = new TwoStepAuth();
                List<ITwoStepAuth> list = await twoStep.GetTwoStepAuthProcessorsAsync();
                if (list.Count == 0)
                    return new EmptyResult();

                // If there is no recovery code, generate one (upgraded system)
                if (user.RecoveryCode == null)
                    await GenerateRecoveryCodeAsync(userDP, user);

                EditModel.ModelProgressEnum progress = (EditModel.ModelProgressEnum)Manager.SessionSettings.SiteSettings.GetValue<int>(IDENTITY_RECOVERY_PROGRESS, (int)EditModel.ModelProgressEnum.New);

                EditModel model = new EditModel() {
                    ModelProgress = progress,
                };
                model.UpdateData(user);

                await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_ComponentsHTML", "clipboardjs.com.clipboard");// add clipboard support which is needed later (after partial form update)

                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> RecoveryCodes_Partial(EditModel model) {

            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(Manager.UserId);
                if (user == null)
                    throw new InternalError("User with id {0} not found", Manager.UserId);

                EditModel.ModelProgressEnum progress = (EditModel.ModelProgressEnum)Manager.SessionSettings.SiteSettings.GetValue<int>(IDENTITY_RECOVERY_PROGRESS, (int)EditModel.ModelProgressEnum.New);

                model.ModelProgress = progress;
                model.UpdateData(user);

                if (!ModelState.IsValid)
                    return PartialView(model);

                string msg = null;
                switch (progress) {
                    case EditModel.ModelProgressEnum.New:
                        progress = EditModel.ModelProgressEnum.ShowLogin;
                        break;
                    case EditModel.ModelProgressEnum.ShowLogin:
#if MVC6
                        if (!await Managers.GetUserManager().CheckPasswordAsync(user, model.Password))
#else
                        if (await Managers.GetUserManager().FindAsync(user.UserName, model.Password) == null)
#endif
                            ModelState.AddModelError(nameof(model.Password), this.__ResStr("badPassword", "The password is invalid"));

                        if (!ModelState.IsValid)
                            return PartialView(model);

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

                return FormProcessed(model, popupText: msg, ForceApply: true);
            }
        }

        private async Task GenerateRecoveryCodeAsync(UserDefinitionDataProvider userDP, UserDefinition user) {
            user.RecoveryCode = Guid.NewGuid().ToString().Substring(0, 12);
            UpdateStatusEnum status = await userDP.UpdateItemAsync(user);
            if (status != UpdateStatusEnum.OK)
                throw new Error(this.__ResStr("cantUpdate", "Updating user information failed - {0}", status));
        }
    }
}
