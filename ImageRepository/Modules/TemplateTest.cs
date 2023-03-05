/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.ImageRepository.Components;

namespace YetaWF.Modules.ImageRepository.Modules {

    public class TemplateTestModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTestModule>, IInstallableModel { }

    [ModuleGuid("{66fb78ed-a185-4251-8115-d783b5554b37}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTestModule : ModuleDefinition2 {

        public TemplateTestModule() {
            Title = this.__ResStr("modTitle", "Image Repository Component Test");
            Name = this.__ResStr("modName", "Image Repository Component Test");
            Description = this.__ResStr("modSummary", "Image Repository Component Test");
            WantSearch = false;
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTestModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class Model {

            [Caption("Image Selection"), Description("Description of image selection")]
            [UIHint("YetaWF_ImageRepository_ImageSelection"), Required, Trim]
            public string? ImageName { get; set; }
            public ImageSelectionInfo? ImageName_Info { get; set; }

            public Model() { }

            public async Task UpdateAsync(ModuleDefinition module) {
                ImageName_Info = new ImageSelectionInfo(module.ModuleGuid, null) {
                    AllowUpload = true,
                };
                await ImageName_Info.InitAsync();
            }
        }

        public async Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model { };
            await model.UpdateAsync(this);
            return await RenderAsync(model);
        }

        [ConditionalAntiForgeryToken]
        public async Task<IResult> UpdateModuleAsync(Model model) {
            await model.UpdateAsync(this);
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Image saved"), OnClose: OnCloseEnum.Return, OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}