using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.CurrencyConverter.Controllers;
using YetaWF.Modules.CurrencyConverter.DataProvider;
using YetaWF.Modules.CurrencyConverter.Modules;

namespace YetaWF.Modules.CurrencyConverter.Views {

    public class CurrencyConverterView : YetaWFView, IYetaWFView<CurrencyConverterModule, CurrencyConverterModuleController.Model> {

        public const string ViewName = "CurrencyConverter";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(CurrencyConverterModule module, CurrencyConverterModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            // based on https://openexchangerates.org

            await Manager.ScriptManager.AddScriptAsync(Package.Domain, Package.Product, ExchangeRateDataProvider.JSFile, Bundle: false);
            string wantFocus = module.WantFocus ? " focusonme" : "";
            FormButton button = new FormButton() { ButtonType = ButtonTypeEnum.Button, Name = "convert", Text = this.__ResStr("btnGo", "Go") };

            hb.Append($@"
<div class='t_converter'>
    <div class='t_amountlabel'>
        {YetaWFManager.HtmlEncode(this.__ResStr("amount", "Amount:"))}
    </div>
    <div class='t_amount{wantFocus}'>
        {await HtmlHelper.ForEditAsync(model, nameof(model.Amount))}
    </div>
    <div class='t_fromlabel'>
        {YetaWFManager.HtmlEncode(this.__ResStr("from", "From:"))}
    </div>
    <div class='t_from'>
        {await HtmlHelper.ForEditAsync(model, nameof(model.FromCountry))}
    </div>
    <div class='t_tolabel'>
        {YetaWFManager.HtmlEncode(this.__ResStr("to", "To:"))}
    </div>
    <div class='t_to'>
        {await HtmlHelper.ForEditAsync(model, nameof(model.ToCountry))}
    </div>
    <div class='t_button'>
        {await button.RenderAsync()}
    </div>
    <div class='t_result_from t_results'>
    </div>
    <div class='t_result_is t_results'>
        {YetaWFManager.HtmlEncode(this.__ResStr("is", " => "))}
    </div>
    <div class='t_result_to t_results'>
    </div>
    <div class='t_disclaimer'>
        {YetaWFManager.HtmlEncode(this.__ResStr("disclaimer", "Conversions are approximate and do not include taxes/fees and other conversion costs"))}
    </div>
</div>");

            return hb.ToYHtmlString();
        }
    }
}
