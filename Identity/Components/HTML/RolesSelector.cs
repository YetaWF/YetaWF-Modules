/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;

namespace YetaWF.Modules.Identity.Components {

    public abstract class RolesSelectorComponentBase : YetaWFComponent {

        public const string TemplateName = "RolesSelector";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Shows a list of roles in a grid. The model contains the list of roles.
    /// </summary>
    /// <remarks>
    /// For information about roles in YetaWF, see the Authorization topic.
    /// </remarks>
    /// <example>
    /// [Caption("Two-Step Authentication"), Description("Shows which roles require mandatory two-step authentication")]
    /// [UIHint("YetaWF_Identity_RolesSelector"), ReadOnly]
    /// public SerializableList&lt;Role&gt; TwoStepAuth { get; set; }
    /// </example>
    public class RolesSelectorDisplayComponent : RolesSelectorComponentBase, IYetaWFComponent<SerializableList<Role>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class Entry {

            [Caption("Use"), Description("Checkbox indicates selection")]
            [UIHint("Boolean"), ReadOnly]
            public bool Used { get { return true; } }

            [Caption("Name"), Description("Role Name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("Role Description")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            public Entry() { }
        }
        internal static GridDefinition GetGridModel(bool header, bool? filter) {
            return new GridDefinition() {
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                ShowFilter = filter,
                AjaxUrl = Utility.UrlFor(typeof(RolesSelectorController), nameof(RolesSelectorController.RolesSelectorDisplay_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        public async Task<string> RenderAsync(SerializableList<Role> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);
            bool exclude2FA = PropData.GetAdditionalAttributeValue("ExcludeUser2FA", false);
            bool? showFilter = PropData.GetAdditionalAttributeValue<bool?>("ShowFilter", null);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header, showFilter)
            };
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList(Exclude2FA: exclude2FA);
                int anonymousRole = Resource.ResourceAccess.GetAnonymousRoleId();
                List<Entry> roles = (from r in allRoles
                                     orderby r.Name
                                     where r.RoleId != anonymousRole &&
                                         model.Contains(new Role { RoleId = r.RoleId }, new RoleComparer())
                                     select new Entry {
                                         Name = r.Name,
                                         Description = r.Description,
                                     }).ToList<Entry>();

                DataSourceResult data = new DataSourceResult {
                    Data = roles.ToList<object>(),
                    Total = roles.Count,
                };
                return Task.FromResult(data);
            };

            hb.Append($@"
<div class='yt_yetawf_identity_rolesselector t_display'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");
            return hb.ToString();
        }
    }

    /// <summary>
    /// Shows a list of all roles in a grid. Individual roles can be selected/deselected. The model contains the list of selected roles.
    /// </summary>
    /// <remarks>
    /// For information about roles in YetaWF, see the Authorization topic.
    /// </remarks>
    /// <example>
    /// [Caption("Two-Step Authentication"), Description("Defines which roles require mandatory two-step authentication")]
    /// [UIHint("YetaWF_Identity_RolesSelector"), AdditionalMetadata("ExcludeUser2FA", true)]
    /// public SerializableList&lt;Role&gt; TwoStepAuth { get; set; }
    /// </example>
    [UsesAdditional("Header", "bool", "true", "Defines whether the grid header is shown.")]
    [UsesAdditional("ShowFilter", "bool", "false", "Defines whether the grid filters are shown.")]
    [UsesAdditional("ExcludeUser2FA", "bool?", "null", "Defines whether the role \"User (two-step authentication setup required)\" is excluded from the list.")]
    public class RolesSelectorEditComponent : RolesSelectorComponentBase, IYetaWFComponent<SerializableList<Role>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class Entry {

            [Caption("Use"), Description("Click checkbox to select")]
            [UIHint("Boolean")]
            public bool Used { get; set; }

            [Caption("Name"), Description("Role Name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("Role Description")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public int RoleId { get; set; }

            public Entry() { }
        }
        internal static GridDefinition GetGridModel(bool header, bool? filter) {
            return new GridDefinition() {
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                ShowFilter = filter,
                AjaxUrl = Utility.UrlFor(typeof(RolesSelectorController), nameof(RolesSelectorController.RolesSelectorEdit_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        public async Task<string> RenderAsync(SerializableList<Role> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);
            bool exclude2FA = PropData.GetAdditionalAttributeValue("ExcludeUser2FA", false);
            bool? showFilter = PropData.GetAdditionalAttributeValue<bool?>("ShowFilter", null);

            if (model == null)
                model = new SerializableList<Role>();

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header, showFilter)
            };
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList(Exclude2FA: exclude2FA);
                int anonymousRole = Resource.ResourceAccess.GetAnonymousRoleId();
                List<Entry> roles = (from r in allRoles
                                     orderby r.Name
                                     where r.RoleId != anonymousRole
                                     select new Entry {
                                         RoleId = r.RoleId,
                                         Name = r.Name,
                                         Description = r.Description,
                                         Used = model.Contains(new Role { RoleId = r.RoleId }, new RoleComparer())
                                     }).ToList<Entry>();

                DataSourceResult data = new DataSourceResult {
                    Data = roles.ToList<object>(),
                    Total = roles.Count,
                };
                return Task.FromResult(data);
            };

            hb.Append($@"
<div class='yt_yetawf_identity_rolesselector t_edit' id='{DivId}'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToString();
        }
    }
}
