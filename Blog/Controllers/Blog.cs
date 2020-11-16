/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Modules;

namespace YetaWF.Modules.Blog.Controllers {

    public class BlogModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.BlogModule> {

        public BlogModuleController() { }

        public class Entry {

            public int Identity { get; set; }
            public int CategoryIdentity { get; set; }

            public MultiString Title { get; set; } = null!;

            [Caption("Author"), Description("The name of the blog author")]
            [UIHint("String"), ReadOnly, SuppressIfNot("AuthorUrl", null)]
            public string? Author { get; set; }

            [Caption("Author"), Description("The optional Url linking to the author's information")]
            [UIHint("Url"), ReadOnly, SuppressEmpty]
            public string? AuthorUrl { get; set; }
            public string? AuthorUrl_Text { get { return Author; } }

            [Caption("Date Published"), Description("The date this entry has been published")]
            [UIHint("Date"), ReadOnly]
            public DateTime DatePublished { get; set; }

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.IconsOnly), ReadOnly]
            public List<ModuleAction> Actions { get; set; }

            [Caption("Summary"), Description("The summary for this blog entry")]
            [UIHint("TextArea"), AdditionalMetadata("Encode", false), ReadOnly]
            public string? DisplayableSummary { get; set; }

            [Caption("View"), Description("View the complete blog entry")]
            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks), ReadOnly]
            public ModuleAction? ViewAction { get; set; }

            public Entry(BlogEntry data, EntryEditModule editMod, EntryDisplayModule? dispMod, ModuleAction? editAction, ModuleAction? viewAction) {
                ObjectSupport.CopyData(data, this);
                ViewAction = viewAction;
                Actions = new List<ModuleAction>();
                Actions.New(editAction);
            }
        }

        public class DisplayModel {
            public int CategoryIdentity { get; set; }
            public List<Entry> BlogEntries { get; set; } = null!;
            public DateTime? StartDate { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> Blog(DateTime? StartDate = null) {
            int category;
            Manager.TryGetUrlArg<int>("BlogCategory", out category);
            BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
            using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
                List<DataProviderSortInfo> sort = new List<DataProviderSortInfo> {
                    new DataProviderSortInfo { Field = nameof(BlogEntry.DatePublished), Order = DataProviderSortInfo.SortDirection.Descending },
                };
                List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo>{
                    new DataProviderFilterInfo { Field = nameof(BlogEntry.Published), Operator = "==", Value = true },
                };
                if (category != 0)
                    filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(BlogEntry.CategoryIdentity), Operator = "==", Value = category });
                DateTime sdShown = DateTime.MaxValue;
                if (StartDate != null) {
                    sdShown = ((DateTime)StartDate).Date;
                    if (sdShown < DateTime.UtcNow)
                        filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(BlogEntry.DatePublished), Operator = "<=", Value = sdShown });
                    else
                        sdShown = DateTime.MaxValue;
                }
                DataProviderGetRecords<BlogEntry> data = await dataProvider.GetItemsAsync(0, config.Entries, sort, filters);
                if (data.Data.Count == 0)
                    return new EmptyResult();

                string rssUrl = string.IsNullOrWhiteSpace(config.FeedMainUrl) ? Manager.CurrentSite.HomePageUrl : config.FeedMainUrl;
                Manager.LinkAltManager.AddLinkAltTag(AreaRegistration.CurrentPackage.AreaName, "application/rss+xml", config.FeedTitle, rssUrl);

                EntryEditModule editMod = new EntryEditModule();
                EntryDisplayModule dispMod = new EntryDisplayModule();

                List<Entry> list = new List<Entry>();
                foreach (BlogEntry d in data.Data) {
                    ModuleAction? viewAction = await dispMod.GetAction_DisplayAsync(d.Identity, ReadMore: d.Summary != d.Text);
                    ModuleAction? editAction = await editMod.GetAction_EditAsync(null, d.Identity);
                    list.Add(new Entry(d, editMod, dispMod, editAction, viewAction));
                }
                DisplayModel model = new DisplayModel() {
                    BlogEntries = list,
                    CategoryIdentity = category,
                    StartDate = sdShown == DateTime.MaxValue ? null : (DateTime?) sdShown,
                };
                return View(model);
            }
        }
    }
}
