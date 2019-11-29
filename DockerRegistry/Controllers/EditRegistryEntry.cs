/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.DockerRegistry.DataProvider;
using YetaWF.Core;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DockerRegistry.Controllers {

    public class EditRegistryEntryModuleController : ControllerImpl<YetaWF.Modules.DockerRegistry.Modules.EditRegistryEntryModule> {

        public EditRegistryEntryModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Server URL"), Description("The URL of the registry server - Must start with https:// or http://")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Required, Trim]
            public string RegistryURL { get; set; }
            [Caption("Registry (Description)"), Description("The descriptive name of the registry")]
            [UIHint("Text40"), StringLength(RegistryEntry.MaxRegistryName), Required, Trim]
            public string RegistryName { get; set; }
            [Caption("User Name"), Description("The user name used to log into the registry server - Omit for anonymous access (if available)")]
            [UIHint("Text40"), StringLength(RegistryEntry.MaxUserName), Trim]
            [RequiredIfSupplied(nameof(Password))]
            public string UserName { get; set; }
            [Caption("Password"), Description("The user name used to log into the registry server - Omit for anonymous access (if available)")]
            [UIHint("Password20"), StringLength(RegistryEntry.MaxPassword), Trim]
            [RequiredIfSupplied(nameof(UserName))]
            public string Password { get; set; }

            [UIHint("Hidden")]
            public int Id { get; set; }

            public RegistryEntry GetData(RegistryEntry data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }

            public void SetData(RegistryEntry data) {
                ObjectSupport.CopyData(data, this);
            }
            public EditModel() { }
        }

        [AllowGet]
        public async Task<ActionResult> EditRegistryEntry(int registryId) {
            using (RegistryEntryDataProvider regDP = new RegistryEntryDataProvider()) {
                EditModel model = new EditModel {};
                RegistryEntry reg = await regDP.GetItemAsync(registryId);
                if (reg == null)
                    throw new Error(this.__ResStr("notFound", "Registry with id {0} not found"), registryId);
                model.SetData(reg);
                Module.Title = this.__ResStr("modTitle", "Registry \"{0}\"", reg.RegistryName);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> EditRegistryEntry_Partial(EditModel model) {
            using (RegistryEntryDataProvider regDP = new RegistryEntryDataProvider()) {
                RegistryEntry reg = await regDP.GetItemAsync(model.Id);
                if (reg == null)
                    throw new Error(this.__ResStr("alreadyDeleted2", "The registry with id {0} has been removed and can no longer be updated.", model.Id));
                ObjectSupport.CopyData(reg, model, ReadOnly: true); // update read only properties in model in case there is an error
                if (!ModelState.IsValid)
                    return PartialView(model);

                reg = model.GetData(reg); // merge new data into original
                model.SetData(reg); // and all the data back into model for final display

                switch (await regDP.UpdateItemAsync(reg)) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyDeleted", "The registry with id {0} has been removed and can no longer be updated", model.Id));
                        return PartialView(model);
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "Registry {0} already exists", model.RegistryName));
                        return PartialView(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Registry saved"), OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
            }
        }
    }
}
