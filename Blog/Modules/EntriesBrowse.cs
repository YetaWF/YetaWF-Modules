/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

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
using YetaWF.Modules.Blog.Controllers;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Modules {

    public class EntriesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, EntriesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{2809e9cf-1de8-41f2-9108-bc84ae7fb2f4}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class EntriesBrowseModule : ModuleDefinition {

        public EntriesBrowseModule() {
            Title = this.__ResStr("modTitle", "Blog Entries");
            Name = this.__ResStr("modName", "Blog Entries");
            Description = this.__ResStr("modSummary", "Displays and manages blog entries");
            DefaultViewName = StandardViews.Browse;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EntriesBrowseModuleDataProvider(); }

        [Category("General"), Caption("Add URL"), Description("The URL to add a new blog entry - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }
        [Category("General"), Caption("Edit URL"), Description("The URL to edit a blog entry - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveItems",
                        this.__ResStr("roleRemItemsC", "Remove Blog Entries"), this.__ResStr("roleRemItems", "The role has permission to remove individual blog entries"),
                        this.__ResStr("userRemItemsC", "Remove Blog Entries"), this.__ResStr("userRemItems", "The user has permission to remove individual blog entries")),
                };
            }
        }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            EntryAddModule mod = new EntryAddModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            return menuList;
        }

        public ModuleAction GetAction_BrowseEntries(string url, int blogCategory) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { BlogCategory = blogCategory },
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Blog Entries"),
                MenuText = this.__ResStr("browseText", "Blog Entries"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage blog entries"),
                Legend = this.__ResStr("browseLegend", "Displays and manages blog entries"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_Remove(int blogEntry) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(EntriesBrowseModuleController), "Remove"),
                NeedsModuleContext = true,
                QueryArgs = new { BlogEntry = blogEntry },
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove blog entry"),
                MenuText = this.__ResStr("removeMenu", "Remove blog entry"),
                Tooltip = this.__ResStr("removeTT", "Remove the blog entry"),
                Legend = this.__ResStr("removeLegend", "Removes the blog entry"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this blog entry?"),
            };
        }
    }
}