/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.CurrencyConverter.DataProvider;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.CurrencyConverter.Views.Shared {

    public class Vendor<TModel> : RazorTemplate<TModel> { }

    public static class CountryHelper {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

#if MVC6
        public static async Task<HtmlString> RenderCountryAsync<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, string model, object HtmlAttributes = null)
#else
        public static async Task<HtmlString> RenderCountryAsync<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string model, object HtmlAttributes = null)
#endif
        {

            using (ExchangeRateDataProvider dp = new ExchangeRateDataProvider()) {
                ExchangeRateData data = await dp.GetItemAsync();
                List<SelectionItem<string>> list = (from r in data.Rates orderby r.CurrencyName select new SelectionItem<string> { Text = r.CurrencyName, Value = r.Code }).ToList();
                return await htmlHelper.RenderDropDownSelectionListAsync(name, model, list, HtmlAttributes: HtmlAttributes);
            }
        }
    }
}