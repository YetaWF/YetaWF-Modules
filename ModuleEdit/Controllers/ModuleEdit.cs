/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ModuleEdit.Controllers {

    public class ModuleEditModuleController : ControllerImpl<YetaWF.Modules.ModuleEdit.Modules.ModuleEditModule> {

        public class ModuleEditModel {
            [UIHint("PropertyListTabbed"), Trim]
            public ModuleDefinition Module { get; set; }

            [UIHint("Hidden")]
            public Guid ModuleGuid { get; set; }
        }

        [HttpGet]
        public ActionResult ModuleEdit(Guid moduleGuid) {
            if (moduleGuid == Guid.Empty)
                throw new InternalError("No moduleGuid provided");

            ModuleDefinition module = ModuleDefinition.Load(moduleGuid);
            if (!module.IsAuthorized(ModuleDefinition.RoleDefinition.Edit))
                return NotAuthorized();

            ModuleEditModel model = new ModuleEditModel() {
                Module = module,
                ModuleGuid = moduleGuid,
            };
            Module.Title = this.__ResStr("modEditTitle", "Module \"{0}\"", module.Title.ToString());
            Manager.CurrentModuleEdited = module;
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult ModuleEdit_Partial(ModuleEditModel model) {
            if (model.ModuleGuid == Guid.Empty)
                throw new InternalError("No moduleGuid provided");
            // we need to find the real type of the module for data binding
            ModuleDefinition origModule = ModuleDefinition.Load(model.ModuleGuid);
            if (!origModule.IsAuthorized(ModuleDefinition.RoleDefinition.Edit))
                return NotAuthorized();

            model.Module = (ModuleDefinition)GetObjectFromModel(origModule.GetType(), "Module");
            Manager.CurrentModuleEdited = model.Module;

            ObjectSupport.CopyData(origModule, model.Module, ReadOnly: true); // update read only properties in model in case there is an error
            model.Module.CustomValidation(ModelState, "Module.");

            if (!ModelState.IsValid)
                return PartialView(model);

            // copy/save
            ObjectSupport.CopyDataFromOriginal(origModule, model.Module);
            model.Module.Temporary = false;
            model.Module.Save();
            return FormProcessed(model, this.__ResStr("okSaved", "Module settings saved"));
        }
    }
}