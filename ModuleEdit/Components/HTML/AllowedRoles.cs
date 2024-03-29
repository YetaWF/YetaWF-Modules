/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ModuleEdit.Components {

    public abstract class AllowedRolesComponentBase : YetaWFComponent {

        public const string TemplateName = "AllowedRoles";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.ModuleEdit package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class AllowedRolesEditComponent : AllowedRolesComponentBase, IYetaWFComponent<SerializableList<ModuleDefinition.AllowedRole>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        private GridDefinition GetGridModel(bool header) {

            return new GridDefinition {
                RecordType = typeof(ModuleDefinition.GridAllowedRole),
                ShowHeader = header,
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<ModuleDefinition.GridAllowedRole> recs = DataProviderImpl<ModuleDefinition.GridAllowedRole>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        private List<ModuleDefinition.GridAllowedRole> GetGridAllowedRoleFromAllowedRoleList(SerializableList<ModuleDefinition.AllowedRole> allowedRoles) {

            List<RoleInfo> list = Resource.ResourceAccess.GetDefaultRoleList();
            List<ModuleDefinition.GridAllowedRole> roles = new List<ModuleDefinition.GridAllowedRole>();

            foreach (RoleInfo r in list) {
                // we have to create a more derived type here to get all the "extra" fields
                ModuleDefinition.GridAllowedRole gridRole = new ModuleDefinition.GridAllowedRole();
                gridRole.RoleId = r.RoleId;
                gridRole.RoleName = new StringTT { Text = r.Name, Tooltip = r.Description };
                roles.Add(gridRole);
            }
            roles = (from r in roles orderby r.RoleName.Text select r).ToList();
            if (allowedRoles != null) {
                foreach (ModuleDefinition.AllowedRole allowedRole in allowedRoles) {
                    ModuleDefinition.GridAllowedRole? role = (from r in roles where r.RoleId == allowedRole.RoleId select r).FirstOrDefault();
                    if (role != null)
                        ObjectSupport.CopyData(allowedRole, role);
                }
            }
            // make a default entry for the superuser and disable it
            ModuleDefinition.AllowedRole superRole = new ModuleDefinition.AllowedRole {
                RoleId = Resource.ResourceAccess.GetSuperuserRoleId(),
            };
            superRole.Edit = superRole.Remove = superRole.View = superRole.Extra1 = superRole.Extra2 = superRole.Extra3 = superRole.Extra4 = superRole.Extra5 = ModuleDefinition.AllowedEnum.Yes;

            ModuleDefinition.GridAllowedRole superuser = (from r in roles where r.RoleId == Resource.ResourceAccess.GetSuperuserRoleId() select r).First();
            ObjectSupport.CopyData(superRole, superuser);
            return roles;
        }
        public async Task<string> RenderAsync(SerializableList<ModuleDefinition.AllowedRole> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.ResourceRedirect = Manager.CurrentModuleEdited;
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                List<ModuleDefinition.GridAllowedRole> list = GetGridAllowedRoleFromAllowedRoleList(model);
                DataSourceResult data = new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count,
                };
                return Task.FromResult(data);
            };

            hb.Append($@"
<div class='yt_yetawf_moduleedit_allowedroles t_display'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToString();
        }
    }
}
