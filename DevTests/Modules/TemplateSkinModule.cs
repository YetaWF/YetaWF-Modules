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
using YetaWF.Core.Skins;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateSkinModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateSkinModule>, IInstallableModel { }

    [ModuleGuid("{b49b3d02-29d4-4466-9ebd-20209f0ba7de}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateSkinModule : ModuleDefinition2 {

        public TemplateSkinModule() {
            Title = this.__ResStr("modTitle", "Skin Selection Test Component");
            Name = this.__ResStr("modName", "Component Test - Skin Selection");
            Description = this.__ResStr("modSummary", "Test module for the SkinSelection component. A test page for this module can be found at Tests > Templates > Skins (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateSkinModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class Model {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Caption("Skin"), Description("Skin")]
            [UIHint("Skin"), Trim]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public SkinDefinition Skin { get; set; }

            [Caption("Skin (Read/Only)"), Description("Skin (Read/Only)")]
            [UIHint("Skin"), ReadOnly]
            public SkinDefinition SkinRO { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public Model() {
                Skin = new SkinDefinition();
                SkinRO = new SkinDefinition();
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
