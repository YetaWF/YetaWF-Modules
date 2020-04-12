/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.PageEdit.Components {

    public abstract class AllowedRolesComponentBase : YetaWFComponent {

        public const string TemplateName = "AllowedRoles";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.PageEdit package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class AllowedRolesEditComponent : AllowedRolesComponentBase, IYetaWFComponent<SerializableList<PageDefinition.AllowedRole>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class GridAllowedRole {

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

            [UIHint("Hidden")]
            public int RoleId { get; set; }

            public bool __editable { get; set; }

            public GridAllowedRole() { __editable = true; }
        }

        private GridDefinition GetGridModel(bool header) {

            return new GridDefinition {
                RecordType = typeof(GridAllowedRole),
                ShowHeader = header,
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<GridAllowedRole> recs = DataProviderImpl<GridAllowedRole>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        private List<GridAllowedRole> GetGridAllowedRoleFromAllowedRoleList(SerializableList<PageDefinition.AllowedRole> allowedRoles) {
            List<RoleInfo> list = Resource.ResourceAccess.GetDefaultRoleList();
            List<GridAllowedRole> roles = (from r in list orderby r.Name select new GridAllowedRole { RoleId = r.RoleId, RoleName = new StringTT { Text = r.Name, Tooltip = r.Description } }).ToList();
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
        public async Task<string> RenderAsync(SerializableList<PageDefinition.AllowedRole> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                List<GridAllowedRole> list = GetGridAllowedRoleFromAllowedRoleList(model);
                DataSourceResult data = new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count,
                };
                return Task.FromResult(data);
            };

            hb.Append($@"
<div class='yt_yetawf_pageedit_allowedroles t_display'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToString();
        }
    }
}
