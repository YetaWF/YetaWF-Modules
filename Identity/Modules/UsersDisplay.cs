/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules;

public class UsersDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, UsersDisplayModule>, IInstallableModel { }

[ModuleGuid("{e6c98552-d1fa-48aa-a690-e5f933dd71ac}"), PublishedModuleGuid]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Configuration")]
public class UsersDisplayModule : ModuleDefinition2 {

    public UsersDisplayModule() : base() {
        Title = this.__ResStr("modTitle", "User");
        Name = this.__ResStr("modName", "Display a User");
        Description = this.__ResStr("modSummary", "Used to display an existing user. This is used by the Users Module to display a user.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new UsersDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string url, string userName) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { UserName = userName },
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Display"),
            MenuText = this.__ResStr("displayText", "Display"),
            Tooltip = this.__ResStr("displayTooltip", "Display account information for user {0}", userName),
            Legend = this.__ResStr("displayLegend", "Displays account information for an existing user"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    public class DisplayModel {

        [Caption("Name"), Description("The name of the user")]
        [UIHint("String"), ReadOnly]
        public string UserName { get; set; }

        [Caption("Email Address"), Description("The email address of this user")]
        [UIHint("Email"), ReadOnly]
        public string Email { get; set; }

        [Caption("User Id"), Description("The internal id of this user")]
        [UIHint("IntValue"), ReadOnly]
        public int UserId { get; set; }

        [Caption("Status"), Description("The user's current account status")]
        [UIHint("Enum"), ReadOnly]
        public UserStatusEnum UserStatus { get; set; }

        [Caption("Roles"), Description("The user's roles")]
        [UIHint("YetaWF_Identity_UserRoles"), ReadOnly]
        public SerializableList<Role> RolesList { get; set; }

        [Caption("Comment"), Description("Comments")]
        [UIHint("TextArea"), ReadOnly]
        public string Comment { get; set; }

        [Caption("Verification Code"), Description("The verification code to verify the user")]
        [UIHint("String"), ReadOnly]
        public string VerificationCode { get; set; }

        [Caption("IP Address"), Description("The IP address from which the user registered on this site")]
        [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
        public string RegistrationIP { get; set; }

        [Caption("Created"), Description("The date/time the user account was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }

        [Caption("Login Provider"), Description("The external login provider(s) defining this account")]
        [UIHint("ListOfStrings"), ReadOnly]
        public List<string> LoginProviders { get; set; }

        [Caption("Last Login"), Description("The last time the user logged into his/her account")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime? LastLoginDate { get; set; }
        [Caption("Last Login IP"), Description("The IP address from which the user last logged on this site")]
        [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
        public string LastLoginIP { get; set; }
        [Caption("Last Password Change"), Description("The last time the user changed his/her password")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime? LastPasswordChangedDate { get; set; }
        [Caption("Password Change IP"), Description("The IP address from which the user last changed the password on this site")]
        [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
        public string PasswordChangeIP { get; set; }
        [Caption("Last Activity"), Description("The last time the user did anything on this site")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime? LastActivityDate { get; set; }
        [Caption("Last Activity IP"), Description("The IP address from which the user did anything on this site")]
        [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
        public string LastActivityIP { get; set; }

        [Caption("Login Failures"), Description("Shows the number of failed login attempts since the last successful login")]
        [UIHint("IntValue"), ReadOnly]
        public int LoginFailures { get; set; }

        public void SetData(UserDefinition user) {
            ObjectSupport.CopyData(user, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        string userName = Manager.RequestQueryString["UserName"] ?? throw new InternalError("User name missing");
        using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
            UserDefinition user = await dataProvider.GetItemAsync(userName);
            if (user == null)
                throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);
            DisplayModel model = new DisplayModel();
            model.SetData(user);
            using (UserLoginInfoDataProvider userLogInfoDP = new UserLoginInfoDataProvider()) {
                List<DataProviderFilterInfo> filters = null;
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(UserDefinition.UserId), Operator = "==", Value = user.UserId });
                DataProviderGetRecords<LoginInfo> list = await userLogInfoDP.GetItemsAsync(0, 0, null, filters);
                model.LoginProviders = (from LoginInfo l in list.Data select l.LoginProvider).ToList();
            }
            Title = this.__ResStr("modDisplayTitle", "User {0}", userName);
            return await RenderAsync(model);
        }
    }
}
