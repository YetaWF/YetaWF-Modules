using System;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace $companynamespace$.Modules.$projectnamespace$.DataProvider;

public class $modelname$ {

    public const int MaxProp1 = 100;
    public const int MaxProp2 = 100;

    [Data_PrimaryKey]
    public int Id { get; set; }

    [StringLength(MaxProp1)]
    public string? Prop1 { get; set; }
    [StringLength(MaxProp2)]
    public string? Prop2 { get; set; }

    public $modelname$() {
        Prop1 = "";
        Prop2 = "";
    }
}

public class $modelname$DataProvider : DataProviderImpl, IInstallableModel {

    private const int KEY = 1;

    public $modelname$DataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
    public $modelname$DataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

    private IDataProvider<int, $modelname$> DataProvider { get { return GetDataProvider(); } }

    private IDataProvider<int, $modelname$>? CreateDataProvider() {
        Package package = $companynamespace$.Modules.$projectnamespace$.AreaRegistration.CurrentPackage;
        return MakeDataProvider(package, package.AreaName + "_$modelname$", SiteIdentity: SiteIdentity, Cacheable: true);
    }

    // API
    // API
    // API

    public static async Task<$modelname$> GetConfigAsync() {
        using ($modelname$DataProvider configDP = new $modelname$DataProvider()) {
            return await configDP.GetItemAsync();
        }
    }
    public async Task<$modelname$> GetItemAsync() {
        $modelname$? config = await DataProvider.GetAsync(KEY);
        if (config == null) {
            config = new $modelname$();
            await AddConfigAsync(config);
        }
        return config;
    }
    private async Task AddConfigAsync($modelname$ data) {
        data.Id = KEY;
        if (!await DataProvider.AddAsync(data))
            throw new InternalError("Unexpected error adding settings");
        await Auditing.AddAuditAsync($"{nameof($modelname$DataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
            "Add $companynamespace$_$projectnamespace$ Config",
            DataBefore: null,
            DataAfter: data,
            ExpensiveMultiInstance: true
        );
    }
    public async Task UpdateConfigAsync($modelname$ data) {
        $modelname$? origData = Auditing.Active ? await GetItemAsync() : null;
        data.Id = KEY;
        UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
        if (status != UpdateStatusEnum.OK)
            throw new InternalError("Unexpected error saving configuration {0}", status);

        await Auditing.AddAuditAsync($"{nameof($modelname$DataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
            "Update $companynamespace$_$projectnamespace$ Config",
            DataBefore: origData,
            DataAfter: data,
            ExpensiveMultiInstance: true
        );
    }
}
