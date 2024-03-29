/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class AuthorizationEditModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.AuthorizationEditModule> {

        public AuthorizationEditModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Resource Name"), Description("The name of this resource")]
            [UIHint("String"), ReadOnly]
            public string ResourceName { get; set; }

            [Caption("Resource Description"), Description("The permissions granted if a user or role has access to this resource")]
            [UIHint("TextAreaSourceOnly"), StringLength(Authorization.MaxResourceDescription)]
            public string ResourceDescription { get; set; }

            [Caption("Allowed Roles"), Description("The roles that are permitted to access this resource")]
            [UIHint("YetaWF_Identity_ResourceRoles")]
            public SerializableList<Role> AllowedRoles { get; set; }

            [Caption("Allowed Users"), Description("The users that are permitted to access this resource")]
            [UIHint("YetaWF_Identity_ResourceUsers")]
            public SerializableList<User> AllowedUsers { get; set; }

            [UIHint("Hidden")]
            public string OriginalName { get; set; }

            public Authorization GetData(Authorization data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }

            public void SetData(Authorization data) {
                ObjectSupport.CopyData(data, this);
                OriginalName = ResourceName;
            }

            public EditModel() { }

        }

        [AllowGet]
        public async Task<ActionResult> AuthorizationEdit(string resourceName) {
            using (AuthorizationDataProvider dataProvider = new AuthorizationDataProvider()) {
                EditModel model = new EditModel { };
                Authorization data = await dataProvider.GetItemAsync(resourceName);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Resource \"{0}\" not found."), resourceName);
                model.SetData(data);
                Module.Title = this.__ResStr("modEditTitle", "Resource \"{0}\"", resourceName);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AuthorizationEdit_Partial(EditModel model) {
            string originalName = model.OriginalName;

            using (AuthorizationDataProvider dataProvider = new AuthorizationDataProvider()) {
                Authorization data = await dataProvider.GetItemAsync(originalName);// get the original resource
                if (data == null)
                    throw new Error(this.__ResStr("alreadyDeleted", "The resource named \"{0}\" has been removed and can no longer be updated.", originalName));

                if (!ModelState.IsValid)
                    return PartialView(model);

                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                switch (await dataProvider.UpdateItemAsync(data)) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("alreadyDeleted", "The resource named \"{0}\" has been removed and can no longer be updated.", originalName));
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError(nameof(model.ResourceName), this.__ResStr("alreadyExists", "A resource named \"{0}\" already exists.", model.ResourceName));
                        return PartialView(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Resource \"{0}\" saved", model.ResourceName), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
