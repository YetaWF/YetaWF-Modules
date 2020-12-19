/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class AddExtensionModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.AddExtensionModule> {

        public AddExtensionModuleController() { }

        [Trim]
        public class AddModel {

            [Caption("Extension"), Description("Defines the extension (digits)")]
            [UIHint("Text10"), Required, Trim]
            [StringLength(ExtensionEntry.MaxExtension)]
            public string Extension { get; set; }

            [Caption("Description"), Description("Describes the extension")]
            [UIHint("Text80"), Required, Trim]
            [StringLength(ExtensionEntry.MaxDescription)]
            public string Description { get; set; }

            [Caption("Phone Numbers"), Description("Defines the phone numbers to call when this extension is entered - At least one phone number is required")]
            [UIHint("Softelvdm_IVR_ListOfPhoneNumbers"), Required]
            public SerializableList<ExtensionPhoneNumber> PhoneNumbers { get; set; }

            [Caption("Users"), Description("Defines the users that can access voice mails for this extension")]
            [UIHint("YetaWF_Identity_ListOfUserNames")]
            public SerializableList<User> Users { get; set; }

            public AddModel() { }

            public ExtensionEntry GetData() {
                ExtensionEntry data = new ExtensionEntry();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [AllowGet]
        public ActionResult AddExtension() {
            AddModel model = new AddModel {};
            ObjectSupport.CopyData(new ExtensionEntry(), model);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddExtension_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            using (ExtensionEntryDataProvider dataProvider = new ExtensionEntryDataProvider()) {
                if (!await dataProvider.AddItemAsync(model.GetData())) {
                    ModelState.AddModelError(nameof(AddModel.Extension), this.__ResStr("dup", "An entry for extension {0} already exists", model.Extension));
                    return PartialView(model);
                }
                return FormProcessed(model, this.__ResStr("okSaved", "New extension saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
