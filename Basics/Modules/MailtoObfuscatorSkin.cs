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

public class MailtoObfuscatorSkinModuleDataProvider : ModuleDefinitionDataProvider<Guid, MailtoObfuscatorSkinModule>, IInstallableModel { }

[ModuleGuid("{749d0ca9-75e5-40b8-82e3-466a11d3b1d2}")] // Published Guid
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class MailtoObfuscatorSkinModule : ModuleDefinition {

    public MailtoObfuscatorSkinModule() {
        Title = this.__ResStr("modTitle", "Mailto Obfuscator");
        Name = this.__ResStr("modName", "Mailto Obfuscator (Skin)");
        Description = this.__ResStr("modSummary", "Skin module implementing mailto: obfuscation using JavaScript. Can be enabled by referencing it site-wide or per page.");
        WantFocus = false;
        WantSearch = false;
        ShowTitle = false;
        Invokable = true;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new MailtoObfuscatorSkinModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public async Task<ActionInfo> RenderModuleAsync() {
        YetaWF.Core.Packages.Package package = YetaWF.Modules.Basics.AreaRegistration.CurrentPackage;
        await Manager.AddOnManager.AddAddOnNamedAsync(package.AreaName, "MailToObfuscator");
        return ActionInfo.Empty;
    }
}
