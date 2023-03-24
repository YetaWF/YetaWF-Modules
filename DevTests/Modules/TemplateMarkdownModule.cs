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
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateMarkdownModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateMarkdownModule>, IInstallableModel { }

    [ModuleGuid("{5873e206-5fc5-45ee-932e-1ca53251ccc5}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateMarkdownModule : ModuleDefinition {

        public TemplateMarkdownModule() {
            Title = this.__ResStr("modTitle", "Markdown Test Template");
            Name = this.__ResStr("modName", "Template Test - Markdown");
            Description = this.__ResStr("modSummary", "Test module for the MarkDown component. A test page for this module can be found at Tests > Templates > MarkDown (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateMarkdownModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public class MarkdownRequired : MarkdownStringBase {
            [Required, AdditionalMetadata("EmHeight", 10)]
            [StringLength(1000)]
            public override string? Text { get; set; }
        }
        public class MarkdownOptional : MarkdownStringBase {
            [StringLength(1000), AdditionalMetadata("EmHeight", 5)]
            public override string? Text { get; set; }
        }

        [Trim]
        public class Model {

            [Caption("Markdown (Required)"), Description("Markdown (Required)")]
            [UIHint("Markdown")]
            public MarkdownRequired Prop1Req { get; set; }

            [Caption("Markdown"), Description("Markdown")]
            [UIHint("Markdown")]
            public MarkdownOptional Prop1 { get; set; }

            [Caption("Markdown (Read/Only)"), Description("Markdown (read/only)")]
            [UIHint("Markdown"), ReadOnly]
            public MarkdownStringBase Prop1RO { get; set; }

            public Model() {
                Prop1Req = new MarkdownRequired { };
                Prop1 = new MarkdownOptional { };
                Prop1RO = new MarkdownStringBase { };
            }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model();
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(Model model) {
            model.Prop1RO = model.Prop1;
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
