"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var DecimalComponent = /** @class */ (function () {
        function DecimalComponent() {
        }
        /**
         * Initializes all decimal fields in the specified tag.
         * @param tag - an element containing Decimal template controls.
         */
        DecimalComponent.prototype.initSection = function (tag) {
            var list = $YetaWF.getElementsBySelector("input.yt_decimal.t_edit", [tag]);
            for (var _i = 0, list_1 = list; _i < list_1.length; _i++) {
                var el = list_1[_i];
                var d = el.getAttribute("data-min");
                var sd = d ? Number(d) : 0.0;
                d = el.getAttribute("data-max");
                var ed = d ? Number(d) : 99999999.99;
                $(el).kendoNumericTextBox({
                    format: "0.00",
                    min: sd, max: ed,
                    culture: YVolatile.Basics.Language
                });
            }
        };
        return DecimalComponent;
    }());
    YetaWF_ComponentsHTML.DecimalComponent = DecimalComponent;
    // initializes new decimal elements on demand
    $YetaWF.addWhenReady(function (section) {
        new DecimalComponent().initSection(section);
    });
    // A <div> is being emptied. Destroy all kendoNumericTextBox the <div> may contain.
    $YetaWF.addClearDiv(function (tag) {
        var list = $YetaWF.getElementsBySelector("input.yt_decimal.t_edit", [tag]);
        for (var _i = 0, list_2 = list; _i < list_2.length; _i++) {
            var el = list_2[_i];
            var numTextBox = $(el).data("kendoNumericTextBox");
            if (numTextBox)
                numTextBox.destroy();
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
