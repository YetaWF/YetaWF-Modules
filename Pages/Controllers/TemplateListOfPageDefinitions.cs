/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Pages.Endpoints;

namespace YetaWF.Modules.Pages.Controllers {

    public class TemplateListOfLocalPagesModuleController : ControllerImpl<YetaWF.Modules.Pages.Modules.TemplateListOfLocalPagesModule> {

        public TemplateListOfLocalPagesModuleController() { }

        [Trim]
        public class Model {

            [Caption("ListOfLocalPages (Required)"), Description("ListOfLocalPages (Required)")]
            [UIHint("YetaWF_Pages_ListOfLocalPages"), Required]
            public List<string> Prop1Req { get; set; }
            public string Prop1Req_AjaxUrl { get { return Utility.UrlFor<TemplateListOfLocalPagesEndpoints>(TemplateListOfLocalPagesEndpoints.AddPage); } }

            [Caption("ListOfLocalPages"), Description("ListOfLocalPages")]
            [UIHint("YetaWF_Pages_ListOfLocalPages")]
            public List<string> Prop1 { get; set; }
            public string Prop1_AjaxUrl { get { return Utility.UrlFor<TemplateListOfLocalPagesEndpoints>(TemplateListOfLocalPagesEndpoints.AddPage); } }

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
            model.Prop1RO = model.Prop1;
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
