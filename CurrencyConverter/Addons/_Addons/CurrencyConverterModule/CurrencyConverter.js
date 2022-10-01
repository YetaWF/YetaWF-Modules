"use strict";
/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */
var YetaWF_CurrencyConverter;
(function (YetaWF_CurrencyConverter) {
    var Converter = /** @class */ (function () {
        function Converter() {
        }
        Converter.findrate = function (code) {
            for (var _i = 0, YetaWF_CurrencyConverter_Rates_1 = YetaWF_CurrencyConverter_Rates; _i < YetaWF_CurrencyConverter_Rates_1.length; _i++) {
                var rate = YetaWF_CurrencyConverter_Rates_1[_i];
                if (rate.Code === code)
                    return rate;
            }
            throw "Country code " + code + " is not available"; /*DEBUG*/
        };
        return Converter;
    }());
    $YetaWF.registerEventHandlerDocument("click", ".YetaWF_CurrencyConverter_CurrencyConverter input[name='convert']", function (ev) {
        var buttonClick = ev.__YetaWFElem;
        var divConverter = $YetaWF.elementClosest(buttonClick, ".t_converter");
        var fromSelect = $YetaWF.getElement1BySelector("select[name='FromCountry']", [divConverter]);
        var toSelect = $YetaWF.getElement1BySelector("select[name='ToCountry']", [divConverter]);
        var amountInput = $YetaWF.getElement1BySelector("input[name='Amount']", [divConverter]);
        var resultFrom = $YetaWF.getElement1BySelector(".t_result_from", [divConverter]);
        var resultTo = $YetaWF.getElement1BySelector(".t_result_to", [divConverter]);
        var resultIs = $YetaWF.getElement1BySelector(".t_result_is", [divConverter]);
        var fromRate = Converter.findrate(fromSelect.value);
        var toRate = Converter.findrate(toSelect.value);
        var amount = Number(amountInput.value) * toRate.Rate / fromRate.Rate;
        resultFrom.innerText = YLocs.YetaWF_CurrencyConverter.FmtResult.format(amountInput.value, fromRate.Code, fromSelect.options[fromSelect.selectedIndex].innerText);
        resultTo.innerText = YLocs.YetaWF_CurrencyConverter.FmtResult.format(amount.toFixed(2), toRate.Code, toSelect.options[toSelect.selectedIndex].innerText);
        resultIs.style.visibility = "visible";
        return false;
    });
})(YetaWF_CurrencyConverter || (YetaWF_CurrencyConverter = {}));

//# sourceMappingURL=CurrencyConverter.js.map
