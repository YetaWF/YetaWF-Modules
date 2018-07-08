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
            var list = tag.querySelectorAll("input.yt_currency.t_edit");
            var len = list.length;
            for (var i = 0; i < len; ++i) {
                var el = list[i];
                var d = el.getAttribute("data-min");
                var sd = d == null ? 0.0 : Number(d);
                d = el.getAttribute("data-max");
                var ed = d == null ? 99999999.99 : Number(d);
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
    YetaWF_Basics.addWhenReady(function (section) {
        new CurrencyComponent().initSection(section);
    });
    // A <div> is being emptied. Destroy all kendoNumericTextBox the <div> may contain.
    YetaWF_Basics.addClearDiv(function (tag) {
        var list = tag.querySelectorAll("input.yt_currency.t_edit");
        var len = list.length;
        for (var i = 0; i < len; ++i) {
            var el = list[i];
            var numTextBox = $(el).data("kendoNumericTextBox");
            if (numTextBox)
                numTextBox.destroy();
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Currency.js.map
