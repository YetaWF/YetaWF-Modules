/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules;

public class RedirectLoggedOnUsersModuleDataProvider : ModuleDefinitionDataProvider<Guid, RedirectLoggedOnUsersModule>, IInstallableModel { }

[ModuleGuid("{840f8753-ff43-41a4-8f27-b7d5d54ae198}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Tools")]
public class RedirectLoggedOnUsersModule : ModuleDefinition2 {

    public RedirectLoggedOnUsersModule() {
        Title = this.__ResStr("modTitle", "Redirect Logged On Users");
        Name = this.__ResStr("modName", "Redirect Logged On Users");
        Description = this.__ResStr("modSummary", "Redirects logged on users to other pages.");
        DefaultViewName = StandardViews.Display;
        ShowFormButtons = false;
        ShowHelp = false;
        ShowTitle = false;
        ShowTitleActions = false;
        WantFocus = false;
        WantSearch = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new RedirectLoggedOnUsersModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return UserLevel_DefaultAllowedRoles; } }

    public async Task<ActionInfo> RenderModuleAsync() {

        if (Manager.HaveUser && !Manager.EditMode) {
            string nextUrl = await Resource.ResourceAccess.GetUserPostLoginUrlAsync(Manager.UserRoles);
            if (string.IsNullOrWhiteSpace(nextUrl))
                nextUrl = Manager.CurrentSite.PostLoginUrl;
            if (!string.IsNullOrWhiteSpace(nextUrl))
                return RedirectToUrl(nextUrl);
        }
        return ActionInfo.Empty;
    }
}
