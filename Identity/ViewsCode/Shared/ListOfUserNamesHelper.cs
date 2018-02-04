/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.Controllers.Shared;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Modules.Identity.Support;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.Identity.Views.Shared {

    public class ListOfUserNamesHelper<TModel> : RazorTemplate<TModel> { }

    public static class ListOfUserNamesHelper {

        public class GridModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        public class NewModel {
            [Caption("User Name"), Description("Please enter a new user name and click Add")]
            [UIHint("Text80"), StringLength(80), Trim]
            public string NewValue { get; set; }
        }

        [Trim]
        public class GridEntryEdit {

            public GridEntryEdit() { }

            [Caption("Delete"), Description("Click to remove this user name from the list")]
            [UIHint("GridDeleteEntry")]
            public int DeleteMe { get; set; }

            [Caption("User Name"), Description("Shows all defined user names")]
            [UIHint("Text80"), StringLength(Globals.MaxUser), UserNameValidation, ListNoDuplicates, Required, Trim]
            public string UserName { get; set; }

            [UIHint("Hidden"), Required, Trim]
            public string __Value { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string __TextKey { get { return __Value; } }
            [UIHint("Raw"), ReadOnly]
            public string __TextDisplay { get { return UserName; } }

            public GridEntryEdit(UserDefinitionDataProvider userDP, int userId) {
                UserDefinition user = userDP.GetItemByUserId(userId);
                if (user == null) {
                    UserName = this.__ResStr("noUser", "({0})", userId);
                } else {
                    UserName = user.UserName; 
                }
                __Value = userId.ToString();
            }
        }
#if MVC6
        public static HtmlString RenderListOfUserNames<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, List<int> model)
#else
        public static HtmlString RenderListOfUserNames<TModel>(this HtmlHelper<TModel> htmlHelper, string name, List<int> model)
#endif
        {
            List<GridEntryEdit> list = new List<GridEntryEdit>();
            using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                if (model != null)
                    list = (from u in model select new GridEntryEdit(userDP, u)).ToList();
            }

            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel ListOfUserNamesModel = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridEntryEdit),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = false,
                    CanAddOrDelete = true,
                    DeleteProperty = "__TextKey",
                    DisplayProperty = "__TextDisplay"
                }
            };

#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => ListOfUserNamesModel.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => ListOfUserNamesModel.GridDef);
#endif
        }
#if MVC6
        public static HtmlString RenderListOfUserNamesAddNew<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, List<int> model)
#else
        public static HtmlString RenderListOfUserNamesAddNew<TModel>(this HtmlHelper<TModel> htmlHelper, string name, List<int> model)
#endif
        {
            HtmlBuilder hb = new HtmlBuilder();
            string ajaxUrl = htmlHelper.GetParentModelSupportProperty<string>(name, "AjaxUrl");

            NewModel newModel = new NewModel();

            hb.Append("<div class='t_newvalue'>");
            hb.Append(htmlHelper.ExtLabelFor(m => newModel.NewValue, "NewValue"));
            hb.Append(htmlHelper.EditorFor(m => newModel.NewValue, "Text80", "NewValue"));
            hb.Append("<input name='btnAdd' type='button' value='Add' data-ajaxurl='{0}' />", YetaWFManager.JserEncode(ajaxUrl));
            hb.Append("</div>");

            return hb.ToHtmlString();
        }

        [Trim]
        public class GridAllEntry {

            public GridAllEntry() { }

            [Caption("User Name"), Description("Defines the user name")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string RawUserName { get { return UserName; }  }

            public GridAllEntry(UserDefinition user) {
                ObjectSupport.CopyData(user, this);
            }
        }
#if MVC6
        public static HtmlString RenderAllUserNames<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, string id)
#else
        public static HtmlString RenderAllUserNames<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string id)
#endif
        {
            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            GridModel model = new GridModel() {
                GridDef = new GridDefinition() {
                    AjaxUrl = YetaWFManager.UrlFor(typeof(ListOfUserNamesHelperController), nameof(ListOfUserNamesHelperController.ListOfUserNamesBrowse_GridData)),
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

        public class GridEntryDisplay {

            [Caption("User Names"), Description("Shows all defined user names")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            [UIHint("Raw"), ReadOnly]
            public int UserId { get; set; }

            public GridEntryDisplay(UserDefinitionDataProvider userDP, int userId) {
                UserDefinition user = userDP.GetItemByUserId(userId);
                if (user == null) {
                    UserName = this.__ResStr("noUser", "({0})", userId);
                } else {
                    ObjectSupport.CopyData(user, this);
                }
                UserId = userId;
            }
        }
#if MVC6
        public static HtmlString RenderListOfUserNamesDisplay<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, List<int> model) {
#else
        public static HtmlString RenderListOfUserNamesDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, List<int> model) {
#endif
            List<GridEntryDisplay> list = new List<GridEntryDisplay>();

            using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                if (model != null)
                    list = (from u in model select new GridEntryDisplay(userDP, u)).ToList();
            }
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
                    InitialPageSize = 10,
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
