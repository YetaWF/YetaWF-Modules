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
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateTestUrlModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTestUrlModule>, IInstallableModel { }

    [ModuleGuid("{fbcc7d7a-5090-48a7-9958-6eff8b8c6d7e}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTestUrlModule : ModuleDefinition2 {

        public TemplateTestUrlModule() {
            Title = this.__ResStr("modTitle", "Url Test Component");
            Name = this.__ResStr("modName", "Component Test - Url");
            Description = this.__ResStr("modSummary", "Test module for the Url component. A test page for this module can be found at Tests > Templates > Url (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTestUrlModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class EditModel {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("Url"), Description("Url")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Url { get; set; }

            [Caption("Url2"), Description("Url2")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? Url2 { get; set; }

            [Caption("Local Url"), Description("Local Url")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
            [StringLength(Globals.MaxUrl), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? LocalUrl { get; set; }

            [Caption("Remote Url"), Description("Remote")]
            [UIHint("Url"), UrlValidation]
            [StringLength(Globals.MaxUrl), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? RemoteUrl { get; set; }

            [Caption("Url"), Description("Url")]
            [UIHint("Url"), ReadOnly]
            public string ROUrl { get; set; }

            [Caption("Local Url"), Description("Local Url")]
            [UIHint("Url"), ReadOnly]
            public string ROLocalUrl { get; set; }

            [Caption("Remote Url"), Description("Remote")]
            [UIHint("Url"), ReadOnly]
            public string RORemoteUrl { get; set; }

            [Caption("New Url"), Description("New Url")]
            [UIHint("Text80"), StringLength(Globals.MaxUrl), UrlValidation(urlType: UrlTypeEnum.New), Trim]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public string? NewUrl { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public EditModel() {
                ROUrl = "/Tests/Text";
                ROLocalUrl = "/Tests/Text";
                RORemoteUrl = "https://softelvdm.com";
            }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            EditModel model = new EditModel {
                Url = "/Tests/Text",
                Url2 = "https://softelvdm.com",
                LocalUrl = "/Tests/Text",
                RemoteUrl = "https://softelvdm.com",
            };
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(EditModel model) {
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
