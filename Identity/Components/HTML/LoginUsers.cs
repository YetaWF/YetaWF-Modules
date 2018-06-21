/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Components {

    public abstract class LoginUsersComponentBase : YetaWFComponent {

        public const string TemplateName = "LoginUsers";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class LoginUsersEditComponent : LoginUsersComponentBase, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(int model) {

            SerializableList<User> users = GetSiblingProperty<SerializableList<User>>($"{PropertyName}_List");
            if (users != null)
                users = new SerializableList<User>(from u in users select u);// copy list
            else
                users = new SerializableList<User>();
            // add the user id that's current (i.e. the model) if it hasn't already been added
            if ((from u in users where u.UserId == model select u).FirstOrDefault() == null)
                users.Add(new User { UserId = model });

            // get list of desired users (ignore users that are invalid, they may have been deleted)
            List<SelectionItem<int>> list = new List<SelectionItem<int>>();
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                foreach (var u in users) {
                    UserDefinition user = await dataProvider.GetItemByUserIdAsync(u.UserId);
                    if (user != null) {
                        list.Add(new SelectionItem<int> {
                            Text = user.UserName,
                            Tooltip = this.__ResStr("selUser", "Select to log in as {0}", user.UserName),
                            Value = user.UserId,
                        });
                    }
                }
            }

            list = (from l in list orderby l.Text select l).ToList();

            // add the superuser if it hasn't already been added
            using (SuperuserDefinitionDataProvider superDataProvider = new SuperuserDefinitionDataProvider()) {
                UserDefinition user = await superDataProvider.GetSuperuserAsync();
                if (user != null) {
                    if ((from l in list where l.Value == user.UserId select l).FirstOrDefault() == null) {
                        list.Insert(0, new SelectionItem<int> {
                            Text = user.UserName,
                            Tooltip = this.__ResStr("selUser", "Select to log in as {0}", user.UserName),
                            Value = user.UserId,
                        });
                    }
                }
            }
            list.Insert(0, new SelectionItem<int> {
                Text = this.__ResStr("noUser", "(none)"),
                Tooltip = this.__ResStr("selLogoff", "Select to log off"),
                Value = 0,
            });
            return await DropDownListIntComponent.RenderDropDownListAsync(this, model, list, "yt_yetawf_identity_loginusers");
        }
    }
}
