using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;
using YetaWF.Modules.Diagnostics.Controllers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Diagnostics.Modules {

    public class AuditRecordsModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuditRecordsModule>, IInstallableModel { }

    [ModuleGuid("{8c1a3287-433f-4354-a3e8-867d5bd87b93}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class AuditRecordsModule : ModuleDefinition {

        public AuditRecordsModule() {
            Title = this.__ResStr("modTitle", "Audit Infos");
            Name = this.__ResStr("modName", "Audit Infos");
            Description = this.__ResStr("modSummary", "Displays and manages audit infos");
            DefaultViewName = StandardViews.PropertyListEdit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AuditRecordsModuleDataProvider(); }

        [Category("General"), Caption("Add Url"), Description("The Url to add a new audit info - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }
        [Category("General"), Caption("Display Url"), Description("The Url to display a audit info - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }
        [Category("General"), Caption("Edit Url"), Description("The Url to edit a audit info - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } } // TODO: adjust default permissions
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveItems",
                        this.__ResStr("roleRemItemsC", "Remove Audit Infos"), this.__ResStr("roleRemItems", "The role has permission to remove individual audit infos"),
                        this.__ResStr("userRemItemsC", "Remove Audit Infos"), this.__ResStr("userRemItems", "The user has permission to remove individual audit infos")),
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
                LinkText = this.__ResStr("browseLink", "Audit Infos"),
                MenuText = this.__ResStr("browseText", "Audit Infos"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage audit infos"),
                Legend = this.__ResStr("browseLegend", "Displays and manages audit infos"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_Remove(int id) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(AuditRecordsModuleController), "Remove"),
                NeedsModuleContext = true,
                QueryArgs = new { Id = id },
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove Audit Info"),
                MenuText = this.__ResStr("removeMenu", "Remove Audit Info"),
                Tooltip = this.__ResStr("removeTT", "Remove the audit info"),
                Legend = this.__ResStr("removeLegend", "Removes the audit info"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove audit info \"{0}\"?", id),
            };
        }
    }
}
