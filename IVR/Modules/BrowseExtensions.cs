/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.Controllers;
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
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Modules {

    public class BrowseExtensionsModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseExtensionsModule>, IInstallableModel { }

    [ModuleGuid("{c90d1c0b-7ed3-4584-8e1e-561714cf7c57}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BrowseExtensionsModule : ModuleDefinition {

        public BrowseExtensionsModule() {
            Title = this.__ResStr("modTitle", "Extensions");
            Name = this.__ResStr("modName", "Extensions");
            Description = this.__ResStr("modSummary", "Displays and manages extensions");
            DefaultViewName = StandardViews.PropertyListEdit;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BrowseExtensionsModuleDataProvider(); }

        [Category("General"), Caption("Add Url"), Description("The Url to add a new extension - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }
        [Category("General"), Caption("Edit Url"), Description("The Url to edit a extension - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveItems",
                        this.__ResStr("roleRemItemsC", "Remove Extension Entries"), this.__ResStr("roleRemItems", "The role has permission to remove individual extensions"),
                        this.__ResStr("userRemItemsC", "Remove Extension Entries"), this.__ResStr("userRemItems", "The user has permission to remove individual extensions")),
                };
            }
        }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            AddExtensionModule mod = new AddExtensionModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            return menuList;
        }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Extensions"),
                MenuText = this.__ResStr("browseText", "Extensions"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage extensions"),
                Legend = this.__ResStr("browseLegend", "Displays and manages extensions"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_Remove(int id) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(BrowseExtensionsModuleController), nameof(BrowseExtensionsModuleController.Remove)),
                NeedsModuleContext = true,
                QueryArgs = new { Id = id},
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove Extension"),
                MenuText = this.__ResStr("removeMenu", "Remove Extension"),
                Tooltip = this.__ResStr("removeTT", "Remove the extension"),
                Legend = this.__ResStr("removeLegend", "Removes the extension"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove extension with id \"{0}\"?", id),
            };
        }
    }
}
