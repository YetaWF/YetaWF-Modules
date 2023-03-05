/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Dashboard.DataProvider;
using YetaWF.Modules.Dashboard.Endpoints;

namespace YetaWF.Modules.Dashboard.Modules;

public class BrowseDataProvidersModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseDataProvidersModule>, IInstallableModel { }

[ModuleGuid("{bb4f1bf1-eebf-4e65-8992-c4f673737c26}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class BrowseDataProvidersModule : ModuleDefinition2 {

    public BrowseDataProvidersModule() {
        Title = this.__ResStr("modTitle", "Data Providers");
        Name = this.__ResStr("modName", "Data Providers");
        Description = this.__ResStr("modSummary", "Displays installed data providers. Installed data providers can be accessed using Admin > Dashboard > Data Providers (standard YetaWF site).");
        DefaultViewName = StandardViews.PropertyListEdit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BrowseDataProvidersModuleDataProvider(); }

    public ModuleAction GetAction_Items(string url) {
        return new ModuleAction() {
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

    public class BrowseItem {

        [Caption("I/O Mode"), Description("The I/O mode supported by this data provider")]
        [UIHint("String"), ReadOnly]
        public string IOModeName { get; set; } = null!;
        [Caption("Type Name"), Description("The type of the supported data provider")]
        [UIHint("String"), ReadOnly]
        public string TypeName { get; set; } = null!;
        [Caption("Implementation Type Name"), Description("The type of the implementation of the supported data provider")]
        [UIHint("String"), ReadOnly]
        public string TypeImplName { get; set; } = null!;

        public BrowseItem(DataProviderInfo data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public class BrowseModel {
        [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<BrowseDataProvidersModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (DataProviderInfoDataProvider dataProvider = new DataProviderInfoDataProvider()) {
                    DataProviderGetRecords<DataProviderInfo> browseItems = dataProvider.GetItems(skip, take, sort, filters);
                    DataSourceResult data = new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                    return Task.FromResult(data);
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
