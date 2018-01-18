/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core;
using YetaWF.Modules.Identity.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;
using System.Linq;
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
            public void Init() {
                using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                    int total;
                    // get 5 sample users
                    List<int> users = (from u in userDP.GetItems(0, 5, null, null, out total) select u.UserId).ToList();
                    Prop1Req = users;
                    Prop1 = users;
                    Prop1RO = users;
                }
            }
            public void Update() {
                using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                    int total;
                    // get 5 sample users
                    List<int> users = (from u in userDP.GetItems(0, 5, null, null, out total) select u.UserId).ToList();
                    Prop1RO = users;
                }
            }
        }

        [AllowGet]
        public ActionResult TemplateListOfUserNames() {
            Model model = new Model { };
            model.Init();
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateListOfUserNames_Partial(Model model) {
            model.Update();
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult AddUserName(string prefix, int newRecNumber, string newValue) {
            using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                UserDefinition user = userDP.GetItem(newValue);
                if (user == null)
                    throw new Error(this.__ResStr("noUser", "User {0} not found", newValue));
                ListOfUserNamesHelper.GridEntryEdit entry = (ListOfUserNamesHelper.GridEntryEdit)Activator.CreateInstance(typeof(ListOfUserNamesHelper.GridEntryEdit));
                entry.UserName = newValue;
                entry.__Value = user.UserId.ToString();
                return GridPartialView(new GridDefinition.GridEntryDefinition(prefix, newRecNumber, entry));
            }
        }
    }
}
