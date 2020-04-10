/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class LoginModuleDataProvider : ModuleDefinitionDataProvider<Guid, LoginModule>, IInstallableModel { }

    [ModuleGuid("{47C80477-1F25-4f9d-902C-E3D8B3A62686}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    [ModuleCategory("Login & Registration")]
    public class LoginModule : ModuleDefinition {

        public LoginModule() {
            Title = this.__ResStr("title", "Login");
            Name = this.__ResStr("title", "Login");
            Description = this.__ResStr("modSummary", "Allows a user to enter account information to log into the site. The Login Module can be accessed using User > Login (standard YetaWF site).");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LoginModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            RegisterModule regMod = (RegisterModule) await ModuleDefinition.CreateUniqueModuleAsync(typeof(RegisterModule));
            ForgotPasswordModule pswdMod = (ForgotPasswordModule) await ModuleDefinition.CreateUniqueModuleAsync(typeof(ForgotPasswordModule));
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            bool closeOnLogin;
            Manager.TryGetUrlArg<bool>("CloseOnLogin", out closeOnLogin, false);
            ModuleAction pswdAction = await pswdMod.GetAction_ForgotPasswordAsync(config.ForgotPasswordUrl, CloseOnLogin: closeOnLogin);
            if (pswdAction != null)
                pswdAction.AddToOriginList = false;
            menuList.New(pswdAction, location);
            ModuleAction registerAction = await regMod.GetAction_RegisterAsync(config.RegisterUrl, Force: true, CloseOnLogin: closeOnLogin);
            if (registerAction != null)
                registerAction.AddToOriginList = false;
            menuList.New(registerAction, location);
            return menuList;
        }

        public async Task<ModuleAction> GetAction_LoginAsync(string url = null, bool Force = false, bool CloseOnLogin = false) {
            if (!Force && Manager.HaveUser) return null; // the login action should not be shown if a user is logged on
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = CloseOnLogin ? new { CloseOnLogin = CloseOnLogin } : null,
                Image = await CustomIconAsync("Login.png"),
                LinkText = this.__ResStr("loginLink", "Login using your existing account"),
                MenuText = this.__ResStr("loginText", "Login"),
                Tooltip = this.__ResStr("loginTooltip", "If you have an account on this site, click to log in"),
                Legend = this.__ResStr("loginLegend", "Logs into the site"),
                Style = Manager.IsInPopup ? ModuleAction.ActionStyleEnum.ForcePopup : ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto | ModuleAction.ActionLocationEnum.InPopup | ModuleAction.ActionLocationEnum.ModuleLinks,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                SaveReturnUrl = true,
                AddToOriginList = true,
            };
        }

        public async Task<ModuleAction> GetAction_LoginAsAsync(int userId, string userName) {
            if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowUserLogon))
                return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(LoginDirectController), nameof(LoginDirectController.LoginAs)),
                QueryArgs = new { UserId = userId },
                Image = await CustomIconAsync("LoginAs.png"),
                LinkText = this.__ResStr("loginAsLink", "Login a user {0}", userName),
                MenuText = this.__ResStr("loginAsText", "not used"),
                Tooltip = this.__ResStr("loginAsTooltip", "Log in as user {0}", userName),
                Legend = this.__ResStr("loginAsLegend", "Logs in as another user"),
                Style = ModuleAction.ActionStyleEnum.OuterWindow,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                DontFollow = true,
            };
        }
        public async Task<ModuleAction> GetAction_ResendVerificationEmailAsync(string userName) {
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(LoginModuleController), nameof(LoginModuleController.ResendVerificationEmail)),
                NeedsModuleContext = true,
                QueryArgs = new { UserName = userName },
                Image = await CustomIconAsync("VerificationEmail.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("sendVerificationLink", "Resend Verification Email"),
                MenuText = this.__ResStr("sendVerificationMenu", "Resend Verification Email"),
                Tooltip = this.__ResStr("sendVerificationTT", "Sends a verification email to you"),
                Legend = this.__ResStr("sendVerificationLegend", "Sends a verification email to the user"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto
            };
        }
    }
}
