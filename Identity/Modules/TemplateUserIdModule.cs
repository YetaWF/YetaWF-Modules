/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules;

public class TemplateUserIdModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateUserIdModule>, IInstallableModel { }

[ModuleGuid("{985c4c49-8103-4b5c-a9ae-2bb108ef58a6}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Tools")]
public class TemplateUserIdModule : ModuleDefinition2 {

    public TemplateUserIdModule() {
        Title = this.__ResStr("modTitle", "UserId Test Component");
        Name = this.__ResStr("modName", "Component Test - UserId");
        Description = this.__ResStr("modSummary", "Test module for the UserId component (edit and display). A test page for this module can be found at Tests > Templates > UserId (standard YetaWF site).");
        DefaultViewName = StandardViews.EditApply;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new TemplateUserIdModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "UserId"),
            MenuText = this.__ResStr("displayText", "UserId"),
            Tooltip = this.__ResStr("displayTooltip", "Display the UserId test template"),
            Legend = this.__ResStr("displayLegend", "Displays the UserId test template"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    [Header("Test case for the YetaWF_Identity_UserId template, which allows selection of 1 user. " +
        "If fewer than 50 users are available, a dropdown list is shown. " +
        "For more than 50, a scrollable grid (Ajax) is used instead to support thousands of users. " +
        "Some of these fields explicitly force a grid display even with fewer than 50 users.")]
    public class Model {

        public enum ControlStatusEnum { Normal, Disabled, }

        [Caption("UserId (Required)"), Description("UserId (Required)")]
        [UIHint("YetaWF_Identity_UserId"), AdditionalMetadata("Force", "DropDown"), Trim]
        [SelectionRequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
        [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
        public int Prop1Req { get; set; }

        [Caption("UserId (Grid, Required)"), Description("UserId (Required)")]
        [UIHint("YetaWF_Identity_UserId"), AdditionalMetadata("Force", "Grid"), Trim]
        [SelectionRequiredIf(nameof(ControlStatus), ControlStatusEnum.Normal)]
        [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
        public int Prop1GridReq { get; set; }

        [Caption("UserId"), Description("UserId")]
        [UIHint("YetaWF_Identity_UserId"), AdditionalMetadata("Force", "DropDown"), Trim]
        [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
        public int Prop1 { get; set; }

        [Caption("UserId (Grid)"), Description("UserId")]
        [UIHint("YetaWF_Identity_UserId"), AdditionalMetadata("Force", "Grid"), Trim]
        [ProcessIf(nameof(ControlStatus), ControlStatusEnum.Normal, Disable = true)]
        public int Prop1Grid { get; set; }

        [Caption("UserId (Read/Only)"), Description("UserId (read/only)")]
        [UIHint("YetaWF_Identity_UserId"), ReadOnly]
        public int Prop1RO { get; set; }

        [Caption("Control Status"), Description("Defines the processing status of the controls")]
        [UIHint("Enum")]
        public ControlStatusEnum ControlStatus { get; set; }

        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Model model = new Model {
            Prop1Req = Manager.UserId,
            Prop1GridReq = Manager.UserId,
            Prop1 = Manager.UserId,
            Prop1Grid = Manager.UserId,
            Prop1RO = Manager.UserId,
        };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        model.Prop1RO = Manager.UserId;
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        return await FormProcessedAsync(model, this.__ResStr("ok", "OK"));
    }
}
