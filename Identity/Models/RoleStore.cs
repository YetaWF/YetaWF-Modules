/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace YetaWF.Modules.Identity.Models {

    /// <summary>
    /// There is one RoleStore per site, stored using PermanentManager
    /// </summary>
    public class RoleStore : IRoleStore<RoleDefinition> {
        public RoleStore() { }
        public int CurrentSiteIdentity { get { return YetaWFManager.Manager.CurrentSite.Identity; } }

        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) { }
        //~RoleStore() { Dispose(false); }

        // IRoleStore
        // IRoleStore
        // IRoleStore

        public async Task<IdentityResult> CreateAsync(RoleDefinition role, CancellationToken cancellationToken) {
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity)) {
                await dataProvider.AddItemAsync(role);
            }
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> DeleteAsync(RoleDefinition role) {
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity)) {
                await dataProvider.RemoveItemAsync(role.Name);
            }
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> UpdateAsync(RoleDefinition role, CancellationToken cancellationToken) {
            RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
            UpdateStatusEnum status = await dataProvider.UpdateItemAsync(role);
            switch (status) {
                default:
                case UpdateStatusEnum.RecordDeleted:
                    throw new Error(this.__ResStr("alreadyDeleted", "The role named \"{0}\" has been removed and can no longer be updated.", role.Name));
                case UpdateStatusEnum.NewKeyExists:
                    throw new Error(this.__ResStr("alreadyExists", "A role named \"{0}\" already exists.", role.Name));
                case UpdateStatusEnum.OK:
                    break;
            }
            return IdentityResult.Success;
        }
        public Task<RoleDefinition> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity)) {
                return Task.FromResult(dataProvider.GetRoleById(roleId));
            }
        }
        public async Task<RoleDefinition> FindByNameAsync(string roleName, CancellationToken cancellationToken) {
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity)) {
                return await dataProvider.GetItemAsync(roleName);
            }
        }
        Task<IdentityResult> IRoleStore<RoleDefinition>.DeleteAsync(RoleDefinition role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        Task<string> IRoleStore<RoleDefinition>.GetRoleIdAsync(RoleDefinition role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        Task<string> IRoleStore<RoleDefinition>.GetRoleNameAsync(RoleDefinition role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        Task IRoleStore<RoleDefinition>.SetRoleNameAsync(RoleDefinition role, string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        Task<string> IRoleStore<RoleDefinition>.GetNormalizedRoleNameAsync(RoleDefinition role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        Task IRoleStore<RoleDefinition>.SetNormalizedRoleNameAsync(RoleDefinition role, string normalizedName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
