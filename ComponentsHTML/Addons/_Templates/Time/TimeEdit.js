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
    var TimeEditComponent = /** @class */ (function (_super) {
        __extends(TimeEditComponent, _super);
        function TimeEditComponent(controlId /*, setup: TimeEditSetup*/) {
            var _this = _super.call(this, controlId, TimeEditComponent.TEMPLATE, TimeEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: "time_change",
                GetValue: function (control) {
                    return control.valueText;
                },
                Enable: function (control, enable, clearOnDisable) {
                    YetaWF_BasicsImpl.elementEnableToggle(control.Hidden, enable);
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, function (tag, control) {
                control.kendoTimePicker.destroy();
            }) || this;
            _this.Hidden = $YetaWF.getElement1BySelector("input[type=\"hidden\"]", [_this.Control]);
            _this.Time = $YetaWF.getElement1BySelector("input[name=\"dtpicker\"]", [_this.Control]);
            $(_this.Time).kendoTimePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.TimeFormat,
                culture: YVolatile.Basics.Language,
                change: function (ev) {
                    var kdPicker = ev.sender;
                    var val = kdPicker.value();
                    if (val == null)
                        _this.setHiddenText(kdPicker.element.val());
                    else
                        _this.setHidden(val);
                    FormsSupport.validateElement(_this.Hidden);
                    var event = document.createEvent("Event");
                    event.initEvent("time_change", true, true);
                    _this.Control.dispatchEvent(event);
                }
            });
            _this.kendoTimePicker = $(_this.Time).data("kendoTimePicker");
            _this.setHidden(_this.kendoTimePicker.value());
            return _this;
        }
        TimeEditComponent.prototype.setHidden = function (dateVal) {
            var s = "";
            if (dateVal != null)
                s = dateVal.toISOString();
            this.Hidden.setAttribute("value", s);
        };
        TimeEditComponent.prototype.setHiddenText = function (dateVal) {
            this.Hidden.setAttribute("value", dateVal ? dateVal : "");
        };
        Object.defineProperty(TimeEditComponent.prototype, "value", {
            get: function () {
                return new Date(this.Hidden.value);
            },
            set: function (val) {
                this.setHidden(val);
                this.kendoTimePicker.value(val);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(TimeEditComponent.prototype, "valueText", {
            get: function () {
                return this.Hidden.value;
            },
            enumerable: true,
            configurable: true
        });
        TimeEditComponent.prototype.clear = function () {
            this.setHiddenText("");
            this.kendoTimePicker.value("");
        };
        TimeEditComponent.prototype.enable = function (enabled) {
            this.kendoTimePicker.enable(enabled);
        };
        TimeEditComponent.TEMPLATE = "yt_time";
        TimeEditComponent.SELECTOR = ".yt_time.t_edit";
        return TimeEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.TimeEditComponent = TimeEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=TimeEdit.js.map
