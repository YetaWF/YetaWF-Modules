/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class RolesSelectorComponentBase : YetaWFComponent {

        public const string TemplateName = "RolesSelector";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class RolesSelectorDisplayComponent : RolesSelectorComponentBase, IYetaWFComponent<SerializableList<Role>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

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

        public async Task<YHtmlString> RenderAsync(SerializableList<Role> model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div class='yt_yetawf_identity_rolesselector t_edit'>");

            bool exclude2FA = PropData.GetAdditionalAttributeValue("ExcludeUser2FA", false);
            bool? showFilter = PropData.GetAdditionalAttributeValue<bool?>("ShowFilter", null);
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

            bool header = PropData.GetAdditionalAttributeValue("Header", true);
            DataSourceResult data = new DataSourceResult {
                Data = roles.ToList<object>(),
                Total = roles.Count,
            };
            GridModel grid = new GridModel() {
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

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            hb.Append($"</div>");
            return hb.ToYHtmlString();
        }
    }
    public class RolesSelectorEditComponent : RolesSelectorComponentBase, IYetaWFComponent<SerializableList<Role>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

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

            public GridEdit() { }
        }
        public async Task<YHtmlString> RenderAsync(SerializableList<Role> model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div class='yt_yetawf_identity_rolesselector t_edit'>");

            bool exclude2FA = PropData.GetAdditionalAttributeValue("ExcludeUser2FA", false);
            bool? showFilter = PropData.GetAdditionalAttributeValue<bool?>("ShowFilter", null);
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

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            DataSourceResult data = new DataSourceResult {
                Data = roles.ToList<object>(),
                Total = roles.Count,
            };
            GridModel grid = new GridModel() {
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

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            hb.Append($"</div>");
            return hb.ToYHtmlString();
        }
    }
}
