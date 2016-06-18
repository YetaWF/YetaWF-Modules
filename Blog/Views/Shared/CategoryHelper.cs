/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Localize;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Views.Shared {

    public static class CategoryHelper {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(CategoryHelper), name, defaultValue, parms); }

        public static MvcHtmlString RenderCategory<TModel>(this HtmlHelper<TModel> htmlHelper, string name, int model, object HtmlAttributes = null) {

            using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                int total;
                List<BlogCategory> data = categoryDP.GetItems(0, 0, null, null, out total);
                List<SelectionItem<int>> list = (from c in data orderby c.Category.ToString() select new SelectionItem<int> {
                    Text = c.Category.ToString(),
                    Tooltip = c.Description,
                    Value = c.Identity
                }).ToList();

                bool showSelect = false;
                bool showAll = false;
                htmlHelper.TryGetControlInfo<bool>("", "ShowAll", out showAll);
                if (model == 0 && !showAll)
                    htmlHelper.TryGetControlInfo<bool>("", "ShowSelectIfNone", out showSelect);
                if (!showAll && !showSelect)
                    htmlHelper.TryGetControlInfo<bool>("", "ShowSelect", out showSelect);
                if (showAll) {
                    list.Insert(0, new SelectionItem<int> {
                        Text = __ResStr("all", "(All)"),
                        Tooltip = __ResStr("allTT", "Displays blogs from all available blog categories"),
                        Value = 0,
                    });
                } else if (showSelect) {
                    list.Insert(0, new SelectionItem<int> {
                        Text = __ResStr("select", "(Select)"),
                        Tooltip = __ResStr("selectTT", "Please select one of the available blog categories"),
                        Value = 0,
                    });
                }
                return htmlHelper.RenderDropDownSelectionList(name, model, list);
            }
        }
    }
}