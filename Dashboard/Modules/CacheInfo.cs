/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Dashboard.Endpoints;

namespace YetaWF.Modules.Dashboard.Modules;

public class CacheInfoModuleDataProvider : ModuleDefinitionDataProvider<Guid, CacheInfoModule>, IInstallableModel { }

[ModuleGuid("{4dbd47e4-783e-4af9-bf3d-fb98a0d16574}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class CacheInfoModule : ModuleDefinition2 {

    public CacheInfoModule() {
        Title = this.__ResStr("modTitle", "Cache Information (HttpContext.Current.Cache)");
        Name = this.__ResStr("modName", "Cache Information (HttpContext.Current.Cache)");
        Description = this.__ResStr("modSummary", "Displays cache information (HttpContext.Current.Cache). Cache information can be accessed using Admin > Dashboard > HttpContext.Current.Cache (standard YetaWF site).");
        DefaultViewName = StandardViews.PropertyListEdit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new CacheInfoModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "HttpContext.Current.Cache"),
            MenuText = this.__ResStr("displayText", "HttpContext.Current.Cache"),
            Tooltip = this.__ResStr("displayTooltip", "Display cache information (HttpContext.Current.Cache)"),
            Legend = this.__ResStr("displayLegend", "Displays cache information (HttpContext.Current.Cache)"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    public class BrowseItem {
        [Caption("Key"), Description("The cache key")]
        [UIHint("String"), ReadOnly]
        public string Key { get; set; } = null!;
        [Caption("Value"), Description("The first 100 bytes of the cache value")]
        [UIHint("String"), ReadOnly]
        public string Value { get; set; } = null!;
        [Caption("Size"), Description("The size of the value (if available)")]
        [UIHint("FileFolderSize"), ReadOnly]
        public long Size { get; set; }
    }

    public class DisplayModel {

        [Caption("Entries"), Description("The current number of cached items")]
        [UIHint("LongValue"), ReadOnly]
        public long CurrentEntryCount { get; set; }

        [Caption("Estimated Size"), Description("The estimated sum of all the items currently in the memory cache - Only available if MemoryCache tracks size, which requires that a SizeLimit is set on the cache")]
        [UIHint("FileFolderSize"), ReadOnly]
        public long? CurrentEstimatedSize { get; set; }

        [Caption("Total Misses"), Description("The total number of cache misses")]
        [UIHint("LongValue"), ReadOnly]
        public long TotalMisses { get; set; }

        [Caption("Total Hits"), Description("The total number of cache hits")]
        [UIHint("LongValue"), ReadOnly]
        public long TotalHits { get; set; }

        [Caption("Cached Items"), Description("The cache keys and the values (either the data type or the first 100 bytes of data are shown)")]
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;

        public void SetData(MemoryCacheStatistics? stats) {
            if (stats is not null)
                ObjectSupport.CopyData(stats, this);
        }
    }

    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<CacheInfoModuleEndpoints>(GridSupport.BrowseGridData),
            SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                return new DataSourceResult {
                    Data = recs.Data.ToList<object>(),
                    Total = recs.Total,
                };
            },
            DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                DataProviderGetRecords<BrowseItem> browseItems = DataProviderImpl<BrowseItem>.GetRecords(GetAllItems(), skip, take, sort, filters);
                DataSourceResult data = new DataSourceResult {
                    Data = browseItems.Data.ToList<object>(),
                    Total = browseItems.Total,
                };
                return Task.FromResult(data);
            },
        };
    }

    private static List<BrowseItem> GetAllItems() {

        // https://stackoverflow.com/questions/45597057/how-to-retrieve-a-list-of-memory-cache-keys-in-asp-net-core

        // Define the collection object for scoping.  It is created as a dynamic object since the collection
        // method returns as an object array which cannot be used in a foreach loop to generate the list.
        dynamic cacheEntriesCollection = null!;

        // This action creates an empty definitions container as defined by the class type.  
        // Pull the _coherentState field for .Net version 7 or higher.  Pull the EntriesCollection 
        // property for .Net version 6 or lower.    Both of these objects are defined as private, 
        // so we need to use Reflection to gain access to the non-public entities.  
        var cacheEntriesFieldCollectionDefinition = typeof(MemoryCache).GetField("_coherentState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var cacheEntriesPropertyCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // .Net 7 or higher.
        // Starting with .Net 7.0, the EntriesCollection object was moved to being a child object of
        // the _coherentState field under the MemoryCache type.  Same process as before with an extra step.
        // Populate the coherentState field variable with the definition from above using the data in
        // our MemoryCache instance.  Then use Reflection to gain access to the private property EntriesCollection.
        if (cacheEntriesFieldCollectionDefinition != null) {
            var coherentStateValueCollection = cacheEntriesFieldCollectionDefinition.GetValue(Manager.MemoryCache);
            var entriesCollectionValueCollection = coherentStateValueCollection.GetType().GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            cacheEntriesCollection = entriesCollectionValueCollection.GetValue(coherentStateValueCollection)!;
        }

        // Define a new list we'll be adding the cache entries to
        List<BrowseItem> browseItems = new List<BrowseItem>();

        foreach (var cacheItem in cacheEntriesCollection) {
            // Get the "Value" from the key/value pair which contains the cache entry   
            ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);

            object? o = cacheItemValue.Value;
            string objectString = string.Empty;
            if (o != null) {
                objectString = o.ToString() ?? string.Empty;
                if (o is byte[] btes) {
                    objectString = System.Text.Encoding.UTF8.GetString(btes, 0, btes.Length);
                }
                // add more types as needed
            }
            BrowseItem browseItem = new BrowseItem {
                Key = cacheItemValue.Key.ToString() ?? string.Empty,
                Value = objectString.TruncateWithEllipse(100) ?? string.Empty,
                Size = cacheItemValue.Size ?? 0,
            };
            browseItems.Add(browseItem);
        }
        return browseItems;
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        DisplayModel model = new DisplayModel();

        MemoryCacheStatistics? stats = Manager.MemoryCache.GetCurrentStatistics();
        model.SetData(stats);

        model.GridDef = GetGridModel();

        //List<ICacheEntry> items = GetAllItems();
        //model.TotalSize = items.Sum(m => m.Size);

        return await RenderAsync(model);
    }
}
