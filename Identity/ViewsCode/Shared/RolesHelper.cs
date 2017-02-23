/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Identity;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Mvc.Rendering;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.Identity.Views.Shared {

    public class Roles<TModel> : RazorTemplate<TModel> { }

    public static class RolesHelper {

        public class RolesModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        public class GridAllowedRole {

            [DontSave]
            public bool __editable { get; set; }

            [Caption("Apply"), Description("Select to apply the role to the user")]
            [UIHint("Boolean")]
            public bool InRole { get; set; }

            [Caption("Name"), Description("Role Name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("Role Description")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [Caption("Role Id"), Description("The id used internally to identify the role")]
            [UIHint("IntValue"), ReadOnly]
            public int DisplayRoleId { get; private set; }

            public int RoleId {
                get {
                    return roleId;
                }
                set {
                    roleId = DisplayRoleId = value;
                }
            }
            private int roleId;

            public GridAllowedRole() { __editable = true; }
        }
#if MVC6
        public static MvcHtmlString RenderResourceAllowedRolesDisplay<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model)
#else
        public static MvcHtmlString RenderResourceAllowedRolesDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model)
#endif
        {
            return htmlHelper.RenderResourceAllowedRoles<TModel>(name, model, ReadOnly: true);
        }
#if MVC6
        public static MvcHtmlString RenderResourceAllowedRoles<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model, bool ReadOnly = false)
#else
        public static MvcHtmlString RenderResourceAllowedRoles<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model, bool ReadOnly = false)
#endif
        {
            List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList();
            int superuserRole = Resource.ResourceAccess.GetSuperuserRoleId();
            List<GridAllowedRole> roles = (from r in allRoles orderby r.Name
                                     select new GridAllowedRole {
                                         RoleId = r.RoleId,
                                         Name = r.Name,
                                         Description = r.Description,
                                         InRole = superuserRole == r.RoleId || (model != null && model.Contains(new Role { RoleId = r.RoleId }, new RoleComparer())),
                                         __editable = (superuserRole != r.RoleId) // we disable the superuser entry
                                     }).ToList<GridAllowedRole>();

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = roles.ToList<object>(),
                Total = roles.Count,
            };
            RolesModel rolesModel = new RolesModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridAllowedRole),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = ReadOnly,
                }
            };
            return MvcHtmlString.Create(htmlHelper.DisplayFor(m => rolesModel.GridDef));
        }
#if MVC6
        public static MvcHtmlString RenderUserAllowedRolesDisplay<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model)
#else
        public static MvcHtmlString RenderUserAllowedRolesDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model)
#endif
        {
            return htmlHelper.RenderUserAllowedRoles<TModel>(name, model, ReadOnly: true);
        }
#if MVC6
        public static MvcHtmlString RenderUserAllowedRoles<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model, bool ReadOnly = false)
#else
        public static MvcHtmlString RenderUserAllowedRoles<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model, bool ReadOnly = false)
#endif
        {
            List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList();
            int userRole = Resource.ResourceAccess.GetUserRoleId();
            List<GridAllowedRole> roles = (from r in allRoles orderby r.Name
                                           select new GridAllowedRole {
                                               RoleId = r.RoleId,
                                               Name = r.Name,
                                               Description = r.Description,
                                               InRole = userRole == r.RoleId || (model != null && model.Contains(new Role { RoleId = r.RoleId }, new RoleComparer())),
                                               __editable = (userRole != r.RoleId) // we disable the user entry
                                           }).ToList<GridAllowedRole>();

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = roles.ToList<object>(),
                Total = roles.Count,
            };
            RolesModel rolesModel = new RolesModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridAllowedRole),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = ReadOnly,
                }
            };
            return MvcHtmlString.Create(htmlHelper.DisplayFor(m => rolesModel.GridDef));
        }
    }
}