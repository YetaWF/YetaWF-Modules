/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateReferencedModulesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateReferencedModulesModule>, IInstallableModel { }

    [ModuleGuid("{8c9deda8-82e5-44fa-9aba-d1625b3c277d}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateReferencedModulesModule : ModuleDefinition2 {

        public TemplateReferencedModulesModule() {
            Title = this.__ResStr("modTitle", "ReferencedModules Test Component");
            Name = this.__ResStr("modName", "Component Test - ReferencedModules");
            Description = this.__ResStr("modSummary", "Test module for the ReferencedModules component. A test page for this module can be found at Tests > Templates > ReferencedModules (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateReferencedModulesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

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

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model { };
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
