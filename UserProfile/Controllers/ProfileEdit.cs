/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.UserProfile.Attributes;
using YetaWF.Modules.UserProfile.DataProvider;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.UserProfile.Controllers {

    // The Countries.txt file at .\CoreComponents\Core\Addons\_Templates\CountryISO3166 is used to define address types for each country.
    // If a 5th entry is present it defines the address type. Otherwise "Generic" is the default.

    public class ProfileEditModuleController : ControllerImpl<YetaWF.Modules.UserProfile.Modules.ProfileEditModule> {

        public ProfileEditModuleController() { }

        [Trim]
        public class EditModel {
            [UIHint("Hidden")]
            public int UserId { get; set; }

            [Caption("Name"), Description("Your name")]
            [UIHint("Text40"), StringLength(UserInfo.MaxName), Required, Trim]
            public string Name { get; set; }
            [Caption("Company Name"), Description("Your company name")]
            [UIHint("Text40"), StringLength(UserInfo.MaxCompanyName), Trim]
            public string CompanyName { get; set; }
            [Caption("Street Address"), Description("The street portion of your mailing address")]
            [UIHint("Text40"), StringLength(UserInfo.MaxAddress), Required, Trim]
            public string Address1 { get; set; }
            [Caption("Street Address (opt)"), Description("The optional second line of your mailing address")]
            [UIHint("Text40"), StringLength(UserInfo.MaxAddress), Trim]
            public string Address2 { get; set; }

            [Caption("Country"), Description("The country for your mailing address")]
            [UIHint("CountryISO3166"), StringLength(UserInfo.MaxCountry), Trim, Required, SubmitFormOnChange(SubmitFormOnChangeAttribute.SubmitTypeEnum.Apply)]
            public string Country { get; set; }
            public string AddressType { get { return string.IsNullOrWhiteSpace(Country) ? null : CountryISO3166.CountryToAddressType(Country); } }

            // US - United States
            [Caption("City"), Description("The city portion of your mailing address")]
            [UIHint("Text40"), StringLength(UserInfo.MaxCity), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.US)]
            public string CityUS { get; set; }
            [Caption("State"), Description("The state of your mailing address")]
            [UIHint("USState"), StringLength(UserInfo.MaxState), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.US)]
            public string StateUS { get; set; }
            [Caption("ZIP Code"), Description("The ZIP code of your mailing address - Use format 00000 or 00000-0000")]
            [UIHint("Text10"), StringLength(UserInfo.MaxZip), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.US)]
            [ZipCodeValidation]
            public string ZipUS { get; set; }

            // Zip1 - Postal code first
            [Caption("Postal Code"), Description("The postal code for your mailing address")]
            [UIHint("Text20"), StringLength(UserInfo.MaxZip), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.Zip1)]
            public string ZipZip1 { get; set; }
            [Caption("City"), Description("The city portion of your mailing address")]
            [UIHint("Text40"), StringLength(UserInfo.MaxCity), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.Zip1)]
            public string CityZip1 { get; set; }

            // ZipLast - Postal code last
            [Caption("City"), Description("The city portion of your mailing address")]
            [UIHint("Text40"), StringLength(UserInfo.MaxCity), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.ZipLast)]
            public string CityZipLast { get; set; }
            [Caption("Postal Code"), Description("The postal code of your mailing address")]
            [UIHint("Text20"), StringLength(UserInfo.MaxZip), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.ZipLast)]
            public string ZipZipLast { get; set; }

            // Generic
            [Caption("City"), Description("The city portion of your mailing address")]
            [UIHint("Text40"), StringLength(UserInfo.MaxCity), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.Generic)]
            public string CityGeneric { get; set; }
            [Caption("Postal Code"), Description("The postal code of your mailing address")]
            [UIHint("Text20"), StringLength(UserInfo.MaxZip), Trim, Required, SuppressIfNot("AddressType", CountryISO3166.Country.Generic)]
            public string ZIPGeneric { get; set; }

#if EXAMPLE
            // DE - Germany (example, could be further customized with specific validation attributes)
            // In Countries.txt file at .\CoreComponents\Core\Addons\_Templates\CountryISO3166, add an address type entry DE as the last entry (instead of Zip1)
            // which enables custom processing for address type "DE" when Germany is selected. The address type "DE" could be any unique string and
            // doesn't have to match the country id.
            // Germany+DE+DEU+276+DE  instead of  Germany+DE+DEU+276+Zip1
            [Caption("PLZ"), Description("The Postleitzahl for your mailing address")]
            [UIHint("Text10"), StringLength(5), Trim, Required, SuppressIfNot("AddressType", "DE")]
            public string ZipDE { get; set; }
            [Caption("City"), Description("The city for your mailing address")]
            [UIHint("Text20"), StringLength(UserInfo.MaxCity), Trim, Required, SuppressIfNot("AddressType", "DE")]
            public string CityDE { get; set; }
#endif
            [Caption("Telephone Number"), Description("Your telephone number - please include country code and extension if necessary")]
            [UIHint("Text40"), StringLength(UserInfo.MaxTelephone), Required, Trim]
            public string Telephone { get; set; }
            [Caption("Email Address"), Description("Your email address - This is defined by your account on this site")]
            [UIHint("Email"), ReadOnly]
            public string Email { get; set; }

            public EditModel() { }

            public UserInfo GetData(UserInfo userInfo) {
                ObjectSupport.CopyData(this, userInfo);
                if (AddressType == CountryISO3166.Country.US) {
                    userInfo.City = CityUS;
                    userInfo.State = StateUS;
                    userInfo.Zip = ZipUS;
                } else if (AddressType == CountryISO3166.Country.Zip1) {
                    userInfo.City = CityZip1;
                    userInfo.State = null;
                    userInfo.Zip = ZipZip1;
                } else if (AddressType == CountryISO3166.Country.ZipLast) {
                    userInfo.City = CityZipLast;
                    userInfo.State = null;
                    userInfo.Zip = ZipZipLast;
#if EXAMPLE
                } else if (AddressType == "DE") {
                    userInfo.City = CityDE;
                    userInfo.State = null;
                    userInfo.Zip = ZipDE;
#endif
                } else if (AddressType == CountryISO3166.Country.Generic) {
                    userInfo.City = CityGeneric;
                    userInfo.State = null;
                    userInfo.Zip = ZIPGeneric;
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
                Email = Manager.UserEmail;
            }
        }

        [AllowGet]
        public async Task<ActionResult> ProfileEdit() {
            Manager.NeedUser();
            using (UserInfoDataProvider userInfoDP = new UserInfoDataProvider()) {
                EditModel model = new EditModel { };
                UserInfo userInfo = await userInfoDP.GetItemAsync(Manager.UserId);
                if (userInfo == null)
                    userInfo = new UserInfo { UserId = Manager.UserId };
                model.SetData(userInfo);
                model.UpdateData(userInfo);
                return View(model);
            }
        }

        [AllowPost]
        [ExcludeDemoMode]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ProfileEdit_Partial(EditModel model) {
            using (UserInfoDataProvider userInfoDP = new UserInfoDataProvider()) {
                Manager.NeedUser();

                bool newUser = false;
                UserInfo userInfo = await userInfoDP.GetItemAsync(model.UserId);
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
                    return PartialView(model);
                }
                if (!ModelState.IsValid)
                    return PartialView(model);

                userInfo = model.GetData(userInfo); // merge new data into original
                model.SetData(userInfo); // and all the data back into model for final display

                if (newUser)
                    await userInfoDP.AddItemAsync(userInfo);
                else
                    await userInfoDP.UpdateItemAsync(userInfo);

                string msg = Module.SaveMessage;
                if (string.IsNullOrWhiteSpace(msg))
                    msg = this.__ResStr("okSaved", "Profile saved");
                if (string.IsNullOrWhiteSpace(Module.PostSaveUrl))
                    return FormProcessed(model, msg);
                else
                    return FormProcessed(model, msg, OnClose: OnCloseEnum.GotoNewPage, NextPage: Module.PostSaveUrl);
            }
        }
    }
}
