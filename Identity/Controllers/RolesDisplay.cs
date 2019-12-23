/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class RolesDisplayModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.RolesDisplayModule> {

        public RolesDisplayModuleController() { }

        [Trim]
        public class DisplayModel {

            [Caption("Name"), Description("The role name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("The intended use of the role")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [Caption("Role Id"), Description("The internal id of the role")]
            [UIHint("IntValue"), ReadOnly]
            public int RoleId { get; set; }

            [Caption("Post Login URL"), Description("The URL where a user with this role is redirected after logging on")]
            [UIHint("Url"), ReadOnly]
            public string PostLoginUrl { get; set; }

            public void SetData(RoleDefinition data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public async Task<ActionResult> RolesDisplay(string name) {
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                RoleDefinition data = await dataProvider.GetItemAsync(name);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Role \"{0}\" not found."), name);
                DisplayModel model = new DisplayModel();
                model.SetData(data);
                Module.Title = this.__ResStr("modDisplayTitle", "{0} Role", name);
                return View(model);
            }
        }
    }
}