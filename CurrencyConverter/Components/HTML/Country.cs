/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.CurrencyConverter.DataProvider;

namespace YetaWF.Modules.CurrencyConverter.Components {

    public abstract class CountryComponent : YetaWFComponent {

        public const string TemplateName = "Country";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class CountryDisplayComponent : CountryComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<string> RenderAsync(string model) {

            using (ExchangeRateDataProvider dp = new ExchangeRateDataProvider()) {
                ExchangeRateData data = await dp.GetItemAsync();
                string currency = (from r in data.Rates where r.Code == model select r.CurrencyName).FirstOrDefault();
                if (string.IsNullOrEmpty(model))
                    return null;
                return HE(currency);
            }
        }
    }
    public class CountryEditComponent : CountryComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<string> RenderAsync(string model) {

            using (ExchangeRateDataProvider dp = new ExchangeRateDataProvider()) {
                ExchangeRateData data = await dp.GetItemAsync();
                List<SelectionItem<string>> list = (from r in data.Rates orderby r.CurrencyName select new SelectionItem<string> { Text = r.CurrencyName, Value = r.Code }).ToList();
                return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_yetawf_currencyconverter_country");
            }
        }
    }
}
