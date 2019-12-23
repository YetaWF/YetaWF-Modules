/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using Softelvdm.Modules.IVR.DataProvider;
using YetaWF.Core;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class AddBlockedNumberModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.AddBlockedNumberModule> {

        public AddBlockedNumberModuleController() { }

        [Trim]
        public class AddModel {

            [Caption("Blocked Number"), Description("Enter the phone number to block")]
            [UIHint("Text20"), StringLength(Globals.MaxPhoneNumber), Required, Trim]
            public string Number { get; set; }

            [Caption("Description"), Description("Enter an optional description of the blocked number")]
            [UIHint("TextAreaSourceOnly"), StringLength(BlockedNumberEntry.MaxDescription)]
            public string Description { get; set; }

            public AddModel() { }

            public BlockedNumberEntry GetData() {
                BlockedNumberEntry data = new BlockedNumberEntry();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [AllowGet]
        public ActionResult AddBlockedNumber() {
            AddModel model = new AddModel {};
            ObjectSupport.CopyData(new BlockedNumberEntry(), model);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddBlockedNumber_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            using (BlockedNumberDataProvider dataProvider = new BlockedNumberDataProvider()) {
                if (!await dataProvider.AddItemAsync(model.GetData())) {
                    ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "Blocked number already exists"));
                    return PartialView(model);
                }
                return FormProcessed(model, this.__ResStr("okSaved", "New blocked number saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
