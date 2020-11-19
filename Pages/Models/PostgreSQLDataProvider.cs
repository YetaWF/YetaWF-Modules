/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Pages.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.PageDefinitionDataProvider), typeof(PageDefinitionDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(PageDefinitionDataProvider.PageDefinitionForModulesProvider), typeof(PageDefinitionDataProvider.PageDefinitionForModulesDataProviderSQL));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.UnifiedSetDataProvider), typeof(UnifiedSetDataProvider));
        }
        class PageDefinitionDataProvider : SQLSimpleObject<Guid, PageDefinition>, IPageDefinitionIOMode {

            public PageDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }

            public async Task<DesignedPagesDictionaryByUrl> GetDesignedPagesAsync() {
                // get the names of all designed pages
                using (SQLSimpleObject<Guid, DesignedPage> dp = new SQLSimpleObject<Guid, DesignedPage>(Options)) {
                    DataProviderGetRecords<DesignedPage> pages = await dp.GetRecordsAsync(0, 0, null, null);
                    DesignedPagesDictionaryByUrl byUrl = new DesignedPagesDictionaryByUrl();
                    foreach (DesignedPage page in pages.Data) {
                        byUrl.Add(page.Url.ToLower(), new PageDefinition.DesignedPage {
                            PageGuid = page.PageGuid,
                            Url = page.Url,
                        });
                    }
                    return byUrl;
                }
            }

            public class DesignedPage {
                public string Url { get; set; } // absolute Url (w/o http: or domain) e.g., /Home or /Test/Page123
                [Data_PrimaryKey]
                public Guid PageGuid { get; set; }
                [Data_Identity]
                public int Identity { get; set; }
            }

            public async Task<List<PageDefinition>> GetPagesFromModuleAsync(Guid moduleGuid) {
                List<DataProviderSortInfo> sorts = DataProviderSortInfo.Join(null, new DataProviderSortInfo { Field = nameof(PageDefinition.Url), Order = DataProviderSortInfo.SortDirection.Ascending });
                List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = nameof(PageDefinitionForModules.ModuleGuid), Operator = "==", Value = moduleGuid });
                using (DataProvider.PageDefinitionDataProvider pageDefDP = new DataProvider.PageDefinitionDataProvider()) {
                    using (PageDefinitionForModulesProvider pageDefForModDP = new PageDefinitionForModulesProvider()) {
                        List<JoinData> joins = new List<JoinData> {
                            new JoinData {MainDP = pageDefDP, JoinDP = pageDefForModDP, MainColumn = nameof(PageDefinition.Identity), JoinColumn = SQLBase.SubTableKeyColumn, UseSite = false },
                        };
                        DataProviderGetRecords<PageDefinition> recs = await GetRecordsAsync(0, 0, sorts, filters, Joins: joins);
                        return recs.Data;
                    }
                }
            }

            public class PageDefinitionForModules {
                public Guid ModuleGuid { get; set; }
                public string Pane { get; set; }
                [Data_Identity]
                public int Identity { get; set; }
            }

            public class PageDefinitionForModulesProvider : DataProviderImpl {

                public PageDefinitionForModulesProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

                private IDataProvider<string, PageDefinitionForModules> DataProvider { get { return GetDataProvider(); } }

                private IDataProvider<string, PageDefinitionForModules> CreateDataProvider() {
                    Package package = YetaWF.Modules.Pages.AreaRegistration.CurrentPackage;
                    return MakeDataProvider(package, package.AreaName + "_ModuleDefinitions", SiteIdentity: SiteIdentity, Cacheable: true, LimitIOMode: SQLBase.ExternalName);
                }
            }
            public class PageDefinitionForModulesDataProviderSQL : SQLSimpleObject<string, PageDefinitionForModules> {
                public PageDefinitionForModulesDataProviderSQL(Dictionary<string, object> options) : base(options) { }
            }
        }
        class UnifiedSetDataProvider : SQLSimpleObject<Guid, UnifiedSetData> {
            public UnifiedSetDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
