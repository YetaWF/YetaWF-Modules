/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
#else
using System.Web.Mvc;
#endif

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
    [ModuleCategory("Login & Registration")]
    public class RegisterModule : ModuleDefinition {

        public RegisterModule() {
            Title = this.__ResStr("title", "New User Registration");
            Name = this.__ResStr("title", "New User Registration");
            Description = this.__ResStr("modSummary", "Used by new users to register a new account on the current site. The User Login Settings Module can be used to disable new user registration. The New User Registration can be accessed using User > Register (standard YetaWF site).");
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

        [Category("General"), Caption("Post Login Url"), Description("Defines the page to display once the user is logged on - If omitted, the Url to return to is determined automatically")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string PostRegisterUrl { get; set; }

        [Category("General"), Caption("Post Login Query String"), Description("Defines whether the original query string is forwarded with the Post Login Url")]
        [UIHint("Boolean"), ProcessIfSupplied(nameof(PostRegisterUrl))]
        [Data_NewValue]
        public bool PostRegisterQueryString { get; set; }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            LoginModule loginMod = (LoginModule) await ModuleDefinition.CreateUniqueModuleAsync(typeof(LoginModule));
            bool closeOnLogin;
            Manager.TryGetUrlArg<bool>("CloseOnLogin", out closeOnLogin, false);
            ModuleAction logAction = await loginMod.GetAction_LoginAsync(config.LoginUrl, Force: true, CloseOnLogin: closeOnLogin);
            if (logAction != null)
                logAction.AddToOriginList = false;
            menuList.New(logAction, location);
            return menuList;
        }
        public async Task<ModuleAction> GetAction_RegisterAsync(string url, bool Force = false, bool CloseOnLogin = false) {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            if (!config.AllowUserRegistration) return null;
            if (!Force && Manager.HaveUser) return null;
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = CloseOnLogin ? new { CloseOnLogin = CloseOnLogin } : null,
                Image = await CustomIconAsync("Register.png"),
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
        public async Task<ModuleAction> GetAction_ApproveAsync(string userName) {
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(RegisterModuleController), nameof(RegisterModuleController.Approve)),
                Image = await CustomIconAsync("Approve.png"),
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
        public async Task<ModuleAction> GetAction_RejectAsync(string userName) {
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(RegisterModuleController), nameof(RegisterModuleController.Reject)),
                Image = await CustomIconAsync("Reject.png"),
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
