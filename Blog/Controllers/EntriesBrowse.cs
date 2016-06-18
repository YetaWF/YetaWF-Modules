/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Modules;

namespace YetaWF.Modules.Blog.Controllers {

    public class EntriesBrowseModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.EntriesBrowseModule> {

        public class BrowseItem {

            public class ExtraData {
                public int BlogCategory { get; set; }
            }

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
                    EntryDisplayModule dispMod = new EntryDisplayModule();
                    actions.New(dispMod.GetAction_Display(Identity), ModuleAction.ActionLocationEnum.GridLinks);
                    EntryEditModule editMod = new EntryEditModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, Identity), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_Remove(Identity), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Id"), Description("The id of this blog entry - used to uniquely identify this blog entry internally")]
            [UIHint("IntValue"), ReadOnly]
            public int Identity { get; set; }

            public int CategoryIdentity { get; set; }

            [Caption("Title"), Description("The title for this blog entry")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; }
            [Caption("Author"), Description("The name of the blog author")]
            [UIHint("String"), ReadOnly]
            public string Author { get; set; }

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
            [UIHint("String"), ReadOnly]
            public string Category { get; set; }

            private EntriesBrowseModule Module { get; set; }

            public BrowseItem(EntriesBrowseModule module, BlogCategoryDataProvider categoryDP, BlogEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [HttpGet]
        public ActionResult EntriesBrowse(int blogCategory = 0) {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("EntriesBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
                ExtraData = new BrowseItem.ExtraData { BlogCategory = blogCategory },
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EntriesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid, int blogCategory) {
            // filter by category
            if (blogCategory != 0) {
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "CategoryIdentity", Operator = "==", Value = blogCategory, });
            }
            using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                    int total;
                    List<BlogEntry> browseItems = entryDP.GetItems(skip, take, sort, filters, out total);
                    GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                    return GridPartialView(new DataSourceResult {
                        Data = (from s in browseItems select new BrowseItem(Module, categoryDP, s)).ToList<object>(),
                        Total = total
                    });
                }
            }
        }

        [HttpPost]
        [Permission("RemoveItems")]
        public ActionResult Remove(int blogEntry) {
            using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
                dataProvider.RemoveItem(blogEntry);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}