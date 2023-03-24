/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

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
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Modules;

public class DisqusLinksModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisqusLinksModule>, IInstallableModel { }

[ModuleGuid("{776adfcd-da5f-4926-b29d-4c06353266c0}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Comments")]
public class DisqusLinksModule : ModuleDefinition {

    public DisqusLinksModule() {
        Title = this.__ResStr("modTitle", "Disqus Links");
        Name = this.__ResStr("modName", "Disqus Links (Skin)");
        Description = this.__ResStr("modSummary", "Adds number of comments to links to pages with Disqus comments. This module is typically used as a module reference by pages or a site to add the number of comments available to pages with links to pages with Disqus comments.");
        WantFocus = false;
        WantSearch = false;
        ShowTitle = false;
        Invokable = true;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new DisqusLinksModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public class DisplayModel {
        public string ShortName { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
            DisqusConfigData config = await dataProvider.GetItemAsync();
            if (config == null)
                return ActionInfo.Empty;
            if (string.IsNullOrWhiteSpace(config.ShortName))
                return ActionInfo.Empty;
            DisplayModel model = new DisplayModel {
                ShortName = config.ShortName,
            };
            return await RenderAsync(model);
        }
    }
}
