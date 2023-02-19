/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateMultiStringModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateMultiStringModule>, IInstallableModel { }

    [ModuleGuid("{4485D52E-A8E3-40fa-9251-B6F7A34CEDA1}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateMultiStringModule : ModuleDefinition2 {

        public TemplateMultiStringModule() {
            Title = this.__ResStr("modTitle", "MultiString Test Component");
            Name = this.__ResStr("modName", "Component Test - MultiString");
            Description = this.__ResStr("modSummary", "Test module for the MultiString component. A test page for this module can be found at Tests > Templates > MultiString (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateMultiStringModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class EditModel {

            public enum ControlStatusEnum { Normal, Disabled, }

            [Category("Core"), Caption("MultiString"), Description("MultiString (Required)")]
            [UIHint("MultiString"), StringLength(200)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public MultiString MultiString { get; set; }

            [Category("Core"), Caption("MultiString10"), Description("MultiString10 (Required)")]
            [UIHint("MultiString10"), StringLength(10)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public MultiString MultiString10 { get; set; }

            [Category("Core"), Caption("MultiString20"), Description("MultiString20 (Required)")]
            [UIHint("MultiString20"), StringLength(20)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public MultiString MultiString20 { get; set; }

            [Category("Core"), Caption("MultiString40"), Description("MultiString40 (Required)")]
            [UIHint("MultiString40"), StringLength(40)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public MultiString MultiString40 { get; set; }

            [Category("Core"), Caption("MultiString80"), Description("MultiString80 (Required)")]
            [UIHint("MultiString80"), StringLength(80)]
            [RequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public MultiString MultiString80 { get; set; }

            [Category("Core"), Caption("MultiString"), Description("MultiString")]
            [UIHint("MultiString"), StringLength(200)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public MultiString MultiStringOpt { get; set; }

            [Category("Core"), Caption("MultiString10"), Description("MultiString10")]
            [UIHint("MultiString10"), StringLength(10)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public MultiString MultiString10Opt { get; set; }

            [Category("Core"), Caption("MultiString20"), Description("MultiString20")]
            [UIHint("MultiString20"), StringLength(20)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public MultiString MultiString20Opt { get; set; }

            [Category("Core"), Caption("MultiString40"), Description("MultiString40")]
            [UIHint("MultiString40"), StringLength(40)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public MultiString MultiString40Opt { get; set; }

            [Category("Core"), Caption("MultiString80"), Description("MultiString80")]
            [UIHint("MultiString80"), StringLength(80)]
            [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
            public MultiString MultiString80Opt { get; set; }

            [Category("Core"), Caption("MultiString"), Description("MultiString")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString MultiStringRO { get; set; }

            [Category("Core"), Caption("MultiString10"), Description("MultiString10")]
            [UIHint("MultiString10"), ReadOnly]
            public MultiString MultiString10RO { get; set; }

            [Category("Core"), Caption("MultiString20"), Description("MultiString20")]
            [UIHint("MultiString20"), ReadOnly]
            public MultiString MultiString20RO { get; set; }

            [Category("Core"), Caption("MultiString40"), Description("MultiString40")]
            [UIHint("MultiString40"), ReadOnly]
            public MultiString MultiString40RO { get; set; }

            [Category("Core"), Caption("MultiString80"), Description("MultiString80")]
            [UIHint("MultiString80"), ReadOnly]
            public MultiString MultiString80RO { get; set; }

            [Caption("Control Status"), Description("Defines the processing status of the controls")]
            [UIHint("Enum")]
            public ControlStatusEnum ControlStatus { get; set; }

            public EditModel() {
                MultiString = new MultiString();
                MultiString10 = new MultiString();
                MultiString20 = new MultiString();
                MultiString40 = new MultiString();
                MultiString80 = new MultiString();
                MultiStringOpt = new MultiString();
                MultiString10Opt = new MultiString();
                MultiString20Opt = new MultiString();
                MultiString40Opt = new MultiString();
                MultiString80Opt = new MultiString();
                MultiStringRO = MultiString;
                MultiString10RO = MultiString10;
                MultiString20RO = MultiString20;
                MultiString40RO = MultiString40;
                MultiString80RO = MultiString80;
            }
        }
        
        public Task<ActionInfo> RenderModuleAsync() {
            EditModel model = new EditModel();
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(EditModel model) {
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
