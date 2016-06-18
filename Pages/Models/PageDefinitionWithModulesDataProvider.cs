using System;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Pages.DataProvider;

namespace YetaWF.Modules.Pages.DataProvider {

    public class PageDefinitionForModules : PageDefinition {
        public Guid ModuleGuid { get; set; }
        public string Pane { get; set; }
    }

    public class PageDefinitionForModulesProvider : DataProviderImpl {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public PageDefinitionForModulesProvider() : base(0) { SetDataProvider(DataProvider); }

        private IDataProvider<string, PageDefinitionForModules> DataProvider
        {
            get
            {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        case WebConfigHelper.IOModeEnum.File:
                            throw new InternalError("File I/O is not supported");
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<string, PageDefinitionForModules>(AreaName + "_ModuleDefinitions", SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<string, PageDefinitionForModules> _dataProvider { get; set; }
    }
}
