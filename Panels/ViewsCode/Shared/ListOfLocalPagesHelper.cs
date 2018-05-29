/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

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
using YetaWF.Modules.Panels.Controllers.Shared;
using System.Threading.Tasks;
using YetaWF.Modules.Panels.Models;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.Panels.Views.Shared {

    public class ListOfLocalPagesHelper<TModel> : RazorTemplate<TModel> { }

    public static class ListOfLocalPagesHelper {

        public class GridModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        public class NewModel {
            [Caption("Page"), Description("Please select a page and click Add to add it to the list of pages")]
            [UIHint("Url"), StringLength(Globals.MaxUrl), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote),
                UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), Trim]
            public string NewValue { get; set; }
        }

        [Trim]
        public class GridEntryEdit {

            public GridEntryEdit() { }

            [Caption("Delete"), Description("Click to remove this page from the list")]
            [UIHint("GridDeleteEntry")]
            public int DeleteMe { get; set; }

            [Caption("Page"), Description("Shows all pages")]
            [UIHint("Url"), ReadOnly]
            public string UrlDisplay { get { return Url; } set { Url = value; } }

            [Caption("Popup"), Description("Defines whether the page is shown in a popup window")]
            [UIHint("Boolean")]
            public bool Popup { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string Url { get; set; } // this is used so we have the pure url (without input field)

            public GridEntryEdit(LocalPage page) {
                Url = page.Url;
                Popup = page.Popup;
            }
        }
#if MVC6
        public static HtmlString RenderListOfLocalPages<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, string id, List<LocalPage> model)
#else
        public static HtmlString RenderListOfLocalPages<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string id, List<LocalPage> model)
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
                    DeleteProperty = nameof(GridEntryEdit.Url),
                    DisplayProperty = nameof(GridEntryEdit.Url)
                }
            };
#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => ListOfLocalPagesModel.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => ListOfLocalPagesModel.GridDef);
#endif
        }
#if MVC6
        public static async Task<HtmlString> RenderListOfLocalPagesAddNewAsync<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, List<LocalPage> model)
#else
        public static async Task<HtmlString> RenderListOfLocalPagesAddNewAsync<TModel>(this HtmlHelper<TModel> htmlHelper, string name, List<LocalPage> model)
#endif
        {
            HtmlBuilder hb = new HtmlBuilder();
            string ajaxUrl = htmlHelper.GetParentModelSupportProperty<string>(name, "AjaxUrl");

            NewModel newModel = new NewModel();

            hb.Append("<div class='t_newvalue'>");
            hb.Append(await htmlHelper.ExtLabelForAsync(m => newModel.NewValue, "NewValue"));
            hb.Append(htmlHelper.EditorFor(m => newModel.NewValue, "Url", "NewValue"));
            hb.Append("<input name='btnAdd' type='button' value='Add' data-ajaxurl='{0}' />", YetaWFManager.HtmlAttributeEncode(ajaxUrl));
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

            [Caption("Popup"), Description("Defines whether the page is shown in a popup window")]
            [UIHint("Boolean"), ReadOnly]
            public bool Popup { get; set; }

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
            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            GridModel model = new GridModel() {
                GridDef = new GridDefinition() {
                    AjaxUrl = YetaWFManager.UrlFor(typeof(ListOfLocalPagesHelperController), nameof(ListOfLocalPagesHelperController.ListOfLocalPagesBrowse_GridData)),
                    Id = id,
                    RecordType = typeof(GridAllEntry),
                    ShowHeader = header,
                }
            };
#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => model.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => model.GridDef);
#endif
        }
    }
}
