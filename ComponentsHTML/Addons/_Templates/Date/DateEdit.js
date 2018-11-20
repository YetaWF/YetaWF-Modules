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
    var DateEditComponent = /** @class */ (function (_super) {
        __extends(DateEditComponent, _super);
        function DateEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId) || this;
            $YetaWF.addObjectDataById(controlId, _this);
            _this.Hidden = $YetaWF.getElement1BySelector("input[type=\"hidden\"]", [_this.Control]);
            _this.Date = $YetaWF.getElement1BySelector("input[name=\"dtpicker\"]", [_this.Control]);
            $(_this.Date).kendoDatePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateFormat,
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
                }
            });
            _this.kendoDatePicker = $(_this.Date).data("kendoDatePicker");
            _this.setHidden(_this.kendoDatePicker.value());
            _this.Date.addEventListener("change", function (event) {
                var val = _this.kendoDatePicker.value();
                if (val == null)
                    _this.setHiddenText(event.target.value);
                else
                    _this.setHidden(val);
                FormsSupport.validateElement(_this.Hidden);
            }, false);
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
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(DateEditComponent.prototype, "valueText", {
            get: function () {
                return this.Hidden.value;
            },
            enumerable: true,
            configurable: true
        });
        DateEditComponent.prototype.clear = function () {
            this.setHiddenText("");
            this.kendoDatePicker.value("");
        };
        DateEditComponent.prototype.enable = function (enabled) {
            this.kendoDatePicker.enable(enabled);
        };
        DateEditComponent.prototype.destroy = function () {
            this.kendoDatePicker.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        };
        DateEditComponent.getControlFromTag = function (elem) { return _super.getControlBaseFromTag.call(this, elem, DateEditComponent.SELECTOR); };
        DateEditComponent.getControlFromSelector = function (selector, tags) { return _super.getControlBaseFromSelector.call(this, selector, DateEditComponent.SELECTOR, tags); };
        DateEditComponent.getControlById = function (id) { return _super.getControlBaseById.call(this, id, DateEditComponent.SELECTOR); };
        DateEditComponent.SELECTOR = ".yt_date.t_edit";
        return DateEditComponent;
    }(YetaWF.ComponentBase));
    YetaWF_ComponentsHTML.DateEditComponent = DateEditComponent;
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBase.clearDiv(tag, DateEditComponent.SELECTOR, function (control) {
            control.destroy();
        });
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=DateEdit.js.map
