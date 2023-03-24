/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Endpoints;

namespace YetaWF.Modules.Blog.Modules;

public class BlogModuleDataProvider : ModuleDefinitionDataProvider<Guid, BlogModule>, IInstallableModel { }

[ModuleGuid("{e1954fdc-0ccb-40bd-b018-c40dc792e894}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Navigation")]
public class BlogModule : ModuleDefinition {

    public BlogModule() {
        Title = this.__ResStr("modTitle", "Blog");
        Name = this.__ResStr("modName", "Blog");
        Description = this.__ResStr("modSummary", "Displays the main blog entry point with all or a single blog category. Add this module to a page as the main entry point to your overall blog tor for individual blog categories. It displays blog entries in short form. The maximum number of entries shown can be limited using the Blog Settings Module (Admin > Settings > Blog Settings, standard YetaWF site).");
        ShowTitleActions = true;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BlogModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public override async Task<List<ModuleAction>> RetrieveModuleActionsAsync() {
        List<ModuleAction> actions = await base.RetrieveModuleActionsAsync();
        actions.New(await GetAction_RssFeedAsync());
        return actions;
    }

    public async Task<ModuleAction?> GetAction_BlogAsync(string? url, int blogCategory = 0, DateTime? StartDate = null, int Count = 0) {
        QueryHelper query = new QueryHelper();
        if (string.IsNullOrWhiteSpace(url))
            url = await BlogConfigData.GetCategoryCanonicalNameAsync(blogCategory);
        else {
            url = ModulePermanentUrl;
            query.Add("BlogCategory", blogCategory.ToString());
        }
        string? date = null;
        if (StartDate != null) {
            query.Add("StartDate", StartDate.ToString());
            date = Formatting.Date_Month_YYYY((DateTime)StartDate);
            if (StartDate >= DateTime.UtcNow)
                date = this.__ResStr("latest", "{0} - Latest", date);
        } else {
            Count = 0;// must have a date for Count to be displayed
        }
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgsDict = query,
            Image = "#Display",
            LinkText = Count != 0 ? this.__ResStr("countLink", "{0} ({1})", date, Count) : this.__ResStr("displayLink", "Blog"),
            MenuText = Count != 0 ? this.__ResStr("countMenu", "{0} ({1})", date, Count) : this.__ResStr("displayText", "Blog"),
            Tooltip = Count != 0 ? this.__ResStr("countTooltip", "Display blog entries starting {0}", date, Count) : this.__ResStr("displayTooltip", "Display the blog"),
            Legend = Count != 0 ? this.__ResStr("countLegend", "Displays the blog entries starting {0}", date, Count) : this.__ResStr("displayLegend", "Displays the blog"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }
    public async Task<ModuleAction?> GetAction_RssFeedAsync(int blogCategory = 0) {
        BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
        if (!config.Feed) return null;
        //if (blogCategory == 0)
        //    manager.TryGetUrlArg<int>("BlogCategory", out blogCategory);
        object? qargsHR = null;
        if (blogCategory != 0) {
            using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                BlogCategory? data = await dataProvider.GetItemAsync(blogCategory);
                if (data != null)
                    qargsHR = new { Title = data.Category.ToString().Truncate(80) };
            }
        }
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(RssEndpoints), RssEndpoints.RssFeed),
            QueryArgs = new { BlogCategory = blogCategory, },
            QueryArgsHR = qargsHR,
            Image = await CustomIconAsync("RssFeed.png"),
            Style = ModuleAction.ActionStyleEnum.NewWindow,
            LinkText = this.__ResStr("rssLink", "RSS Feed"),
            MenuText = this.__ResStr("rssMenu", "RSS Feed"),
            Tooltip = this.__ResStr("rssTT", "Display the blog's RSS Feed"),
            Legend = this.__ResStr("rssLegend", "Displays the blog's RSS Feed"),
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }

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

    public async Task<ActionInfo> RenderModuleAsync(DateTime startDate, int blogCategory) {
        int category = blogCategory;

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
            if (startDate != DateTime.MinValue) {
                sdShown = startDate.Date;
                if (sdShown < DateTime.UtcNow)
                    filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(BlogEntry.DatePublished), Operator = "<=", Value = sdShown });
                else
                    sdShown = DateTime.MaxValue;
            }
            DataProviderGetRecords<BlogEntry> data = await dataProvider.GetItemsAsync(0, config.Entries, sort, filters);
            if (data.Data.Count == 0)
                return ActionInfo.Empty;

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
                StartDate = sdShown == DateTime.MaxValue ? null : (DateTime?)sdShown,
            };
            return await RenderAsync(model);
        }
    }
}
