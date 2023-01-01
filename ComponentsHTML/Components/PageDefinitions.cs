/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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

    /// <summary>
    /// Base class for the PageDefinitions component implementation.
    /// </summary>
    public abstract class PageDefinitionsComponentBase : YetaWFComponent {

        internal const string TemplateName = "PageDefinitions";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Displays the model as a grid listing all the pages with detail information. The model cannot be null. If the model is an empty list, an empty grid is rendered.
    /// </summary>
    /// <example>
    /// [Category("Pages"), Caption("Pages"), Description("The pages where this module is used")]
    /// [UIHint("PageDefinitions"), ReadOnly]
    /// public List&lt;PageDefinition&gt; Pages {
    ///     get {
    ///         if (_pages == null)
    ///             _pages = PageDefinition.GetPagesFromModule(ModuleGuid);
    ///         return _pages;
    ///     }
    /// }
    /// private List&lt;PageDefinition&gt; _pages;
    /// </example>
    [UsesAdditional("Header", "bool", "true", "Defines whether the grid header is shown.")]
    public class PageDefinitionsDisplayComponent : PageDefinitionsComponentBase, IYetaWFComponent<List<PageDefinition>> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        internal class Entry {

            [Caption("Url"), Description("The Url used to identify this page")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; } = null!;

            [Caption("Title"), Description("The page title which will appear as title in the browser window")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; } = null!;

            [Caption("Description"), Description("The page description (not usually visible, entered by page designer, used for search keywords)")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Description { get; set; } = null!;

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
                AjaxUrl = Utility.UrlFor(typeof(PageDefinitionsController), nameof(PageDefinitionsController.PageDefinitionsDisplay_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(List<PageDefinition> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
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

            return hb.ToString();
        }
    }
}
