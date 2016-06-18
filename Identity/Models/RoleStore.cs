/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNet.Identity;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Models {

    /// <summary>
    /// There is one RoleStore per site, stored using PermanentManager
    /// </summary>
    public class RoleStore : IRoleStore<RoleDefinition>
    {
        public RoleStore(float f) { /* THIS float IS TO REMIND ME SO I DON'T ACCIDENTALLY ALLOCATE A NEW ONE */ 
            CurrentSiteIdentity = YetaWFManager.Manager.CurrentSite.Identity;
        }
        public int CurrentSiteIdentity { get; private set; }

        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) { }
        //~RoleStore() { Dispose(false); }

        // IRoleStore
        // IRoleStore
        // IRoleStore
        
        public System.Threading.Tasks.Task CreateAsync(RoleDefinition role) {
            return System.Threading.Tasks.Task.Run(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                dataProvider.AddItem(role);
            });
        }

        public System.Threading.Tasks.Task DeleteAsync(RoleDefinition role) {
            return System.Threading.Tasks.Task.Run(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                dataProvider.RemoveItem(role.Name);
            });
        }

        public System.Threading.Tasks.Task<RoleDefinition> FindByIdAsync(string roleId) {
            return System.Threading.Tasks.Task.Run(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                return dataProvider.GetRoleById(roleId);
            });
        }

        public System.Threading.Tasks.Task<RoleDefinition> FindByNameAsync(string roleName) {
            return System.Threading.Tasks.Task.Run(() => {
                RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider(this.CurrentSiteIdentity);
                RoleDefinition role = dataProvider.GetItem(roleName);
                return role;
            });
        }

        public System.Threading.Tasks.Task UpdateAsync(RoleDefinition role) {
            return System.Threading.Tasks.Task.Run(() => {
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
    }
}