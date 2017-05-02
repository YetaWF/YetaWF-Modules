/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Pages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
#if MVC6
using System.Net;
#else
using System.Web.Security.AntiXss;
#endif

namespace YetaWF.Modules.Blog.Scheduler {

    public class NewsSiteMap : IScheduling {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public const string EventSiteMaps = "YetaWF.Blog: Build News Site Map";

        private const string NEWSSITEMAPFMT = "blognews-sitemap-{0}.xml";
        private const string NEWSSITEMAPTEMPFMT = "blognews-sitemap-{0}.temp.xml";

        public void RunItem(SchedulerItemBase evnt) {
            if (evnt.EventName != EventSiteMaps)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            Create(slow: true);
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
        public void Create(bool slow = false) {
            string file = GetTempFile();
            File.Delete(file);

            // header
            File.AppendAllText(file,
                "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n" +
                "<urlset xsi:schemaLocation=\"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns =\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\" >\r\n"
            );

            // Dynamic Urls in types
            DynamicUrlsImpl dynamicUrls = new DynamicUrlsImpl();
            List<Type> types = dynamicUrls.GetDynamicUrlTypes();
            foreach (Type type in types) {
                object obj = Activator.CreateInstance(type);
                ISiteMapDynamicUrls iSiteMap = obj as ISiteMapDynamicUrls;
                if (iSiteMap != null) {
                    BlogEntryDataProvider blogEntryDP = obj as BlogEntryDataProvider;
                    if (blogEntryDP != null) { // limit to blog entries
                        iSiteMap.FindDynamicUrls(AddNewsSiteMapPage, ValidForNewsSiteMap);
                    }
                }
            }
            // end
            File.AppendAllText(file,
                "</urlset>\r\n"
            );

            string finalFile = GetFile();
            File.Delete(finalFile);
            File.Move(file, finalFile);
        }

        private void AddNewsSiteMapPage(PageDefinition page, string url, DateTime? dateUpdated, PageDefinition.SiteMapPriorityEnum priority, PageDefinition.ChangeFrequencyEnum changeFrequency, object obj) {
            BlogEntry blogEntry = (BlogEntry)obj;
            AddUrl(GetTempFile(), page, blogEntry, url, dateUpdated);
        }
        private bool ValidForNewsSiteMap(PageDefinition page) {
            if (!string.IsNullOrWhiteSpace(page.RedirectToPageUrl)) // no redirected pages
                return false;
            if (!page.IsAuthorized_View_Anonymous()) // only pages that can be accessed by anonymous users
                return false;
            return true;
        }
        private void AddUrl(string file, PageDefinition page, BlogEntry blogEntry, string canonicalUrl, DateTime? lastMod) {
            if (!blogEntry.Published)
                return;
            if (blogEntry.DatePublished < DateTime.UtcNow.AddDays(-3))
                return; // too old
            canonicalUrl = Manager.CurrentSite.MakeFullUrl(canonicalUrl);
            if (!ValidForNewsSiteMap(page))
                return;
            foreach (LanguageData lang in MultiString.Languages) {
                string blogTitle, kwds, publishDate, blogCategory, langId;
                publishDate = XmlConvert.ToString(blogEntry.DatePublished, XmlDateTimeSerializationMode.Utc);
#if MVC6
                canonicalUrl = WebUtility.HtmlEncode(canonicalUrl);
                blogTitle = WebUtility.HtmlEncode(blogEntry.Title[lang.Id]);
                kwds = WebUtility.HtmlEncode(blogEntry.Keywords[lang.Id]);
                blogCategory = WebUtility.HtmlEncode(blogEntry.Category[lang.Id]);
                langId = WebUtility.HtmlEncode(lang.Id);
#else
                canonicalUrl = AntiXssEncoder.XmlEncode(canonicalUrl);
                blogTitle = AntiXssEncoder.XmlEncode(blogEntry.Title[lang.Id]);
                kwds = AntiXssEncoder.XmlEncode(blogEntry.Keywords[lang.Id]);
                blogCategory = AntiXssEncoder.XmlEncode(blogEntry.Category[lang.Id]);
                langId = AntiXssEncoder.XmlEncode(lang.Id);
#endif
                File.AppendAllText(file, string.Format(
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
                    "  </url>\r\n", canonicalUrl, blogCategory,  langId, publishDate, blogTitle, kwds
                ));
            }
        }
        /// <summary>
        /// Remove the news site map for the current site.
        /// </summary>
        public void Remove() {
            string file = GetFile();
            File.Delete(file);
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
}