﻿/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using System;
using System.Threading;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Identity;
#else
using Microsoft.AspNet.Identity;
#endif

namespace YetaWF.Modules.Identity.Models {

    /// <summary>
    /// There is one RoleStore per site, stored using PermanentManager
    /// </summary>
    public class RoleStore : IRoleStore<RoleDefinition>
    {
#if MVC6
        public RoleStore()
#else
        public RoleStore(float var) /* THIS float IS TO REMIND ME SO I DON'T ACCIDENTALLY ALLOCATE A NEW ONE */
#endif
        {
            CurrentSiteIdentity = YetaWFManager.Manager.CurrentSite.Identity;
        }
        public int CurrentSiteIdentity { get; private set; }

        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) { }
        //~RoleStore() { Dispose(false); }

        // IRoleStore
        // IRoleStore
        // IRoleStore

#if MVC6
        public Task<IdentityResult> CreateAsync(RoleDefinition role, CancellationToken cancellationToken) {
            return Task.Run<IdentityResult>(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                dataProvider.AddItem(role);
                return IdentityResult.Success;
            });
        }
#else
        public Task CreateAsync(RoleDefinition role) {
            return Task.Run(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                dataProvider.AddItem(role);
            });
        }
#endif
#if MVC6
        public Task<IdentityResult> DeleteAsync(RoleDefinition role) {
            return Task.Run<IdentityResult>(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                dataProvider.RemoveItem(role.Name);
                return IdentityResult.Success;
            });
        }
#else
        public Task DeleteAsync(RoleDefinition role) {
            return Task.Run(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                dataProvider.RemoveItem(role.Name);
            });
        }
#endif
#if MVC6
        public Task<IdentityResult> UpdateAsync(RoleDefinition role, CancellationToken cancellationToken) {
            return Task.Run<IdentityResult>(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                UpdateStatusEnum status = dataProvider.UpdateItem(role);
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
            });
        }
#else
        public Task UpdateAsync(RoleDefinition role) {
            return Task.Run(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                UpdateStatusEnum status = dataProvider.UpdateItem(role);
                switch (status) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("alreadyDeleted", "The role named \"{0}\" has been removed and can no longer be updated.", role.Name));

                    case UpdateStatusEnum.NewKeyExists:
                        throw new Error(this.__ResStr("alreadyExists", "A role named \"{0}\" already exists.", role.Name));
                    case UpdateStatusEnum.OK:
                        break;
                }
            });
        }
#endif
#if MVC6
        public Task<RoleDefinition> FindByIdAsync(string roleId, CancellationToken cancellationToken)
#else
        public Task<RoleDefinition> FindByIdAsync(string roleId)
#endif
        {
            return Task.Run(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                return dataProvider.GetRoleById(roleId);
            });
        }

#if MVC6
        public Task<RoleDefinition> FindByNameAsync(string roleName, CancellationToken cancellationToken)
#else
        public Task<RoleDefinition> FindByNameAsync(string roleName)
#endif
        {
            return Task.Run(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                RoleDefinition role = dataProvider.GetItem(roleName);
                return role;
            });
        }
#if MVC6
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
#else
#endif
    }
}