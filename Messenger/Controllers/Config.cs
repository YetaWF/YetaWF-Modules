/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Messenger.DataProvider;
using YetaWF.Core.Serializers;
using YetaWF.Core.Identity;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class ConfigModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.ConfigModule> {

        public ConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("Message Acceptance"), Description("Defines whether messaging between users is immediately allowed - Message acceptance is only used for User role to User role messaging - All other roles never require message acceptance")]
            [UIHint("Enum")]
            public MessageAcceptanceEnum MessageAcceptance { get; set; }

            [Caption("Allow Same Roles"), Description("Defines whether messaging between roles of the same type is allowed for all users with that role")]
            [UIHint("Boolean")]
            public bool SameRoleToSameRole { get; set; }

            [Caption("Allowed Roles"), Description("Defines the roles for whom messaging between roles of the same type is allowed")]
            [UIHint("YetaWF_Identity_RolesSelector"), ProcessIf("SameRoleToSameRole", true)]
            public SerializableList<Role> AllowedSameRoles { get; set; }

            [Caption("Roles -> Users"), Description("Defines the roles that are allowed to send messages all users")]
            [UIHint("YetaWF_Identity_RolesSelector")]
            public SerializableList<Role> AllowedRolesToAllUsers { get; set; }

            [Caption("Users -> Roles"), Description("Defines the roles to which all users are allowed to send messages")]
            [UIHint("YetaWF_Identity_RolesSelector")]
            public SerializableList<Role> AllUsersToAllowedRoles { get; set; }

            public ConfigData GetData(ConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(ConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public ActionResult Config() {
            using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
                Model model = new Model { };
                ConfigData data = dataProvider.GetItem();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The messenger settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult Config_Partial(Model model) {
            using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
                ConfigData data = dataProvider.GetItem();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                dataProvider.UpdateConfig(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Messenger Settings saved"));
            }
        }
    }
}
