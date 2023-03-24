/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
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
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Modules;
public class UsersAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, UsersAddModule>, IInstallableModel { }

[ModuleGuid("{55928a06-793e-46d1-929e-e403a59de98a}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Configuration")]
public class UsersAddModule : ModuleDefinition {

    public UsersAddModule() {
        Title = this.__ResStr("modTitle", "New User");
        Name = this.__ResStr("modName", "New User");
        Description = this.__ResStr("modSummary", "Adds a new user. This is used by the Users Module to add a new user.");
        DefaultViewName = StandardViews.Add;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new UsersAddModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Add(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add"),
            MenuText = this.__ResStr("addText", "Add"),
            Tooltip = this.__ResStr("addTooltip", "Create a new user"),
            Legend = this.__ResStr("addLegend", "Creates a new user"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class AddModel {
        [Caption("Name"), Description("The name of the user")]
        [UIHint("Text40"), StringLength(Globals.MaxUser), UserNameValidation, Trim]
        [SuppressIf(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
        [RequiredIfNot(nameof(RegistrationType), RegistrationTypeEnum.EmailOnly)]
        public string UserName { get; set; }

        [Caption("Email Address"), Description("The email address of this user")]
        [UIHint("Email"), SuppressIf("RegistrationType", RegistrationTypeEnum.NameOnly), StringLength(Globals.MaxEmail), EmailValidation, Trim]
        [RequiredIfNot(nameof(RegistrationType), RegistrationTypeEnum.NameOnly)]
        public string Email { get; set; }

        [Caption("Password"), Description("The password for this user")]
        [UIHint("Text20"), StringLength(Globals.MaxPswd), Required]
        public string Password { get; set; }

        [Caption("Status"), Description("The user's current account status")]
        [UIHint("Enum"), Required]
        public UserStatusEnum UserStatus { get; set; }

        [Caption("New Password"), Description("Defines whether the user must change the password")]
        [UIHint("Boolean")]
        public bool NeedsNewPassword { get; set; }

        [Caption("Roles"), Description("The user's roles")]
        [UIHint("YetaWF_Identity_UserRoles")]
        public SerializableList<Role> RolesList { get; set; }

        [Caption("Comment"), Description("Comments")]
        [UIHint("TextAreaSourceOnly"), StringLength(UserDefinition.MaxComment)]
        public string Comment { get; set; }

        [UIHint("Hidden")]
        public RegistrationTypeEnum RegistrationType { get; set; }

        public AddModel() { }

        public UserDefinition SetData(UserDefinition data) {
            ObjectSupport.CopyData(data, this);
            return data;
        }
        public UserDefinition GetData() {
            UserDefinition data = new UserDefinition();
            ObjectSupport.CopyData(this, data);
            return data;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        AddModel model = new AddModel();
        model.SetData(new UserDefinition());
        model.RegistrationType = config.RegistrationType;
        return await RenderAsync(model);
    }

    public async Task<IResult> UpdateModuleAsync(AddModel model) {

        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {

            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            switch (config.RegistrationType) {
                default:
                case RegistrationTypeEnum.NameAndEmail: { // Email == model.Email
                        List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = nameof(UserDefinition.Email), Operator = "==", Value = model.Email, });
                        UserDefinition userExists = await dataProvider.GetItemAsync(filters);
                        if (userExists != null) {
                            ModelState.AddModelError(nameof(model.Email), this.__ResStr("emailUsed", "An account using email address {0} already exists.", model.Email));
                            return await PartialViewAsync(model);
                        }
                        break;
                    }
                case RegistrationTypeEnum.EmailOnly:
                    model.UserName = model.Email;
                    break;
                case RegistrationTypeEnum.NameOnly:
                    model.UserName = model.Email;
                    break;
            }

            UserDefinition user = model.GetData();

            IPasswordHasher<UserDefinition> passwordHasher = (IPasswordHasher<UserDefinition>)Manager.ServiceProvider.GetService(typeof(IPasswordHasher<UserDefinition>));
            string hashedNewPassword = passwordHasher.HashPassword(user, model.Password);
            user.PasswordPlainText = config.SavePlainTextPassword ? model.Password : null;
            user.PasswordHash = hashedNewPassword;

            if (!await dataProvider.AddItemAsync(user))
                throw new Error(this.__ResStr("alreadyExists", "A user named \"{0}\" already exists."), model.UserName);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New user saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}

