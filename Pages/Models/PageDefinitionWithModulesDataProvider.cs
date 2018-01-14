/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Pages.DataProvider {

    public class PageDefinitionForModules : PageDefinition {
        public Guid ModuleGuid { get; set; }
        public string Pane { get; set; }
    }

    public class PageDefinitionForModulesProvider : DataProviderImpl {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public PageDefinitionForModulesProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, PageDefinitionForModules> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, PageDefinitionForModules> CreateDataProvider() {
            Package package = YetaWF.Modules.Pages.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package.AreaName,
                () => { // File
                    throw new InternalError("File I/O is not supported");
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<string, PageDefinitionForModules>(AreaName + "_ModuleDefinitions", dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { AreaName = AreaName + "_ModuleDefinitions", CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                }
            );
        }
    }
}
