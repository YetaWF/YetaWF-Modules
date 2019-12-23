/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyLogin#License */

using System;
using YetaWF.Core;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.TinyLogin.Support;
using System.Threading.Tasks;
#if MVC6
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.TinyLogin.Modules {

    public class TinyLoginModuleDataProvider : ModuleDefinitionDataProvider<Guid, TinyLoginModule>, IInstallableModel { }

    [ModuleGuid("{9e929bdc-8810-4710-ab3d-b7bced570e02}")] // Published Guid
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class TinyLoginModule : ModuleDefinition {

        public const int MaxTooltip = 100;

        public TinyLoginModule()
            : base() {
            Title = this.__ResStr("modTitle", "Tiny Login");
            Name = this.__ResStr("modName", "Tiny Login");
            Description = this.__ResStr("modSummary", "Provides Login/Register links and displays a logged on user's account name");
            AllowUserRegistration = true;
            UserTooltip = new MultiString();
            ShowTitle = false;
            WantSearch = false;
            WantFocus = false;
            Print = false;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TinyLoginModuleDataProvider(); }
        public override bool ShowActionMenu { get { return false; } }

        [Category("General"), Caption("Use Popup Windows"), Description("Use popup windows to Login and Register")]
        [UIHint("Boolean")]
        [Data_DontSave]
        public bool UsePopup { get { return !UseFullPage; } set { UseFullPage = !value; } }
        [Data_NewValue]
        public bool UseFullPage { get; set; }

        [Category("General"), Caption("Allow New Users"), Description("Allow registration of new users")]
        [UIHint("Boolean")]
        public bool AllowUserRegistration { get; set; }

        [Category("General")]
        [Caption("Log On Url"), Description("The Url where the user is redirected when the login link is clicked")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string LogonUrl { get; set; }

        [Category("General")]
        [Caption("Log Off Url"), Description("The Url where the user is redirected when the logoff link is clicked")]
        [UIHint("Text80"), LogoffUrlValidationAttribute]
        [StringLength(Globals.MaxUrl), Trim]
        public string LogoffUrl { get; set; }

        [Category("General")]
        [Caption("Register Url"), Description("The Url where the user is redirected when the register link is clicked")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string RegisterUrl { get; set; }

        [Category("General")]
        [Caption("User Url"), Description("The Url where the user is redirected when the user name is clicked")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string UserUrl { get; set; }

        [Category("General")]
        [Caption("User Tooltip"), Description("The tooltip shown for the user name link")]
        [UIHint("MultiString80"), StringLength(MaxTooltip), Trim]
        public MultiString UserTooltip { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction> GetAction_LoginAsync(string url) {
            return new ModuleAction(this) {
                Url = url,
                LinkText = this.__ResStr("loginLink", "Login"),
                MenuText = this.__ResStr("loginText", "Login"),
                Image = await CustomIconAsync("Login.png"),
                Tooltip = this.__ResStr("loginTooltip", "Click to log into this site using your existing account"),
                Legend = this.__ResStr("loginLegend", "Logs into this site using your existing account"),
                Style = UseFullPage ? ModuleAction.ActionStyleEnum.Normal : ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
        public async Task<ModuleAction> GetAction_RegisterAsync(string url) {
            if (!AllowUserRegistration) return null;
            return new ModuleAction(this) {
                Url = url,
                LinkText = this.__ResStr("registerLink", "Register"),
                MenuText = this.__ResStr("registerText", "Register"),
                Image = await CustomIconAsync("Register.png"),
                Tooltip = this.__ResStr("registerTooltip", "Click to register a new account for access to this site"),
                Legend = this.__ResStr("registerLegend", "register to access this site with a new account"),
                Style = UseFullPage ? ModuleAction.ActionStyleEnum.Normal : ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
        public async Task<ModuleAction> GetAction_LogoffAsync(string url) {
            return new ModuleAction(this) {
                Url = url,
                LinkText = this.__ResStr("logoffLink", "Logout"),
                MenuText = this.__ResStr("logoffText", "Logout"),
                Image = await CustomIconAsync("Logoff.png"),
                Tooltip = this.__ResStr("logoffTooltip", "Click to log off from this site"),
                Legend = this.__ResStr("logoffLegend", "Logs you out from this site"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                DontFollow = true,
            };
        }
        public async Task<ModuleAction> GetAction_UserNameAsync(string url, string userName, string tooltip) {
            return new ModuleAction(this) {
                Url = url,
                LinkText = userName,
                MenuText = userName,
                Image = await CustomIconAsync("UserName.png"),
                Tooltip = tooltip,
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}