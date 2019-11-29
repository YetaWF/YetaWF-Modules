/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.DockerRegistry.DataProvider;
using YetaWF.Core;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DockerRegistry.Controllers {

    public class AddRegistryEntryModuleController : ControllerImpl<YetaWF.Modules.DockerRegistry.Modules.AddRegistryEntryModule> {

        public AddRegistryEntryModuleController() { }

        [Trim]
        public class AddModel {

            [Caption("Server URL"), Description("The URL of the registry server - Must start with https:// or http://")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Required, Trim]
            public string RegistryURL { get; set; }
            [Caption("Registry Name"), Description("The descripitve name of the registry")]
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

            public AddModel() { }

            public RegistryEntry GetData() {
                RegistryEntry data = new RegistryEntry();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [AllowGet]
        public ActionResult AddRegistryEntry() {
            AddModel model = new AddModel {};
            ObjectSupport.CopyData(new RegistryEntry(), model);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddRegistryEntry_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            using (RegistryEntryDataProvider dataProvider = new RegistryEntryDataProvider()) {
                if (!await dataProvider.AddItemAsync(model.GetData())) {
                    ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "A registry named \"{0}\" already exists", model.RegistryName));
                    return PartialView(model);
                }
                return FormProcessed(model, this.__ResStr("okSaved", "New registry saved"), OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
            }
        }
        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(int registryId) {
            using (RegistryEntryDataProvider regDP = new RegistryEntryDataProvider()) {
                if (await regDP.RemoveItemAsync(registryId))
                    return Reload(null, Reload: ReloadEnum.Page, PopupText: this.__ResStr("removed", "The registry server entry has been successfully removed"));
                else
                    return Reload(null, Reload: ReloadEnum.Module, PopupText: this.__ResStr("notfound", "The registry server entry does not exist"), PopupTitle: this.__ResStr("error", "Error"));
            }
        }
    }
}
