/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserSettings#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.UserSettings.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.UserSettings.Controllers {

    public class MinimalSettingsEditModuleController : ControllerImpl<YetaWF.Modules.UserSettings.Modules.MinimalSettingsEditModule> {

        public MinimalSettingsEditModuleController() { }

        [Trim]
        public class EditModel {

            [UIHint("Hidden")]
            public int Key { get; set; }

            [Caption("Time Zone"), Description("Your time zone - all dates/times within this web site will be adjusted for the specified time zone")]
            [UIHint("TimeZone"), StringLength(UserData.MaxTimeZone), Required]
            public string? TimeZone { get; set; }

            [Caption("Date Format"), Description("The desired date format when dates are displayed on this website")]
            [UIHint("Enum"), Required]
            public Formatting.DateFormatEnum DateFormat { get; set; }

            [Caption("Time Format"), Description("The desired time format when times are displayed on this website")]
            [UIHint("Enum"), Required]
            public Formatting.TimeFormatEnum TimeFormat { get; set; }

            public UserData GetData(UserData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(UserData data) {
                ObjectSupport.CopyData(data, this);
            }
            public EditModel() { }
        }

        [AllowGet]
        public async Task<ActionResult> MinimalSettingsEdit() {
            using (UserDataProvider dataProvider = new UserDataProvider()) {
                EditModel model = new EditModel { };
                UserData data = await dataProvider.GetItemAsync();
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> MinimalSettingsEdit_Partial(EditModel model) {
            using (UserDataProvider dataProvider = new UserDataProvider()) {
                UserData data = await dataProvider.GetItemAsync();
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                await dataProvider.UpdateItemAsync(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Your settings have been successfully saved"), ForceRedirect: true);
            }
        }
    }
}