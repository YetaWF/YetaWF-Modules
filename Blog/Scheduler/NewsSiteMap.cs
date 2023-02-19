/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using YetaWF.Core.IO;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Pages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Scheduler;

public class NewsSiteMap : IScheduling {

    private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

    public const string EventSiteMaps = "YetaWF.Blog: Build News Site Map";

    private const string NEWSSITEMAPFMT = "blognews-sitemap-{0}.xml";
    private const string NEWSSITEMAPTEMPFMT = "blognews-sitemap-{0}.temp.xml";

    public async Task RunItemAsync(SchedulerItemBase evnt) {
        if (evnt.EventName != EventSiteMaps)
            throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
        await CreateAsync(slow: true);
    }

    public SchedulerItemBase[] GetItems() {
        return new SchedulerItemBase[]{
            new SchedulerItemBase {
                Name=this.__ResStr("eventName", "Build a Blog Based News Site Map"),
                Description = this.__ResStr("eventDesc", "Builds a blog based news site map"),
                EventName = EventSiteMaps,
                Enabled = true,
                EnableOnStartup = true,
                RunOnce = false,
                Startup = false,
                SiteSpecific = true,
                Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Days, Value=1 },
            },
        };
    }

    public NewsSiteMap() { }

    /// <summary>
    /// Create a blog based news site map for the current site.
    /// </summary>
    public async Task CreateAsync(bool slow = false) {
        string file = GetTempFile();
        await FileSystem.FileSystemProvider.DeleteFileAsync(file);

        // header
        await FileSystem.FileSystemProvider.AppendAllTextAsync(file,
            "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n" +
            "<urlset xsi:schemaLocation=\"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns =\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\" >\r\n"
        );

        // Dynamic Urls in types
        DynamicUrlsImpl dynamicUrls = new DynamicUrlsImpl();
        List<Type> types = dynamicUrls.GetDynamicUrlTypes();
        foreach (Type type in types) {
            object obj = Activator.CreateInstance(type)!;
            ISiteMapDynamicUrls? iSiteMap = obj as ISiteMapDynamicUrls;
            if (iSiteMap != null) {
                BlogEntryDataProvider? blogEntryDP = obj as BlogEntryDataProvider;
                if (blogEntryDP != null) { // limit to blog entries
                    await iSiteMap.FindDynamicUrlsAsync(AddNewsSiteMapPageAsync, ValidForNewsSiteMap);
                }
            }
        }
        // end
        await FileSystem.FileSystemProvider.AppendAllTextAsync(file,
            "</urlset>\r\n"
        );

        string finalFile = GetFile();
        await FileSystem.FileSystemProvider.DeleteFileAsync(finalFile);
        await FileSystem.FileSystemProvider.MoveFileAsync(file, finalFile);
    }

    private async Task AddNewsSiteMapPageAsync(PageDefinition page, string url, DateTime? dateUpdated, PageDefinition.SiteMapPriorityEnum priority, PageDefinition.ChangeFrequencyEnum changeFrequency, object obj) {
        BlogEntry blogEntry = (BlogEntry)obj;
        await AddUrlAsync(GetTempFile(), page, blogEntry, url, dateUpdated);
    }
    private bool ValidForNewsSiteMap(PageDefinition page) {
        if (!string.IsNullOrWhiteSpace(page.RedirectToPageUrl)) // no redirected pages
            return false;
        if (!page.IsAuthorized_View_Anonymous()) // only pages that can be accessed by anonymous users
            return false;
        return true;
    }
    private async Task AddUrlAsync(string file, PageDefinition page, BlogEntry blogEntry, string canonicalUrl, DateTime? lastMod) {
        if (!blogEntry.Published)
            return;
        if (blogEntry.DatePublished < DateTime.UtcNow.AddDays(-3))
            return; // too old
        canonicalUrl = Manager.CurrentSite.MakeUrl(canonicalUrl, PagePageSecurity: page.PageSecurity);
        if (!ValidForNewsSiteMap(page))
            return;
        foreach (LanguageData lang in MultiString.Languages) {
            string blogTitle, kwds, publishDate, blogCategory, langId;
            publishDate = XmlConvert.ToString(blogEntry.DatePublished, XmlDateTimeSerializationMode.Utc);
            canonicalUrl = WebUtility.HtmlEncode(canonicalUrl);
            blogTitle = WebUtility.HtmlEncode(blogEntry.Title[lang.Id]);
            kwds = WebUtility.HtmlEncode(blogEntry.Keywords[lang.Id]);
            blogCategory = WebUtility.HtmlEncode((await blogEntry.GetCategoryAsync())[lang.Id]);
            langId = WebUtility.HtmlEncode(lang.Id);
            await FileSystem.FileSystemProvider.AppendAllTextAsync(file, string.Format(
                "  <url>\r\n" +
                "    <loc>{0}</loc>\r\n" +
                "    <news:news>\r\n" +
                "      <news:publication>\r\n" +
                "        <news:name>{1}</news:name>\r\n" +
                "        <news:language>{2}</news:language>\r\n" +
                "      </news:publication>\r\n" +
                "      <news:genres>Blog</news:genres>\r\n" +
                "      <news:publication_date>{3}</news:publication_date>\r\n" +
                "      <news:title>{4}</news:title>\r\n" +
                "      <news:keywords>{5}</news:keywords>\r\n" +
                "    </news:news>\r\n" +
                "  </url>\r\n", canonicalUrl, blogCategory, langId, publishDate, blogTitle, kwds
            ));
        }
    }
    /// <summary>
    /// Remove the news site map for the current site.
    /// </summary>
    public async Task RemoveAsync() {
        string file = GetFile();
        await FileSystem.FileSystemProvider.DeleteFileAsync(file);
    }
    /// <summary>
    /// Returns the news site map file name
    /// </summary>
    /// <returns></returns>
    public string GetNewsSiteMapFileName() {
        return GetFile();
    }
    private string GetTempFile() {
        return Path.Combine(YetaWFManager.RootFolder, string.Format(NEWSSITEMAPTEMPFMT, Manager.CurrentSite.SiteDomain.ToLower()));
    }
    private string GetFile() {
        return Path.Combine(YetaWFManager.RootFolder, string.Format(NEWSSITEMAPFMT, Manager.CurrentSite.SiteDomain.ToLower()));
    }
}
