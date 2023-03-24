/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

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
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Modules;

public class DisplayStepsModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisplayStepsModule>, IInstallableModel { }

[ModuleGuid("{fff6a061-5b49-4501-ad70-3138ec1bf1b3}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class DisplayStepsModule : ModuleDefinition {

    public DisplayStepsModule() {
        Title = this.__ResStr("modTitle", "Steps");
        Name = this.__ResStr("modName", "Display Steps");
        Description = this.__ResStr("modSummary", "Displays steps to complete an action, usually a sequence of forms or pages. A sample page is available at Tests > Modules > Steps (standard YetaWF site).");
        StepInfo = new Models.StepInfo();
        UsePartialFormCss = false;
        ShowTitle = false;
        WantFocus = false;
        WantSearch = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new DisplayStepsModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    [Copy] // not shown in property page
    public StepInfo StepInfo { get; set; }

    public override Task ModuleSavingAsync() {
        StepInfo.Saving(nameof(StepInfo), ModuleGuid); // update internal information
        return Task.CompletedTask;
    }

    public class Model {

        [UIHint("YetaWF_Panels_StepInfo")]
        public StepInfo StepInfo { get; set; }

        public Model() {
            StepInfo = new StepInfo();
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (Manager.IsInPopup) return ActionInfo.Empty;
        Model model = new Model();
        model.StepInfo = StepInfo;
        return await RenderAsync(model);
    }
    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        StepInfo = model.StepInfo;
        await SaveAsync();
        model.StepInfo = StepInfo;
        return await FormProcessedAsync(model, OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace);
    }
}
