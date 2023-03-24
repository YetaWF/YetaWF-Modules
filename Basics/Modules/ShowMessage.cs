/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Basics.Modules;

public class ShowMessageModuleDataProvider : ModuleDefinitionDataProvider<Guid, ShowMessageModule>, IInstallableModel { }

[ModuleGuid("{b486cdfc-3726-4549-889e-1f833eb49865}"), PublishedModuleGuid] // This permanent Guid is used in the YetaWF Core
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class ShowMessageModule : ModuleDefinition {

    public ShowMessageModule() : base() {
        Title = this.__ResStr("modTitle", "An Error Occurred");
        Name = this.__ResStr("modName", "Message (Page)");
        Description = this.__ResStr("modSummary", "Displays an error message (used when no page/module context is available).");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ShowMessageModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public async Task<ActionInfo> RenderModuleAsync(string? message, int code) {
        if (code != 0)
            Manager.CurrentResponse.StatusCode = (int)code;
        return await RenderAsync(message ?? string.Empty, ViewName: "ShowMessage", UseAreaViewName: false);
    }
}