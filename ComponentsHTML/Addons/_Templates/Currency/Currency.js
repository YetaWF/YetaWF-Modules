"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var CurrencyComponent = /** @class */ (function () {
        function CurrencyComponent() {
        }
        /**
         * Initializes all currency fields in the specified tag.
         * @param tag - an element containing Currency template controls.
         */
        CurrencyComponent.prototype.initSection = function (tag) {
            var list = $YetaWF.getElementsBySelector("input.yt_currency.t_edit", [tag]);
            for (var _i = 0, list_1 = list; _i < list_1.length; _i++) {
                var el = list_1[_i];
                var d = el.getAttribute("data-min");
                var sd = d ? Number(d) : 0.0;
                d = el.getAttribute("data-max");
                var ed = d ? Number(d) : 99999999.99;
                $(el).kendoNumericTextBox({
                    format: YVolatile.YetaWF_ComponentsHTML.CurrencyFormat,
                    min: sd, max: ed,
                    culture: YVolatile.Basics.Language
                });
            }
        };
        return CurrencyComponent;
    }());
    YetaWF_ComponentsHTML.CurrencyComponent = CurrencyComponent;
    // initializes new currency elements on demand
    $YetaWF.addWhenReady(function (section) {
        new CurrencyComponent().initSection(section);
    });
    // A <div> is being emptied. Destroy all kendoNumericTextBox the <div> may contain.
    $YetaWF.addClearDiv(function (tag) {
        var list = $YetaWF.getElementsBySelector("input.yt_currency.t_edit", [tag]);
        for (var _i = 0, list_2 = list; _i < list_2.length; _i++) {
            var el = list_2[_i];
            var numTextBox = $(el).data("kendoNumericTextBox");
            if (numTextBox)
                numTextBox.destroy();
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
