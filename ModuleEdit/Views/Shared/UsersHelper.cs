/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ModuleEdit.Views.Shared {

    public class Users<TModel> : RazorTemplate<TModel> { }

    public static class UsersHelper {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public class UsersModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        public static MvcHtmlString RenderAllowedUsers<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<ModuleDefinition.AllowedUser> model) {
            Type gridEntryType;
            if (!htmlHelper.TryGetControlInfo<Type>("", "GridEntry", out gridEntryType))
                gridEntryType = typeof(ModuleDefinition.GridAllowedUser);

            List<ModuleDefinition.GridAllowedUser> list = new List<ModuleDefinition.GridAllowedUser>();
            if (model != null) {
                // we have to create a more derived type here to get all the "extra" fields
                foreach (ModuleDefinition.AllowedUser u in model) {
                    ModuleDefinition.GridAllowedUser user = (ModuleDefinition.GridAllowedUser) Activator.CreateInstance(gridEntryType);
                    ObjectSupport.CopyData(u, user);
                    user.SetUser(u.UserId);
                    list.Add(user);
                }
            }
            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            UsersModel UsersModel = new UsersModel() {
                GridDef = new GridDefinition() {
                    RecordType = gridEntryType,
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 5,
                    ShowHeader = header,
                    ReadOnly = false,
                    CanAddOrDelete = true,
                    DeleteProperty = "UserId",
                    DisplayProperty = "DisplayUserName",
                    ResourceRedirect = Manager.CurrentModuleEdited,
                }
            };
            return htmlHelper.DisplayFor(m => UsersModel.GridDef);
        }
    }
}