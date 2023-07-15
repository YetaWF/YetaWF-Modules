/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Endpoints;
using YetaWF.Modules.Blog.Scheduler;

namespace YetaWF.Modules.Blog.Modules;

public class CategoriesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, CategoriesBrowseModule>, IInstallableModel { }

[ModuleGuid("{9e372eb2-7aab-49c1-98d5-2d5c6de3d724}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Categories")]
public class CategoriesBrowseModule : ModuleDefinition {

    public CategoriesBrowseModule() {
        Title = this.__ResStr("modTitle", "Blog Categories");
        Name = this.__ResStr("modName", "Blog Categories");
        Description = this.__ResStr("modSummary", "Displays and manages blog categories. It is accessible using Admin > Blog > Categories (standard YetaWF site). It is used to add, edit and remove blog categories and view blog entries. It also offers automated Google news sitemap generation.");
        DefaultViewName = StandardViews.Browse;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new CategoriesBrowseModuleDataProvider(); }

    [Category("General"), Caption("Add URL"), Description("The URL to add a new blog category - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? AddUrl { get; set; }
    [Category("General"), Caption("Display URL"), Description("The URL to display a blog category - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }
    [Category("General"), Caption("Edit URL"), Description("The URL to edit a blog category - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? EditUrl { get; set; }
    [Category("General"), Caption("Browse Entries URL"), Description("The URL to browse blog entries - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? BrowseEntriesUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveItems",
                    this.__ResStr("roleRemItemsC", "Remove Blog Categories"), this.__ResStr("roleRemItems", "The role has permission to remove individual blog categories"),
                    this.__ResStr("userRemItemsC", "Remove Blog Categories"), this.__ResStr("userRemItems", "The user has permission to remove individual blog categories")),
                new RoleDefinition("NewsSiteMap",
                    this.__ResStr("roleNewsSiteMapC", "Manage News Site Map"), this.__ResStr("roleNewsSiteMaps", "The role has permission to manage news site map"),
                    this.__ResStr("userNewsSiteMapC", "Manage News Site Map"), this.__ResStr("userNewsSiteMaps", "The user has permission to manage news site map")),
            };
        }
    }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        CategoryAddModule mod = new CategoryAddModule();
        menuList.New(mod.GetAction_Add(AddUrl), location);
        return menuList;
    }

    public ModuleAction? GetAction_Categories(string? url) {
        return new ModuleAction() {
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
    public ModuleAction? GetAction_Remove(int blogCategory) {
        if (!IsAuthorized("RemoveItems")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(CategoriesBrowseModuleEndpoints), CategoriesBrowseModuleEndpoints.Remove),
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
    public ModuleAction? GetAction_CreateNewsSiteMap() {
        if (!IsAuthorized("NewsSiteMap")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(CategoriesBrowseModuleEndpoints), CategoriesBrowseModuleEndpoints.CreateNewsSiteMap),
            QueryArgs = new { },
            Image = "#Add",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("screAuthLink", "Create News Site Map"),
            MenuText = this.__ResStr("screAuthMenu", "Create News Site Map"),
            Tooltip = this.__ResStr("screAuthTT", "Create a news site map"),
            Legend = this.__ResStr("screAuthLegend", "Creates a news site map"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = this.__ResStr("screAuthConfirm", "Are you sure you want to create a new news site map?"),
            PleaseWaitText = this.__ResStr("screAuthPlsWait", "Creating news site map..."),
        };
    }
    public ModuleAction? GetAction_RemoveNewsSiteMap() {
        if (!IsAuthorized("NewsSiteMap")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(CategoriesBrowseModuleEndpoints), CategoriesBrowseModuleEndpoints.RemoveNewsSiteMap),
            QueryArgs = new { },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("sremAuthLink", "Remove News Site Map"),
            MenuText = this.__ResStr("sremAuthMenu", "Remove News Site Map"),
            Tooltip = this.__ResStr("sremAuthTT", "Remove current news site map"),
            Legend = this.__ResStr("sremAuthLegend", "Removes the current news site map"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = this.__ResStr("sremAuthConfirm", "Are you sure you want to remove the current news site map?"),
        };
    }
    public async Task<ModuleAction?> GetAction_DownloadNewsSiteMapAsync() {
        if (!IsAuthorized("NewsSiteMap")) return null;
        NewsSiteMap sm = new NewsSiteMap();
        string filename = sm.GetNewsSiteMapFileName();
        if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
            return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(CategoriesBrowseModuleEndpoints), nameof(CategoriesBrowseModuleEndpoints.DownloadNewsSiteMap)),
            CookieAsDoneSignal = true,
            Image = await CustomIconAsync("Download.png"),
            LinkText = this.__ResStr("downloadLink", "Download News Site Map"),
            MenuText = this.__ResStr("downloadMenu", "Download News Site Map"),
            Tooltip = this.__ResStr("downloadTT", "Download the news site map file"),
            Legend = this.__ResStr("downloadLegend", "Downloads the news site map file"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();

                EntriesBrowseModule browseEntriesMod = new EntriesBrowseModule();
                actions.New(browseEntriesMod.GetAction_BrowseEntries(Module.BrowseEntriesUrl, Identity), ModuleAction.ActionLocationEnum.GridLinks);
                CategoryDisplayModule dispMod = new CategoryDisplayModule();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Identity), ModuleAction.ActionLocationEnum.GridLinks);
                CategoryEditModule editMod = new CategoryEditModule();
                actions.New(editMod.GetAction_Edit(Module.EditUrl, Identity), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(Module.GetAction_Remove(Identity), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }
        }

        [Caption("Id"), Description("The id of this blog category - used to uniquely identify this blog category internally")]
        [UIHint("IntValue"), ReadOnly]
        public int Identity { get; set; }

        [Caption("Category"), Description("The name of this blog category")]
        [UIHint("MultiString"), ReadOnly]
        public MultiString Category { get; set; } = null!;

        [Caption("Description"), Description("The description of the blog category - the category's description is shown at the top of each blog entry to describe your blog")]
        [UIHint("MultiString"), ReadOnly]
        public MultiString Description { get; set; } = null!;

        [Caption("Date Created"), Description("The creation date of the blog category")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime DateCreated { get; set; }

        [Caption("Use Captcha"), Description("Defines whether anonymous users entering comments are presented with a Captcha to insure they are not automated spam scripts")]
        [UIHint("Boolean"), ReadOnly]
        public bool UseCaptcha { get; set; }

        [Caption("Comment Approval"), Description("Defines whether submitted comments must be approved before being publicly viewable")]
        [UIHint("Enum"), ReadOnly]
        public BlogCategory.ApprovalType CommentApproval { get; set; }

        [Caption("Syndicated"), Description("Defines whether the blog category can be subscribed to by news readers (entries must be published before they can be syndicated)")]
        [UIHint("Boolean"), ReadOnly]
        public bool Syndicated { get; set; }

        [Caption("Email Address"), Description("The email address used as email address responsible for the blog category")]
        [UIHint("String"), ReadOnly]
        public string? SyndicationEmail { get; set; }

        [Caption("Syndication Copyright"), Description("The optional copyright information shown when the blog is accessed by news readers")]
        [UIHint("MultiString"), ReadOnly]
        public MultiString SyndicationCopyright { get; set; } = null!;

        private CategoriesBrowseModule Module { get; set; }

        public BrowseItem(CategoriesBrowseModule module, BlogCategory data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }

    public class BrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }

    public GridDefinition GetGridModel() {
        return new GridDefinition {
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<CategoriesBrowseModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                    DataProviderGetRecords<BlogCategory> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
