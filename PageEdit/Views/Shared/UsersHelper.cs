/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using YetaWF.Core.Identity;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.PageEdit.Views.Shared {

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

            [Caption("User"), Description("User Description")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int DisplayUserId { get; set; }

            [Caption("View"), Description("The user has permission to view the page")]
            [UIHint("Enum")]
            public PageDefinition.AllowedEnum View { get; set; }

            [Caption("Edit"), Description("The user has permission to edit the page")]
            [UIHint("Enum")]
            public PageDefinition.AllowedEnum Edit { get; set; }

            [Caption("Remove"), Description("The user has permission to remove the page")]
            [UIHint("Enum")]
            public PageDefinition.AllowedEnum Remove { get; set; }

            [UIHint("RawInt"), ReadOnly]
            public int UserId  { get; set; }
            [UIHint("Raw"), ReadOnly]
            public string UserName { get; set; }

            public GridAllowedUser(PageDefinition.AllowedUser allowedUser) {
                ObjectSupport.CopyData(allowedUser, this);
                UserId = DisplayUserId = allowedUser.UserId;
                UserName = Resource.ResourceAccess.GetUserName(allowedUser.UserId);
            }
            public GridAllowedUser(int userId) {
                UserId = DisplayUserId = userId;
                View = PageDefinition.AllowedEnum.Yes;
                UserName = Resource.ResourceAccess.GetUserName(userId);
            }
        }

        public static MvcHtmlString RenderAllowedUsers<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<PageDefinition.AllowedUser> model) {

            List<GridAllowedUser> list;
            if (model != null)
                list = (from u in model select new GridAllowedUser(u)).ToList();
            else
                list = new List<GridAllowedUser>();

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            UsersModel UsersModel = new UsersModel() {
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
            return htmlHelper.DisplayFor(m => UsersModel.GridDef);
        }
    }
}