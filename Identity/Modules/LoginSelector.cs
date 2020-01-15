/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class LoginSelectorModuleDataProvider : ModuleDefinitionDataProvider<Guid, LoginSelectorModule>, IInstallableModel { }

    [ModuleGuid("{9cdb39c8-4f37-4d30-9eee-68f4bc7420a0}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class LoginSelectorModule : ModuleDefinition {

        public LoginSelectorModule() : base() {
            Name = this.__ResStr("modName", "User Login Selector");
            Title = this.__ResStr("modTitle", "User Login Selector");
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
            Users = new SerializableList<User>();
            Description = this.__ResStr("modSummary", "User login selector (used during development)");
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LoginSelectorModuleDataProvider(); }

        [Category("General"), Caption("User List"), Description("List of user accounts that can be used to quickly log into the site")]
        [UIHint("YetaWF_Identity_ResourceUsers")]
        [Data_Binary]
        public SerializableList<User> Users { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles {
            get {
#if DEBUG
                return AnonymousLevel_DefaultAllowedRoles;
#else
                return SuperuserLevel_DefaultAllowedRoles;
#endif
            }
        }

        // Add a user from a site template
        public void AddUser(string userName) {
            YetaWFManager.Syncify(async () => { // rarely used so sync is OK
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    UserDefinition user = await dataProvider.GetItemAsync(userName);
                    if (user != null) {
                        Users.Add(new User { UserId = user.UserId });
                        await SaveAsync();
                    }
                }
            });
        }
    }
}
