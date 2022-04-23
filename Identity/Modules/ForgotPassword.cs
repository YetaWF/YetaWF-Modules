/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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

    public class ForgotPasswordModuleDataProvider : ModuleDefinitionDataProvider<Guid, ForgotPasswordModule>, IInstallableModel { }

    [ModuleGuid("{3437ee4d-747f-4bf1-aa3c-d0417751b24b}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    [ModuleCategory("Login & Registration")]
    public class ForgotPasswordModule : ModuleDefinition {

        public ForgotPasswordModule() {
            Title = this.__ResStr("modTitle", "Forgot Password?");
            Name = this.__ResStr("modName", "Forgot Password?");
            Description = this.__ResStr("modSummary", "Allows a user to retrieve/reset a forgotten password for an existing account. This is used by the Login Module.");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ForgotPasswordModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction> GetAction_ForgotPasswordAsync(string url, bool CloseOnLogin = false) {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            if (config.SavePlainTextPassword) {
                return new ModuleAction(this) {
                    Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                    QueryArgs = CloseOnLogin ? new { CloseOnLogin = CloseOnLogin } : null,
                    Image = "#Help",
                    LinkText = this.__ResStr("forgotLink", "Forgot your password?"),
                    MenuText = this.__ResStr("forgotText", "Forgot your password?"),
                    Tooltip = this.__ResStr("forgotTooltip", "If you have an account and forgot your password, click to have an email sent to you with your password"),
                    Legend = this.__ResStr("forgotLegend", "Used to send an email to you with your password if you have an account and forgot your password"),
                    Style = Manager.IsInPopup ? ModuleAction.ActionStyleEnum.ForcePopup : ModuleAction.ActionStyleEnum.Normal,
                    Location = ModuleAction.ActionLocationEnum.NoAuto | ModuleAction.ActionLocationEnum.InPopup | ModuleAction.ActionLocationEnum.ModuleLinks,
                    Category = ModuleAction.ActionCategoryEnum.Update,
                    Mode = ModuleAction.ActionModeEnum.Any,
                    SaveReturnUrl = false,
                    AddToOriginList = false,
                };
            } else {
                return new ModuleAction(this) {
                    Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                    QueryArgs = CloseOnLogin ? new { CloseOnLogin = CloseOnLogin } : null,
                    Image = "#Help",
                    LinkText = this.__ResStr("resetLink", "Forgot your password?"),
                    MenuText = this.__ResStr("resetText", "Forgot your password?"),
                    Tooltip = this.__ResStr("resetTooltip", "If you have an account and forgot your password, click to have an email sent to you to reset your password"),
                    Legend = this.__ResStr("resetLegend", "Used to send an email to you to reset your password if you have an account and forgot your password"),
                    Style = Manager.IsInPopup ? ModuleAction.ActionStyleEnum.ForcePopup : ModuleAction.ActionStyleEnum.Normal,
                    Location = ModuleAction.ActionLocationEnum.NoAuto | ModuleAction.ActionLocationEnum.InPopup | ModuleAction.ActionLocationEnum.ModuleLinks,
                    Category = ModuleAction.ActionCategoryEnum.Update,
                    Mode = ModuleAction.ActionModeEnum.Any,
                    SaveReturnUrl = false,
                    AddToOriginList = false,
                };
            }
        }
    }
}
