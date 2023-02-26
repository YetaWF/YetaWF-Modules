/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.SyntaxHighlighter.DataProvider;

namespace YetaWF.Modules.SyntaxHighlighter.Modules;

public class SkinHighlightJSModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinHighlightJSModule>, IInstallableModel { }

[ModuleGuid("{25068AC6-BA74-4644-8B46-9D7FEC291E45}"), PublishedModuleGuid]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Highlight.js")]
public class SkinHighlightJSModule : ModuleDefinition2 {

    public SkinHighlightJSModule() {
        Title = this.__ResStr("modTitle", "Skin Syntax Highlighter (Highlight.js)");
        Name = this.__ResStr("modName", "Syntax Highlighter Highlight.js (Skin)");
        Description = this.__ResStr("modSummary", "Skin module supporting syntax highlighting in modules using Highlight.js, referenced by sites, pages or modules, in which case <pre> .. </pre> sections are rendered using syntax highlighting.");
        WantFocus = false;
        ShowTitle = false;
        WantSearch = false;
        Invokable = true;
        InvokeInPopup = true;
        InvokeInAjax = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SkinHighlightJSModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public async Task<ActionInfo> RenderModuleAsync() {
        ConfigData config = await ConfigDataProvider.GetConfigAsync();

        Package package = AreaRegistration.CurrentPackage;
        await Manager.AddOnManager.AddAddOnNamedAsync(package.AreaName, "SkinHighlightJS", config.HighlightJSSkin);

        return ActionInfo.Empty;
    }
}