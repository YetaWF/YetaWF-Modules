/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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
    [ModuleCategory("Tools")]
    public class LoginSelectorModule : ModuleDefinition {

        public LoginSelectorModule() : base() {
            Name = this.__ResStr("modName", "User Login Selector");
            Title = this.__ResStr("modTitle", "User Login Selector");
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
            Users = new SerializableList<User>();
            Description = this.__ResStr("modSummary", "Used during site development to quickly switch between predefined user accounts. It is normally added to a skin as a skin module. Use the module's Module Settings to add predefined user accounts to the list of accounts offered by this module. In debug builds this module is always shown for easy switching between users during development. In Release builds, this module is only shown to a logged on Superuser.");
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
