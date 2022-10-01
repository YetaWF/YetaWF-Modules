/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Identity.DataProvider;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Serializers;
using YetaWF.Core.Identity;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class TemplateListOfUserNamesModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.TemplateListOfUserNamesModule> {

        public TemplateListOfUserNamesModuleController() { }

        [Trim]
        public class Model {

            [Caption("User Names (Required)"), Description("List of User Names (Required)")]
            [UIHint("YetaWF_Identity_ListOfUserNames"), ListNoDuplicates, Required, Trim]
            public SerializableList<User> Prop1Req { get; set; }

            [Caption("User Names"), Description("List of User Names")]
            [UIHint("YetaWF_Identity_ListOfUserNames"), ListNoDuplicates, Trim]
            public SerializableList<User> Prop1 { get; set; }

            [Caption("User Names (Read/Only)"), Description("List of User Names (read/only)")]
            [UIHint("YetaWF_Identity_ListOfUserNames"), ReadOnly]
            public SerializableList<User> Prop1RO { get; set; }

            public Model() {
                Prop1Req = new SerializableList<User>();
                Prop1 = new SerializableList<User>();
                Prop1RO = new SerializableList<User>();
            }
            public async Task Init() {
                using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                    // get 5 sample users
                    DataProviderGetRecords<UserDefinition> recs = await userDP.GetItemsAsync(0, 5, null, null);
                    SerializableList<User> users = new SerializableList<User>((from u in recs.Data select new User { UserId = u.UserId }).ToList());
                    Prop1Req = users;
                    Prop1 = users;
                    Prop1RO = users;
                }
            }
            public async Task Update() {
                using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                    // get 5 sample users
                    DataProviderGetRecords<UserDefinition> recs = await userDP.GetItemsAsync(0, 5, null, null);
                    SerializableList<User> users = new SerializableList<User>((from u in recs.Data select new User { UserId = u.UserId }).ToList());
                    Prop1RO = users;
                }
            }
        }

        [AllowGet]
        public async Task<ActionResult> TemplateListOfUserNames() {
            Model model = new Model { };
            await model.Init();
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> TemplateListOfUserNames_Partial(Model model) {
            await model.Update();
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
