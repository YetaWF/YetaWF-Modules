/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Pages.Views.Shared;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Pages.Controllers {

    public class TemplateListOfLocalPagesModuleController : ControllerImpl<YetaWF.Modules.Pages.Modules.TemplateListOfLocalPagesModule> {

        public TemplateListOfLocalPagesModuleController() { }

        [Trim]
        public class Model {

            [Caption("ListOfLocalPages (Required)"), Description("ListOfLocalPages (Required)")]
            [UIHint("YetaWF_Pages_ListOfLocalPages"), Required]
            public List<string> Prop1Req { get; set; }
            public string Prop1Req_AjaxUrl { get { return YetaWFManager.UrlFor(typeof(TemplateListOfLocalPagesModuleController), nameof(AddPage)); } }

            [Caption("ListOfLocalPages"), Description("ListOfLocalPages")]
            [UIHint("YetaWF_Pages_ListOfLocalPages")]
            public List<string> Prop1 { get; set; }
            public string Prop1_AjaxUrl { get { return YetaWFManager.UrlFor(typeof(TemplateListOfLocalPagesModuleController), nameof(AddPage)); } }

            [Caption("ListOfLocalPages (Read/Only)"), Description("ListOfLocalPages (read/only)")]
            [UIHint("YetaWF_Pages_ListOfLocalPages"), ReadOnly]
            public List<string> Prop1RO { get; set; }

            public Model() {
                Prop1Req = new List<string>();
                Prop1 = new List<string>();
                Prop1RO = new List<string>();
            }
        }

        [AllowGet]
        public ActionResult TemplateListOfLocalPages() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateListOfLocalPages_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddPage(string prefix, int newRecNumber, string newValue) {
            // Validation
            UrlValidationAttribute attr = new UrlValidationAttribute(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local);
            if (!attr.IsValid(newValue))
                throw new Error(attr.ErrorMessage);
            // add new grid record
            ListOfLocalPagesHelper.GridEntryEdit entry = (ListOfLocalPagesHelper.GridEntryEdit)Activator.CreateInstance(typeof(ListOfLocalPagesHelper.GridEntryEdit));
            entry.Url = newValue;
            return await GridPartialViewAsync(new GridDefinition.GridEntryDefinition(prefix, newRecNumber, entry));
        }
    }
}
