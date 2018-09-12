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
    var IntValueEditComponent = /** @class */ (function (_super) {
        __extends(IntValueEditComponent, _super);
        function IntValueEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId) || this;
            _this.kendoNumericTextBox = null;
            $YetaWF.addObjectDataById(controlId, _this);
            $(_this.Control).kendoNumericTextBox({
                decimals: 0, format: "n0",
                min: setup.Min, max: setup.Max,
                placeholder: setup.NoEntryText,
                step: setup.Step,
                downArrowText: "",
                upArrowText: ""
            });
            _this.kendoNumericTextBox = $(_this.Control).data("kendoNumericTextBox");
            return _this;
        }
        Object.defineProperty(IntValueEditComponent.prototype, "value", {
            get: function () {
                return parseInt(this.Control.value);
            },
            set: function (val) {
                if (this.kendoNumericTextBox == null) {
                    this.Control.value = val.toString();
                }
                else {
                    this.kendoNumericTextBox.value(val);
                }
            },
            enumerable: true,
            configurable: true
        });
        IntValueEditComponent.prototype.clear = function () {
            if (this.kendoNumericTextBox == null) {
                this.Control.value = "";
            }
            else {
                this.kendoNumericTextBox.value("");
            }
        };
        IntValueEditComponent.prototype.enable = function (enabled) {
            if (this.kendoNumericTextBox == null) {
                $YetaWF.elementEnableToggle(this.Control, enabled);
            }
            else {
                this.kendoNumericTextBox.enable(enabled);
            }
        };
        IntValueEditComponent.prototype.destroy = function () {
            if (this.kendoNumericTextBox)
                this.kendoNumericTextBox.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        };
        IntValueEditComponent.getControlFromTag = function (elem) { return _super.getControlBaseFromTag.call(this, elem, IntValueEditComponent.SELECTOR); };
        IntValueEditComponent.getControlFromSelector = function (selector, tags) { return _super.getControlBaseFromSelector.call(this, selector, IntValueEditComponent.SELECTOR, tags); };
        IntValueEditComponent.SELECTOR = "input.yt_intvalue_base.t_edit.k-input[name]";
        return IntValueEditComponent;
    }(YetaWF.ComponentBase));
    YetaWF_ComponentsHTML.IntValueEditComponent = IntValueEditComponent;
    // A <div> is being emptied. Destroy all IntValues the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBase.clearDiv(tag, IntValueEditComponent.SELECTOR, function (control) {
            control.destroy();
        });
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=IntValueEdit.js.map
