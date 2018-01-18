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
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.Identity.Views.Shared {

    public class RolesSelector<TModel> : RazorTemplate<TModel> { }

    public static class RolesSelectorHelper {

        public class GridModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        public class GridEdit {

            [Caption("Use"), Description("Click checkbox to select")]
            [UIHint("Boolean")]
            public bool ForceTwoStep { get; set; }

            [Caption("Name"), Description("Role Name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("Role Description")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [UIHint("IntValue"), ReadOnly]
            public int RoleId {
                get {
                    return roleId;
                }
                set {
                    roleId = value;
                }
            }
            private int roleId;

            public GridEdit() {  }
        }
#if MVC6
        public static HtmlString RenderRolesSelector<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model)
#else
        public static HtmlString RenderRolesSelector<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model)
#endif
        {
            if (model == null)
                model = new SerializableList<Role>();
            bool exclude2FA = htmlHelper.GetControlInfo<bool>(name, "ExcludeUser2FA", false);
            bool? showFilter = htmlHelper.GetControlInfo<bool?>(name, "ShowFilter", null);
            List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList(Exclude2FA: exclude2FA);
            int anonymousRole = Resource.ResourceAccess.GetAnonymousRoleId();
            List<GridEdit> roles = (from r in allRoles
                                           orderby r.Name
                                           where r.RoleId != anonymousRole
                                           select new GridEdit {
                                               RoleId = r.RoleId,
                                               Name = r.Name,
                                               Description = r.Description,
                                               ForceTwoStep = model.Contains(new Role { RoleId = r.RoleId }, new RoleComparer())
                                           }).ToList<GridEdit>();

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = roles.ToList<object>(),
                Total = roles.Count,
            };
            GridModel rolesModel = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridEdit),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = false,
                    ShowFilter = showFilter,
                }
            };
#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => rolesModel.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => rolesModel.GridDef);
#endif
        }
        public class GridDisplay {

            [Caption("Use"), Description("Checkbox indicates selection")]
            [UIHint("Boolean")]
            public bool ForceTwoStep { get { return true; } }

            [Caption("Name"), Description("Role Name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("Role Description")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [UIHint("IntValue"), ReadOnly]
            public int RoleId {
                get {
                    return roleId;
                }
                set {
                    roleId = value;
                }
            }
            private int roleId;

            public GridDisplay() { }
        }
#if MVC6
        public static HtmlString RenderRolesSelectorDisplay<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model)
#else
        public static HtmlString RenderRolesSelectorDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model)
#endif
        {
            if (model == null)
                model = new SerializableList<Role>();
            bool exclude2FA = htmlHelper.GetControlInfo<bool>(name, "ExcludeUser2FA", false);
            bool? showFilter = htmlHelper.GetControlInfo<bool?>(name, "ShowFilter", null);
            List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList(Exclude2FA: exclude2FA);
            int anonymousRole = Resource.ResourceAccess.GetAnonymousRoleId();
            List<GridDisplay> roles = (from r in allRoles
                                    orderby r.Name
                                    where r.RoleId != anonymousRole &&
                                        model.Contains(new Role { RoleId = r.RoleId }, new RoleComparer())
                                    select new GridDisplay {
                                        RoleId = r.RoleId,
                                        Name = r.Name,
                                        Description = r.Description,
                                    }).ToList<GridDisplay>();

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = roles.ToList<object>(),
                Total = roles.Count,
            };
            GridModel rolesModel = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridDisplay),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = true,
                    ShowFilter = showFilter,
                }
            };
#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => rolesModel.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => rolesModel.GridDef);
#endif
        }
    }
}