/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Messenger.DataProvider {

    public class ActiveUser {

        public const int MaxConnectionId = 100;

        [Data_PrimaryKey, Data_Identity]
        public int Key { get; set; }

        public DateTime Created { get; set; }

        [StringLength(MaxConnectionId)]
        public string ConnectionId { get; set; } = null!;

        public int UserId { get; set; }

        [StringLength(Globals.MaxIP)]
        [Data_NewValue]
        public string IPAddress { get; set; } = null!;

        public ActiveUser() {
            Created = DateTime.UtcNow;
        }
    }

    public class ActiveUsersDataProvider : DataProviderImpl, IInstallableModel, IInitializeApplicationStartup, IInitializeApplicationStartupFirstNodeOnly {

        public async Task InitializeFirstNodeStartupAsync() {
            // remove all connection info
            try { // ignore errors if database table not yet defined
                await RemoveItemsAsync(null);
            } catch (Exception) { }
        }
        public Task InitializeApplicationStartupAsync() { return Task.CompletedTask; }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ActiveUsersDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<int, object, ActiveUser> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<int, object, ActiveUser>? CreateDataProvider() {
            Package package = YetaWF.Modules.Messenger.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_ActiveUsers");
        }

        // API
        // API
        // API

        public bool Usable { get { return DataProvider != null; } }

        public async Task<ActiveUser?> GetItemAsync(int key) {
            if (!Usable) return null;
            return await DataProvider.GetByIdentityAsync(key);
        }
        public async Task AddConnectionAsync(string connectionId, int userId) {
            if (!Usable) return;
            try { // ignore errors if database table not yet defined
                await AddItemAsync(new ActiveUser {
                    ConnectionId = connectionId,
                    UserId = userId,
                    IPAddress = YetaWFManager.HaveManager ? YetaWFManager.Manager.UserHostAddress.Truncate(Globals.MaxIP) : string.Empty,
                });
            } catch (Exception) { }
        }
        public async Task<bool> AddItemAsync(ActiveUser data) {
            if (!Usable) return false;
            return await DataProvider.AddAsync(data);
        }

        public async Task<bool> RemoveItemAsync(int key) {
            if (!Usable) return false;
            return await DataProvider.RemoveByIdentityAsync(key);
        }
        public async Task<DataProviderGetRecords<ActiveUser>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) {
            if (!Usable) throw new Error(this.__ResStr("notEnabled", "Active users are not tracked - not enabled"));
            return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }

        public async Task RemoveConnectionAsync(string connectionId) {
            if (!Usable) return;
            try { // ignore errors if database table not yet defined
                List<DataProviderFilterInfo>? filters = null;
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(ActiveUser.ConnectionId), Operator = "==", Value = connectionId });
                await RemoveItemsAsync(filters);
            } catch (Exception) { }
        }
        public Task<int> RemoveItemsAsync(List<DataProviderFilterInfo>? filters) {
            if (!Usable) return Task.FromResult(0);
            return DataProvider.RemoveRecordsAsync(filters);
        }
    }
}
