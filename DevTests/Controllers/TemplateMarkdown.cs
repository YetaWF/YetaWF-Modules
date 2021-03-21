/* Copyright � 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateMarkdownModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateMarkdownModule> {

        public TemplateMarkdownModuleController() { }

        public class MarkdownRequired : MarkdownStringBase {
            [Required, AdditionalMetadata("EmHeight", 10)]
            [StringLength(1000)]
            public override string? Text { get; set; } = null!;
        }
        public class MarkdownOptional : MarkdownStringBase {
            [StringLength(1000), AdditionalMetadata("EmHeight", 5)]
            public override string? Text { get; set; } = null!;
        }

        [Trim]
        public class Model {

            [Caption("Markdown (Required)"), Description("Markdown (Required)")]
            [UIHint("Markdown")]
            public MarkdownRequired Prop1Req { get; set; }

            [Caption("Markdown"), Description("Markdown")]
            [UIHint("Markdown")]
            public MarkdownOptional Prop1 { get; set; }

            [Caption("Markdown (Read/Only)"), Description("Markdown (read/only)")]
            [UIHint("Markdown"), ReadOnly]
            public MarkdownStringBase Prop1RO { get; set; }

            public Model() {
                Prop1Req = new MarkdownRequired { };
                Prop1 = new MarkdownOptional { };
                Prop1RO = new MarkdownStringBase { };
            }
        }

        [AllowGet]
        public ActionResult TemplateMarkdown() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateMarkdown_Partial(Model model) {
            model.Prop1RO = model.Prop1;
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
