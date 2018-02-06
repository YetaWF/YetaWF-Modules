/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Pages.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.PageDefinitionDataProvider), typeof(PageDefinitionDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.UnifiedSetDataProvider), typeof(UnifiedSetDataProvider));
        }
        class PageDefinitionDataProvider : FileDataProvider<Guid, PageDefinition>, IPageDefinitionIOMode {

            public PageDefinitionDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }

            public DesignedPagesDictionaryByUrl GetDesignedPages() {
                DesignedPagesDictionaryByUrl byUrl = new DesignedPagesDictionaryByUrl();
                List<Guid> pageGuids = FileDataProvider<Guid, PageDefinition>.GetListOfKeys(BaseFolder);
                foreach (var pageGuid in pageGuids) {
                    PageDefinition page = Get(pageGuid);
                    if (page == null)
                        throw new InternalError("No PageDefinition for guid {0}", pageGuid);
                    PageDefinition.DesignedPage desPage = new PageDefinition.DesignedPage() { Url = page.Url, PageGuid = page.PageGuid };
                    byUrl.Add(page.Url.ToLower(), desPage);
                }
                return byUrl;
            }
            public List<PageDefinition> GetPagesFromModule(Guid moduleGuid) {
                int total;
                List<PageDefinition> pages = GetRecords(0, 0, null, null, out total);
                List<PageDefinition> pagesWithModule = new List<PageDefinition>();
                foreach (PageDefinition page in pages) {
                    PageDefinition p = (from m in page.ModuleDefinitions where m.ModuleGuid == moduleGuid select page).FirstOrDefault();
                    if (p != null)
                        pagesWithModule.Add(p);
                }
                return pagesWithModule.OrderBy(p => p.Url).ToList();
            }
        }
        class UnifiedSetDataProvider : FileDataProvider<Guid, UnifiedSetData> {
            public UnifiedSetDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
        }
    }
}
