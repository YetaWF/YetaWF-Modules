"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var DecimalEditComponent = /** @class */ (function (_super) {
        __extends(DecimalEditComponent, _super);
        function DecimalEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId) || this;
            $(_this.Control).kendoNumericTextBox({
                format: "0.00",
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                downArrowText: "",
                upArrowText: ""
            });
            _this.kendoNumericTextBox = $(_this.Control).data("kendoNumericTextBox");
            return _this;
        }
        Object.defineProperty(DecimalEditComponent.prototype, "value", {
            get: function () {
                return this.kendoNumericTextBox.value();
            },
            set: function (val) {
                if (val == null)
                    this.kendoNumericTextBox.value("");
                else
                    this.kendoNumericTextBox.value(val);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(DecimalEditComponent.prototype, "valueText", {
            get: function () {
                return this.value ? this.value.toString() : "";
            },
            enumerable: true,
            configurable: true
        });
        DecimalEditComponent.prototype.clear = function () {
            this.kendoNumericTextBox.value("");
        };
        DecimalEditComponent.prototype.enable = function (enabled) {
            this.kendoNumericTextBox.enable(enabled);
        };
        DecimalEditComponent.SELECTOR = "input.yt_decimal.t_edit.k-input[name]";
        return DecimalEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.DecimalEditComponent = DecimalEditComponent;
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBaseDataImpl.clearDiv(tag, DecimalEditComponent.SELECTOR, function (control) {
            control.kendoNumericTextBox.destroy();
        });
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=DecimalEdit.js.map
