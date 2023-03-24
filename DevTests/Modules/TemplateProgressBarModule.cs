/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateProgressBarModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateProgressBarModule>, IInstallableModel { }

    [ModuleGuid("{d5fcf9ab-06a7-456b-8d7c-2e0de134b1e8}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateProgressBarModule : ModuleDefinition {

        public TemplateProgressBarModule() {
            Title = this.__ResStr("modTitle", "ProgressBar Test Template");
            Name = this.__ResStr("modName", "Template Test - ProgressBar");
            Description = this.__ResStr("modSummary", "ProgressBar test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateProgressBarModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class Model {

            [Caption("ProgressBar"), Description("ProgressBar")]
            [UIHint("ProgressBar"), ReadOnly]
            public float Bar { get; set; }
            public float Bar_Min { get { return 0; } }
            public float Bar_Max { get { return 100; } }

            [Caption("Value"), Description("The progress bar value")]
            [UIHint("IntValue4"), Range(0, 100), Required]
            public int Value { get; set; }

            public bool __applyShown { get { return false; } }

            public Model() {
                Value = 25;
                Bar = Value;
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
