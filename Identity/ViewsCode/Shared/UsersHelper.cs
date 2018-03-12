/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Identity;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using YetaWF.Core.Support;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.Identity.Views.Shared {

    public class Users<TModel> : RazorTemplate<TModel> { }

    public static class UsersHelper {

        public class UsersModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        public class GridAllowedUser {

            [Caption("Delete"), Description("Click to delete a user")]
            [UIHint("GridDeleteEntry")]
            public int DeleteMe { get; set; }

            [Caption("User"), Description("User Name")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserNameFromId { get; set; }

            [UIHint("RawInt"), ReadOnly]
            public int UserId { get; set; }
            [UIHint("Raw"), ReadOnly]
            public string UserName { get; set; }

            public GridAllowedUser(int userId, string userName) {
                UserId = UserNameFromId = userId;
                UserName = userName;
            }
        }
#if MVC6
        public static async Task<HtmlString> RenderResourceAllowedUsersAsync<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, SerializableList<User> model)
#else
        public static async Task<HtmlString> RenderResourceAllowedUsersAsync<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<User> model)
#endif
        {
            List<GridAllowedUser> users = new List<GridAllowedUser>();
            if (model != null) {
                foreach (User u in model) {
                    string userName = await Resource.ResourceAccess.GetUserNameAsync(u.UserId);
                    users.Add(new GridAllowedUser(u.UserId, userName));
                }
            }

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = users.ToList<object>(),
                Total = users.Count,
            };
            UsersModel usersModel = new UsersModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridAllowedUser),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 5,
                    ShowHeader = header,
                    ReadOnly = false,
                    CanAddOrDelete = true,
                    DeleteProperty = "UserId",
                    DisplayProperty = "UserName"
                }
            };
#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => usersModel.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => usersModel.GridDef);
#endif
        }

        public class GridAllowedUserDisplay {

            [Caption("User"), Description("User Name")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserId { get; set; }

            public GridAllowedUserDisplay(int userId) {
                UserId = userId;
            }
        }
#if MVC6
        public static HtmlString RenderResourceAllowedUsersDisplay<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, SerializableList<User> model)
#else
        public static HtmlString RenderResourceAllowedUsersDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<User> model)
#endif
        {
            List<GridAllowedUserDisplay> users;
            if (model == null)
                users = new List<GridAllowedUserDisplay>();
            else
                users = (from u in model select new GridAllowedUserDisplay(u.UserId)).ToList();

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = users.ToList<object>(),
                Total = users.Count,
            };
            UsersModel usersModel = new UsersModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridAllowedUserDisplay),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 5,
                    ShowHeader = header,
                    ReadOnly = true,
                }
            };
#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => usersModel.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => usersModel.GridDef);
#endif
        }
    }
}