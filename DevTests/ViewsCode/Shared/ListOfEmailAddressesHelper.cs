/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.DevTests.Views.Shared {

    public class ListOfEmailAddressesHelper<TModel> : RazorTemplate<TModel> { }

    public static class ListOfEmailAddressesHelper {

        public class GridModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        public class NewModel {
            [Caption("Email Address"), Description("Please enter a new email address and click Add")]
            [UIHint("Text80"), StringLength(80), Trim]
            public string NewValue { get; set; }
        }

        [Trim]
        public class GridEntryEdit {

            public GridEntryEdit() { }

            [Caption("Delete"), Description("Click to remove this email address from the list")]
            [UIHint("GridDeleteEntry")]
            public int DeleteMe { get; set; }

            [Caption("Email Address"), Description("Shows all defined email addresses")]
            [UIHint("Text80"), StringLength(Globals.MaxEmail), EmailValidation, ListNoDuplicates, Required, Trim]
            public string __Value { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string __TextKey { get { return __Value; } }
            [UIHint("Raw"), ReadOnly]
            public string __TextDisplay { get { return __Value; } }

            public GridEntryEdit(string text) {
                __Value = text;
            }
        }
#if MVC6
        public static HtmlString RenderListOfEmailAddresses<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, List<string> model)
#else
        public static HtmlString RenderListOfEmailAddresses<TModel>(this HtmlHelper<TModel> htmlHelper, string name, List<string> model)
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
            GridModel ListOfEmailAddressesModel = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridEntryEdit),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 5,
                    ShowHeader = header,
                    ReadOnly = false,
                    CanAddOrDelete = true,
                    DeleteProperty = "__TextKey",
                    DisplayProperty = "__TextDisplay"
                }
            };

#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => ListOfEmailAddressesModel.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => ListOfEmailAddressesModel.GridDef);
#endif
        }
#if MVC6
        public static async Task<HtmlString> RenderListOfEmailAddressesAddNewAsync<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, List<string> model)
#else
        public static async Task<HtmlString> RenderListOfEmailAddressesAddNewAsync<TModel>(this HtmlHelper<TModel> htmlHelper, string name, List<string> model)
#endif
        {
            HtmlBuilder hb = new HtmlBuilder();
            string ajaxUrl = htmlHelper.GetParentModelSupportProperty<string>(name, "AjaxUrl");

            NewModel newModel = new NewModel();

            hb.Append("<div class='t_newvalue'>");
            hb.Append(await htmlHelper.ExtLabelForAsync(m => newModel.NewValue, "NewValue"));
            hb.Append(htmlHelper.EditorFor(m => newModel.NewValue, "Text80", "NewValue"));
            hb.Append("<input name='btnAdd' type='button' value='Add' data-ajaxurl='{0}' />", YetaWFManager.HtmlAttributeEncode(ajaxUrl));
            hb.Append("</div>");

            return hb.ToHtmlString();
        }

        public class GridEntryDisplay {

            [Caption("Email Address"), Description("Shows all defined email addresses")]
            [UIHint("String"), ReadOnly]
            public string EmailAddress { get; set; }

            public GridEntryDisplay(string text) {
                EmailAddress = text;
            }
        }
#if MVC6
        public static HtmlString RenderListOfEmailAddressesDisplay<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, List<string> model) {
#else
        public static HtmlString RenderListOfEmailAddressesDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, List<string> model) {
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
                    InitialPageSize = 5,
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
