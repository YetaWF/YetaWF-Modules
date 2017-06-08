/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Pages.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.Pages.Views.Shared {

    public class ListOfLocalPagesHelper<TModel> : RazorTemplate<TModel> { }

    public static class ListOfLocalPagesHelper {

        public class GridModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        public class NewModel {
            [Caption("Page"), Description("Please select a page and click Add to add it to the Unified Page Set")]
            [UIHint("Url"), StringLength(Globals.MaxUrl), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), Trim]
            public string NewValue { get; set; }
        }

        [Trim]
        public class GridEntryEdit {

            public GridEntryEdit() { }

            [Caption("Delete"), Description("Click to delete this page")]
            [UIHint("GridDeleteEntry")]
            public int DeleteMe { get; set; }

            [Caption("Page"), Description("Shows all pages part of this Unified Page Set")]
            [UIHint("Url"), ReadOnly]
            public string Url { get { return __Value; } set { __Value = value; } }

            [UIHint("Text80"), StringLength(Globals.MaxUrl), Required, Trim]
            public string __Value { get; set; }
            [UIHint("Raw"), ReadOnly]
            public string __TextKey { get { return __Value; } }
            [UIHint("Raw"), ReadOnly]
            public string __TextDisplay { get { return __Value; } }

            public GridEntryEdit(string url) {
                Url = url;
            }
        }
#if MVC6
        public static HtmlString RenderListOfLocalPages<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, string id, List<string> model)
#else
        public static HtmlString RenderListOfLocalPages<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string id, List<string> model)
#endif
        {
            List<GridEntryEdit> list = new List<GridEntryEdit>();
            if (model != null)
                list = (from u in model select new GridEntryEdit(u)).ToList();

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel ListOfLocalPagesModel = new GridModel() {
                GridDef = new GridDefinition() {
                    Id = id,
                    RecordType = typeof(GridEntryEdit),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 0,
                    ShowHeader = header,
                    ReadOnly = false,
                    CanAddOrDelete = true,
                    DeleteProperty = "__TextKey",
                    DisplayProperty = "__TextDisplay"
                }
            };
#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => ListOfLocalPagesModel.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => ListOfLocalPagesModel.GridDef);
#endif
        }
#if MVC6
        public static HtmlString RenderListOfLocalPagesAddNew<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, List<string> model)
#else
        public static HtmlString RenderListOfLocalPagesAddNew<TModel>(this HtmlHelper<TModel> htmlHelper, string name, List<string> model)
#endif
        {
            HtmlBuilder hb = new HtmlBuilder();
            string ajaxUrl = htmlHelper.GetParentModelSupportProperty<string>(name, "AjaxUrl");

            NewModel newModel = new NewModel();

            hb.Append("<div class='t_newvalue'>");
            hb.Append(htmlHelper.ExtLabelFor(m => newModel.NewValue, "NewValue"));
            hb.Append(htmlHelper.EditorFor(m => newModel.NewValue, "Url", "NewValue"));
            hb.Append("<input name='btnAdd' type='button' value='Add' data-ajaxurl='{0}' />", YetaWFManager.JserEncode(ajaxUrl));
            hb.Append("</div>");

            return hb.ToHtmlString();
        }

        public class GridAllModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [Trim]
        public class GridAllEntry {

            public GridAllEntry() { }

            [Caption("Page"), Description("Defines the page name")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; }

            [Caption("Title"), Description("The page title which will appear as title in the browser window")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; }

            [Caption("Page Skin"), Description("The skin used to display the page")]
            [UIHint("PageSkin"), ReadOnly]
            public SkinDefinition SelectedSkin { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string RawUrl { get { return Url; } }

            public GridAllEntry(PageDefinition page) {
                ObjectSupport.CopyData(page, this);
            }
        }
#if MVC6
        public static HtmlString RenderAllPages<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, string id)
#else
        public static HtmlString RenderAllPages<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string id)
#endif
        {
            List<GridAllEntry> list = new List<GridAllEntry>();
            using (PageDefinitionDataProvider pagesDP = new PageDefinitionDataProvider()) {
                int total;
                List<PageDefinition> pages = pagesDP.GetItems(0, 0, null, null, out total);
                list = (from p in pages select new GridAllEntry(p)).ToList();
            }
            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel model = new GridModel() {
                GridDef = new GridDefinition() {
                    Id = id,
                    RecordType = typeof(GridAllEntry),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = true,
                }
            };
#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => model.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => model.GridDef);
#endif
        }

        public class GridEntryDisplay {

            [Caption("Page"), Description("Shows all page Urls that are part of the Unified Page Set")]
            [UIHint("String"), ReadOnly]
            public string Url { get; set; }

            public GridEntryDisplay(string url) {
                Url = url;
            }
        }

#if MVC6
        public static HtmlString RenderListOfLocalPagesDisplay<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, List<string> model) {
#else
        public static HtmlString RenderListOfLocalPagesDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, List<string> model) {
#endif
            List<GridEntryDisplay> list = new List<GridEntryDisplay>();
            if (model != null)
                list = (from u in model select new GridEntryDisplay(u)).ToList();

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridEntryDisplay),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 20,
                    ShowHeader = header,
                    ReadOnly = true,
                }
            };
#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => grid.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => grid.GridDef);
#endif
        }
    }
}
