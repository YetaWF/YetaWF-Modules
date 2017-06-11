/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.UserProfile.DataProvider;
using YetaWF.Modules.UserProfile.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.UserProfile.Controllers {

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

            [Caption("Mailing Address Location"), Description("Select the location of your mailing address")]
            [UIHint("Enum")]
            public AddressTypeEnum AddressType { get; set; }

            [Caption("City"), Description("The city portion of your mailing address")]
            [UIHint("Text40"), StringLength(UserInfo.MaxCity), Trim, Required, ProcessIf("AddressType", AddressTypeEnum.US)]
            public string CityUS { get; set; }
            [Caption("State"), Description("The state of your mailing address")]
            [UIHint("USState"), StringLength(UserInfo.MaxState), Trim, Required, ProcessIf("AddressType", AddressTypeEnum.US)]
            public string StateUS { get; set; }
            [Caption("ZIP Code"), Description("The ZIP code of your mailing address - Use format 00000 or 00000-0000")]
            [UIHint("Text10"), StringLength(UserInfo.MaxZip), Trim, Required, ProcessIf("AddressType", AddressTypeEnum.US)]
            [ZipCodeValidation]
            public string ZipUS { get; set; }

            [Caption("City & Postal Code"), Description("The city and postal code for your mailing address")]
            [UIHint("Text40"), StringLength(UserInfo.MaxCity), Trim, Required, ProcessIf("AddressType", AddressTypeEnum.International)]
            public string CityInternational { get; set; }
            [Caption("Country"), Description("The country for your mailing address")]
            [UIHint("Text40"), StringLength(UserInfo.MaxCountry), Trim, Required, ProcessIf("AddressType", AddressTypeEnum.International)]
            public string Country { get; set; }

            [Caption("Telephone Number"), Description("Your telephone number - please include country code and extensions if necessary")]
            [UIHint("Text40"), StringLength(UserInfo.MaxTelephone), Required, Trim]
            public string Telephone { get; set; }
            [Caption("Email Address"), Description("Your email address - This is defined by your account on this site")]
            [UIHint("Email"), ReadOnly]
            public string Email { get; set; }

            public EditModel() { }

            public UserInfo GetData(UserInfo data) {
                ObjectSupport.CopyData(this, data);
                if (AddressType == AddressTypeEnum.US) {
                    data.City = CityUS;
                    data.State = StateUS;
                    data.Zip = ZipUS;
                    data.Country = null;
                } else {
                    data.City = CityInternational;
                    data.State = null;
                    data.Zip = null;
                    data.Country = Country;
                }
                return data;
            }
            public void SetData(UserInfo data) {
                ObjectSupport.CopyData(data, this);
                AddressType = string.IsNullOrWhiteSpace(Country) ? AddressTypeEnum.US : AddressTypeEnum.International;
                if (AddressType == AddressTypeEnum.US) {
                    CityUS = data.City;
                    StateUS = data.State;
                    ZipUS = data.Zip;
                } else {
                    CityInternational = data.City;
                }
            }
            public void UpdateData(UserInfo userInfo) {
                UserId = Manager.UserId;
                Email = Manager.UserEmail;
            }
        }

        [AllowGet]
        public ActionResult ProfileEdit() {
            Manager.NeedUser();
            using (UserInfoDataProvider userInfoDP = new UserInfoDataProvider()) {
                EditModel model = new EditModel { };
                UserInfo userInfo = userInfoDP.GetItem(Manager.UserId);
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
        public ActionResult ProfileEdit_Partial(EditModel model) {
            using (UserInfoDataProvider userInfoDP = new UserInfoDataProvider()) {
                Manager.NeedUser();

                bool newUser = false;
                UserInfo userInfo = userInfoDP.GetItem(model.UserId);
                if (userInfo == null) {
                    newUser = true;
                    userInfo = new UserInfo();
                }

                model.UpdateData(userInfo);
                if (!ModelState.IsValid)
                    return PartialView(model);

                userInfo = model.GetData(userInfo); // merge new data into original
                model.SetData(userInfo); // and all the data back into model for final display

                if (newUser)
                    userInfoDP.AddItem(userInfo);
                else
                    userInfoDP.UpdateItem(userInfo);
                return FormProcessed(model, this.__ResStr("okSaved", "Profile saved"));
            }
        }
    }
}
