/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Rss;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Text.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Text.Controllers {

    public class RssController : YetaWFController {

        public ActionResult RssFeed(Guid moduleGuid) {
            TextModule mod = (TextModule) ModuleDefinition.Load(moduleGuid, AllowNone: true);
            if (mod == null || !mod.Feed)
                throw new Error(this.__ResStr("noFeed", "The feed is no longer available"));

            ModuleAction action = mod.GetAction_RssFeed(mod.ModuleGuid);
            string url = action.GetCompleteUrl();

            SyndicationFeed feed;
            List<SyndicationItem> items = new List<SyndicationItem>();
            feed = new SyndicationFeed(mod.FeedTitle, mod.FeedSummary,
                string.IsNullOrWhiteSpace(mod.FeedMainUrl) ? new Uri(url) : new Uri(Manager.CurrentSite.MakeUrl(mod.FeedMainUrl)),
                items);

            action = mod.GetAction_RssDetail(mod.FeedDetailUrl, mod.ModuleGuid, mod.AnchorId);
            url = action.GetCompleteUrl();
            SyndicationItem sItem = new SyndicationItem(mod.Title, mod.Contents, new Uri(url));
            sItem.PublishDate = mod.FeedPublishDate??DateTime.MinValue;
            items.Add(sItem);

            if (mod.FeedImage != null)
                feed.ImageUrl = new Uri(Manager.CurrentSite.MakeUrl(ImageHelper.FormatUrl(YetaWF.Core.Modules.ModuleImageSupport.ImageType, null, mod.FeedImage, ForceHttpHandler: true)));
            feed.LastUpdatedTime = mod.FeedUpdateDate??DateTime.MinValue;

            return new RssResult(feed);
        }
    }
}
