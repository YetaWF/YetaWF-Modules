/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Messenger.DataProvider {

    public class Connection {

        public const int MaxConnectionId = 100;

        [Data_PrimaryKey]
        [StringLength(MaxConnectionId)]
        public string ConnectionId { get; set; }

        [Data_Index, StringLength(Globals.MaxUser)]
        public string Name { get; set; }
        [Data_Index, StringLength(Globals.MaxIP)]
        public string IpAddress { get; set; }
        public DateTime LastSeen { get; set; }

        public Connection() {
            LastSeen = DateTime.UtcNow;
        }
    }

    public class ConnectionDataProvider : DataProviderImpl, IInstallableModel, IInitializeApplicationStartup {

        // Startup

        public async Task InitializeApplicationStartupAsync(bool firstNode) {
            if (firstNode) {
                // clear all connections from db
                // removes all sites
                await RemoveItemsAsync(null);
            }
        }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ConnectionDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public ConnectionDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, Connection> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, Connection> CreateDataProvider() {
            Package package = YetaWF.Modules.Messenger.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Connections", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        private static AsyncLock _lockObject = new AsyncLock();

        public async Task<Connection> GetItemAsync(string key) {
            return await DataProvider.GetAsync(key);
        }
        public async Task<bool> AddItemAsync(Connection data) {
            return await DataProvider.AddAsync(data);
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(Connection data) {
            return await DataProvider.UpdateAsync(data.ConnectionId, data.ConnectionId, data);
        }
        public async Task<bool> RemoveItemAsync(string key) {
            return await DataProvider.RemoveAsync(key);
        }
        public async Task<DataProviderGetRecords<Connection>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return await DataProvider.RemoveRecordsAsync(filters);
        }
        public async Task<Connection> GetEntryAsync(string name) {
            List<DataProviderFilterInfo> filters = null;
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "Name", Operator = "==", Value = name });
            DataProviderGetRecords<Connection> conns = await GetItemsAsync(0, 1, null, filters);
            return conns.Data.FirstOrDefault();
        }

        public async Task UpdateEntryAsync(string name, string ipAddress, string connectionId) {
            using (await _lockObject.LockAsync()) {
                try {
                    Connection conn = await GetItemAsync(connectionId);
                    if (conn == null) {
                        conn = new Connection {
                            ConnectionId = connectionId,
                            IpAddress = ipAddress,
                            Name = name,
                        };
                        await AddItemAsync(conn);
                    } else {
                        conn.IpAddress = ipAddress;
                        conn.Name = name;
                        conn.LastSeen = DateTime.UtcNow;
                        await UpdateItemAsync(conn);
                    }
                } catch (Exception) { }
            }
        }
    }
}
