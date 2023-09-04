/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyBar#License */

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Menus.Modules;
using YetaWF.Modules.TinyBar.Support;
using static YetaWF.Modules.ComponentsHTML.Components.MenuComponentBase;

namespace YetaWF.Modules.TinyBar.Modules;

public class TinyBarModuleDataProvider : ModuleDefinitionDataProvider<Guid, TinyBarModule>, IInstallableModel { }

[ModuleGuid("{20ABACDB-97FC-40a0-ABFE-29813B6D5968}"), PublishedModuleGuid]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class TinyBarModule : MenuModule {

    public const int MaxTooltip = 100;

    public TinyBarModule() : base() {
        Title = this.__ResStr("modTitle", "Tiny Bar");
        Name = this.__ResStr("modName", "Tiny Bar");
        Description = this.__ResStr("modSummary", "Provides user account info, search and language selection. This module is typically added to every page (as a skin module) so users can select the language and access user information.");
        ShowTitle = false;
        WantSearch = false;
        WantFocus = false;
        Print = false;
        UsePartialFormCss = false;

        LogoffUrl = "/!api/YetaWF_Identity/LoginDirect/Logoff";
        OpenOnHover = false;
        HoverDelay = 300;

        //AllowUserRegistration = true;
        UserTooltip = new MultiString();
    }

    public override IModuleDefinitionIO GetDataProvider() { return new TinyBarModuleDataProvider(); }

    // Properties that are inherited but we don't to show them and don't need them
    [Data_DontSave]
    public new string? EditUrl { get; set; }
    [Data_DontSave]
    public new MenuComponentBase.OrientationEnum Orientation { get; set; }
    [Data_DontSave]
    public new int VerticalWidth { get; set; }
    [Data_DontSave]
    public new bool MiniScroll { get; set; }
    [Data_DontSave]
    public new int SmallMenuMaxWidth { get; set; }
    [Data_DontSave]
    public new string? LICssClass { get; set; }

    [Category("General"), Caption("Allow New Users"), Description("Allow registration of new users")]
    [UIHint("Boolean")]
    [Data_NewValue]
    public bool AllowUserRegistration { get; set; }

    [Category("General"), Caption("Use Popup Windows"), Description("Use popup windows to Login and Register")]
    [UIHint("Boolean")]
    [Data_NewValue]
    public bool UsePopup { get; set; }

    [Category("General")]
    [Caption("Log On Url"), Description("The Url where the user is redirected when the login link is clicked")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
    [StringLength(Globals.MaxUrl), Trim]
    [Data_NewValue]
    public string? LogonUrl { get; set; }

    [Category("General")]
    [Caption("Register Url"), Description("The Url where the user is redirected when the register link is clicked")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
    [StringLength(Globals.MaxUrl), Trim]
    [Data_NewValue]
    public string? RegisterUrl { get; set; }

    [Category("General")]
    [Caption("Search Url"), Description("The Url where the user is redirected when the search icon is clicked - If the Search Url is omitted, the search icon is not available")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
    [StringLength(Globals.MaxUrl), Trim]
    [Data_NewValue]
    public string? SearchUrl { get; set; }

    [Category("General")]
    [Caption("User Url"), Description("The Url where the user is redirected when the user name is clicked")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
    [StringLength(Globals.MaxUrl), Trim]
    public string? UserUrl { get; set; }

    [Category("General")]
    [Caption("User Tooltip"), Description("The tooltip shown for the user name link")]
    [UIHint("MultiString80"), StringLength(MaxTooltip), Trim]
    public MultiString UserTooltip { get; set; }

    [Category("General")]
    [Caption("Log Off Url"), Description("The Url where the user is redirected when the logoff link is clicked")]
    [UIHint("Text80"), LogoffUrlValidation]
    [StringLength(Globals.MaxUrl), Trim]
    public string? LogoffUrl { get; set; }

    [Category("General"), Caption("Open On Hover"), Description("Defines whether the dropdown is automatically opened when hovering over the menu")]
    [UIHint("Boolean")]
    [Data_NewValue]
    public bool OpenOnHover { get; set; }

    [Category("General"), Caption("Hover Delay"), Description("Specifies the delay (in milliseconds) before the menu is closed - Used to avoid accidental closure on leaving")]
    [UIHint("IntValue4"), Range(0, 10000)]
    [Data_NewValue]
    public new int HoverDelay { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public async Task<ModuleAction> GetAction_LoginAsync(string? url) {
        return new ModuleAction() {
            Url = url,
            LinkText = this.__ResStr("loginLink", "Login"),
            MenuText = this.__ResStr("loginText", "Login"),
            Image = await CustomIconAsync("Login.png"),
            Tooltip = this.__ResStr("loginTooltip", "Click to log into this site using your existing account"),
            Legend = this.__ResStr("loginLegend", "Logs into this site using your existing account"),
            Style = UsePopup ? ModuleAction.ActionStyleEnum.Popup : ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }
    public async Task<ModuleAction?> GetAction_RegisterAsync(string? url) {
        if (!AllowUserRegistration) return null;
        return new ModuleAction() {
            Url = url,
            LinkText = this.__ResStr("registerLink", "Register"),
            MenuText = this.__ResStr("registerText", "Register"),
            Image = await CustomIconAsync("Register.png"),
            Tooltip = this.__ResStr("registerTooltip", "Click to register a new account for access to this site"),
            Legend = this.__ResStr("registerLegend", "register to access this site with a new account"),
            Style = UsePopup ? ModuleAction.ActionStyleEnum.Popup : ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }

    public async Task<ModuleAction?> GetAction_LogoffAsync(string? url) {
        if (string.IsNullOrWhiteSpace(url)) return null;
        return new ModuleAction() {
            Url = url,
            LinkText = this.__ResStr("logoffLink", "Logout"),
            MenuText = this.__ResStr("logoffText", "Logout"),
            Image = await CustomIconAsync("Logoff.png"),
            ImageSVG = SkinSVGs.GetSkin("FAV_UserLogoff"),
            Tooltip = this.__ResStr("logoffTooltip", "Click to log off from this site"),
            Legend = this.__ResStr("logoffLegend", "Logs you out from this site"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            DontFollow = true,
        };
    }
    public async Task<ModuleAction> GetAction_UserNameAsync(string? url, string userName, string tooltip) {
        return new ModuleAction() {
            Url = url,
            LinkText = userName,
            MenuText = userName,
            ImageSVG = SkinSVGs.GetSkin("FAV_UserLogin"),
            Image = await CustomIconAsync("UserName.png"),
            Tooltip = tooltip,
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }

    [Trim]
    public class TinyBarModel {
        public bool LoggedOn { get; set; }
        public MenuData MenuData { get; set; } = null!;

        public TinyBarModel() { }
    }

    public new async Task<ActionInfo> RenderModuleAsync() {

        ModuleAction? searchAction = null;
        if (!string.IsNullOrWhiteSpace(SearchUrl)) {
            ModuleDefinition? searchMod = await ModuleDefinition.LoadAsync(new Guid("{c7991e91-c691-449a-a911-e5feacfba8a4}"), AllowNone: true);// Search Module
            if (searchMod != null) {
                searchAction = await searchMod.GetModuleActionAsync("Search", SearchUrl, null);
                if (searchAction != null) {
                    searchAction.CssClass = CssManager.CombineCss(searchAction.CssClass, "t_search");
                    searchAction.MenuText = string.Empty;
                }
            }
        }

        ModuleAction? shoppingAction = null;
        ModuleDefinition ? shppingCart = await ModuleDefinition.LoadAsync(new Guid("{11bdfc0e-c2b0-4d8a-bfb2-16369fc991f0}"), AllowNone: true);// StoreCatalog,TinyShoppingCart module
        if (shppingCart != null) {
            shoppingAction = await shppingCart.GetModuleActionAsync("ShoppingCart");
            if (shoppingAction != null) {
                shoppingAction.CssClass = CssManager.CombineCss(shoppingAction.CssClass, "t_shopping");
                shoppingAction.MenuText = string.Empty;
            }
        }

        // language selection
        List<ModuleAction> langActions = new List<ModuleAction>();
        if (MultiString.Languages.Count > 0) {
            foreach (LanguageData lang in MultiString.Languages) {
                string url = "/";// changed client side
                QueryHelper qh = QueryHelper.FromUrl(url, out string urlOnly);
                qh.Add(Globals.Link_Language, lang.Id);
                url = qh.ToUrl(urlOnly);
                langActions.New(new ModuleAction {
                    MenuText = lang.ShortName,
                    LinkText = lang.ShortName,
                    Tooltip = lang.Description,
                    Url = url,
                });
            }
        }

        // start of menu (user name)
        List<ModuleAction> userActions = new List<ModuleAction>();
        if (Manager.HaveUser) {
            userActions.New(await GetAction_UserNameAsync(UserUrl, Manager.UserName!, UserTooltip));
            // user-defined menu
            MenuList definedMenu = await GetMenu();
            if (definedMenu.Count > 0) {
                userActions.New(new ModuleAction { Separator = true });
                foreach (ModuleAction action in definedMenu)
                    userActions.New(action);
            }
            // menu end - logoff
            if (!string.IsNullOrWhiteSpace(LogoffUrl)) {
                userActions.New(new ModuleAction { Separator = true });
                userActions.New(await GetAction_LogoffAsync(LogoffUrl));
            }
        }

        // menu root
        MenuList menu = new MenuList();
        menu.New(searchAction);
        menu.New(shoppingAction);
        if (langActions.Count > 0) {
            menu.New(new ModuleAction {
                MenuText = "",
                ImageSVG = SkinSVGs.GetSkin("FAV_Languages"),
                SubMenu = new Core.Serializers.SerializableList<ModuleAction>(langActions),
                Tooltip = this.__ResStr("langSel", "Click to select one of the available languages this website supports"),
                CssClass = "t_lang",
            });
        }
        if (Manager.HaveUser) {
            menu.New(new ModuleAction {
                MenuText = (Manager.UserEmail?.Substring(0, 1) ?? Manager.UserName?.Substring(0, 1) ?? "?").ToUpper(),
                SubMenu = new Core.Serializers.SerializableList<ModuleAction>(userActions),
                CssClass = "t_user",
            });
        } else {
            ModuleAction? loginAction = await GetAction_LoginAsync(LogonUrl);
            if (loginAction != null) {
                loginAction.CssClass = CssManager.CombineCss(loginAction.CssClass, "t_login");
                menu.New(loginAction);
            }
            ModuleAction? registerAction = await GetAction_RegisterAsync(RegisterUrl);
            if (registerAction != null) {
                registerAction.CssClass = CssManager.CombineCss(registerAction.CssClass, "t_register");
                menu.New(registerAction);
            }
        }

        TinyBarModel model = new TinyBarModel {
            LoggedOn = Manager.HaveUser,
            MenuData = new MenuData {
                HoverDelay = HoverDelay,
                Orientation = MenuComponentBase.OrientationEnum.Horizontal,
                OpenOnHover = OpenOnHover,
                MenuList = menu,
                SmallMenuMaxWidth = 0,
                VerticalWidth = 0,
                HorizontalAlign = MenuComponentBase.HorizontalAlignEnum.Left,
                WantArrows = false,
            },
        };
        return await RenderAsync(model);
    }
}