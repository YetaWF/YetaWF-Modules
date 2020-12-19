/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
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
    public class RolesAddModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.RolesAddModule> {

        public RolesAddModuleController() { }

        [Trim]
        public class AddModel {

            [Caption("Name"), Description("The role name")]
            [UIHint("Text40"), StringLength(RoleDefinition.MaxName), RoleNameValidation, Required, Trim]
            public string Name { get; set; }

            [Caption("Description"), Description("The intended use of the role")]
            [UIHint("Text80"), StringLength(RoleDefinition.MaxDescription)]
            public string Description { get; set; }

            [Caption("Post Login URL"), Description("The URL where a user with this role is redirected after logging on")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
            public string PostLoginUrl { get; set; }

            public AddModel() { }

            public RoleDefinition GetData() {
                RoleDefinition data = new RoleDefinition();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [AllowGet]
        public ActionResult RolesAdd() {
            AddModel model = new AddModel { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> RolesAdd_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                if (!await dataProvider.AddItemAsync(model.GetData()))
                    throw new Error(this.__ResStr("alreadyExists", "A role named \"{0}\" already exists."), model.Name);
                return FormProcessed(model, this.__ResStr("okSaved", "New role \"{0}\" saved", model.Name), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
