using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateMaskedEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateMaskedEditModule>, IInstallableModel { }

    [ModuleGuid("{8494d18a-a8da-40e4-a3d6-05fb75b76d53}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateMaskedEditModule : ModuleDefinition {

        public TemplateMaskedEditModule() {
            Title = this.__ResStr("modTitle", "MaskedEdit Test Template");
            Name = this.__ResStr("modName", "Template Test - MaskedEdit");
            Description = this.__ResStr("modSummary", "MaskedEdit test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateMaskedEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("SSN (Required)"), Description("Social Security Number (Required)")]
            [UIHint("SSN"), SSNValidation, Required, Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Prop1Req { get; set; }

            [Caption("SSN"), Description("Social Security Number")]
            [UIHint("SSN"), SSNValidation, Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Prop1 { get; set; }

            [Caption("SSN (Read/Only)"), Description("Social Security Number (read/only)")]
            [UIHint("SSN"), ReadOnly]
            public string? Prop1RO { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public Model() { }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model();
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            model.Prop1RO = model.Prop1;
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
