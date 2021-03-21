/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using Softelvdm.Modules.IVR.Models.Attributes;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Controllers {

    public class EditExtensionModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.EditExtensionModule> {

        public EditExtensionModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Extension"), Description("Defines the extension (digits)")]
            [UIHint("Text10"), ExtensionValidation, Required, Trim]
            [StringLength(ExtensionEntry.MaxExtension)]
            public string? Extension { get; set; } = null!;

            [Caption("Description"), Description("Describes the extension - This text is used to identify the extension when call screening")]
            [UIHint("Text40"), Required, Trim]
            [StringLength(ExtensionEntry.MaxDescription)]
            public string? Description { get; set; }

            [Caption("Phone Numbers"), Description("Defines the phone numbers to call when this extension is entered - At least one phone number is required")]
            [UIHint("Softelvdm_IVR_ListOfPhoneNumbers"), Required]
            public SerializableList<ExtensionPhoneNumber>? PhoneNumbers { get; set; }

            [Caption("Users"), Description("Defines the users that can access voice mails for this extension")]
            [UIHint("YetaWF_Identity_ListOfUserNames")]
            public SerializableList<User>? Users { get; set; }

            [UIHint("Hidden")]
            public string OriginalExtension { get; set; } = null!;

            public ExtensionEntry GetData(ExtensionEntry data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }

            public void SetData(ExtensionEntry data) {
                ObjectSupport.CopyData(data, this);
                OriginalExtension = Extension;
            }
            public EditModel() { }
        }

        [AllowGet]
        public async Task<ActionResult> EditExtension(string extension) {
            using (ExtensionEntryDataProvider dataProvider = new ExtensionEntryDataProvider()) {
                EditModel model = new EditModel { };
                ExtensionEntry? data = await dataProvider.GetItemAsync(extension);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Extension \"{0}\" not found"), extension);
                model.SetData(data);
                Module.Title = this.__ResStr("modTitle", "Extension \"{0}\"", extension);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> EditExtension_Partial(EditModel model) {
            string originalExtension = model.OriginalExtension;

            using (ExtensionEntryDataProvider dataProvider = new ExtensionEntryDataProvider()) {
                ExtensionEntry? data = await dataProvider.GetItemAsync(originalExtension);// get the original item
                if (data == null) {
                    ModelState.AddModelError("Extension", this.__ResStr("alreadyDeleted", "Extension {0} has been removed and can no longer be updated", originalExtension));
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
                        ModelState.AddModelError("Name", this.__ResStr("alreadyDeleted", "Extension {0} has been removed and can no longer be updated", model.OriginalExtension));
                        return PartialView(model);
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "Extension {0} already exists.", data.Extension));
                        return PartialView(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Extension saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
