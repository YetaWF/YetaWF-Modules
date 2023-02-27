/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.UserProfile.DataProvider;

namespace YetaWF.Modules.UserProfile.Modules;

public class ProfileEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, ProfileEditModule>, IInstallableModel { }

[ModuleGuid("{9ba8e8dc-7e04-492c-850d-27f0ca6fa2d3}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class ProfileEditModule : ModuleDefinition2 {

    public ProfileEditModule() {
        Title = this.__ResStr("modTitle", "User Profile");
        Name = this.__ResStr("modName", "Edit User Profile");
        Description = this.__ResStr("modSummary", "Edits the logged on user's profile (name, address). This is accessible using User > My Profile (standard YetaWF site).");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ProfileEditModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Post Save Url"), Description("Defines the page to display once the form is saved - If omitted, the Url to return to is determined automatically - This property is ignored when the module is displayed in a popup")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
    [StringLength(Globals.MaxUrl), Trim]
    public string? PostSaveUrl { get; set; }

    [Category("General"), Caption("Save Message"), Description("Defines the message displayed when the profile is successfully saved - If omitted, a default message is shown")]
    [UIHint("Text80"), StringLength(ModuleDefinition.MaxTitle), Trim]
    public string? SaveMessage { get; set; }

    public ModuleAction? GetAction_Edit(string? url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Profile"),
            MenuText = this.__ResStr("editText", "Profile"),
            Tooltip = this.__ResStr("editTooltip", "Edit your user profile"),
            Legend = this.__ResStr("editLegend", "Edits your user profile"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class EditModel {
        [UIHint("Hidden")]
        public int UserId { get; set; }

        [Caption("Name"), Description("Your name")]
        [UIHint("Text40"), StringLength(UserInfo.MaxName), Required, Trim]
        public string? Name { get; set; }
        [Caption("Company Name"), Description("Your company name")]
        [UIHint("Text40"), StringLength(UserInfo.MaxCompanyName), Trim]
        public string? CompanyName { get; set; }
        [Caption("Street Address"), Description("The street portion of your mailing address")]
        [UIHint("Text40"), StringLength(UserInfo.MaxAddress), Required, Trim]
        public string? Address1 { get; set; }
        [Caption("Street Address (opt)"), Description("The optional second line of your mailing address")]
        [UIHint("Text40"), StringLength(UserInfo.MaxAddress), Trim]
        public string? Address2 { get; set; }

        [Caption("Country"), Description("The country for your mailing address")]
        [UIHint("CountryISO3166"), StringLength(UserInfo.MaxCountry), Trim, Required, SubmitFormOnChange(SubmitFormOnChangeAttribute.SubmitTypeEnum.Apply)]
        public string? Country { get; set; }
        public string? AddressType { get { return string.IsNullOrWhiteSpace(Country) ? null : CountryISO3166.CountryToAddressType(Country); } }

        // US - United States
        [Caption("City"), Description("The city portion of your mailing address")]
        [UIHint("Text40"), StringLength(UserInfo.MaxCity), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.US)]
        public string? CityUS { get; set; }
        [Caption("State"), Description("The state of your mailing address")]
        [UIHint("USState"), StringLength(UserInfo.MaxState), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.US)]
        public string? StateUS { get; set; }
        [Caption("ZIP Code"), Description("The ZIP code of your mailing address - Use format 00000 or 00000-0000")]
        [UIHint("Text10"), StringLength(UserInfo.MaxZip), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.US)]
        [ZipCodeValidation]
        public string? ZipUS { get; set; }

        // Zip1 - Postal code first
        [Caption("Postal Code"), Description("The postal code for your mailing address")]
        [UIHint("Text20"), StringLength(UserInfo.MaxZip), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.Zip1)]
        public string? ZipZip1 { get; set; }
        [Caption("City"), Description("The city portion of your mailing address")]
        [UIHint("Text40"), StringLength(UserInfo.MaxCity), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.Zip1)]
        public string? CityZip1 { get; set; }

        // ZipLast - Postal code last
        [Caption("City"), Description("The city portion of your mailing address")]
        [UIHint("Text40"), StringLength(UserInfo.MaxCity), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.ZipLast)]
        public string? CityZipLast { get; set; }
        [Caption("Postal Code"), Description("The postal code of your mailing address")]
        [UIHint("Text20"), StringLength(UserInfo.MaxZip), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.ZipLast)]
        public string? ZipZipLast { get; set; }

        // Generic
        [Caption("City"), Description("The city portion of your mailing address")]
        [UIHint("Text40"), StringLength(UserInfo.MaxCity), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.Generic)]
        public string? CityGeneric { get; set; }
        [Caption("Postal Code"), Description("The postal code of your mailing address")]
        [UIHint("Text20"), StringLength(UserInfo.MaxZip), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.Generic)]
        public string? ZIPGeneric { get; set; }

#if EXAMPLE
        // DE - Germany (example, could be further customized with specific validation attributes)
        // In Countries.txt file at .\CoreComponents\Core\Addons\_Templates\CountryISO3166, add an address type entry DE as the last entry (instead of Zip1)
        // which enables custom processing for address type "DE" when Germany is selected. The address type "DE" could be any unique string and
        // doesn't have to match the country id.
        // Germany+DE+DEU+276+DE  instead of  Germany+DE+DEU+276+Zip1
        [Caption("PLZ"), Description("The Postleitzahl for your mailing address")]
        [UIHint("Text10"), StringLength(5), Trim, Required, SuppressIfNot("AddressType", "DE")]
        public string? ZipDE { get; set; }
        [Caption("City"), Description("The city for your mailing address")]
        [UIHint("Text20"), StringLength(UserInfo.MaxCity), Trim, Required, SuppressIfNot("AddressType", "DE")]
        public string? CityDE { get; set; }
#endif
        [Caption("Telephone Number"), Description("Your telephone number - please include country code and extension if necessary")]
        [UIHint("Text40"), StringLength(UserInfo.MaxTelephone), Required, Trim]
        public string? Telephone { get; set; }
        [Caption("Email Address"), Description("Your email address - This is defined by your account on this site")]
        [UIHint("Email"), ReadOnly]
        public string? Email { get; set; }

        public EditModel() { }

        public UserInfo GetData(UserInfo userInfo) {
            ObjectSupport.CopyData(this, userInfo);
            if (AddressType == CountryISO3166.Country.US) {
                userInfo.City = CityUS ?? string.Empty;
                userInfo.State = StateUS ?? string.Empty;
                userInfo.Zip = ZipUS ?? string.Empty;
            } else if (AddressType == CountryISO3166.Country.Zip1) {
                userInfo.City = CityZip1 ?? string.Empty;
                userInfo.State = string.Empty;
                userInfo.Zip = ZipZip1 ?? string.Empty;
            } else if (AddressType == CountryISO3166.Country.ZipLast) {
                userInfo.City = CityZipLast ?? string.Empty;
                userInfo.State = string.Empty;
                userInfo.Zip = ZipZipLast ?? string.Empty;
#if EXAMPLE
            } else if (AddressType == "DE") {
                userInfo.City = CityDE;
                userInfo.State = null;
                userInfo.Zip = ZipDE;
#endif
            } else if (AddressType == CountryISO3166.Country.Generic) {
                userInfo.City = CityGeneric ?? string.Empty;
                userInfo.State = string.Empty;
                userInfo.Zip = ZIPGeneric ?? string.Empty;
            } else
                throw new InternalError("Invalid address type {0}", AddressType);
            return userInfo;
        }
        public void SetData(UserInfo userInfo) {
            ObjectSupport.CopyData(userInfo, this);
            if (AddressType == CountryISO3166.Country.US) {
                CityUS = userInfo.City;
                StateUS = userInfo.State;
                ZipUS = userInfo.Zip;
            } else if (AddressType == CountryISO3166.Country.Zip1) {
                CityZip1 = userInfo.City;
                ZipZip1 = userInfo.Zip;
            } else if (AddressType == CountryISO3166.Country.ZipLast) {
                CityZipLast = userInfo.City;
                ZipZipLast = userInfo.Zip;
#if EXAMPLE
            } else if (AddressType == "DE") {
                CityDE = userInfo.City;
                ZipDE = userInfo.Zip;
#endif
            } else {
                CityGeneric = userInfo.City;
                ZIPGeneric = userInfo.Zip;
            }
        }
        public void UpdateData(UserInfo userInfo) {
            UserId = Manager.UserId;
            Email = Manager.UserEmail ?? string.Empty;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Manager.NeedUser();
        using (UserInfoDataProvider userInfoDP = new UserInfoDataProvider()) {
            EditModel model = new EditModel { };
            UserInfo? userInfo = await userInfoDP.GetItemAsync(Manager.UserId);
            if (userInfo == null)
                userInfo = new UserInfo { UserId = Manager.UserId };
            model.SetData(userInfo);
            model.UpdateData(userInfo);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        using (UserInfoDataProvider userInfoDP = new UserInfoDataProvider()) {
            Manager.NeedUser();

            bool newUser = false;
            UserInfo? userInfo = await userInfoDP.GetItemAsync(model.UserId);
            if (userInfo == null) {
                newUser = true;
                userInfo = new UserInfo();
            }
            model.UpdateData(userInfo);

            // in case of apply, we're just updating the form with new fields - we chose a postback when switching
            // the country as potentially many different address formats could be supported which could overwhelm client side processing
            // so we only provide the fields for one country to the form.
            if (IsApply) {
                ModelState.Clear();
                return await PartialViewAsync(model);
            }
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);

            userInfo = model.GetData(userInfo); // merge new data into original
            model.SetData(userInfo); // and all the data back into model for final display

            if (newUser)
                await userInfoDP.AddItemAsync(userInfo);
            else
                await userInfoDP.UpdateItemAsync(userInfo);

            string? msg = SaveMessage;
            if (string.IsNullOrWhiteSpace(msg))
                msg = this.__ResStr("okSaved", "Profile saved");
            if (string.IsNullOrWhiteSpace(PostSaveUrl))
                return await FormProcessedAsync(model, msg);
            else
                return await FormProcessedAsync(model, msg, OnClose: OnCloseEnum.GotoNewPage, NextPage: PostSaveUrl);
        }
    }
}