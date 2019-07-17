"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
    var DateTimeEditComponent = /** @class */ (function (_super) {
        __extends(DateTimeEditComponent, _super);
        function DateTimeEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, DateTimeEditComponent.TEMPLATE, DateTimeEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: "datetime_change",
                GetValue: function (control) {
                    return control.valueText;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, function (tag, control) {
                control.kendoDateTimePicker.destroy();
            }) || this;
            _this.Hidden = $YetaWF.getElement1BySelector("input[type=\"hidden\"]", [_this.Control]);
            _this.Date = $YetaWF.getElement1BySelector("input[name=\"dtpicker\"]", [_this.Control]);
            $(_this.Date).kendoDateTimePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateTimeFormat,
                min: setup.Min, max: setup.Max,
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
                    event.initEvent("datetime_change", true, true);
                    _this.Control.dispatchEvent(event);
                }
            });
            _this.kendoDateTimePicker = $(_this.Date).data("kendoDateTimePicker");
            _this.setHidden(_this.kendoDateTimePicker.value());
            return _this;
        }
        DateTimeEditComponent.prototype.setHidden = function (dateVal) {
            var s = "";
            if (dateVal != null)
                s = dateVal.toISOString();
            this.Hidden.setAttribute("value", s);
        };
        DateTimeEditComponent.prototype.setHiddenText = function (dateVal) {
            this.Hidden.setAttribute("value", dateVal ? dateVal : "");
        };
        Object.defineProperty(DateTimeEditComponent.prototype, "value", {
            get: function () {
                return new Date(this.Hidden.value);
            },
            set: function (val) {
                this.setHidden(val);
                this.kendoDateTimePicker.value(val);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(DateTimeEditComponent.prototype, "valueText", {
            get: function () {
                return this.Hidden.value;
            },
            enumerable: true,
            configurable: true
        });
        DateTimeEditComponent.prototype.clear = function () {
            this.setHiddenText("");
            this.kendoDateTimePicker.value("");
        };
        DateTimeEditComponent.prototype.enable = function (enabled) {
            this.kendoDateTimePicker.enable(enabled);
        };
        DateTimeEditComponent.TEMPLATE = "yt_datetime";
        DateTimeEditComponent.SELECTOR = ".yt_datetime.t_edit";
        return DateTimeEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.DateTimeEditComponent = DateTimeEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=DateTimeEdit.js.map
