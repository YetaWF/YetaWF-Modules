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

namespace YetaWF.Modules.Blog.Modules {

    public class EntriesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, EntriesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{2809e9cf-1de8-41f2-9108-bc84ae7fb2f4}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Entries")]
    public class EntriesBrowseModule : ModuleDefinition2 {

        public EntriesBrowseModule() {
            Title = this.__ResStr("modTitle", "Blog Entries");
            Name = this.__ResStr("modName", "Blog Entries");
            Description = this.__ResStr("modSummary", "Displays and manages blog entries. It is accessible using Admin > Blog > Entries (standard YetaWF site). It is used to add, edit, view and remove blog entries.");
            DefaultViewName = StandardViews.Browse;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EntriesBrowseModuleDataProvider(); }

        [Category("General"), Caption("Add URL"), Description("The URL to add a new blog entry - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string? AddUrl { get; set; }
        [Category("General"), Caption("Edit URL"), Description("The URL to edit a blog entry - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string? EditUrl { get; set; }

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

        public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
            EntryAddModule mod = new EntryAddModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            return menuList;
        }

        public ModuleAction? GetAction_BrowseEntries(string? url, int blogCategory) {
            return new ModuleAction() {
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
        public ModuleAction? GetAction_Remove(int blogEntry) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction() {
                Url = Utility.UrlFor(typeof(EntriesBrowseModuleEndpoints), EntriesBrowseModuleEndpoints.Remove),
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

        public class BrowseItem {
            public class ExtraData {
                public int BlogCategory { get; set; }
            }

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands { get; set; } = null!;

            public async Task<List<ModuleAction>> __GetCommandsAsync() {
                List<ModuleAction> actions = new List<ModuleAction>();
                EntryDisplayModule dispMod = new EntryDisplayModule();
                actions.New(await dispMod.GetAction_DisplayAsync(Identity), ModuleAction.ActionLocationEnum.GridLinks);
                EntryEditModule editMod = new EntryEditModule();
                actions.New(await editMod.GetAction_EditAsync(Module.EditUrl, Identity), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(Module.GetAction_Remove(Identity), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }


            [Caption("Id"), Description("The id of this blog entry - used to uniquely identify this blog entry internally")]
            [UIHint("IntValue"), ReadOnly]
            public int Identity { get; set; }

            public int CategoryIdentity { get; set; }

            [Caption("Title"), Description("The title for this blog entry")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; } = null!;
            [Caption("Author"), Description("The name of the blog author")]
            [UIHint("String"), ReadOnly]
            public string? Author { get; set; }

            [Caption("Allow Comments"), Description("Defines whether comments can be entered for this blog entry")]
            [UIHint("Boolean"), ReadOnly]
            public bool OpenForComments { get; set; }

            [Caption("Comments To Approve"), Description("The number of comments that need approval")]
            [UIHint("IntValue"), ReadOnly]
            public int CommentsUnapproved { get; set; }

            [Caption("Comments"), Description("The number of comments")]
            [UIHint("IntValue"), ReadOnly]
            public int Comments { get; set; }

            [Caption("Published"), Description("Defines whether this entry has been published and is viewable by everyone")]
            [UIHint("Boolean"), ReadOnly]
            public bool Published { get; set; }

            [Caption("Date Published"), Description("The date this entry has been published")]
            [UIHint("Date"), ReadOnly]
            public DateTime DatePublished { get; set; }

            [Caption("Date Created"), Description("The date this entry was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateCreated { get; set; }

            [Caption("Date Updated"), Description("The date this entry was updated")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateUpdated { get; set; }

            [Caption("Category"), Description("The name of the blog category")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Category { get; set; } = null!;

            private EntriesBrowseModule Module { get; set; }

            public BrowseItem(EntriesBrowseModule module, BlogCategoryDataProvider categoryDP, BlogEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }

        public GridDefinition GetGridModel(int blogCategory) {
            return new GridDefinition {
                ModuleGuid = ModuleGuid,
                SettingsModuleGuid = PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<EntriesBrowseModuleEndpoints>(GridSupport.BrowseGridData),
                ExtraData = new BrowseItem.ExtraData { BlogCategory = blogCategory },
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    // filter by category
                    if (blogCategory != 0) {
                        filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(BlogEntry.CategoryIdentity), Operator = "==", Value = blogCategory, });
                    }
                    using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                        using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                            DataProviderGetRecords<BlogEntry> browseItems = await entryDP.GetItemsAsync(skip, take, sort, filters);
                            return new DataSourceResult {
                                Data = (from s in browseItems.Data select new BrowseItem(this, categoryDP, s)).ToList<object>(),
                                Total = browseItems.Total
                            };
                        }
                    }
                },
            };
        }

        public async Task<ActionInfo> RenderModuleAsync() {
            if (!int.TryParse(Manager.RequestQueryString["BlogCategory"], out int category)) category = 0;
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(category)
            };
            return await RenderAsync(model);
        }
    }
}