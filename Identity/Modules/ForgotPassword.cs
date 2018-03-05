/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class ForgotPasswordModuleDataProvider : ModuleDefinitionDataProvider<Guid, ForgotPasswordModule>, IInstallableModel { }

    [ModuleGuid("{3437ee4d-747f-4bf1-aa3c-d0417751b24b}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class ForgotPasswordModule : ModuleDefinition {

        public ForgotPasswordModule() {
            Title = this.__ResStr("modTitle", "Forgot Password?");
            Name = this.__ResStr("modName", "Forgot Password?");
            Description = this.__ResStr("modSummary", "Sends an email for a forgotten password");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ForgotPasswordModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        public override MenuList GetModuleMenuList(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = base.GetModuleMenuList(renderMode, location);
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            RegisterModule regMod = (RegisterModule)ModuleDefinition.CreateUniqueModule(typeof(RegisterModule));
            LoginModule loginMod = (LoginModule)ModuleDefinition.CreateUniqueModule(typeof(LoginModule));
            bool closeOnLogin;
            Manager.TryGetUrlArg<bool>("CloseOnLogin", out closeOnLogin, false);

            ModuleAction logAction = loginMod.GetAction_Login(Manager.CurrentSite.LoginUrl, Force: true, CloseOnLogin: closeOnLogin);
            if (logAction != null)
                logAction.AddToOriginList = false;
            menuList.New(logAction, location);
            ModuleAction regAction = await regMod.GetAction_RegisterAsync(config.RegisterUrl, Force: true, CloseOnLogin: closeOnLogin);
            if (regAction != null)
                regAction.AddToOriginList = false;
            menuList.New(regAction, location);
            return menuList;
        }

        public async Task<ModuleAction> GetAction_ForgotPassword(string url, bool CloseOnLogin = false) {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            if (!config.SavePlainTextPassword) return null;
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = CloseOnLogin ? new { CloseOnLogin = CloseOnLogin } : null,
                Image = "#Help",
                LinkText = this.__ResStr("regLink", "Forgot your password?"),
                MenuText = this.__ResStr("regText", "Forgot your password?"),
                Tooltip = this.__ResStr("regTooltip", "If you have an account and forgot your password, click to have an email sent to you with your password"),
                Legend = this.__ResStr("regLegend", "Used to send an email to you with your password if you have an account and forgot your password"),
                Style = Manager.IsInPopup ? ModuleAction.ActionStyleEnum.ForcePopup : ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto | ModuleAction.ActionLocationEnum.InPopup | ModuleAction.ActionLocationEnum.ModuleLinks,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                SaveReturnUrl = true,
                AddToOriginList = false,
            };
        }
    }
}