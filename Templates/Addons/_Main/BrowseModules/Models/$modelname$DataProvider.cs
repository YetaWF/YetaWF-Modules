using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace $companynamespace$.Modules.$projectnamespace$.DataProvider;

public class $modelname$ {

    public const int Max$modelkeyname$ = 50;
    public const int MaxField1 = 100;

    [Data_PrimaryKey, StringLength(Max$modelkeyname$)]
    public $modelkey$ $modelkeyname$ { get; set; } = null!;

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }

    [StringLength(MaxField1)]
    public string? Field1 { get; set; }

    public $modelname$() { }
}

public class $modelname$DataProvider : DataProviderImpl, IInstallableModel {

    public $modelname$DataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
    public $modelname$DataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

    private IDataProvider<$modelkey$, $modelname$> DataProvider { get { return GetDataProvider(); } }

    private IDataProvider<$modelkey$, $modelname$>? CreateDataProvider() {
        Package package = $companynamespace$.Modules.$projectnamespace$.AreaRegistration.CurrentPackage;
        return MakeDataProvider(package, package.AreaName + "_$modelname$", SiteIdentity: SiteIdentity, Cacheable: true);
    }

    // API
    // API
    // API

    public Task<$modelname$?> GetItemAsync($modelkey$ $modelkeynamelower$) {
        return DataProvider.GetAsync($modelkeynamelower$);
    }
    public Task<bool> AddItemAsync($modelname$ $modelnamelower$) {
        $modelnamelower$.Created = DateTime.UtcNow;
        return DataProvider.AddAsync($modelnamelower$);
    }
    public Task<UpdateStatusEnum> UpdateItemAsync($modelname$ $modelnamelower$) {
        return UpdateItemAsync($modelnamelower$.$modelkeyname$, $modelnamelower$);
    }
    public Task<UpdateStatusEnum> UpdateItemAsync($modelkey$ original$modelkeyname$, $modelname$ $modelnamelower$) {
        $modelnamelower$.Updated = DateTime.UtcNow;
        return DataProvider.UpdateAsync(original$modelkeyname$, $modelnamelower$.$modelkeyname$, $modelnamelower$);
    }
    public Task<bool> RemoveItemAsync($modelkey$ $modelkeynamelower$) {
        return DataProvider.RemoveAsync($modelkeynamelower$);
    }
    public Task<DataProviderGetRecords<$modelname$>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) {
        return DataProvider.GetRecordsAsync(skip, take, sort, filters);
    }
    public Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
        return DataProvider.RemoveRecordsAsync(filters);
    }
}
