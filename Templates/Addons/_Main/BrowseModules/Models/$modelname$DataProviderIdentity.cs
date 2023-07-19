$pp $dp$ == SQLIdentity
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace $companynamespace$.Modules.$projectnamespace$.DataProvider;

public class $modelname$ {

    public const int MaxField1 = 100;

    [Data_Identity]
    public int Id { get; set; }

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }

    [Data_PrimaryKey]
    public $modelkey$ $modelkeyname$ { get; set; }

    [StringLength(MaxField1)]
    public string? Field1 { get; set; }

    public $modelname$() { }
}

public class $modelname$DataProvider : DataProviderImpl, IInstallableModel {

    public $modelname$DataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
    public $modelname$DataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

    private IDataProviderIdentity<$modelkey$, object, $modelname$> DataProvider { get { return GetDataProvider(); } }

    private IDataProviderIdentity<$modelkey$, object, $modelname$>? CreateDataProvider() {
        Package package = $companynamespace$.Modules.$projectnamespace$.AreaRegistration.CurrentPackage;
        return MakeDataProvider(package, package.AreaName + "_$modelname$", SiteIdentity: SiteIdentity, Cacheable: true);
    }

    // API
    // API
    // API

    public Task<$modelname$?> GetItemAsync($modelkey$ $modelkeynamelower$) {
        return DataProvider.GetAsync($modelkeynamelower$, null);
    }
    public Task<$modelname$?> GetItemByIdentityAsync(int id) {
        return DataProvider.GetByIdentityAsync(id);
    }
    public async Task<bool> AddItemAsync($modelname$ data) {
        data.Created = DateTime.UtcNow;
        if (!await DataProvider.AddAsync(data))
            return false;
        await Auditing.AddAuditAsync($"{nameof($modelname$DataProvider)}.{nameof(AddItemAsync)}", Dataset, Guid.Empty,
            $"Add $objectname$ {data.Id}",
            DataBefore: null,
            DataAfter: data
        );
        return true;
    }
    public async Task<UpdateStatusEnum> UpdateItemAsync($modelname$ data) {

        $modelname$? origData = Auditing.Active ? await GetItemByIdentityAsync(data.Id) : null;

        data.Updated = DateTime.UtcNow;
        UpdateStatusEnum status = await DataProvider.UpdateByIdentityAsync(data.Id, data);
        if (status != UpdateStatusEnum.OK)
            return status;

        await Auditing.AddAuditAsync($"{nameof($modelname$DataProvider)}.{nameof(UpdateItemAsync)}", Dataset, Guid.Empty,
            $"Update $objectname$ {data.Id}",
            DataBefore: origData,
            DataAfter: data
        );
        return UpdateStatusEnum.OK;
    }
    public async Task<bool> RemoveItemByIdentityAsync(int id) {

        $modelname$? origData = Auditing.Active ? await GetItemByIdentityAsync(id) : null;

        if (!await DataProvider.RemoveByIdentityAsync(id))
            return false;

        await Auditing.AddAuditAsync($"{nameof($modelname$DataProvider)}.{nameof(RemoveItemByIdentityAsync)}", Dataset, Guid.Empty,
            $"Remove $objectname$ {id}",
            DataBefore: origData,
            DataAfter: null
        );
        return true;
    }
    public Task<DataProviderGetRecords<$modelname$>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) {
        return DataProvider.GetRecordsAsync(skip, take, sort, filters);
    }
}
