/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feed#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Core.Support;
using YetaWF.Core.Pages;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Linq;
using System.Xml;

namespace YetaWF.Modules.Feed.Modules;

// For documentation about Google Feed please see https://developers.google.com/feed/

public class FeedModuleDataProvider : ModuleDefinitionDataProvider<Guid, FeedModule>, IInstallableModel { }

[ModuleGuid("{04c32e25-f9bf-4baf-9602-3c929ce77790}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class FeedModule : ModuleDefinition {

    public FeedModule() : base() {
        Title = this.__ResStr("modTitle", "News Feed");
        Name = this.__ResStr("modName", "News Feed");
        Description = this.__ResStr("modSummary", "Displays the defined news feed. Use the module's Module Settings to define the desired news feed. Sample News Feed Modules can be accessed at Tests > News Feed (standard YetaWF site).");
        FeedUrl = "https://YetaWF.com/NewsFeed";
        Interval = 5;
        NumEntries = 10;
        WantSearch = false;
        WantFocus = false;
        Print = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new FeedModuleDataProvider(); }

    [Category("General"), Caption("News Feed Url"), Description("The Url providing the news feed")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Remote), StringLength(Globals.MaxUrl), UrlValidation, Required, Trim]
    public string FeedUrl { get; set; }

    [Category("General"), Caption("News Entries"), Description("The maximum number of news entries displayed (between 2 and 50)")]
    [UIHint("IntValue2"), Required, Range(2, 50)]
    public int NumEntries { get; set; }

    [Category("General"), Caption("Interval"), Description("The number of seconds after which the primary news item is replaced with the next item - specify 0 to disable")]
    [UIHint("IntValue2"), Required, Range(0, 30)]
    public int Interval { get; set; }

    [Category("Variables"), Caption("Cache Key"), Description("The name used to cache the news information")]
    [UIHint("String"), ReadOnly]
    public string CacheKey { get { return YetaWF.Modules.Feed.AreaRegistration.CurrentPackage.AreaName + "_" + ModuleGuidName + "_Rss_" + Manager.CurrentSite.Identity; } }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public async Task<ModuleAction> GetAction_NewsAsync() {
        return new ModuleAction() {
            Url = FeedUrl,
            Image = await CustomIconAsync("Feed.png"),
            LinkText = Title,
            MenuText = Title,
            Tooltip = this.__ResStr("displayTooltip", "Display the {0} news feed", Title),
            Legend = this.__ResStr("displayLegend", "Displays the {0} news feed", Title),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }
    public override Task ModuleSavingAsync() {
        return RemoveCachedInfoAsync();// whenever the module is saved, we remove the cached information
    }
    public override Task ModuleRemovingAsync() {
        return RemoveCachedInfoAsync();// whenever the module is removed, we remove the cached information
    }
    private async Task RemoveCachedInfoAsync() {
        await using (ICacheDataProvider localCacheDP = YetaWF.Core.IO.Caching.GetLocalCacheProvider()) {
            await localCacheDP.RemoveAsync<DisplayModel>(CacheKey);
        }
    }

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

    public async Task<ActionInfo> RenderModuleAsync() {
        DisplayModel model;
        GetObjectInfo<DisplayModel> cacheInfo;
        await using (ICacheDataProvider localCacheDP = YetaWF.Core.IO.Caching.GetLocalCacheProvider()) {
            cacheInfo = await localCacheDP.GetAsync<DisplayModel>(CacheKey);
            if (!cacheInfo.Success || cacheInfo.RequiredData.CacheExpires < DateTime.UtcNow) {
                model = new DisplayModel();
                string url = FeedUrl;
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
                model.Entries = new SerializableList<Entry>((from e in model.Entries orderby e.PublishDate descending select e).Take(NumEntries));
                model.CacheExpires = DateTime.UtcNow.AddMinutes(5);// only retrieve news feed every 5 minutes

                await localCacheDP.AddAsync(CacheKey, model);
            } else {
                model = cacheInfo.RequiredData;
            }

            if (model.Entries.Count == 0)
                return ActionInfo.Empty;

            return await RenderAsync(model);
        }
    }

    private string? GetFeedUrl(Collection<SyndicationLink> links) {
        SyndicationLink? link = (from l in links where l.RelationshipType == "alternate" select l).FirstOrDefault();
        if (link == null) link = links.FirstOrDefault();
        if (link == null) return null;
        return link.Uri.ToString();
    }
}
