/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Modules.Identity.Components;
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
            public List<int> Prop1Req { get; set; }
            public string Prop1Req_AjaxUrl { get { return YetaWFManager.UrlFor(typeof(TemplateListOfUserNamesModuleController), nameof(AddUserName)); } }

            [Caption("User Names"), Description("List of User Names")]
            [UIHint("YetaWF_Identity_ListOfUserNames"), ListNoDuplicates, Trim]
            public List<int> Prop1 { get; set; }
            public string Prop1_AjaxUrl { get { return YetaWFManager.UrlFor(typeof(TemplateListOfUserNamesModuleController), nameof(AddUserName)); } }

            [Caption("User Names (Read/Only)"), Description("List of User Names (read/only)")]
            [UIHint("YetaWF_Identity_ListOfUserNames"), ReadOnly]
            public List<int> Prop1RO { get; set; }

            public Model() {
                Prop1Req = new List<int>();
                Prop1 = new List<int>();
                Prop1RO = new List<int>();
            }
            public async Task Init() {
                using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                    // get 5 sample users
                    DataProviderGetRecords<UserDefinition> recs = await userDP.GetItemsAsync(0, 5, null, null);
                    List<int> users = (from u in recs.Data select u.UserId).ToList();
                    Prop1Req = users;
                    Prop1 = users;
                    Prop1RO = users;
                }
            }
            public async Task Update() {
                using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                    // get 5 sample users
                    DataProviderGetRecords<UserDefinition> recs = await userDP.GetItemsAsync(0, 5, null, null);
                    List<int> users = (from u in recs.Data select u.UserId).ToList();
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

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddUserName(string data, string fieldPrefix, string newUser) {
            using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemAsync(newUser);
                if (user == null)
                    throw new Error(this.__ResStr("noUser", "User {0} not found", newUser));
                List<ListOfUserNamesEditComponent.Entry> list = YetaWFManager.JsonDeserialize<List<ListOfUserNamesEditComponent.Entry>>(data);
                if ((from l in list where l.UserId == user.UserId select l).FirstOrDefault() != null)
                    throw new Error(this.__ResStr("dupUser", "User {0} has already been added", newUser));
                ListOfUserNamesEditComponent.Entry entry = new ListOfUserNamesEditComponent.Entry {
                    UserName = newUser,
                    UserId = user.UserId,
                };
                return await GridRecordViewAsync(await ListOfUserNamesEditComponent.GridRecordAsync(fieldPrefix, entry));
            }
        }
    }
}
