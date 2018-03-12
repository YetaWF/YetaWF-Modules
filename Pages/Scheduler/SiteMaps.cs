/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
#if MVC6
using System.Net;
#else
using System.Web.Security.AntiXss;
#endif

namespace YetaWF.Modules.Pages.Scheduler {

    public class SiteMaps : IScheduling {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public const string EventSiteMaps = "YetaWF.Pages: Build Site Map";

        private const string SITEMAPFMT = "sitemap-{0}.xml";
        private const string SITEMAPTEMPFMT = "sitemap-{0}.temp.xml";

        private List<Guid> PagesFound = new List<Guid>();

        public Task RunItemAsync(SchedulerItemBase evnt) {
            if (evnt.EventName != EventSiteMaps)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            return CreateAsync(slow: true);
        }

        public SchedulerItemBase[] GetItems() {
            return new SchedulerItemBase[]{
                new SchedulerItemBase {
                    Name=this.__ResStr("eventName", "Build Site Map"),
                    Description = this.__ResStr("eventDesc", "Builds site maps"),
                    EventName = EventSiteMaps,
                    Enabled = true,
                    EnableOnStartup = true,
                    RunOnce = false,
                    Startup = false,
                    SiteSpecific = true,
                    Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Weeks, Value=1 },
                },
            };
        }

        public SiteMaps() { }

        /// <summary>
        /// Create a site map for the current site.
        /// </summary>
        public async Task CreateAsync(bool slow = false) {
            string file = GetTempFile();
            File.Delete(file);

            // header
            File.AppendAllText(file,
                "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n" +
                "<urlset xsi:schemaLocation=\"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns =\"http://www.sitemaps.org/schemas/sitemap/0.9\" >\r\n"
            );

            // Dynamic Urls in types
            DynamicUrlsImpl dynamicUrls = new DynamicUrlsImpl();
            List<Type> types = dynamicUrls.GetDynamicUrlTypes();
            foreach (Type type in types) {
                ISiteMapDynamicUrls iSiteMap = Activator.CreateInstance(type) as ISiteMapDynamicUrls;
                if (iSiteMap != null) {
                    await iSiteMap.FindDynamicUrlsAsync(AddSiteMapPageAsync, ValidForSiteMap);
                }
            }

            // search all designed modules that have dynamic urls
            List<DesignedModule> desMods = await DesignedModules.LoadDesignedModulesAsync();
            foreach (DesignedModule desMod in desMods) {
                ModuleDefinition mod = await ModuleDefinition.LoadAsync(desMod.ModuleGuid, AllowNone: true);
                if (mod != null) {
                    ISiteMapDynamicUrls iSiteMap = mod as ISiteMapDynamicUrls;
                    if (iSiteMap != null) {
                        await iSiteMap.FindDynamicUrlsAsync(AddSiteMapPageAsync, ValidForSiteMap);
                    }
                }
            }

            // Designed pages
            List<Guid> pages = PageDefinition.GetDesignedGuids();
            foreach (Guid pageGuid in pages) {
                PageDefinition page = await PageDefinition.LoadAsync(pageGuid);
                if (page == null)
                    continue;
                if (!PagesFound.Contains(page.PageGuid)) // don't include same again (this could be a page that generates dynamic Urls)
                    AddUrl(file, page, page.EvaluatedCanonicalUrl, page.Updated, page.SiteMapPriority, page.ChangeFrequency);
            }

            // end
            File.AppendAllText(file,
                "</urlset>\r\n"
            );

            string finalFile = GetFile();
            File.Delete(finalFile);
            File.Move(file, finalFile);
        }

        private Task AddSiteMapPageAsync(PageDefinition page, string url, DateTime? dateUpdated, PageDefinition.SiteMapPriorityEnum priority, PageDefinition.ChangeFrequencyEnum changeFrequency, object obj) {
            AddUrl(GetTempFile(), page, url, dateUpdated, priority, changeFrequency);
            return Task.CompletedTask;
        }
        private bool ValidForSiteMap(PageDefinition page) {
            if (!string.IsNullOrWhiteSpace(page.RedirectToPageUrl)) // no redirected pages
                return false;
            if (!page.IsAuthorized_View_Anonymous()) // only pages that can be accessed by anonymous users
                return false;
            if (page.SiteMapPriority == PageDefinition.SiteMapPriorityEnum.Excluded) // excluded from site map
                return false;
            return true;
        }
        private void AddUrl(string file, PageDefinition page, string canonicalUrl, DateTime? lastMod, PageDefinition.SiteMapPriorityEnum siteMapPriority, PageDefinition.ChangeFrequencyEnum changeFrequency) {
            if (!PagesFound.Contains(page.PageGuid)) // keep track of pages so we don't add it as a designed page in case it was dynamic
                PagesFound.Add(page.PageGuid);
            canonicalUrl = Manager.CurrentSite.MakeUrl(canonicalUrl, PagePageSecurity: page.PageSecurity);
            if (!ValidForSiteMap(page))
                return;
            string cf = GetChangeFrequencyText(changeFrequency);
            float prio = GetPriority(siteMapPriority);
            var w3clastMod = lastMod != null ? string.Format("    <lastmod>{0}</lastmod>\r\n", XmlConvert.ToString((DateTime)lastMod, XmlDateTimeSerializationMode.Utc)) : "";
#if MVC6
            canonicalUrl = WebUtility.HtmlEncode(canonicalUrl);
#else
            canonicalUrl = AntiXssEncoder.XmlEncode(canonicalUrl);
#endif
            File.AppendAllText(file, string.Format(
                "  <url>\r\n" +
                "    <loc>{0}</loc>\r\n" +
                "{1}" +
                "    <changefreq>{2}</changefreq>\r\n" +
                "    <priority>{3}</priority>\r\n" +
                "  </url>\r\n", canonicalUrl, w3clastMod, cf, prio)
            );
        }

        private float GetPriority(PageDefinition.SiteMapPriorityEnum siteMapPriority) {
            if (siteMapPriority == PageDefinition.SiteMapPriorityEnum.Default) {
                siteMapPriority = (PageDefinition.SiteMapPriorityEnum)Manager.CurrentSite.SiteMapPriority;
                if ((int)siteMapPriority == 0)
                    siteMapPriority = PageDefinition.SiteMapPriorityEnum.Medium;
            }
            int value = (int)siteMapPriority;
            float prio = ((float)value) / 100;
            prio = (float)Math.Max(0, Math.Min(1.0, prio));
            return prio;
        }

        private string GetChangeFrequencyText(PageDefinition.ChangeFrequencyEnum changeFrequency) {
            if (changeFrequency == PageDefinition.ChangeFrequencyEnum.Default)
                changeFrequency = (PageDefinition.ChangeFrequencyEnum)Manager.CurrentSite.DefaultChangeFrequency;
            switch (changeFrequency) {
                case PageDefinition.ChangeFrequencyEnum.Always: return "always";
                case PageDefinition.ChangeFrequencyEnum.Daily: return "daily";
                case PageDefinition.ChangeFrequencyEnum.Hourly: return "hourly";
                case PageDefinition.ChangeFrequencyEnum.Monthly: return "monthly";
                case PageDefinition.ChangeFrequencyEnum.Never: return "never";
                default:
                case PageDefinition.ChangeFrequencyEnum.Weekly: return "weekly";
                case PageDefinition.ChangeFrequencyEnum.Yearly: return "yearly";
            }
        }

        /// <summary>
        /// Remove the site map for the current site.
        /// </summary>
        public void Remove() {
            string file = GetFile();
            File.Delete(file);
        }

        /// <summary>
        /// Returns the site map file name
        /// </summary>
        /// <returns></returns>
        public string GetSiteMapFileName() {
            return GetFile();
        }

        private string GetTempFile() {
            return Path.Combine(YetaWFManager.RootFolder, string.Format(SITEMAPTEMPFMT, Manager.CurrentSite.SiteDomain.ToLower()));
        }
        private string GetFile() {
            return Path.Combine(YetaWFManager.RootFolder, string.Format(SITEMAPFMT, Manager.CurrentSite.SiteDomain.ToLower()));
        }
    }
}