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

    public class TestExprAttributesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TestExprAttributesModule>, IInstallableModel { }

    [ModuleGuid("{8B4EAD54-BC75-459f-8388-4056CB1234D3}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TestExprAttributesModule : ModuleDefinition2 {

        public TestExprAttributesModule() {
            Title = this.__ResStr("modTitle", "Test ExprAttributes");
            Name = this.__ResStr("modName", "Test ExprAttributes");
            Description = this.__ResStr("modSummary", "Test module for various property attributes. This is used for internal testing only.");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TestExprAttributesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class EditModel {

            [Caption("On/Off ProcessIf"), Description("")]
            [UIHint("Boolean")]
            public bool OnOff { get; set; }

            [Caption("String1"), Description("")]
            [UIHint("Text80"), StringLength(80), Trim]
            [ProcessIf(nameof(OnOff), true, Disable = true)]
            public string? String1 { get; set; }

            [Caption("String2"), Description("")]
            [UIHint("Text80"), StringLength(80), Trim]
            [ProcessIf(nameof(OnOff), true)]
            public string? String2 { get; set; }

            public EditModel() { }
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
