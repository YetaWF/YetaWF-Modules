/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
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
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Modules;

public class UserAccountModuleDataProvider : ModuleDefinitionDataProvider<Guid, UserAccountModule>, IInstallableModel { }

[ModuleGuid("{222d53c2-8c9e-41df-8366-96060a4f5b57}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Login & Registration")]
public class UserAccountModule : ModuleDefinition {

    public UserAccountModule() : base() {
        Title = this.__ResStr("modTitle", "User Account");
        Name = this.__ResStr("modName", "User Account");
        Description = this.__ResStr("modSummary", "Used to edit the current user's account user name or email address and displays additional information. The User Account Module can be accessed using User > Account (standard YetaWF site).");
        DefaultViewName = StandardViews.EditApply;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new UserAccountModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return UserLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Account"),
            MenuText = this.__ResStr("editText", "Account"),
            Tooltip = this.__ResStr("editTooltip", "Edit your account information"),
            Legend = this.__ResStr("editLegend", "Edits the current user's account information"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,

        };
    }

    [Trim]
    public class EditModel {

        [Caption("Name"), Description("Your user name")]
        [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Trim]
        [SuppressIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
        [RequiredIfNot(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
        public string UserName { get; set; }

        [Caption("Email Address"), Description("Enter your email address that can be used by this site to communicate with you")]
        [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
        [SuppressIf(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
        [RequiredIfNot(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
        public string Email { get; set; }

        [Caption("Last Login"), Description("The last time you logged into your account")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime? LastLoginDate { get; set; }

        [Caption("Last Login IP"), Description("The IP address from which the user last logged on this site")]
        [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
        public string LastLoginIP { get; set; }

        [Caption("Last Password Change"), Description("The last time you changed your password")]
        [UIHint("DateTime"), ReadOnly]
        [SuppressEmpty]
        public DateTime? LastPasswordChangedDate { get; set; }
        [Caption("Password Change IP"), Description("The IP address from which the user last changed the password on this site")]
        [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
        [SuppressEmpty]
        public string PasswordChangeIP { get; set; }

        [UIHint("Hidden")]
        public RegistrationTypeEnum RegistrationType { get; set; }
        [UIHint("Hidden")]
        public string OriginalUserName { get; set; }

        public UserDefinition GetData() {
            UserDefinition data = new UserDefinition();
            ObjectSupport.CopyData(this, data);
            return data;
        }

        public void SetData(UserDefinition data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            EditModel model = new EditModel {
                RegistrationType = config.RegistrationType,
            };

            // make sure this user exists
            if (!Manager.CurrentContext.User.Identity.IsAuthenticated) {
                throw new Error(this.__ResStr("noUser", "There is no logged on user."));
            }
            string userName = Manager.CurrentContext.User.Identity.Name;
            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            UserDefinition user;
            user = await userManager.FindByNameAsync(userName);
            if (user == null)
                throw new Error(this.__ResStr("notFound", "User \"{0}\" not found."), userName);
            model.SetData(user);
            model.OriginalUserName = user.UserName;

            Title = this.__ResStr("modEditTitle", "User Account");
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        // make sure this user exists
        UserManager<UserDefinition> userManager = Managers.GetUserManager();
        UserDefinition user;
        user = await userManager.FindByNameAsync(model.OriginalUserName);
        if (user == null)
            throw new Error(this.__ResStr("alreadyDeleted", "The user named \"{0}\" has been removed and can no longer be updated.", model.OriginalUserName));
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        // update email/user name - can't use an existing email address
        // get the registration module for some defaults
        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        switch (config.RegistrationType) {
            default:
            case RegistrationTypeEnum.NameAndEmail: {
                    using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                        List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = nameof(UserDefinition.Email), Operator = "==", Value = model.Email, });
                        UserDefinition userExists = await dataProvider.GetItemAsync(filters);
                        if (userExists != null && user.UserName != userExists.UserName) {
                            ModelState.AddModelError(nameof(model.Email), this.__ResStr("emailUsed", "An account using email address {0} already exists.", model.Email));
                            return await PartialViewAsync(model);
                        }
                    }
                    break;
                }
            case RegistrationTypeEnum.EmailOnly:
                model.UserName = model.Email;
                break;
            case RegistrationTypeEnum.NameOnly:
                model.Email = model.UserName;
                break;
        }

        // save new user info
        ObjectSupport.CopyData(model, user); // merge new data into original
        model.SetData(user); // and all the data back into model for final display

        if (model.OriginalUserName != user.UserName) {
            // user name changed - change data through data provider directly
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UpdateStatusEnum status = await dataProvider.UpdateItemAsync(model.OriginalUserName, user);
                switch (status) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        ModelState.AddModelError(nameof(model.UserName), this.__ResStr("alreadyDeleted", "The user named \"{0}\" has been removed and can no longer be updated.", model.OriginalUserName));
                        return await PartialViewAsync(model);
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError(nameof(model.UserName), this.__ResStr("alreadyExists", "A user named \"{0}\" already exists.", model.UserName));
                        return await PartialViewAsync(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
            }
            // log the user off and back on so new name takes effect
            //IAuthenticationManager authManager = HttpContext.GetOwinContext().Authentication;
            //deleted, done in UserLogff authManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalBearer);
            await LoginModule.UserLogoffAsync();
            await LoginModule.UserLoginAsync(user);
        } else {
            IdentityResult result;
            result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Error(string.Join(" - ", (from e in result.Errors select e.Description)));
        }
        return await FormProcessedAsync(model, this.__ResStr("okSaved", "Your account information has been saved"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage, ForceReload: true);// reload for tiny login module to refresh
    }
}
