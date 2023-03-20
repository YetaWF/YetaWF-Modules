/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Modules;

namespace YetaWF.Modules.Blog.Endpoints;

public class RssEndpoints : YetaWFEndpoints {

    internal const string RssFeed = "RssFeed";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(RssEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(RssEndpoints)));

        group.MapGet(RssFeed, async (HttpContext context, [FromQuery] Guid __ModuleGuid, int blogCategory) => {
            BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
            if (!config.Feed)
                throw new Error(__ResStr("noFeed", "The feed is no longer available"));

            int categoryIdentity = blogCategory;
            BlogCategory? category = null;
            if (categoryIdentity != 0) {
                using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                    category = await categoryDP.GetItemAsync(categoryIdentity);
                    if (category == null || !category.Syndicated)
                        throw new Error(__ResStr("noFeed", "The feed is no longer available"));
                }
            }

            using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
                List<DataProviderSortInfo> sort = new List<DataProviderSortInfo> {
                    new DataProviderSortInfo { Field = nameof(BlogEntry.DatePublished), Order = DataProviderSortInfo.SortDirection.Descending },
                };
                List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo> {
                    new DataProviderFilterInfo { Field = nameof(BlogEntry.Published), Operator = "==", Value = true },
                };
                if (categoryIdentity != 0)
                    filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(BlogEntry.CategoryIdentity), Operator = "==", Value = categoryIdentity });
                DataProviderGetRecords<BlogEntry> data = await dataProvider.GetItemsAsync(0, 0, sort, filters);

                string url = string.IsNullOrWhiteSpace(config.FeedMainUrl) ? Manager.CurrentSite.HomePageUrl : config.FeedMainUrl;

                List<SyndicationItem> items = new List<SyndicationItem>();
                EntryDisplayModule dispMod = new EntryDisplayModule();

                DateTime lastUpdated = DateTime.MinValue;
                foreach (BlogEntry blogEntry in data.Data) {
                    if (categoryIdentity == 0) {
                        using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                            category = await categoryDP.GetItemAsync(blogEntry.CategoryIdentity);
                            if (category == null || !category.Syndicated)
                                continue;
                        }
                    }
                    ModuleAction viewAction = await dispMod.GetAction_DisplayAsync(blogEntry.Identity);
                    if (viewAction == null) continue;
                    SyndicationItem sItem = new SyndicationItem(blogEntry.Title.ToString(), blogEntry.Text, new Uri(viewAction.GetCompleteUrl()));
                    DateTime updDate = blogEntry.DateUpdated ?? blogEntry.DateCreated;
                    sItem.LastUpdatedTime = updDate;
                    if (category != null) {
                        if (!string.IsNullOrEmpty(category.SyndicationEmail))
                            sItem.Authors.Add(new SyndicationPerson(category.SyndicationEmail));
                        sItem.Categories.Add(new SyndicationCategory(category.Category.ToString()));
                        if (!string.IsNullOrEmpty(category.SyndicationCopyright.ToString()))
                            sItem.Copyright = new TextSyndicationContent(category.SyndicationCopyright.ToString());
                    }
                    sItem.PublishDate = blogEntry.DatePublished;
                    if (!string.IsNullOrEmpty(blogEntry.DisplayableSummary))
                        sItem.Summary = new TextSyndicationContent(blogEntry.DisplayableSummary);
                    lastUpdated = updDate > lastUpdated ? updDate : lastUpdated;

                    items.Add(sItem);
                }

                SyndicationFeed feed;
                if (category != null) {
                    feed = new SyndicationFeed(category.Category.ToString(), category.Description.ToString(), new Uri(Manager.CurrentSite.MakeUrl(url)), items);
                } else {
                    feed = new SyndicationFeed(config.FeedTitle, config.FeedSummary, new Uri(Manager.CurrentSite.MakeUrl(url)), items);
                }
                if (config.FeedImage != null)
                    feed.ImageUrl = new Uri(Manager.CurrentSite.MakeUrl(ImageHTML.FormatUrl(BlogConfigData.ImageType, null, config.FeedImage))); //$$$ caching issue
                if (lastUpdated != DateTime.MinValue)
                    feed.LastUpdatedTime = lastUpdated;


                Utility.AllowSyncIO(context);

                Rss20FeedFormatter formatter = new Rss20FeedFormatter(feed);
                MemoryStream ms = new MemoryStream();
                using (XmlWriter writer = XmlWriter.Create(ms)) {
                    formatter.WriteTo(writer);
                }
                ms.Position = 0;
                return Results.Stream(ms, "application/rss+xml");
            }
        });
    }
}
