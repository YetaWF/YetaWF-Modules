/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.Modules;

public class SkinSiteAnnouncementsModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinSiteAnnouncementsModule>, IInstallableModel { }

[ModuleGuid("{54F6B691-B835-4568-90AA-AA9B308D4272}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class SkinSiteAnnouncementsModule : ModuleDefinition2 {

    public SkinSiteAnnouncementsModule() {
        Title = this.__ResStr("modTitle", "Skin Site Announcements");
        Name = this.__ResStr("modName", "Site Announcements (Skin)");
        Description = this.__ResStr("modSummary", "Skin module used to support site announcements");
        WantFocus = false;
        ShowTitle = false;
        WantSearch = false;
        Invokable = true;
        InvokeInPopup = false;
        InvokeInAjax = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SkinSiteAnnouncementsModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public async Task<ActionInfo> RenderModuleAsync() {

        await SignalR.UseAsync();
        await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.AreaName, nameof(SkinSiteAnnouncementsModule));
        Manager.ScriptManager.AddLast($"{AreaRegistration.CurrentPackage.AreaName}_{ClassName}", $"new YetaWF_Messenger.SkinSiteAnnouncementsModule('{Utility.JserEncode(ModuleHtmlId)}');");

        return ActionInfo.Empty;
    }
}
