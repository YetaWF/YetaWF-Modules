/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Rss;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Controllers {

    public class RssController : YetaWFController {

        // Windows RSS Publisher's Guide http://blogs.msdn.com/b/rssteam/archive/2005/08/02/publishersguide.aspx

        public async Task<ActionResult> RssFeed(int? blogCategory) {
            BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
            if (!config.Feed)
                throw new Error(this.__ResStr("noFeed", "The feed is no longer available"));

            int categoryIdentity = blogCategory ?? 0;
            BlogCategory category = null;
            if (categoryIdentity != 0) {
                using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                    category = await categoryDP.GetItemAsync(categoryIdentity);
                    if (!category.Syndicated)
                        throw new Error(this.__ResStr("noFeed", "The feed is no longer available"));
                }
            }

            using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
                List<DataProviderSortInfo> sort = new List<DataProviderSortInfo> {
                    new DataProviderSortInfo { Field = "DatePublished", Order = DataProviderSortInfo.SortDirection.Descending },
                };
                List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo> {
                    new DataProviderFilterInfo { Field = "Published", Operator = "==", Value = true },
                };
                if (categoryIdentity != 0)
                    filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "CategoryIdentity", Operator = "==", Value = categoryIdentity });
                DataProviderGetRecords<BlogEntry> data = await dataProvider.GetItemsAsync(0, 0, sort, filters);

                string url = string.IsNullOrWhiteSpace(config.FeedMainUrl) ? Manager.CurrentSite.HomePageUrl : config.FeedMainUrl;

                List<SyndicationItem> items = new List<SyndicationItem>();
                EntryDisplayModule dispMod = new EntryDisplayModule();

                DateTime lastUpdated = DateTime.MinValue;
                foreach (BlogEntry blogEntry in data.Data) {
                    if (categoryIdentity == 0) {
                        using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                            category = await categoryDP.GetItemAsync(blogEntry.CategoryIdentity);
                            if (!category.Syndicated)
                                continue;
                        }
                    }
                    ModuleAction viewAction = await dispMod.GetAction_DisplayAsync(blogEntry.Identity);
                    if (viewAction == null) continue;
                    SyndicationItem sItem = new SyndicationItem(blogEntry.Title.ToString(), blogEntry.Text, new Uri(viewAction.GetCompleteUrl()));
                    DateTime updDate = blogEntry.DateUpdated ?? blogEntry.DateCreated;
                    sItem.LastUpdatedTime = updDate;
                    if (!string.IsNullOrEmpty(category.SyndicationEmail))
                        sItem.Authors.Add(new SyndicationPerson(category.SyndicationEmail));
                    sItem.Categories.Add(new SyndicationCategory(category.Category.ToString()));
                    if (!string.IsNullOrEmpty(category.SyndicationCopyright.ToString()))
                        sItem.Copyright = new TextSyndicationContent(category.SyndicationCopyright.ToString());
                    sItem.PublishDate = blogEntry.DatePublished;
                    if (!string.IsNullOrEmpty(blogEntry.DisplayableSummary))
                        sItem.Summary = new TextSyndicationContent(blogEntry.DisplayableSummary);
                    lastUpdated = updDate > lastUpdated ? updDate : lastUpdated;

                    items.Add(sItem);
                }

                SyndicationFeed feed;
                if (categoryIdentity != 0) {
                    feed = new SyndicationFeed(category.Category.ToString(), category.Description.ToString(), new Uri(Manager.CurrentSite.MakeUrl(url)), items);
                } else {
                    feed = new SyndicationFeed(config.FeedTitle, config.FeedSummary, new Uri(Manager.CurrentSite.MakeUrl(url)), items);
                }
                if (config.FeedImage != null)
                    feed.ImageUrl = new Uri(Manager.CurrentSite.MakeUrl(ImageHTML.FormatUrl(BlogConfigData.ImageType, null, config.FeedImage))); //$$$ caching issue
                if (lastUpdated != DateTime.MinValue)
                    feed.LastUpdatedTime = lastUpdated;
                return new RssResult(feed);
            }
        }
    }
}
