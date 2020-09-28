"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ProgressBarComponent = /** @class */ (function (_super) {
        __extends(ProgressBarComponent, _super);
        function ProgressBarComponent(controlId) {
            var _this = _super.call(this, controlId, ProgressBarComponent.TEMPLATE, ProgressBarComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: function (control) {
                    return control.value.toString();
                },
                Enable: null,
            }) || this;
            _this.Timer = 0;
            _this.ShownValue = 0;
            _this.NewValue = 0;
            _this.ValueDiv = $YetaWF.getElement1BySelector("div.ui-progressbar-value", [_this.Control]);
            _this.Min = Number($YetaWF.getAttribute(_this.Control, "aria-valuemin"));
            _this.Max = Number($YetaWF.getAttribute(_this.Control, "aria-valuemax"));
            _this.Value = Number($YetaWF.getAttribute(_this.Control, "aria-valuenow"));
            _this.ShownValue = _this.Value;
            return _this;
        }
        ProgressBarComponent.prototype.setValue = function (newValue) {
            var _this = this;
            this.Value = newValue;
            this.NewValue = newValue;
            var step = (this.Max - this.Min) / (ProgressBarComponent.MAXTIME / ProgressBarComponent.INCRTIME);
            if (!this.Timer) {
                this.Timer = setInterval(function () {
                    var newValue;
                    if (_this.NewValue > _this.ShownValue) {
                        newValue = _this.ShownValue + step;
                        if (newValue >= _this.Value) {
                            newValue = _this.Value;
                            clearInterval(_this.Timer);
                            _this.Timer = 0;
                        }
                    }
                    else { //if (this.NewValue <= this.Value) {
                        newValue = _this.ShownValue - step;
                        if (newValue <= _this.Value) {
                            newValue = _this.Value;
                            clearInterval(_this.Timer);
                            _this.Timer = 0;
                        }
                    }
                    _this.ShownValue = newValue;
                    _this.ValueDiv.style.width = newValue + "%";
                }, ProgressBarComponent.INCRTIME);
            }
        };
        Object.defineProperty(ProgressBarComponent.prototype, "value", {
            // API
            get: function () {
                return this.Value;
            },
            set: function (val) {
                if (val > this.Max)
                    val = this.Max;
                if (val < this.Min)
                    val = this.Min;
                this.setValue(val);
            },
            enumerable: false,
            configurable: true
        });
        ProgressBarComponent.prototype.show = function () {
            this.Control.style.display = "";
        };
        ProgressBarComponent.prototype.hide = function () {
            this.Control.style.display = "none";
        };
        ProgressBarComponent.TEMPLATE = "yt_progressbar";
        ProgressBarComponent.SELECTOR = ".yt_progressbar.t_display";
        ProgressBarComponent.MAXTIME = 0.5;
        ProgressBarComponent.INCRTIME = 0.02;
        return ProgressBarComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.ProgressBarComponent = ProgressBarComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=ProgressBar.js.map
