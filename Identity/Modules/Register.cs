/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class RegisterModuleDataProvider : ModuleDefinitionDataProvider<Guid, RegisterModule>, IInstallableModel { }

    public enum RegistrationTypeEnum {
        [EnumDescription("Name Only", "The user's name is used to register on this site")]
        NameOnly = 0,
        [EnumDescription("Email Only", "The user's email address is used to register on this site")]
        EmailOnly = 1,
        [EnumDescription("Name and Email", "The user's name and email address are used to register on this site")]
        NameAndEmail = 2,
    }

    [ModuleGuid("{60E09334-3ECA-466f-BDF9-9933971B0991}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class RegisterModule : ModuleDefinition {

        public RegisterModule() {
            Title = this.__ResStr("title", "New User Registration");
            Name = this.__ResStr("title", "New User Registration");
            Description = this.__ResStr("modSummary", "User registration");
            ShowHelp = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new RegisterModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new LoginConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("ChangeAccounts",
                        this.__ResStr("roleChangeC", "Change Accounts"), this.__ResStr("roleChange", "The role has permission to change other users' account status"),
                        this.__ResStr("userChangeC", "Change Accounts"), this.__ResStr("userChange", "The user has permission to change other users' account status")),
                };
            }
        }

        public override MenuList GetModuleMenuList(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = base.GetModuleMenuList(renderMode, location);
            LoginModule loginMod = (LoginModule)ModuleDefinition.CreateUniqueModule(typeof(LoginModule));
            bool closeOnLogin;
            Manager.TryGetUrlArg<bool>("CloseOnLogin", out closeOnLogin, false);
            ModuleAction logAction = loginMod.GetAction_Login(Manager.CurrentSite.LoginUrl, Force: true, CloseOnLogin: closeOnLogin);
            if (logAction != null)
                logAction.AddToOriginList = false;
            menuList.New(logAction, location);
            return menuList;
        }
        public ModuleAction GetAction_Register(string url, bool Force = false, bool CloseOnLogin = false) {
            LoginConfigData config = LoginConfigDataProvider.GetConfig();
            if (!config.AllowUserRegistration) return null;
            if (!Force && Manager.HaveUser) return null;
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = CloseOnLogin ? new { CloseOnLogin = CloseOnLogin } : null,
                Image = "Register.png",
                LinkText = this.__ResStr("regLink", "Register a new user account"),
                MenuText = this.__ResStr("regText", "Register"),
                Tooltip = this.__ResStr("regTooltip", "If you don't have an account on this site, click to register"),
                Legend = this.__ResStr("regLegend", "Allows you to register on this site as a new user"),
                Style = Manager.IsInPopup ? ModuleAction.ActionStyleEnum.ForcePopup : ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto | ModuleAction.ActionLocationEnum.InPopup | ModuleAction.ActionLocationEnum.ModuleLinks,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                SaveReturnUrl = true,
                AddToOriginList = true,
            };
        }
        public ModuleAction GetAction_Approve(string userName) {
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(RegisterModuleController), "Approve"),
                Image = "Approve.png",
                NeedsModuleContext = true,
                QueryArgs = new { UserName = userName },
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("sendApprovedLink", "Approve User"),
                MenuText = this.__ResStr("sendApprovedMenu", "Approve User"),
                Legend = this.__ResStr("sendApprovedLegend", "Marks the user account as approved"),
                Tooltip = this.__ResStr("sendApprovedTT", "Marks the user account as approved"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
        public ModuleAction GetAction_Reject(string userName) {
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(RegisterModuleController), "Reject"),
                Image = "Reject.png",
                NeedsModuleContext = true,
                QueryArgs = new { UserName = userName },
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("sendRejectedLink", "Reject User"),
                MenuText = this.__ResStr("sendRejectedMenu", "Reject User"),
                Legend = this.__ResStr("sendRejectedLegend", "Marks the user account as rejected"),
                Tooltip = this.__ResStr("sendRejectedTT", "Marks the user account as rejected"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
    }
}