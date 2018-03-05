﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

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
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.PageEdit.Views.Shared {

    public class Users<TModel> : RazorTemplate<TModel> { }

    public static class UsersHelper {

        public class UsersModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        public class GridAllowedUser {

            protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

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
                UserName = Manager.Syncify<string>(() => Resource.ResourceAccess.GetUserNameAsync(allowedUser.UserId));//$$$ what to do
            }
            public GridAllowedUser(int userId) {
                UserId = DisplayUserId = userId;
                View = PageDefinition.AllowedEnum.Yes;
                UserName = Manager.Syncify<string>(() => Resource.ResourceAccess.GetUserNameAsync(userId));//$$$ what to do
            }
        }
#if MVC6
        public static HtmlString RenderAllowedUsers<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, SerializableList<PageDefinition.AllowedUser> model)
#else
        public static HtmlString RenderAllowedUsers<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<PageDefinition.AllowedUser> model)
#endif
        {
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

#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => UsersModel.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => UsersModel.GridDef);
#endif
        }
    }
}