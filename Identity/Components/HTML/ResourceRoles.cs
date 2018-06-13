using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Identity.Components {

    public abstract class ResourceRolesComponentBase : YetaWFComponent {

        public const string TemplateName = "ResourceRoles";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public class GridAllowedRole {

            [DontSave]
            [UIHint("Boolean")]
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

            [UIHint("Hidden")]
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
        protected async Task<YHtmlString> RenderAsync(SerializableList<YetaWF.Core.Identity.Role> model, bool ReadOnly) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div class='yt_yetawf_identity_resourceroles t_edit'>");

            List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList();
            int superuserRole = Resource.ResourceAccess.GetSuperuserRoleId();
            List<GridAllowedRole> roles = (from r in allRoles orderby r.Name
                                           select new GridAllowedRole {
                                               RoleId = r.RoleId,
                                               Name = r.Name,
                                               Description = r.Description,
                                               InRole = superuserRole == r.RoleId || (model != null && model.Contains(new Role { RoleId = r.RoleId }, new RoleComparer())),
                                               __editable = (superuserRole != r.RoleId) // we disable the superuser entry
                                           }).ToList();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            DataSourceResult data = new DataSourceResult {
                Data = roles.ToList<object>(),
                Total = roles.Count,
            };
            GridModel grid = new GridModel() {
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

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            hb.Append($"</div>");
            return hb.ToYHtmlString();
        }
    }

    public class ResourceRolesDisplayComponent : ResourceRolesComponentBase, IYetaWFComponent<SerializableList<Role>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(SerializableList<YetaWF.Core.Identity.Role> model) {
            return await RenderAsync(model, true);
        }
    }
    public class ResourceRolesEditComponent : ResourceRolesComponentBase, IYetaWFComponent<SerializableList<Role>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(SerializableList<YetaWF.Core.Identity.Role> model) {
            return await RenderAsync(model, false);
        }
    }
}
