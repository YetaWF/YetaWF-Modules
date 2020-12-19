/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class RolesEditModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.RolesEditModule> {

        public RolesEditModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Name"), Description("The name of this role")]
            [UIHint("Text40"), StringLength(RoleDefinition.MaxName), RoleNameValidation, Required, Trim]
            public string Name { get; set; }

            [Caption("Description"), Description("The intended use of the role")]
            [UIHint("Text80"), StringLength(RoleDefinition.MaxDescription)]
            public string Description { get; set; }

            [Caption("Post Login URL"), Description("The URL where a user with this role is redirected after logging on")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
            public string PostLoginUrl { get; set; }

            [UIHint("Hidden")]
            public string OriginalName { get; set; }

            public RoleDefinition GetData(RoleDefinition role) {
                ObjectSupport.CopyData(this, role);
                return role;
            }

            public void SetData(RoleDefinition role) {
                ObjectSupport.CopyData(role, this);
                OriginalName = Name;
            }
        }

        [AllowGet]
        public async Task<ActionResult> RolesEdit(string name) {
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                EditModel model = new EditModel { };
                RoleDefinition data = await dataProvider.GetItemAsync(name);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Role \"{0}\" not found."), name);
                model.SetData(data);
                Module.Title = this.__ResStr("modEditTitle", "Role \"{0}\"", name);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> RolesEdit_Partial(EditModel model) {
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                string originalRole = model.OriginalName;
                RoleDefinition role = await dataProvider.GetItemAsync(originalRole);// get the original item
                if (role == null)
                    throw new Error(this.__ResStr("alreadyDeleted", "The role named \"{0}\" has been removed and can no longer be updated.", originalRole));

                if (!ModelState.IsValid)
                    return PartialView(model);

                role = model.GetData(role); // merge new data into original
                model.SetData(role); // and all the data back into model for final display

                switch (await dataProvider.UpdateItemAsync(originalRole, role)) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("alreadyDeleted", "The role named \"{0}\" has been removed and can no longer be updated.", originalRole));
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError(nameof(model.Name), this.__ResStr("alreadyExists", "A role named \"{0}\" already exists.", model.Name));
                        return PartialView(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Role \"{0}\" saved", role.Name), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
