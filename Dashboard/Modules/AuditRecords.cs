/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Dashboard.Controllers;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Dashboard.Modules {

    public class AuditRecordsModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuditRecordsModule>, IInstallableModel { }

    [ModuleGuid("{8c1a3287-433f-4354-a3e8-867d5bd87b93}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class AuditRecordsModule : ModuleDefinition {

        public AuditRecordsModule() {
            Title = this.__ResStr("modTitle", "Audit Information");
            Name = this.__ResStr("modName", "Audit Information");
            Description = this.__ResStr("modSummary", "Displays and manages audit information. Audit information can be accessed using Admin > Dashboard > Audit Information (standard YetaWF site).");
            DefaultViewName = StandardViews.PropertyListEdit;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AuditRecordsModuleDataProvider(); }

        [Category("General"), Caption("Display Url"), Description("The Url to display an audit record - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveItems",
                        this.__ResStr("roleRemItemsC", "Remove Audit Information"), this.__ResStr("roleRemItems", "The role has permission to remove individual audit records"),
                        this.__ResStr("userRemItemsC", "Remove Audit Information"), this.__ResStr("userRemItems", "The user has permission to remove individual audit records")),
                };
            }
        }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            return menuList;
        }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Audit Information"),
                MenuText = this.__ResStr("browseText", "Audit Information"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage audit information"),
                Legend = this.__ResStr("browseLegend", "Displays and manages audit information"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_Remove(int id) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(AuditRecordsModuleController), "Remove"),
                NeedsModuleContext = true,
                QueryArgs = new { Id = id },
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove Audit Record"),
                MenuText = this.__ResStr("removeMenu", "Remove Audit Record"),
                Tooltip = this.__ResStr("removeTT", "Remove the audit record"),
                Legend = this.__ResStr("removeLegend", "Removes the audit record"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove audit record \"{0}\"?", id),
            };
        }
    }
}
