/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Dashboard.DataProvider {

    public class DataProviderInfo {

        public string IOModeName { get; set; }
        public string TypeName { get; set; }
        public string TypeImplName { get; set; }

        public DataProviderInfo() { }
    }

    public class DataProviderInfoDataProvider : DataProviderImpl {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public DataProviderInfoDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { }

        // API
        // API
        // API

        public DataProviderGetRecords<DataProviderInfo> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            List<DataProviderInfo> list = (from r in DataProviderImpl.RegisteredExternalDataProviders select new DataProviderInfo {
                IOModeName = r.IOModeName,
                TypeName = r.Type.FullName,
                TypeImplName = r.TypeImpl.FullName,
            }).ToList();
            return DataProviderImpl<DataProviderInfo>.GetRecords(list, skip, take, sort, filters);
        }
    }
}
