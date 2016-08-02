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

    public class CategoriesBrowseModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CategoriesBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

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
            public MultiString Category { get; set; }

            [Caption("Description"), Description("The description of the blog category - the category's description is shown at the top of each blog entry to describe your blog")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Description { get; set; }

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
            public string SyndicationEmail { get; set; }

            [Caption("Syndication Copyright"), Description("The optional copyright information shown when the blog is accessed by news readers")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString SyndicationCopyright { get; set; }

            private CategoriesBrowseModule Module { get; set; }

            public BrowseItem(CategoriesBrowseModule module, BlogCategory data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [HttpGet]
        public ActionResult CategoriesBrowse() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("CategoriesBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoriesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                int total;
                List<BlogCategory> browseItems = dataProvider.GetItems(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in browseItems select new BrowseItem(Module, s)).ToList<object>(),
                    Total = total
                });
            }
        }

        [HttpPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public ActionResult Remove(int blogCategory) {
            using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                dataProvider.RemoveItem(blogCategory);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}