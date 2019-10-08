/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserSettings#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.UserSettings.DataProvider;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.UserSettings.Controllers {

    public class SettingsEditModuleController : ControllerImpl<YetaWF.Modules.UserSettings.Modules.SettingsEditModule> {

        public SettingsEditModuleController() { }

        [Trim]
        public class EditModel {

            [UIHint("Hidden")]
            public int Key { get; set; }

            [Caption("Time Zone"), Description("Your time zone - all dates/times within this web site will be adjusted for the specified time zone")]
            [UIHint("TimeZone"), StringLength(UserData.MaxTimeZone), Required]
            public string TimeZone { get; set; }

            [Caption("Date Format"), Description("The desired date format when dates are displayed on this website")]
            [UIHint("Enum"), Required]
            public Formatting.DateFormatEnum DateFormat { get; set; }

            [Caption("Time Format"), Description("The desired time format when times are displayed on this website")]
            [UIHint("Enum"), Required]
            public Formatting.TimeFormatEnum TimeFormat { get; set; }

            [Caption("Grid Actions"), Description("The desired display method for available actions in grids")]
            [UIHint("Enum")]
            public Grid.GridActionsEnum GridActions { get; set; }

            [Caption("Language"), Description("The default language used for the entire site (only used when localization is enabled)")]
            [UIHint("LanguageId"), StringLength(LanguageData.MaxId)]
            public string LanguageId { get; set; }

            [Caption("Show Search Toolbar"), Description("Defines whether the search toolbar is always shown on grids - If not shown, it can still be accessed using the search button in each grid, at the bottom of the grid, next to the refresh button")]
            [UIHint("Boolean")]
            public bool ShowGridSearchToolbar { get; set; }

            [Caption("Show Page Ownership"), Description("Defines whether pages that can't be seen by anonymous users or regular users are shown with special background colors - Requires a skin that supports ownership display")]
            [UIHint("Boolean"), SuppressIf(nameof(ShowDevInfo), false)]
            public bool ShowPageOwnership { get; set; }

            [Caption("Show Module Ownership"), Description("Defines whether modules that can't be seen by anonymous users or regular users are shown with special background colors - Requires a skin that supports ownership display")]
            [UIHint("Boolean"), SuppressIf(nameof(ShowDevInfo), false)]
            public bool ShowModuleOwnership { get; set; }

            [Caption("Show Enum Values"), Description("Defines whether enumerated values (in dropdown lists) show their numeric value. Numeric values are typically only useful for programming purposes")]
            [UIHint("Boolean"), SuppressIf(nameof(ShowDevInfo), false)]
            public bool ShowEnumValue { get; set; }

            [Caption("Show Variables"), Description("Defines whether variable names are shown for properties and all available variables are listed on property pages. Variables are used for variable substitution in modules and pages and of course for programming purposes")]
            [UIHint("Boolean"), SuppressIf(nameof(ShowDevInfo), false)]
            public bool ShowVariables { get; set; }

            [Caption("Show Internal Data"), Description("Defines whether internal information is shown (e.g., ids)")]
            [UIHint("Boolean"), SuppressIf(nameof(ShowDevInfo), false)]
            public bool ShowInternals { get; set; }

            [Caption("Confirm Delete"), Description("Defines whether delete actions must be confirmed before items are deleted")]
            [UIHint("Boolean")]
            public bool ConfirmDelete { get; set; }

            [Caption("Confirm Actions"), Description("Defines whether actions must be confirmed before they are executed. This is normally used for actions that need a prompt but are not destructive (delete) in nature")]
            [UIHint("Boolean")]
            public bool ConfirmActions { get; set; }

            public bool ShowDevInfo { get; set; }

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
        public async Task<ActionResult> SettingsEdit() {
            using (UserDataProvider dataProvider = new UserDataProvider()) {
                EditModel model = new EditModel { };
                model.ShowDevInfo = Module.IsAuthorized("Development Info");
                UserData data = await dataProvider.GetItemAsync();
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> SettingsEdit_Partial(EditModel model) {
            using (UserDataProvider dataProvider = new UserDataProvider()) {
                UserData data = await dataProvider.GetItemAsync();
                model.ShowDevInfo = Module.IsAuthorized("Development Info");
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