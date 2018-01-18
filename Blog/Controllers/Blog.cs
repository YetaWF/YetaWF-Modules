/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Controllers {

    public class BlogModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.BlogModule> {

        public BlogModuleController() { }

        public class Entry {

            public int Identity { get; set; }
            public int CategoryIdentity { get; set; }

            public MultiString Title { get; set; }

            [Caption("Author"), Description("The name of the blog author")]
            [UIHint("String"), ReadOnly, SuppressIfNotEqual("AuthorUrl", null)]
            public string Author { get; set; }

            [Caption("Author"), Description("The optional Url linking to the author's information")]
            [UIHint("Url"), ReadOnly, SuppressEmpty]
            public string AuthorUrl { get; set; }
            public string AuthorUrl_Text { get { return Author; } }

            [Caption("Date Published"), Description("The date this entry has been published")]
            [UIHint("Date"), ReadOnly]
            public DateTime DatePublished { get; set; }

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.IconsOnly), ReadOnly]
            public List<ModuleAction> Actions { get; set; }

            [Caption("Summary"), Description("The summary for this blog entry")]
            [UIHint("TextArea"), AdditionalMetadata("Encode", false), ReadOnly]
            public string DisplayableSummary { get; set; }

            [Caption("View"), Description("View the complete blog entry")]
            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks), ReadOnly]
            public ModuleAction ViewAction { get; set; }

            public Entry(BlogEntry data, EntryEditModule editMod, EntryDisplayModule dispMod) {
                ObjectSupport.CopyData(data, this);
                ViewAction = dispMod.GetAction_Display(data.Identity, ReadMore: data.Summary.ToString() != data.Text.ToString());
                Actions = new List<ModuleAction>();
                Actions.New(editMod.GetAction_Edit(null, data.Identity));
            }
        }

        public class DisplayModel {
            public int CategoryIdentity { get; set; }
            public List<Entry> BlogEntries { get; set; }
            public DateTime? StartDate { get; set; }
        }

        [AllowGet]
        public ActionResult Blog(DateTime? StartDate = null) {
            int category;
            Manager.TryGetUrlArg<int>("BlogCategory", out category);
            BlogConfigData config = BlogConfigDataProvider.GetConfig();
            using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
                List<DataProviderSortInfo> sort = new List<DataProviderSortInfo> {
                    new DataProviderSortInfo { Field = "DatePublished", Order = DataProviderSortInfo.SortDirection.Descending },
                };
                List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo>{
                    new DataProviderFilterInfo { Field = "Published", Operator = "==", Value = true },
                };
                if (category != 0)
                    filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "CategoryIdentity", Operator = "==", Value = category });
                DateTime sdShown = DateTime.MaxValue;
                if (StartDate != null) {
                    sdShown = ((DateTime)StartDate).Date;
                    if (sdShown < DateTime.UtcNow)
                        filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "DatePublished", Operator = "<=", Value = sdShown });
                    else
                        sdShown = DateTime.MaxValue;
                }
                int total;
                List<BlogEntry> data = dataProvider.GetItems(0, config.Entries, sort, filters, out total);
                if (data.Count == 0)
                    return new EmptyResult();

                string rssUrl = string.IsNullOrWhiteSpace(config.FeedMainUrl) ? Manager.CurrentSite.HomePageUrl : config.FeedMainUrl;
                Manager.LinkAltManager.AddLinkAltTag(AreaRegistration.CurrentPackage.AreaName, "application/rss+xml", config.FeedTitle, rssUrl);

                EntryEditModule editMod = new EntryEditModule();
                EntryDisplayModule dispMod = new EntryDisplayModule();
                DisplayModel model = new DisplayModel() {
                    BlogEntries = (from d in data select new Entry(d, editMod, dispMod)).ToList(),
                    CategoryIdentity = category,
                    StartDate = sdShown == DateTime.MaxValue ? null : (DateTime?) sdShown,
                };
                return View(model);
            }
        }
    }
}
