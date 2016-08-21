/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using YetaWF.Core.Identity;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ModuleEdit.Views.Shared {

    public class Roles<TModel> : RazorTemplate<TModel> { }

    public static class RolesHelper {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public class RolesModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        private static List<ModuleDefinition.GridAllowedRole> GetGridAllowedRoleFromAllowedRoleList(SerializableList<ModuleDefinition.AllowedRole> allowedRoles, Type gridEntryType) {

            List<RoleInfo> list = Resource.ResourceAccess.GetDefaultRoleList();
            List<ModuleDefinition.GridAllowedRole> roles = new List<ModuleDefinition.GridAllowedRole>();

            foreach (RoleInfo r in list) {
                // we have to create a more derived type here to get all the "extra" fields
                ModuleDefinition.GridAllowedRole gridRole = (ModuleDefinition.GridAllowedRole) Activator.CreateInstance(gridEntryType);
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
        public static MvcHtmlString RenderAllowedRoles<TModel>(this HtmlHelper<TModel> htmlHelper, string name, SerializableList<ModuleDefinition.AllowedRole> model) {
            Type gridEntryType;
            if (!htmlHelper.TryGetControlInfo<Type>("", "GridEntry", out gridEntryType))
                gridEntryType = typeof(ModuleDefinition.GridAllowedRole);

            List<ModuleDefinition.GridAllowedRole> list = GetGridAllowedRoleFromAllowedRoleList(model, gridEntryType);

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            RolesModel rolesModel = new RolesModel() {
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
            return htmlHelper.DisplayFor(m => rolesModel.GridDef);
        }
    }
}