/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using YetaWF.Core.Support;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Views.Shared {

    public class LoginUsersHelper<TModel> : RazorTemplate<TModel> { }

    public static class LoginUsersHelper {

        public static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(LoginUsersHelper), name, defaultValue, parms); }
#if MVC6
        public static HtmlString RenderLoginUsers(this IHtmlHelper htmlHelper, string name, int model, object HtmlAttributes = null)
#else
        public static HtmlString RenderLoginUsers(this HtmlHelper htmlHelper, string name, int model, object HtmlAttributes = null)
#endif
        {

            SerializableList<User> users = htmlHelper.GetParentModelSupportProperty<SerializableList<User>>(name, "List");
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
                    UserDefinition user = dataProvider.GetItemByUserId(u.UserId);
                    if (user != null) {
                        list.Add(new SelectionItem<int> {
                            Text = user.UserName,
                            Tooltip = __ResStr("selUser", "Select to log in as {0}", user.UserName),
                            Value = user.UserId,
                        });
                    }
                }
            }

            list = (from l in list orderby l.Text select l).ToList();

            // add the superuser if it hasn't already been added
            using (SuperuserDefinitionDataProvider superDataProvider = new SuperuserDefinitionDataProvider()) {
                UserDefinition user = superDataProvider.GetSuperuser();
                if (user != null) {
                    if ((from l in list where l.Value == user.UserId select l).FirstOrDefault() == null) {
                        list.Insert(0, new SelectionItem<int> {
                            Text = user.UserName,
                            Tooltip = __ResStr("selUser", "Select to log in as {0}", user.UserName),
                            Value = user.UserId,
                        });
                    }
                }
            }
            list.Insert(0, new SelectionItem<int> {
                Text = __ResStr("noUser", "(none)"),
                Tooltip = __ResStr("selLogoff", "Select to log off"),
                Value = 0,
            });
            return htmlHelper.RenderDropDownSelectionList<int>(name, model, list, HtmlAttributes: HtmlAttributes);
        }
    }
}