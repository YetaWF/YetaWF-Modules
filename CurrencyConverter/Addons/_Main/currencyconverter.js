/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

$(document).ready(function () {

    $('body').on('click', '.YetaWF_CurrencyConverter_CurrencyConverter input[name="convert"]', function () {
        var $ctl = $(this).closest('.t_converter');
        if ($ctl.length != 1) throw "Can't find t_converter";/*DEBUG*/
        var $from = $('select[name="FromCountry"]', $ctl);
        if ($from.length != 1) throw "Can't find FromCountry";/*DEBUG*/
        var $to = $('select[name="ToCountry"]', $ctl);
        if ($to.length != 1) throw "Can't find ToCountry";/*DEBUG*/
        var $amount = $('input[name="Amount"]', $ctl);
        if ($amount.length != 1) throw "Can't find Amount";/*DEBUG*/
        var $result_from = $('.t_result_from', $ctl);
        if ($result_from.length != 1) throw "Can't find Result_from"/*DEBUG*/;
        var $result_to = $('.t_result_to', $ctl);
        if ($result_to.length != 1) throw "Can't find Result_to";/*DEBUG*/

        var rates = YetaWF_CurrencyConverter_Rates;
        var total = rates.length;
        var fromCode = $from.val();
        var fromRate = findrate(rates, total, fromCode);
        var toCode = $to.val();
        var toRate = findrate(rates, total, toCode);
        var amount = $amount.val() * toRate / fromRate;
        $result_from.text(YLocs.YetaWF_CurrencyConverter.FmtResult.format($amount.val(), fromCode, $("option:selected", $from).text()));
        $result_to.text(YLocs.YetaWF_CurrencyConverter.FmtResult.format( amount.toFixed(2), toCode, $("option:selected", $to).text()));
        $('.t_result_is', $ctl).css("visibility", "visible")

        function findrate(rates, total, code) {
            for (var index = 0 ; index < total; ++index) {
                var rate = rates[index];
                if (rate.Code == code) return rate.Rate;
            }
            throw "Country code " + code + " is not available";/*DEBUG*/
        }
    });

});