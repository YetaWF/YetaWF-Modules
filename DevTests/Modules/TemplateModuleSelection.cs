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
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateModuleSelectionModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateModuleSelectionModule>, IInstallableModel { }

    [ModuleGuid("{06D2A133-9E6A-4c02-A2C5-A9963C5A9667}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateModuleSelectionModule : ModuleDefinition {

        public TemplateModuleSelectionModule() {
            Title = this.__ResStr("modTitle", "ModuleSelection Test Component");
            Name = this.__ResStr("modName", "Component Test - ModuleSelection");
            Description = this.__ResStr("modSummary", "Test module for the ModuleSelection component. A test page for this module can be found at Tests > Templates > ModuleSelection (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateModuleSelectionModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class EditModel {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("Module Selection"), Description("Existing module")]
            [UIHint("ModuleSelection"), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public Guid Module { get; set; }

            [Caption("Module Selection (New)"), Description("New module")]
            [UIHint("ModuleSelection"), AdditionalMetadata("New", true), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public Guid ModuleNew { get; set; }

            [Caption("Module Selection (R/O)"), Description("Existing module, read/only")]
            [UIHint("ModuleSelection")]
            [ReadOnly]
            public Guid ROModule { get; set; }

            [Caption("Module Selection (New, R/O)"), Description("New module, read/only")]
            [UIHint("ModuleSelection"), AdditionalMetadata("New", true)]
            [ReadOnly]
            public Guid ROModuleNew { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public EditModel() { }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            EditModel model = new EditModel {
                ROModule = PermanentGuid,// use this module as displayed module
                ROModuleNew = PermanentGuid,
            };
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(EditModel model) {
            model.ROModule = PermanentGuid;
            model.ROModuleNew = PermanentGuid;
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
