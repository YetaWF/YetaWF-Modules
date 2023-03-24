/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateEmailModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateEmailModule>, IInstallableModel { }

    [ModuleGuid("{d7c549fe-09f2-494f-8ac7-db332d579589}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateEmailModule : ModuleDefinition {

        public TemplateEmailModule() {
            Title = this.__ResStr("modTitle", "Email Test Component");
            Name = this.__ResStr("modName", "Component Test - Email");
            Description = this.__ResStr("modSummary", "Test module for the Email component. A test page for this module can be found at Tests > Templates > Email (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateEmailModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("Email (Required)"), Description("Email (Required)")]
            [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? EmailReq { get; set; }

            [Caption("Email"), Description("Email")]
            [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Email { get; set; }

            [Caption("Email (Read/Only)"), Description("Email (read/only)")]
            [UIHint("Email"), ReadOnly]
            public string EmailRO { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public Model() {
                EmailRO = "mikevdm@mikevdm.com";
            }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model();
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
