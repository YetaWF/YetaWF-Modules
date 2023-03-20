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
using YetaWF.Core.Endpoints;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Text.Modules;

namespace YetaWF.Modules.Text.Endpoints;

public class RssEndpoints : YetaWFEndpoints {

    internal const string RssFeed = "RssFeed";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(RssEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(RssEndpoints)));

        group.MapGet($"{RssFeed}/{{*rest}}", async (HttpContext context, [FromQuery] Guid __ModuleGuid, string rest) => {
            TextModule module = await GetModuleAsync<TextModule>(__ModuleGuid);
            if (!module.Feed)
                throw new Error(__ResStr("noFeed", "The feed is no longer available"));

            ModuleAction action = await module.GetAction_RssFeedAsync() ?? throw new InternalError($"Rss Feed Url not defined");
            string url = action.GetCompleteUrl();

            SyndicationFeed feed;
            List<SyndicationItem> items = new List<SyndicationItem>();
            feed = new SyndicationFeed(module.FeedTitle, module.FeedSummary,
                string.IsNullOrWhiteSpace(module.FeedMainUrl) ? new Uri(url) : new Uri(Manager.CurrentSite.MakeUrl(module.FeedMainUrl)),
                items);

            action = await module.GetAction_RssDetailAsync() ?? throw new InternalError($"Rss Feed Detail Url not defined");
            url = action.GetCompleteUrl();
            SyndicationItem sItem = new SyndicationItem(module.Title, module.Contents, new Uri(url));
            sItem.PublishDate = module.FeedPublishDate ?? DateTime.MinValue;
            items.Add(sItem);

            if (module.FeedImage != null)
                feed.ImageUrl = new Uri(Manager.CurrentSite.MakeUrl(ImageHTML.FormatUrl(YetaWF.Core.Modules.ModuleImageSupport.ImageType, null, module.FeedImage, CacheBuster: module.DateUpdated.Ticks.ToString())));
            feed.LastUpdatedTime = module.FeedUpdateDate ?? DateTime.MinValue;

            Utility.AllowSyncIO(context);

            Rss20FeedFormatter formatter = new Rss20FeedFormatter(feed);
            MemoryStream ms = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(ms)) {
                formatter.WriteTo(writer);
            }
            ms.Position = 0;
            return Results.Stream(ms, "application/rss+xml");
        });
    }
}
