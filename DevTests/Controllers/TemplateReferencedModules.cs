/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateReferencedModulesModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateReferencedModulesModule> {

        public TemplateReferencedModulesModuleController() { }

        [Trim]
        public class Model {

            protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

            [Caption("ReferencedModules (Required)"), Description("ReferencedModules (Required)")]
            [UIHint("ReferencedModules"), Required, Trim]
            public SerializableList<ModuleDefinition.ReferencedModule> Prop1Req { get; set; }

            [Caption("ReferencedModules"), Description("ReferencedModules")]
            [UIHint("ReferencedModules"), Trim]
            public SerializableList<ModuleDefinition.ReferencedModule> Prop1 { get; set; }

            [Caption("ReferencedModules (Read/Only)"), Description("ReferencedModules (read/only)")]
            [UIHint("ReferencedModules"), ReadOnly]
            public SerializableList<ModuleDefinition.ReferencedModule> Prop1RO { get; set; }

            public Model() {
                Prop1Req = new SerializableList<ModuleDefinition.ReferencedModule>();
                Prop1 = new SerializableList<ModuleDefinition.ReferencedModule>();
                List<AddOnManager.Module> allMods = Manager.AddOnManager.GetUniqueInvokedCssModules();
                Prop1RO = new SerializableList<ModuleDefinition.ReferencedModule>(
                    (from AddOnManager.Module a in allMods select new ModuleDefinition.ReferencedModule { ModuleGuid = a.ModuleGuid }).ToList()
                );
            }
        }

        [AllowGet]
        public ActionResult TemplateReferencedModules() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateReferencedModules_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
