/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.IO;
using System.Text;
using System.Linq;
#if MVC6
using System.Net;
#else
using System.Web.Security.AntiXss;
#endif

namespace YetaWF.Modules.Pages.Scheduler {

    public class PageList {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public PageList() { }

        /// <summary>
        /// Create a page list for the current site.
        /// </summary>
        public async Task<string> CreateAsync() {

            StringBuilder sb = new StringBuilder();

            // Designed pages (only)
            List<Guid> pageGuids = await PageDefinition.GetDesignedGuidsAsync();
            List<string> pages = new List<string>();
            foreach (Guid guid in pageGuids) {
                PageDefinition page = await PageDefinition.LoadPageDefinitionAsync(guid);
                if (page != null) {
                    pages.Add(page.EvaluatedCanonicalUrl);
                }
            }
            pages = pages.OrderBy(s => s).ToList();
            foreach (string page in pages) {
                string url = Manager.CurrentSite.MakeFullUrl(page);
                sb.AppendLine($"{url}");
            }
            return sb.ToString();
        }
    }
}