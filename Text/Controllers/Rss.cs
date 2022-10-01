/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Rss;
using YetaWF.Modules.Text.Modules;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Text.Controllers {

    public class RssController : YetaWFController {

        public async Task<ActionResult> RssFeed(Guid moduleGuid) {
            TextModule? mod = (TextModule?)await ModuleDefinition.LoadAsync(moduleGuid, AllowNone: true);
            if (mod == null || !mod.Feed)
                throw new Error(this.__ResStr("noFeed", "The feed is no longer available"));

            ModuleAction action = await mod.GetAction_RssFeedAsync(mod.ModuleGuid) ?? throw new InternalError($"Rss Feed URL not defined");
            string url = action.GetCompleteUrl();

            SyndicationFeed feed;
            List<SyndicationItem> items = new List<SyndicationItem>();
            feed = new SyndicationFeed(mod.FeedTitle, mod.FeedSummary,
                string.IsNullOrWhiteSpace(mod.FeedMainUrl) ? new Uri(url) : new Uri(Manager.CurrentSite.MakeUrl(mod.FeedMainUrl)),
                items);

            action = await mod.GetAction_RssDetailAsync(mod.FeedDetailUrl, mod.ModuleGuid, mod.AnchorId) ?? throw new InternalError($"Rss Feed Detail URL not defined");
            url = action.GetCompleteUrl();
            SyndicationItem sItem = new SyndicationItem(mod.Title, mod.Contents, new Uri(url));
            sItem.PublishDate = mod.FeedPublishDate ?? DateTime.MinValue;
            items.Add(sItem);

            if (mod.FeedImage != null)
                feed.ImageUrl = new Uri(Manager.CurrentSite.MakeUrl(ImageHTML.FormatUrl(YetaWF.Core.Modules.ModuleImageSupport.ImageType, null, mod.FeedImage, CacheBuster:mod.DateUpdated.Ticks.ToString())));
            feed.LastUpdatedTime = mod.FeedUpdateDate ?? DateTime.MinValue;

            return new RssResult(feed);
        }
    }
}
