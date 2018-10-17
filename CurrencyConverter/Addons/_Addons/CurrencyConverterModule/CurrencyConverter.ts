/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

interface Rate {
    CurrencyName: string;
    Code: string;
    Rate: number;
}

declare var YetaWF_CurrencyConverter_Rates: Rate[];

namespace YetaWF_CurrencyConverter {

    export interface IPackageLocs {
        FmtResult: string;
    }

    class Converter {
        public static findrate(code): Rate {
            for (let rate of YetaWF_CurrencyConverter_Rates) {
                if (rate.Code == code) return rate;
            }
            throw "Country code " + code + " is not available";/*DEBUG*/
        }
    }

    $YetaWF.registerEventHandlerDocument("click", ".YetaWF_CurrencyConverter_CurrencyConverter input[name='convert']", (ev: MouseEvent): boolean => {

        var buttonClick = ev.__YetaWFElem;

        var divConverter = $YetaWF.elementClosest(buttonClick, ".t_converter");

        var fromSelect = $YetaWF.getElement1BySelector("select[name='FromCountry']", [divConverter]) as HTMLSelectElement;
        var toSelect = $YetaWF.getElement1BySelector("select[name='ToCountry']", [divConverter]) as HTMLSelectElement;
        var amountInput = $YetaWF.getElement1BySelector("input[name='Amount']", [divConverter]) as HTMLInputElement;
        var result_from = $YetaWF.getElement1BySelector(".t_result_from", [divConverter]) as HTMLDivElement;
        var result_to = $YetaWF.getElement1BySelector(".t_result_to", [divConverter]) as HTMLDivElement;
        var result_is = $YetaWF.getElement1BySelector(".t_result_is", [divConverter]) as HTMLDivElement;

        var fromRate = Converter.findrate(fromSelect.value);
        var toRate = Converter.findrate(toSelect.value);

        var amount = Number(amountInput.value) * toRate.Rate / fromRate.Rate;
        result_from.innerText = YLocs.YetaWF_CurrencyConverter.FmtResult.format(amountInput.value, fromRate.Code, fromSelect.options[fromSelect.selectedIndex].innerText);
        result_to.innerText = YLocs.YetaWF_CurrencyConverter.FmtResult.format(amount.toFixed(2), toRate.Code, toSelect.options[toSelect.selectedIndex].innerText);

        result_is.style.visibility = "visible";

        return false;
    });
}