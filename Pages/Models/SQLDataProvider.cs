/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.DataProvider.SQL2;

namespace YetaWF.Modules.Pages.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.PageDefinitionDataProvider), typeof(PageDefinitionDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(PageDefinitionDataProvider.PageDefinitionForModulesProvider), typeof(PageDefinitionDataProvider.PageDefinitionForModulesDataProviderSQL));
            DataProviderImpl.RegisterExternalDataProvider(SQL2Base.ExternalName, typeof(DataProvider.UnifiedSetDataProvider), typeof(UnifiedSetDataProvider));
        }
        class PageDefinitionDataProvider : SQLSimpleObject<Guid, PageDefinition>, IPageDefinitionIOMode {

            public PageDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }

            public DesignedPagesDictionaryByUrl GetDesignedPages() {
                // get the names of all designed pages
                using (SQLSimpleObject<Guid, DesignedPage> dp = new SQLSimpleObject<Guid, DesignedPage>(Options)) {
                    int total;
                    List<DesignedPage> pages = dp.GetRecords(0, 0, null, null, out total);
                    DesignedPagesDictionaryByUrl byUrl = new DesignedPagesDictionaryByUrl();
                    foreach (DesignedPage page in pages) {
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

            public List<PageDefinition> GetPagesFromModule(Guid moduleGuid) {
                int total;
                List<DataProviderSortInfo> sorts = DataProviderSortInfo.Join(null, new DataProviderSortInfo { Field = "Url", Order = DataProviderSortInfo.SortDirection.Ascending });
                List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "ModuleGuid", Operator = "==", Value = moduleGuid });
                using (DataProvider.PageDefinitionDataProvider pageDefDP = new DataProvider.PageDefinitionDataProvider()) {
                    using (PageDefinitionForModulesProvider pageDefForModDP = new PageDefinitionForModulesProvider()) {
                        List<JoinData> joins = new List<JoinData> {
                            new JoinData {MainDP = pageDefDP, JoinDP = pageDefForModDP, MainColumn = nameof(PageDefinition.Identity), JoinColumn = SQL2Base.SubTableKeyColumn, UseSite = false },
                        };
                        return GetRecords(0, 0, sorts, filters, out total, Joins: joins);
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
                    Package package = YetaWF.Modules.Pages.Controllers.AreaRegistration.CurrentPackage;
                    return MakeDataProvider2(package, package.AreaName + "_ModuleDefinitions", SiteIdentity: SiteIdentity, Cacheable: true);
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
