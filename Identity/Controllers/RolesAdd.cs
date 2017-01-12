/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Support;

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

            public AddModel() { }

            public RoleDefinition GetData() {
                RoleDefinition data = new RoleDefinition();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [HttpGet]
        public ActionResult RolesAdd() {
            AddModel model = new AddModel { };
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult RolesAdd_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                if (!dataProvider.AddItem(model.GetData()))
                    throw new Error(this.__ResStr("alreadyExists", "A role named \"{0}\" already exists."), model.Name);
                return FormProcessed(model, this.__ResStr("okSaved", "New role \"{0}\" saved", model.Name), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
