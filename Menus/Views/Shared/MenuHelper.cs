/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Menus#License */

using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;

namespace YetaWF.Modules.Menus.Views.Shared {

    public class Menu<TModel> : RazorTemplate<TModel> { }

    public static class MenuHelper {

        public enum DirectionEnum {
            [EnumDescription("Opens towards the bottom")]
            Bottom = 0,
            [EnumDescription("Opens towards the top")]
            Top = 1,
            [EnumDescription("Opens towards the left")]
            Left = 2,
            [EnumDescription("Opens towards the right")]
            Right = 3,
        }

        public enum OrientationEnum {
            [EnumDescription("Horizontal menu")]
            Horizontal = 0,
            [EnumDescription("Vertical menu")]
            Vertical = 1,
        }

        public enum AnimationEnum {
            [EnumDescription("Slides up")]
            SlideUp = 0,
            [EnumDescription("Slides down")]
            SlideDown = 1,
            [EnumDescription("Fades in")]
            FadeIn = 2,
            [EnumDescription("Expands up")]
            ExpandsUp = 3,
            [EnumDescription("Expands down")]
            ExpandsDown = 4,
        }

        public class MenuData {
            public MenuList MenuList { get; set; }
            public DirectionEnum Direction { get; set; }
            public OrientationEnum Orientation { get; set; }
            public string CssClass { get; set; }
            public int HoverDelay { get; set; }
            public bool ShowPath { get; set; }

            public string GetDirection() {
                switch (Direction) {
                    default: return "default";
                    case MenuHelper.DirectionEnum.Top: return "top";
                    case MenuHelper.DirectionEnum.Bottom: return "bottom";
                    case MenuHelper.DirectionEnum.Left: return "left";
                    case MenuHelper.DirectionEnum.Right: return "right";
                }
            }
            public string GetOrientation() {
                switch (Orientation) {
                    default:
                    case MenuHelper.OrientationEnum.Horizontal: return "horizontal";
                    case MenuHelper.OrientationEnum.Vertical: return "vertical";
                }
            }
            //public string GetOpenEffects() {
            //    return GetEffects(OpenAnimation);
            //}
            //public string GetCloseEffects() {
            //    return GetEffects(CloseAnimation);
            //}
            //public string GetEffects(AnimationEnum anim) {
            //    switch (anim) {
            //        default:
            //        case AnimationEnum.SlideUp: return "slideIn:up";
            //        case AnimationEnum.SlideDown: return "slideIn:down";
            //        case AnimationEnum.FadeIn: return "fadeIn";
            //        case AnimationEnum.ExpandsDown: return "expand:down";
            //        case AnimationEnum.ExpandsUp: return "expand:up";
            //    }
            //}
        }

        //public class RolesModel {
        //    [UIHint("Grid")]
        //    public GridDefinition GridDef { get; set; }
        //}

        //public class GridAllowedRole {

        //    [DontSave, GridAttribute(Hidden = true, OnlySubmitWhenChecked = true)]
        //    public bool __editable { get; set; }
            
        //    [Caption("Apply"), Description("Select to apply the role to the user")]
        //    [GridAttribute(OnlySubmitWhenChecked = true)]
        //    [UIHint("Boolean")]
        //    public bool InRole { get; set; }

        //    [Caption("Role"), Description("Role Description")]
        //    [UIHint("YetaWF_Identity_RoleId"), ReadOnly]
        //    public int RoleDescription { get; private set; }
            
        //    [UIHint("Hidden")]
        //    public int RoleId {
        //        get {
        //            return roleId;
        //        }
        //        set {
        //            RoleDescription = roleId = value;
        //        }
        //    }
        //    private int roleId;

        //    public GridAllowedRole() { __editable = true; }
        //}

        //public static MvcHtmlString RenderResourceAllowedRolesDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model) {
        //    return htmlHelper.RenderResourceAllowedRoles<TModel>(name, model, ReadOnly: true);
        //}
        //public static MvcHtmlString RenderResourceAllowedRoles<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model, bool ReadOnly = false) {

        //    List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList();
        //    int superuserRole = Resource.ResourceAccess.GetSuperuserRoleId();
        //    List<GridAllowedRole> roles = (from r in allRoles orderby r.Name
        //                             select new GridAllowedRole {
        //                                 RoleId = r.RoleId,
        //                                 InRole = superuserRole == r.RoleId || (model != null && model.Contains(new Role { RoleId = r.RoleId }, new RoleComparer())),
        //                                 __editable = (superuserRole != r.RoleId) // we disable the superuser entry
        //                             }).ToList<GridAllowedRole>();

        //    bool header;
        //    if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
        //        header = true;
        //    DataSourceResult data = new DataSourceResult {
        //        Data = roles.ToList<object>(),
        //        Total = roles.Count,
        //    };
        //    RolesModel rolesModel = new RolesModel() {
        //        GridDef = new GridDefinition() {
        //            RecordType = typeof(GridAllowedRole),
        //            Data = data,
        //            SupportReload = false,
        //            PageSizes = new List<int>(),
        //            InitialPageSize = 10,
        //            ShowHeader = header,
        //            ReadOnly = ReadOnly,
        //        }
        //    };
        //    return htmlHelper.DisplayFor(m => rolesModel.GridDef);
        //}

        //public static MvcHtmlString RenderUserAllowedRolesDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model) {
        //    return htmlHelper.RenderUserAllowedRoles<TModel>(name, model, ReadOnly: true);
        //}
        //public static MvcHtmlString RenderUserAllowedRoles<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<Role> model, bool ReadOnly = false) {

        //    List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList();
        //    int userRole = Resource.ResourceAccess.GetUserRoleId();
        //    List<GridAllowedRole> roles = (from r in allRoles orderby r.Name
        //                                   select new GridAllowedRole {
        //                                       RoleId = r.RoleId,
        //                                       InRole = userRole == r.RoleId || (model != null && model.Contains(new Role { RoleId = r.RoleId }, new RoleComparer())),
        //                                       __editable = (userRole != r.RoleId) // we disable the user entry
        //                                   }).ToList<GridAllowedRole>();

        //    bool header;
        //    if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
        //        header = true;
        //    DataSourceResult data = new DataSourceResult {
        //        Data = roles.ToList<object>(),
        //        Total = roles.Count,
        //    };
        //    RolesModel rolesModel = new RolesModel() {
        //        GridDef = new GridDefinition() {
        //            RecordType = typeof(GridAllowedRole),
        //            Data = data,
        //            SupportReload = false,
        //            PageSizes = new List<int>(),
        //            InitialPageSize = 10,
        //            ShowHeader = header,
        //            ReadOnly = ReadOnly,
        //        }
        //    };
        //    return htmlHelper.DisplayFor(m => rolesModel.GridDef);
        //}
    }
}