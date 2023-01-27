/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Endpoints;
using YetaWF.Modules.Blog.Modules;

namespace YetaWF.Modules.Blog.Controllers {

    public class EntriesBrowseModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.EntriesBrowseModule> {

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

        internal static GridDefinition GetGridModel(ModuleDefinition module, int blogCategory) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
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
                                Data = (from s in browseItems.Data select new BrowseItem((EntriesBrowseModule)module, categoryDP, s)).ToList<object>(),
                                Total = browseItems.Total
                            };
                        }
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult EntriesBrowse(int blogCategory = 0) {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module, blogCategory)
            };
            return View(model);
        }
    }
}