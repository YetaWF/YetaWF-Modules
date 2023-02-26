/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Pages.Support;

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
            PageDefinition? page = await PageDefinition.LoadPageDefinitionAsync(guid);
            if (page != null)
                pages.Add(page.EvaluatedCanonicalUrl!);
        }
        pages = pages.OrderBy(s => s).ToList();
        foreach (string page in pages)
            sb.AppendLine(Manager.CurrentSite.MakeFullUrl(page));
        return sb.ToString();
    }
}