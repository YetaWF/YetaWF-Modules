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
            $YetaWF.addObjectDataById(controlId, _this);
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
                return parseFloat(this.Control.value);
            },
            set: function (val) {
                this.kendoNumericTextBox.value(val);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(DecimalEditComponent.prototype, "valueText", {
            get: function () {
                return this.Control.value;
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
        DecimalEditComponent.prototype.destroy = function () {
            this.kendoNumericTextBox.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        };
        DecimalEditComponent.getControlFromTag = function (elem) { return _super.getControlBaseFromTag.call(this, elem, DecimalEditComponent.SELECTOR); };
        DecimalEditComponent.getControlFromSelector = function (selector, tags) { return _super.getControlBaseFromSelector.call(this, selector, DecimalEditComponent.SELECTOR, tags); };
        DecimalEditComponent.getControlById = function (id) { return _super.getControlBaseById.call(this, id, DecimalEditComponent.SELECTOR); };
        DecimalEditComponent.SELECTOR = "input.yt_decimal.t_edit.k-input[name]";
        return DecimalEditComponent;
    }(YetaWF.ComponentBase));
    YetaWF_ComponentsHTML.DecimalEditComponent = DecimalEditComponent;
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBase.clearDiv(tag, DecimalEditComponent.SELECTOR, function (control) {
            control.destroy();
        });
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
