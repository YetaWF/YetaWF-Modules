"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
/// <reference types="kendo-ui" />
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var DateEditComponent = /** @class */ (function (_super) {
        __extends(DateEditComponent, _super);
        function DateEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, DateEditComponent.TEMPLATE, DateEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: DateEditComponent.EVENT,
                GetValue: function (control) {
                    return control.valueText;
                },
                Enable: function (control, enable, clearOnDisable) {
                    YetaWF_BasicsImpl.elementEnableToggle(control.Hidden, enable);
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                }
            }, false, function (tag, control) {
                control.kendoDatePicker.destroy();
            }) || this;
            _this.Hidden = $YetaWF.getElement1BySelector("input[type=\"hidden\"]", [_this.Control]);
            _this.Date = $YetaWF.getElement1BySelector("input[name=\"dtpicker\"]", [_this.Control]);
            $(_this.Date).kendoDatePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateFormat,
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                close: function (e) {
                    e.preventDefault();
                },
                change: function (ev) {
                    var kdPicker = ev.sender;
                    var val = kdPicker.value();
                    if (val == null)
                        _this.setHiddenText(kdPicker.element.val());
                    else
                        _this.setHidden(val);
                    FormsSupport.validateElement(_this.Hidden);
                    var event = document.createEvent("Event");
                    event.initEvent(DateEditComponent.EVENT, true, true);
                    _this.Control.dispatchEvent(event);
                }
            });
            _this.kendoDatePicker = $(_this.Date).data("kendoDatePicker");
            _this.setHidden(_this.kendoDatePicker.value());
            return _this;
        }
        DateEditComponent.prototype.setHidden = function (dateVal) {
            var s = "";
            if (dateVal != null)
                s = dateVal.toISOString();
            this.Hidden.setAttribute("value", s);
        };
        DateEditComponent.prototype.setHiddenText = function (dateVal) {
            this.Hidden.setAttribute("value", dateVal ? dateVal : "");
        };
        Object.defineProperty(DateEditComponent.prototype, "value", {
            get: function () {
                return new Date(this.Hidden.value);
            },
            set: function (val) {
                this.setHidden(val);
                this.kendoDatePicker.value(val);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateEditComponent.prototype, "valueText", {
            get: function () {
                return this.Hidden.value;
            },
            enumerable: false,
            configurable: true
        });
        DateEditComponent.prototype.clear = function () {
            this.setHiddenText("");
            this.kendoDatePicker.value("");
        };
        DateEditComponent.prototype.enable = function (enabled) {
            this.kendoDatePicker.enable(enabled);
        };
        DateEditComponent.prototype.close = function () {
            this.kendoDatePicker.close();
        };
        DateEditComponent.closeAll = function () {
            var ctrls = $YetaWF.getElementsBySelector(DateEditComponent.SELECTOR);
            for (var _i = 0, ctrls_1 = ctrls; _i < ctrls_1.length; _i++) {
                var ctrl = ctrls_1[_i];
                var c = DateEditComponent.getControlFromTag(ctrl, DateEditComponent.SELECTOR);
                c.close();
            }
        };
        DateEditComponent.TEMPLATE = "yt_date";
        DateEditComponent.SELECTOR = ".yt_date.t_edit";
        DateEditComponent.EVENT = "date_change";
        return DateEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.DateEditComponent = DateEditComponent;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, function (ev) {
        DateEditComponent.closeAll();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        DateEditComponent.closeAll();
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=DateEdit.js.map
