using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
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

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class AllowedRolesEditComponent : AllowedRolesComponentBase, IYetaWFComponent<SerializableList<ModuleDefinition.AllowedRole>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class GridModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        private List<ModuleDefinition.GridAllowedRole> GetGridAllowedRoleFromAllowedRoleList(SerializableList<ModuleDefinition.AllowedRole> allowedRoles, Type gridEntryType) {

            List<RoleInfo> list = Resource.ResourceAccess.GetDefaultRoleList();
            List<ModuleDefinition.GridAllowedRole> roles = new List<ModuleDefinition.GridAllowedRole>();

            foreach (RoleInfo r in list) {
                // we have to create a more derived type here to get all the "extra" fields
                ModuleDefinition.GridAllowedRole gridRole = (ModuleDefinition.GridAllowedRole)Activator.CreateInstance(gridEntryType);
                gridRole.RoleId = r.RoleId;
                gridRole.RoleName = new StringTT { Text = r.Name, Tooltip = r.Description };
                roles.Add(gridRole);
            }
            roles = (from r in roles orderby r.RoleName.Text select r).ToList();
            if (allowedRoles != null) {
                foreach (ModuleDefinition.AllowedRole allowedRole in allowedRoles) {
                    ModuleDefinition.GridAllowedRole role = (from r in roles where r.RoleId == allowedRole.RoleId select r).FirstOrDefault();
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
        public async Task<YHtmlString> RenderAsync(SerializableList<ModuleDefinition.AllowedRole> model) {

            HtmlBuilder hb = new HtmlBuilder();

            Type gridEntryType = PropData.GetAdditionalAttributeValue<Type>("GridEntry", null);
            if (gridEntryType == null)
                gridEntryType = typeof(ModuleDefinition.GridAllowedRole);

            hb.Append($"<div class='yt_yetawf_moduleedit_allowedroles t_edit'>");

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            List<ModuleDefinition.GridAllowedRole> list = GetGridAllowedRoleFromAllowedRoleList(model, gridEntryType);

            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = gridEntryType,
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = false,
                    ResourceRedirect = Manager.CurrentModuleEdited,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            hb.Append($"</div>");
            return hb.ToYHtmlString();
        }
    }
}
