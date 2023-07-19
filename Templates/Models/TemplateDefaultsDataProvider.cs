using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.Templates.DataProvider;

public class TemplateDefaults {

    public const int MaxCompany = 50;
    public const int MaxCompanyUrl = 80;
    public const int MaxDomain = 80;
    public const int MaxProject = 30;
    public const int MaxModuleName = 30;
    public const int MaxModelName = 30;
    public const int MaxObjectName = 30;
    public const int MaxBrowseKeyType = 30;
    public const int MaxBrowseKeyName = 30;

    public enum DataProviderType {
        [EnumDescription("None", "No data provider")]
        None = 0,
        [EnumDescription("SQL & File", "SQL and File data provider")]
        SQLFile = 1,
        [EnumDescription("SQL with Identity", "SQL data provider with Identity column (no file support)")]
        SQLIdentity = 2,
    }

    [Data_PrimaryKey]
    public int UserId { get; set; }

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }

    [StringLength(MaxCompany)]
    public string Company { get; set; } = string.Empty;
    [StringLength(MaxCompanyUrl)]
    public string CompanyUrl { get; set; } = string.Empty;
    [StringLength(MaxDomain)]
    public string Domain { get; set; } = string.Empty;
    [StringLength(MaxProject)]
    public string Project { get; set; } = string.Empty;

    [Data_NewValue]
    [StringLength(MaxModuleName)]
    public string ConfigModuleName { get; set; } = string.Empty;
    [Data_NewValue]
    [StringLength(MaxModelName)]
    public string ConfigModelName { get; set; } = "ConfigData";
    [Data_NewValue]
    [StringLength(MaxObjectName)]
    public string ConfigObjectName { get; set; } = "Configuration Settings";

    [Data_NewValue]
    [StringLength(MaxModuleName)]
    public string TemplateModuleName { get; set; } = string.Empty;

    [Data_NewValue]
    [StringLength(MaxModuleName)]
    public string BrowseModuleName { get; set; } = string.Empty;
    [Data_NewValue]
    public DataProviderType BrowseDataProvider { get; set; } = DataProviderType.SQLFile;
    [Data_NewValue]
    [StringLength(MaxModelName)]
    public string BrowseModelName { get; set; } = string.Empty;
    [Data_NewValue]
    [StringLength(MaxObjectName)]
    public string BrowseObjectName { get; set; } = string.Empty;
    [Data_NewValue]
    [StringLength(MaxBrowseKeyType)]
    public string BrowseKeyType { get; set; } = string.Empty;
    [Data_NewValue]
    [StringLength(MaxBrowseKeyName)]
    public string BrowseKeyName { get; set; } = string.Empty;

    [Data_NewValue]
    [StringLength(MaxModuleName)]
    public string? SkinModuleName { get; set; }

    public TemplateDefaults() { }
}

public class TemplateDefaultsDataProvider : DataProviderImpl, IInstallableModel {

    public TemplateDefaultsDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

    private IDataProvider<int, TemplateDefaults> DataProvider { get { return GetDataProvider(); } }

    private IDataProvider<int, TemplateDefaults>? CreateDataProvider() {
        Package package = YetaWF.Modules.Templates.AreaRegistration.CurrentPackage;
        return MakeDataProvider(package, package.AreaName + "_TemplateDefaults", Cacheable: true);
    }

    // API
    // API
    // API

    public Task<TemplateDefaults?> GetItemAsync(int userId) {
        return DataProvider.GetAsync(userId);
    }
    public async Task<bool> AddItemAsync(TemplateDefaults template) {
        template.UserId = Manager.UserId;
        if (await UpdateItemAsync(template) == UpdateStatusEnum.OK)
            return true;
        template.Created = DateTime.UtcNow;
        if (await DataProvider.AddAsync(template))
            return true;
        return false;
    }
    public Task<UpdateStatusEnum> UpdateItemAsync(TemplateDefaults template) {
        return UpdateItemAsync(template.UserId, template);
    }
    private Task<UpdateStatusEnum> UpdateItemAsync(int originalUserId, TemplateDefaults template) {
        template.Updated = DateTime.UtcNow;
        return DataProvider.UpdateAsync(originalUserId, template.UserId, template);
    }
}
