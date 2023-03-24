/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
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

    public class TemplateTimeZoneModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTimeZoneModule>, IInstallableModel { }

    [ModuleGuid("{e088faf1-ab2b-446f-b2f8-de0ffd7e4125}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTimeZoneModule : ModuleDefinition {

        public TemplateTimeZoneModule() {
            Title = this.__ResStr("modTitle", "TimeZone Test Component");
            Name = this.__ResStr("modName", "Component Test - TimeZone");
            Description = this.__ResStr("modSummary", "Test module for the TimeZone component. A test page for this module can be found at Tests > Templates > TimeZone (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTimeZoneModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("TimeZone (Required)"), Description("TimeZone (Required)")]
            [UIHint("TimeZone"), StringLength(Globals.MaxTimeZone), Trim]
            [SelectionRequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? TimeZoneReq { get; set; }

            [Caption("TimeZone (Required w/Select)"), Description("TimeZone (Required, with (select))")]
            [UIHint("TimeZone"), AdditionalMetadata("ShowDefault", false), StringLength(Globals.MaxTimeZone), Trim]
            [SelectionRequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? TimeZoneReqSel { get; set; }

            [Caption("TimeZone"), Description("TimeZone (optional)")]
            [UIHint("TimeZone"), AdditionalMetadata("ShowDefault", false), StringLength(Globals.MaxTimeZone), Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? TimeZone { get; set; }

            [Caption("TimeZone (Read/Only)"), Description("TimeZone (read/only)")]
            [UIHint("TimeZone"), ReadOnly]
            public string TimeZoneRO { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public Model() {
                TimeZoneRO = TimeZoneInfo.Local.Id;
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
