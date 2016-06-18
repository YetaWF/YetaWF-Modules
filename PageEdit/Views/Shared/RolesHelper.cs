/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/PageEdit#License */

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

    public class Roles<TModel> : RazorTemplate<TModel> { }

    public static class RolesHelper {

        public class RolesModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        public class GridAllowedRole {

            [DontSave]
            public bool __editable { get; set; }

            [Caption("Role"), Description("Role Description")]
            [UIHint("StringTT"), ReadOnly]
            public StringTT RoleName { get; set; }

            [Caption("View"), Description("The role has permission to view the page")]
            [UIHint("Enum")]
            public PageDefinition.AllowedEnum View { get; set; }

            [Caption("Edit"), Description("The role has permission to edit the page")]
            [UIHint("Enum")]
            public PageDefinition.AllowedEnum Edit { get; set; }

            [Caption("Remove"), Description("The role has permission to remove the page")]
            [UIHint("Enum")]
            public PageDefinition.AllowedEnum Remove { get; set; }

            public int RoleId { get; set; }

            public GridAllowedRole() { __editable = true; }
        }

        private static List<GridAllowedRole> GetGridAllowedRoleFromAllowedRoleList(SerializableList<PageDefinition.AllowedRole> allowedRoles) {
            List<RoleInfo> list = Resource.ResourceAccess.GetDefaultRoleList();
            List<GridAllowedRole> roles = (from r in list select new GridAllowedRole { RoleId = r.RoleId, RoleName = new StringTT { Text = r.Name, Tooltip = r.Description } }).ToList();
            if (allowedRoles != null) {
                foreach (PageDefinition.AllowedRole allowedRole in allowedRoles) {
                    GridAllowedRole role = (from r in roles where r.RoleId == allowedRole.RoleId select r).FirstOrDefault();
                    if (role != null)
                        ObjectSupport.CopyData(allowedRole, role);
                }
            }
            GridAllowedRole superuser = (from r in roles where r.RoleId == Resource.ResourceAccess.GetSuperuserRoleId() select r).First();
            superuser.View = PageDefinition.AllowedEnum.Yes;
            superuser.Edit = PageDefinition.AllowedEnum.Yes;
            superuser.Remove = PageDefinition.AllowedEnum.Yes;
            superuser.__editable = false;
            return roles;
        }
        public static MvcHtmlString RenderAllowedRoles<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<PageDefinition.AllowedRole> model) {

            List<GridAllowedRole> list = GetGridAllowedRoleFromAllowedRoleList(model);

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            RolesModel rolesModel = new RolesModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridAllowedRole),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = false,
                }
            };
            return htmlHelper.DisplayFor(m => rolesModel.GridDef);
        }
    }
}