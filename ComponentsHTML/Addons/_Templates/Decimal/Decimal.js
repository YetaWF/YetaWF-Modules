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
            var list = tag.querySelectorAll("input.yt_decimal.t_edit");
            var len = list.length;
            for (var i = 0; i < len; ++i) {
                var el = list[i];
                var d = el.getAttribute("data-min");
                var sd = d == null ? 0.0 : Number(d);
                d = el.getAttribute("data-max");
                var ed = d == null ? 99999999.99 : Number(d);
                $(el).kendoNumericTextBox({
                    format: "0.00",
                    min: sd, max: ed,
                    culture: YConfigs.Basics.Language
                });
            }
        };
        return DecimalComponent;
    }());
    YetaWF_ComponentsHTML.DecimalComponent = DecimalComponent;
    // initializes new decimal elements on demand
    YetaWF_Basics.addWhenReady(function (section) {
        new DecimalComponent().initSection(section);
    });
    // A <div> is being emptied. Destroy all kendoNumericTextBox the <div> may contain.
    YetaWF_Basics.addClearDiv(function (tag) {
        var list = tag.querySelectorAll("input.yt_decimal.t_edit");
        var len = list.length;
        for (var i = 0; i < len; ++i) {
            var el = list[i];
            var numTextBox = $(el).data("kendoNumericTextBox");
            if (numTextBox)
                numTextBox.destroy();
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Decimal.js.map
