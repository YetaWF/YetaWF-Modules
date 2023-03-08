/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Visitors.DataProvider;

public class VisitorEntry {

    public const int MaxSessionId = 50;
    public const int MaxError = 200;
    public const int MaxUserAgent = 400;
    public const int MaxCity = 30;
    public const int MaxRegionCode = 10;
    public const int MaxCountryCode = 10;
    public const int MaxContinentCode = 10;

    public const string Unknown = "??";

    [Data_PrimaryKey, Data_Identity]
    public int Key { get; set; }

    [StringLength(MaxSessionId)]
    public string? SessionId { get; set; }
    [Data_Index]
    public DateTime AccessDateTime { get; set; }
    public int UserId { get; set; }
    [StringLength(Globals.MaxIP)]
    public string IPAddress { get; set; } = null!;
    [StringLength(Globals.MaxUrl)]
    public string? Url { get; set; }
    [StringLength(Globals.MaxUrl)]
    public string? Referrer { get; set; }
    [StringLength(MaxUserAgent)]
    public string? UserAgent { get; set; }
    [StringLength(MaxError)]
    public string? Error { get; set; }

    public float Latitude { get; set; }
    public float Longitude { get; set; }
    [StringLength(MaxContinentCode)]
    public string? ContinentCode { get; set; }
    [StringLength(MaxCountryCode)]
    public string? CountryCode { get; set; }
    [StringLength(MaxRegionCode)]
    public string? RegionCode { get; set; }
    [StringLength(MaxCity)]
    public string? City { get; set; }

    public VisitorEntry() { }
}

public interface VisitorEntryDataProviderIOMode {
    Task<VisitorEntryDataProvider.Info> GetStatsAsync();
    Task UpdateSameIPAddressesAsync(VisitorEntry visitorEntry);
}

public class VisitorEntryDataProvider : DataProviderImpl, IInstallableModel, IInitializeApplicationStartup {

    public Task InitializeApplicationStartupAsync() {
        ErrorHandling.RegisterCallback(AddVisitEntryError);
        PageLogging.RegisterCallback(AddVisitEntryUrlAsync);
        return Task.CompletedTask;
    }

    public bool Usable { get { return DataProvider != null; } }

    // IMPLEMENTATION
    // IMPLEMENTATION
    // IMPLEMENTATION

    public VisitorEntryDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
    public VisitorEntryDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

    private IDataProvider<int, VisitorEntry> DataProvider { get { return GetDataProvider(); } }
    private VisitorEntryDataProviderIOMode DataProviderIOMode { get { return GetDataProvider(); } }

    private IDataProvider<int, VisitorEntry>? CreateDataProvider() {
        Package package = YetaWF.Modules.Visitors.AreaRegistration.CurrentPackage;
        return MakeDataProvider(package, package.AreaName, SiteIdentity: SiteIdentity, Cacheable: true, Parms: new { NoLanguages = true } );
    }

    // API
    // API
    // API

    public async Task<VisitorEntry?> GetItemAsync(int key) {
        if (!Usable) return null;
        return await DataProvider.GetAsync(key);
    }
    public async Task<bool> AddItemAsync(VisitorEntry data) {
        if (!Usable || SiteIdentity == 0) return false;
        data.Referrer = data.Referrer?.Truncate(Globals.MaxUrl);
        data.Url = data.Url?.Truncate(Globals.MaxUrl);
        return await DataProvider.AddAsync(data);
    }
    public async Task<UpdateStatusEnum> UpdateItemAsync(VisitorEntry data) {
        if (!Usable) return UpdateStatusEnum.RecordDeleted;
        return await UpdateItemAsync(data.Key, data);
    }
    public async Task<UpdateStatusEnum> UpdateItemAsync(int originalKey, VisitorEntry data) {
        if (!Usable) return UpdateStatusEnum.RecordDeleted;
        return await DataProvider.UpdateAsync(originalKey, data.Key, data);
    }
    public async Task<bool> RemoveItemAsync(int key) {
        if (!Usable) return false;
        return await DataProvider.RemoveAsync(key);
    }
    public async Task<DataProviderGetRecords<VisitorEntry>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) {
        if (!Usable)
            return new DataProviderGetRecords<VisitorEntry>();
        return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
    }
    public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
        if (!Usable) return 0;
        return await DataProvider.RemoveRecordsAsync(filters);
    }
    public class Info {
        public int TodaysAnonymous { get; set; }
        public int TodaysUsers { get; set; }
        public int YesterdaysAnonymous { get; set; }
        public int YesterdaysUsers { get; set; }
    }
    public async Task<Info> GetStatsAsync() {
        if (!Usable) return new VisitorEntryDataProvider.Info();
        return await DataProviderIOMode.GetStatsAsync();
    }
    public async Task UpdateSameIPAddressesAsync(VisitorEntry visitorEntry) {
        // update all records that have the same IP Address
        await DataProviderIOMode.UpdateSameIPAddressesAsync(visitorEntry);
    }

    // LOGGING CALLBACKS
    // LOGGING CALLBACKS
    // LOGGING CALLBACKS

    public static void AddVisitEntryError(string error) {
        YetaWFManager.Syncify(async () => {
            await AddVisitEntryAsync(null, error);
        });
    }
    public static Task AddVisitEntryUrlAsync(string url, bool full) {
        return AddVisitEntryAsync(url);
    }

    private static async Task AddVisitEntryAsync(string? url, string? error = null) {

        if (!InCallback) {
            InCallback = true;

            try {

                if (!YetaWFManager.HaveManager || !YetaWFManager.Manager.HaveCurrentSite || !YetaWFManager.Manager.HaveCurrentContext) return;
                YetaWFManager manager = YetaWFManager.Manager;

                using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {

                    if (!visitorDP.Usable) return;

                    string? sessionId = manager.CurrentSessionId;
                    if (url == null)
                        url = manager.CurrentRequestUrl;
                    string userAgent = manager.CurrentRequest.Headers["User-Agent"].ToString();
                    string referrer = manager.ReferrerUrl;

                    VisitorEntry visitorEntry = new VisitorEntry {
                        SessionId = sessionId,
                        AccessDateTime = DateTime.UtcNow,
                        UserId = manager.UserId,
                        IPAddress = manager.UserHostAddress.Truncate(Globals.MaxIP),
                        Url = url != null ? url.Truncate(Globals.MaxUrl) : "",
                        Referrer = referrer != null ? referrer.Truncate(Globals.MaxUrl) : "",
                        UserAgent = userAgent != null ? userAgent.Truncate(VisitorEntry.MaxUserAgent) : "",
                        Longitude = 0.0f,
                        Latitude = 0.0f,
                        ContinentCode = VisitorEntry.Unknown,
                        CountryCode = VisitorEntry.Unknown,
                        RegionCode = VisitorEntry.Unknown,
                        City = VisitorEntry.Unknown,
                        Error = error?.Truncate(VisitorEntry.MaxError),
                    };
                    await visitorDP.AddItemAsync(visitorEntry);
                }
            } catch (Exception) {
            } finally {
                InCallback = false;
            }
        }
    }
    private static bool InCallback = false;

    // IINSTALLABLEMODEL
    // IINSTALLABLEMODEL
    // IINSTALLABLEMODEL

    public new async Task<bool> IsInstalledAsync() {
        if (!Usable) return false;
        return await DataProvider.IsInstalledAsync();
    }
    public new async Task<bool> InstallModelAsync(List<string> errorList) {
        if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Installing new models is not possible when distributed caching is enabled");
        if (!Usable) return true;
        return await DataProvider.InstallModelAsync(errorList);
    }
    public new async Task<bool> UninstallModelAsync(List<string> errorList) {
        if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Uninstalling models is not possible when distributed caching is enabled");
        if (!Usable) return true;
        return await DataProvider.UninstallModelAsync(errorList);
    }
    public new async Task AddSiteDataAsync() {
        if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Adding site data is not possible when distributed caching is enabled");
        if (!Usable) return;
        await DataProvider.AddSiteDataAsync();
    }
    public new async Task RemoveSiteDataAsync() {
        if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Removing site data is not possible when distributed caching is enabled");
        if (!Usable) return;
        await DataProvider.RemoveSiteDataAsync();
    }
    public new Task<DataProviderExportChunk> ExportChunkAsync(int chunk, SerializableList<SerializableFile> fileList) {
        // we don't export visitor data
        return Task.FromResult(new DataProviderExportChunk());
        //if (!Usable) return false;
        //return DataProvider.ExportChunk(chunk, fileList, out obj);
    }
    public new Task ImportChunkAsync(int chunk, SerializableList<SerializableFile> fileList, object obj) {
        // we don't import visitor data
        return Task.CompletedTask;
        //if (!Usable) return;
        //DataProvider.ImportChunk(chunk, fileList, obj);
    }
}
