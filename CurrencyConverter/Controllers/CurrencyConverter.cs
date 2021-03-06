/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
using YetaWF.Modules.CurrencyConverter.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.CurrencyConverter.Controllers {

    public class CurrencyConverterModuleController : ControllerImpl<YetaWF.Modules.CurrencyConverter.Modules.CurrencyConverterModule> {

        public CurrencyConverterModuleController() { }

        public class Model {
            [UIHint("YetaWF_CurrencyConverter_Country")]
            public string FromCountry { get; set; }
            [UIHint("YetaWF_CurrencyConverter_Country")]
            public string ToCountry { get; set; }
            [UIHint("Currency")]
            public decimal Amount { get; set; }

            public SerializableList<ExchangeRateEntry> Rates { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> CurrencyConverter(decimal? amount) {
            decimal specificAmount = (amount == null) ? Module.DefaultAmount : (decimal) amount;
            using (ExchangeRateDataProvider dp = new ExchangeRateDataProvider()) {
                ExchangeRateData data = await dp.GetItemAsync();
                Model model = new Model() {
                    Rates = data.Rates,
                    FromCountry = Module.FromCountry,
                    ToCountry = Module.ToCountry,
                    Amount = specificAmount,
                };
                return View(model);
            }
        }
    }
}