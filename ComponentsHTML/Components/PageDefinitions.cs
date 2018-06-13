using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class PageDefinitionsComponentBase : YetaWFComponent {

        public const string TemplateName = "PageDefinitions";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class PageDefinitionsDisplayComponent : PageDefinitionsComponentBase, IYetaWFComponent<List<PageDefinition>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class GridDisplay {

            public GridDisplay(PageDefinition m) {
                ObjectSupport.CopyData(m, this);
            }

            [Caption("Url"), Description("The Url used to identify this page")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; }

            [Caption("Title"), Description("The page title which will appear as title in the browser window")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; }

            [Caption("Description"), Description("The page description (not usually visible, entered by page designer, used for search keywords)")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Description { get; set; }
        }
        protected static Task<DataSourceResult> GetDataSourceResultDisplayAsync(List<PageDefinition> model) {

            DataSourceResult data = new DataSourceResult {
                Data = (from m in model select new GridDisplay(m)).ToList<object>(),
                Total = model.Count,
            };
            return Task.FromResult(data);
        }
        public async Task<YHtmlString> RenderAsync(List<PageDefinition> model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div class='yt_pagedefinitions t_edit'>");

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridDisplay),
                    Data = await GetDataSourceResultDisplayAsync(model),
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = true,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            hb.Append($"</div>");
            return hb.ToYHtmlString();
        }
    }
}
