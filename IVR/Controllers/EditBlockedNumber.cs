/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using Softelvdm.Modules.IVR.DataProvider;
using YetaWF.Core;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class EditBlockedNumberModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.EditBlockedNumberModule> {

        public EditBlockedNumberModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Blocked Number"), Description("Shows the blocked phone number")]
            [UIHint("Softelvdm_IVR_PhoneNumber"), StringLength(Globals.MaxPhoneNumber), ReadOnly]
            public string Number { get; set; }

            [Caption("Description"), Description("The description of the blocked number")]
            [UIHint("TextAreaSourceOnly"), StringLength(BlockedNumberEntry.MaxDescription)]
            public string Description { get; set; }

            public BlockedNumberEntry GetData(BlockedNumberEntry data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }

            [UIHint("Hidden"), ReadOnly]
            public string OriginalNumber { get; set; }

            public void SetData(BlockedNumberEntry data) {
                ObjectSupport.CopyData(data, this);
                OriginalNumber = data.Number;
            }
            public EditModel() { }
        }

        [AllowGet]
        public async Task<ActionResult> EditBlockedNumber(string number) {
            using (BlockedNumberDataProvider dataProvider = new BlockedNumberDataProvider()) {
                EditModel model = new EditModel {};
                BlockedNumberEntry data = await dataProvider.GetItemAsync(number);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Blocked phone number \"{0}\" not found"), number);
                model.SetData(data);
                Module.Title = this.__ResStr("modTitle", "Blocked Phone Number \"{0}\"", number);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> EditBlockedNumber_Partial(EditModel model) {

            using (BlockedNumberDataProvider dataProvider = new BlockedNumberDataProvider()) {
                BlockedNumberEntry data = await dataProvider.GetItemAsync(model.OriginalNumber);// get the original item
                if (data == null) {
                    ModelState.AddModelError("Number", this.__ResStr("alreadyDeleted", "Blocked number {0} has been removed and can no longer be updated", model.OriginalNumber));
                    return PartialView(model);
                }
                ObjectSupport.CopyData(data, model, ReadOnly: true); // update read only properties in model in case there is an error
                if (!ModelState.IsValid)
                    return PartialView(model);

                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                switch (await dataProvider.UpdateItemAsync(data)) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyDeleted", "Blocked number {0} has been removed and can no longer be updated", model.OriginalNumber));
                        return PartialView(model);
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "Blocked number {0} already exists", model.Number));
                        return PartialView(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Blocked number saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
