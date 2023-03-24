/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

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

    public class TemplateTextAreaSourceOnlyModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTextAreaSourceOnlyModule>, IInstallableModel { }

    [ModuleGuid("{00f87a82-ca91-4436-8353-2cc09cb2b89c}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTextAreaSourceOnlyModule : ModuleDefinition {

        public TemplateTextAreaSourceOnlyModule() {
            Title = this.__ResStr("modTitle", "TextAreaSourceOnly Test Component");
            Name = this.__ResStr("modName", "Component Test - TextAreaSourceOnly");
            Description = this.__ResStr("modSummary", "Test module for the TextAreaSourceOnly component. A test page for this module can be found at Tests > Templates > TextAreaSourceOnly (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTextAreaSourceOnlyModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("TextAreaSourceOnly (Required)"), Description("TextAreaSourceOnly (Required)")]
            [UIHint("TextAreaSourceOnly"), StringLength(0), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Prop1Req { get; set; }
            public string Prop1Req_PlaceHolder { get { return this.__ResStr("prop1ReqPH", "(This is a required field)"); } }

            [Caption("TextAreaSourceOnly"), Description("TextAreaSourceOnly")]
            [UIHint("TextAreaSourceOnly"), StringLength(0), Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Prop1 { get; set; }
            public string Prop1_PlaceHolder { get { return this.__ResStr("prop1PH", "(This is an optional field)"); } }

            [Caption("TextAreaSourceOnly (Read/Only)"), Description("TextAreaSourceOnly (read/only)")]
            [UIHint("TextAreaSourceOnly"), ReadOnly]
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
            model.Prop1RO = model.Prop1Req;
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
