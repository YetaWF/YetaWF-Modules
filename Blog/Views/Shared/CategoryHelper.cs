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

                if (list.Count == 0) {
                    list.Insert(0, new SelectionItem<int> {
                        Text = __ResStr("none", "(None Available)"),
                        Tooltip = __ResStr("noneTT", "There are no blog categories"),
                        Value = 0,
                    });
                } else {
                    bool showSelect = false;
                    bool showAll = htmlHelper.GetControlInfo<bool>("", "ShowAll", false);
                    if (model == 0 && !showAll)
                        showSelect = htmlHelper.GetControlInfo<bool>("", "ShowSelectIfNone", false);
                    if (!showAll && !showSelect)
                        showSelect = htmlHelper.GetControlInfo<bool>("", "ShowSelect", false);
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
                }
                return htmlHelper.RenderDropDownSelectionList(name, model, list);
            }
        }
    }
}