/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core;
using System.Threading.Tasks;
using YetaWF.Modules.ComponentsHTML.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateListOfEmailAddressesModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateListOfEmailAddressesModule> {

        public TemplateListOfEmailAddressesModuleController() { }

        [Trim]
        public class Model {

            [Caption("Email Addresses (Required)"), Description("List of email addresses (Required)")]
            [UIHint("YetaWF_DevTests_ListOfEmailAddresses"), ListNoDuplicates, EmailValidation, StringLength(Globals.MaxEmail), Required, Trim]
            public List<string> Prop1Req { get; set; }
            public string Prop1Req_AjaxUrl { get { return YetaWFManager.UrlFor(typeof(TemplateListOfEmailAddressesModuleController), nameof(AddEmailAddressHTML)); } }

            [Caption("Email Addresses"), Description("List of email addresses")]
            [UIHint("YetaWF_DevTests_ListOfEmailAddresses"), ListNoDuplicates, EmailValidation, StringLength(Globals.MaxEmail), Trim]
            public List<string> Prop1 { get; set; }
            public string Prop1_AjaxUrl { get { return YetaWFManager.UrlFor(typeof(TemplateListOfEmailAddressesModuleController), nameof(AddEmailAddressHTML)); } }

            [Caption("Email Addresses (Read/Only)"), Description("List of email addresses (read/only)")]
            [UIHint("YetaWF_DevTests_ListOfEmailAddresses"), ReadOnly]
            public List<string> Prop1RO { get; set; }

            public Model() {
                Prop1Req = new List<string>() { "aa1@somedomain.com", "aa2@somedomain.com", "aa3@somedomain.com", "aa4@somedomain.com", "aa5@somedomain.com", "aa6@somedomain.com", "aa7@somedomain.com", "aa8@somedomain.com", "aa9@somedomain.com", "aa10@somedomain.com" };
                Prop1 = new List<string>() { "aa1@somedomain.com", "aa2@somedomain.com", "aa3@somedomain.com", "aa4@somedomain.com", "aa5@somedomain.com", "aa6@somedomain.com", "aa7@somedomain.com", "aa8@somedomain.com", "aa9@somedomain.com", "aa10@somedomain.com" };
                Prop1RO = new List<string>() { "aa1@somedomain.com", "aa2@somedomain.com", "aa3@somedomain.com", "aa4@somedomain.com", "aa5@somedomain.com", "aa6@somedomain.com", "aa7@somedomain.com", "aa8@somedomain.com", "aa9@somedomain.com", "aa10@somedomain.com" };
            }
        }

        [AllowGet]
        public ActionResult TemplateListOfEmailAddresses() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateListOfEmailAddresses_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddEmailAddressHTML(string prefix, int newRecNumber, string newValue) {
            // Validation
            EmailValidationAttribute attr = new EmailValidationAttribute();
            if (!attr.IsValid(newValue))
                throw new Error(attr.ErrorMessage);
            // add new grid record
            ListOfEmailAddressesEditComponent.GridEdit entry = new ComponentsHTML.Components.ListOfEmailAddressesEditComponent.GridEdit(newValue);
            return await GridPartialViewAsync(new GridDefinition.GridEntryDefinition(prefix, newRecNumber, entry));
        }
    }
}
