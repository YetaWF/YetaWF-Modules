/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feed#License */

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using YetaWF.Core.Controllers;
using YetaWF.Core.IO;
using YetaWF.Core.Serializers;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Feed.Controllers {

    public class FeedModuleController : ControllerImpl<YetaWF.Modules.Feed.Modules.FeedModule> {

    public FeedModuleController() { }

        public class Entry {
            public string Title { get; set; } = null!;
            public string? Description { get; set; } = null!;
            public SerializableList<string> Links { get; set; } = null!;
            public SerializableList<Author> Authors { get; set; } = null!;

            public DateTime PublishDate { get; set; }
        }
        public class Author {
            public string? Email { get; set; }
            public string? Name { get; set; }
            public string? Url { get; set; }
        }

        public class DisplayModel {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime LastUpdate { get; set; }
            public SerializableList<Entry> Entries { get; set; } = null!;
            public string? Url { get; set; }
            public DateTime CacheExpires { get; set; }

            public DisplayModel() {
                Entries = new SerializableList<Entry>();
            }
        }

        [AllowGet]
        public async Task<ActionResult> Feed() {
            DisplayModel model;
            GetObjectInfo<DisplayModel> cacheInfo;
            using (ICacheDataProvider localCacheDP = YetaWF.Core.IO.Caching.GetLocalCacheProvider()) {
                cacheInfo = await localCacheDP.GetAsync<DisplayModel>(Module.CacheKey);
                if (!cacheInfo.Success || cacheInfo.RequiredData.CacheExpires < DateTime.UtcNow) {
                    model = new DisplayModel();
                    string url = Module.FeedUrl;
                    XmlReader reader = XmlReader.Create(url);
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    reader.Close();
                    model.Title = feed.Title.Text;
                    model.Description = feed.Description.Text;
                    model.LastUpdate = feed.LastUpdatedTime.DateTime;
                    model.Url = GetFeedUrl(feed.Links);
                    foreach (SyndicationItem item in feed.Items) {
                        string? desc = null;
                        if (string.IsNullOrWhiteSpace(desc))
                            if (item.Summary != null) desc = item.Summary.Text;
                        if (string.IsNullOrWhiteSpace(desc)) {
                            if (item.Content != null) {
                                TextSyndicationContent? tsc = item.Content as TextSyndicationContent;
                                desc = tsc?.Text;
                            }
                        }
                        Entry entry = new Entry {
                            Title = item.Title.Text,
                            Description = desc,
                            Links = new SerializableList<string>(),
                            Authors = new SerializableList<Author>(),
                            PublishDate = item.PublishDate.DateTime,
                        };
                        foreach (SyndicationLink l in item.Links) {
                            entry.Links.Add(l.Uri.ToString());
                        }
                        foreach (SyndicationPerson a in item.Authors) {
                            Author author = new Author();
                            author.Email = a.Email;
                            author.Name = a.Name;
                            if (a.Uri != null) author.Url = a.Uri.ToString();
                        }
                        model.Entries.Add(entry);
                    }
                    model.Entries = new SerializableList<Entry>((from e in model.Entries orderby e.PublishDate descending select e).Take(Module.NumEntries));
                    model.CacheExpires = DateTime.UtcNow.AddMinutes(5);// only retrieve news feed every 5 minutes

                    await localCacheDP.AddAsync(Module.CacheKey, model);
                } else {
                    model = cacheInfo.RequiredData;
                }

                if (model.Entries.Count == 0)
                    return new EmptyResult();

                return View(model);
            }
        }

        private string? GetFeedUrl(Collection<SyndicationLink> links) {
            SyndicationLink? link = (from l in links where l.RelationshipType == "alternate" select l).FirstOrDefault();
            if (link == null) link = links.FirstOrDefault();
            if (link == null) return null;
            return link.Uri.ToString();
        }
    }
}