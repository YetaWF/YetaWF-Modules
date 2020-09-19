/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
#endif

namespace YetaWF.Modules.Dashboard.Modules {

    public class BrowseDataProvidersModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseDataProvidersModule>, IInstallableModel { }

    [ModuleGuid("{bb4f1bf1-eebf-4e65-8992-c4f673737c26}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BrowseDataProvidersModule : ModuleDefinition {

        public BrowseDataProvidersModule() {
            Title = this.__ResStr("modTitle", "Data Providers");
            Name = this.__ResStr("modName", "Data Providers");
            Description = this.__ResStr("modSummary", "Displays installed data providers. Installed data providers can be accessed using Admin > Dashboard > Data Providers (standard YetaWF site).");
            DefaultViewName = StandardViews.PropertyListEdit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BrowseDataProvidersModuleDataProvider(); }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Data Providers"),
                MenuText = this.__ResStr("browseText", "Data Providers"),
                Tooltip = this.__ResStr("browseTooltip", "Display installed data providers"),
                Legend = this.__ResStr("browseLegend", "Displays  installed data providers"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
    }
}
