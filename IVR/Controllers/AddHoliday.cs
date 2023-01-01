/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class AddHolidayModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.AddHolidayModule> {

        public AddHolidayModuleController() { }

        [Trim]
        public class AddModel {

            [Caption("Date"), Description("The date the holiday occurs")]
            [UIHint("Date")]
            public DateTime HolidayDate { get; set; }

            [Caption("Description"), Description("The description of the holiday")]
            [UIHint("Text80"), StringLength(HolidayEntry.MaxDescription), Trim]
            public string? Description { get; set; }

            public AddModel() { }

            public HolidayEntry GetData() {
                HolidayEntry data = new HolidayEntry();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [AllowGet]
        public ActionResult AddHoliday() {
            AddModel model = new AddModel {};
            ObjectSupport.CopyData(new HolidayEntry(), model);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddHoliday_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            using (HolidayEntryDataProvider dataProvider = new HolidayEntryDataProvider()) {
                if (!await dataProvider.AddItemAsync(model.GetData())) {
                    ModelState.AddModelError(nameof(AddModel.HolidayDate), this.__ResStr("dup", "An entry for {0} already exists", Formatting.FormatDate(model.HolidayDate)));
                    return PartialView(model);
                }
                return FormProcessed(model, this.__ResStr("okSaved", "New holiday saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
