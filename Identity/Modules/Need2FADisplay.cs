/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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

public class Need2FADisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, Need2FADisplayModule>, IInstallableModel { }

[ModuleGuid("{661bb0dd-769f-4850-bd6f-3d1c563e84b2}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Two Step Authentication")]
public class Need2FADisplayModule : ModuleDefinition2 {

    public Need2FADisplayModule() {
        Title = this.__ResStr("modTitle", "Two-Step Authentication Setup Required");
        Name = this.__ResStr("modName", "Two-Step Authentication Setup Required");
        Description = this.__ResStr("modSummary", "Displays a warning that the user must complete two-step authentication setup.");

        Invokable = true;
        ShowTitle = false;
        WantFocus = false;
        WantSearch = false;
        Print = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new Need2FADisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public class DisplayModel {
        public ModuleAction SetupAction { get; set; }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!Manager.Need2FA) return ActionInfo.Empty;
        if (Manager.EditMode) return ActionInfo.Empty;
        if (Manager.IsInPopup) return ActionInfo.Empty;

        SelectTwoStepSetupModule mod2FA = (SelectTwoStepSetupModule)await ModuleDefinition.LoadAsync(ModuleDefinition.GetPermanentGuid(typeof(SelectTwoStepSetupModule)));
        if (mod2FA == null)
            throw new InternalError("Two-step authentication setup module not found");

        ModuleAction action2FA = await mod2FA.GetAction_SelectTwoStepSetupAsync(null);
        if (action2FA == null)
            throw new InternalError("Two-step authentication setup action not found");

        DisplayModel model = new DisplayModel {
            SetupAction = action2FA,
        };
        return await RenderAsync(model);
    }
}
