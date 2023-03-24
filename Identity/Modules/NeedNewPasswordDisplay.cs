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
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules;

public class NeedNewPasswordDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, NeedNewPasswordDisplayModule>, IInstallableModel { }

[ModuleGuid("{E6B2C413-EBD6-439c-B69A-586C49BF17E7}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Login & Registration")]
public class NeedNewPasswordDisplayModule : ModuleDefinition {

    public NeedNewPasswordDisplayModule() {
        Title = this.__ResStr("modTitle", "New Password Required");
        Name = this.__ResStr("modName", "New Password Required");
        Description = this.__ResStr("modSummary", "Displays a warning that the user must change the login password.");

        Invokable = true;
        ShowTitle = false;
        WantFocus = false;
        WantSearch = false;
        Print = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new NeedNewPasswordDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public class DisplayModel {
        public ModuleAction ChangePasswordAction { get; set; }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!Manager.NeedNewPassword) return ActionInfo.Empty;
        if (Manager.EditMode) return ActionInfo.Empty;
        if (Manager.IsInPopup) return ActionInfo.Empty;
        using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
            if (await logInfoDP.IsExternalUserAsync(Manager.UserId))
                return ActionInfo.Empty;
        }

        UserPasswordModule modNewPassword = (UserPasswordModule)await ModuleDefinition.LoadAsync(ModuleDefinition.GetPermanentGuid(typeof(UserPasswordModule)));
        if (modNewPassword == null)
            throw new InternalError($"nameof(UserPasswordModule) module not found");

        LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
        ModuleAction actionNewPassword = modNewPassword.GetAction_UserPassword(config.ChangePasswordUrl);
        if (actionNewPassword == null)
            throw new InternalError("Change password action not found");

        DisplayModel model = new DisplayModel {
            ChangePasswordAction = actionNewPassword,
        };
        return await RenderAsync(model);
    }
}
