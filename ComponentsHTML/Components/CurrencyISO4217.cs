using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class CurrencyISO4217Component : YetaWFComponent {

        public const string TemplateName = "CurrencyISO4217";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class CurrencyISO4217DisplayComponent : CurrencyISO4217Component, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(string model) {

            string currency = await CurrencyISO4217.IdToCurrencyAsync(model, AllowMismatch: true);
            return new YHtmlString(currency);
        }
    }
    public class CurrencyISO4217EditComponent : CurrencyISO4217Component, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            bool includeSiteCurrency = PropData.GetAdditionalAttributeValue<bool>("SiteCurrency", true);

            List<CurrencyISO4217.Currency> currencies = await CurrencyISO4217.GetCurrenciesAsync(IncludeSiteCurrency: includeSiteCurrency);
            List<SelectionItem<string>> list = (from l in currencies select new SelectionItem<string>() {
                Text = l.Name,
                Value = l.Id,
            }).ToList();
            list.Insert(0, new SelectionItem<string> {
                Text = this.__ResStr("default", "(select)"),
                Value = "",
            });
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_currencyiso4217");
        }
    }
}
