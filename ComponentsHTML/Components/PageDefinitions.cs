/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class PageDefinitionsComponentBase : YetaWFComponent {

        public const string TemplateName = "PageDefinitions";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class PageDefinitionsDisplayComponent : PageDefinitionsComponentBase, IYetaWFComponent<List<PageDefinition>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class Entry {

            [Caption("Url"), Description("The Url used to identify this page")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; }

            [Caption("Title"), Description("The page title which will appear as title in the browser window")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; }

            [Caption("Description"), Description("The page description (not usually visible, entered by page designer, used for search keywords)")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Description { get; set; }

            public Entry() { }
            public Entry(PageDefinition m) {
                ObjectSupport.CopyData(m, this);
            }
        }
        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                AjaxUrl = YetaWFManager.UrlFor(typeof(PageDefinitionsController), nameof(PageDefinitionsController.PageDefinitionsDisplay_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        public async Task<YHtmlString> RenderAsync(List<PageDefinition> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                DataSourceResult data = new DataSourceResult {
                    Data = (from m in model select new Entry(m)).ToList<object>(),
                    Total = model.Count,
                };
                return Task.FromResult(data);
            };

            hb.Append($@"
<div class='yt_pagedefinitions t_display'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToYHtmlString();
        }
    }
}
