/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.BootstrapCarousel.Models;

namespace YetaWF.Modules.BootstrapCarousel.Modules;

public class CarouselDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, CarouselDisplayModule>, IInstallableModel { }

[ModuleGuid("{d2ddaf6d-dce0-4250-95b2-48a769e04c74}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class CarouselDisplayModule : ModuleDefinition {

    public CarouselDisplayModule() {
        Title = this.__ResStr("modTitle", "Bootstrap Carousel");
        Name = this.__ResStr("modName", "Bootstrap Carousel");
        Description = this.__ResStr("modSummary", "Displays a Bootstrap-like carousel (does not require Bootstrap). A sample Bootstrap Carousel can be found at Tests > Bootstrap Carousel (standard YetaWF site).");
        SlideShow = new CarouselInfo();
        WantFocus = false;
        WantSearch = false;
        ShowTitle = false;
        Print = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new CarouselDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Copy] // not shown in property page, but copy during module settings edit
    public CarouselInfo SlideShow { get; set; }

    public override Task ModuleSavingAsync() {
        return SlideShow.SavingAsync("SlideShow", ModuleGuid); // update internal information (images)
    }

    public ModuleAction GetAction_Display(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Bootstrap Carousel"),
            MenuText = this.__ResStr("displayText", "Bootstrap Carousel"),
            Tooltip = this.__ResStr("displayTooltip", "Display the Bootstrap carousel"),
            Legend = this.__ResStr("displayLegend", "Displays a Bootstrap carousel"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,

        };
    }

    public class Model {
        [UIHint("YetaWF_BootstrapCarousel_SlideShow")]
        public CarouselInfo SlideShow { get; set; }

        public Model() {
            SlideShow = new CarouselInfo();
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!Manager.EditMode && SlideShow.Slides.Count == 0) return ActionInfo.Empty;
        Model model = new Model {
            SlideShow = SlideShow
        };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        SlideShow = model.SlideShow;
        await SaveAsync();
        model.SlideShow = SlideShow;
        return await FormProcessedAsync(model, OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace);
    }
}
