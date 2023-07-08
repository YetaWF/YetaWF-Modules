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
using YetaWF.Modules.Blog.Components;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Support;

namespace YetaWF.Modules.Blog.Modules;

public class DisqusModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisqusModule>, IInstallableModel { }

[ModuleGuid("{3ba64dfb-9292-4f9b-937e-0c8fe110bf45}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Comments")]
public class DisqusModule : ModuleDefinition {

    public DisqusModule() {
        Title = this.__ResStr("modTitle", "Comments");
        Name = this.__ResStr("modName", "Disqus Comments");
        Description = this.__ResStr("modSummary", "Maintains all comments for a blog entry on the current page using the Disqus service. This module is typically used on the same page as the Entry Display Module.");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new DisqusModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public class DisplayModel {
        public string ShortName { get; set; } = null!;
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
            DisqusConfigData config = await dataProvider.GetItemAsync();
            if (config == null)
                throw new Error(this.__ResStr("notFound", "The Disqus settings could not be found"));
            if (string.IsNullOrWhiteSpace(config.ShortName))
                throw new Error(this.__ResStr("notShortName", "The Disqus settings must be updated to define the site's Shortname"));
            DisplayModel model = new DisplayModel {
                ShortName = config.ShortName,
            };
            return await RenderAsync(model);
        }
    }
}
