/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Web.Mvc;
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
using YetaWF.Modules.Blog.Controllers;

namespace YetaWF.Modules.Blog.Modules {

    public class CategoriesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, CategoriesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{9e372eb2-7aab-49c1-98d5-2d5c6de3d724}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CategoriesBrowseModule : ModuleDefinition {

        public CategoriesBrowseModule() {
            Title = this.__ResStr("modTitle", "Blog Categories");
            Name = this.__ResStr("modName", "Blog Categories");
            Description = this.__ResStr("modSummary", "Displays and manages blog categories");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CategoriesBrowseModuleDataProvider(); }

        [Category("General"), Caption("Add URL"), Description("The URL to add a new blog category - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }
        [Category("General"), Caption("Display URL"), Description("The URL to display a blog category - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }
        [Category("General"), Caption("Edit URL"), Description("The URL to edit a blog category - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }
        [Category("General"), Caption("Browse Entries URL"), Description("The URL to browse blog entries - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string BrowseEntriesUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveItems",
                        this.__ResStr("roleRemItemsC", "Remove Blog Categories"), this.__ResStr("roleRemItems", "The role has permission to remove individual blog categories"),
                        this.__ResStr("userRemItemsC", "Remove Blog Categories"), this.__ResStr("userRemItems", "The user has permission to remove individual blog categories")),
                };
            }
        }

        public override MenuList GetModuleMenuList(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = base.GetModuleMenuList(renderMode, location);
            CategoryAddModule mod = new CategoryAddModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            return menuList;
        }

        public ModuleAction GetAction_Categories(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Blog Categories"),
                MenuText = this.__ResStr("browseText", "Blog Categories"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage blog categories"),
                Legend = this.__ResStr("browseLegend", "Displays and manages blog categories"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_Remove(int blogCategory) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(CategoriesBrowseModuleController), "Remove"),
                NeedsModuleContext = true,
                QueryArgs = new { BlogCategory = blogCategory },
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove blog category"),
                MenuText = this.__ResStr("removeMenu", "Remove blog category"),
                Tooltip = this.__ResStr("removeTT", "Remove the blog category"),
                Legend = this.__ResStr("removeLegend", "Removes the blog category"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this blog category?"),
            };
        }
    }
}