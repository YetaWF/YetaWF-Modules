/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using System.Collections.Generic;
using YetaWF.Core.Pages;
using YetaWF.Core.Identity;
using YetaWF.Core.Addons;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ModuleEdit.Controllers {

    public class ModuleEditModuleController : ControllerImpl<YetaWF.Modules.ModuleEdit.Modules.ModuleEditModule> {

        public class ModuleEditModel {

            [UIHint("PropertyList")]
            public ModuleDefinition Module { get; set; }

            [UIHint("Hidden")]
            public Guid ModuleGuid { get; set; }

            internal async Task UpdateDataAsync() {
                await ObjectSupport.HandlePropertyAsync<List<PageDefinition>>(nameof(ModuleDefinition.Pages), nameof(ModuleDefinition.__GetPagesAsync), Module);
            }
        }

        [AllowGet]
        [ResourceAuthorize(CoreInfo.Resource_ModuleSettings)]
        public async Task<ActionResult> ModuleEdit(Guid moduleGuid) {
            if (moduleGuid == Guid.Empty)
                throw new InternalError("No moduleGuid provided");

            ModuleDefinition module = await ModuleDefinition.LoadAsync(moduleGuid);
            ModuleEditModel model = new ModuleEditModel() {
                Module = module,
                ModuleGuid = moduleGuid,
            };
            Module.Title = this.__ResStr("modEditTitle", "Module \"{0}\"", module.Title.ToString());
            await model.UpdateDataAsync();
            Manager.CurrentModuleEdited = module;
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        [ResourceAuthorize(CoreInfo.Resource_ModuleSettings)]
        public async Task<ActionResult> ModuleEdit_Partial(ModuleEditModel model) {

            if (model.ModuleGuid == Guid.Empty)
                throw new InternalError("No moduleGuid provided");

            // we need to find the real type of the module for data binding
            ModuleDefinition origModule = await ModuleDefinition.LoadAsync(model.ModuleGuid);
            await ObjectSupport.HandlePropertyAsync<List<PageDefinition>>(nameof(ModuleDefinition.Pages), nameof(ModuleDefinition.__GetPagesAsync), origModule);

            ModuleDefinition mod = (ModuleDefinition)await GetObjectFromModelAsync(origModule.GetType(), nameof(model.Module));
            if (!ModelState.IsValid)
                return PartialView(model);

            mod.CustomValidation(ModelState, "Module.");
            if (!ModelState.IsValid)
                return PartialView(model);

            model.Module = mod;
            await model.UpdateDataAsync();
            Manager.CurrentModuleEdited = model.Module;

            ObjectSupport.CopyData(origModule, model.Module, ReadOnly: true); // update read only properties in model in case there is an error
            ObjectSupport.CopyDataFromOriginal(origModule, model.Module);

            // copy/save
            model.Module.Temporary = false;
            await model.Module.SaveAsync();
            return FormProcessed(model, this.__ResStr("okSaved", "Module settings saved"));
        }
    }
}