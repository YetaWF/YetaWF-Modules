"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
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
        function DateEditComponent(controlId) {
            var _this = _super.call(this, controlId) || this;
            $YetaWF.addObjectDataById(controlId, _this);
            _this.DatePicker = $YetaWF.getElement1BySelector("input[name='dtpicker']", [_this.Control]);
            _this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [_this.Control]);
            var sd = new Date(1900, 1 - 1, 1);
            var y = _this.DatePicker.getAttribute("data-min-y");
            if (y != null)
                sd = new Date(Number(y), Number(_this.DatePicker.getAttribute("data-min-m")) - 1, Number(_this.DatePicker.getAttribute("data-min-d")));
            y = _this.DatePicker.getAttribute("data-max-y");
            var ed = new Date(2199, 12 - 1, 31);
            if (y != null)
                ed = new Date(Number(y), Number(_this.DatePicker.getAttribute("data-max-m")) - 1, Number(_this.DatePicker.getAttribute("data-max-d")));
            var thisObj = _this;
            $(_this.DatePicker).kendoDatePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateFormat,
                min: sd, max: ed,
                culture: YVolatile.Basics.Language,
                change: function (ev) {
                    var kdPicker = ev.sender;
                    var val = kdPicker.value();
                    if (val == null)
                        thisObj.setHiddenText(kdPicker.element.val());
                    else
                        thisObj.setHiddenValue(val);
                    FormsSupport.validateElement(_this.Hidden);
                    var event = document.createEvent("Event");
                    event.initEvent("date_change", false, true);
                    _this.Control.dispatchEvent(event);
                }
            });
            _this.KendoDatePicker = $(_this.DatePicker).data("kendoDatePicker");
            _this.setHiddenValue(_this.KendoDatePicker.value());
            _this.DatePicker.addEventListener("change", function (event) {
                var val = _this.KendoDatePicker.value();
                if (val == null)
                    thisObj.setHiddenText(event.target.value);
                else
                    thisObj.setHiddenValue(val);
                FormsSupport.validateElement(_this.Hidden);
            }, false);
            return _this;
        }
        DateEditComponent.prototype.setHiddenText = function (dateVal) {
            this.Hidden.setAttribute("value", dateVal ? dateVal : "");
        };
        DateEditComponent.prototype.setHiddenValue = function (dateVal) {
            var s = "";
            if (dateVal != null)
                s = dateVal.toUTCString();
            this.Hidden.setAttribute("value", s);
        };
        Object.defineProperty(DateEditComponent.prototype, "value", {
            get: function () {
                return this.DatePicker.value;
            },
            enumerable: true,
            configurable: true
        });
        DateEditComponent.prototype.clear = function () {
            this.KendoDatePicker.value("");
        };
        DateEditComponent.prototype.enable = function (enabled) {
            this.KendoDatePicker.enable(enabled);
        };
        DateEditComponent.prototype.destroy = function () {
            this.KendoDatePicker.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        };
        DateEditComponent.getControlFromTag = function (elem) { return _super.getControlBaseFromTag.call(this, elem, DateEditComponent.SELECTOR); };
        DateEditComponent.getControlFromSelector = function (selector, tags) { return _super.getControlBaseFromSelector.call(this, selector, DateEditComponent.SELECTOR, tags); };
        DateEditComponent.SELECTOR = ".yt_date.t_edit";
        return DateEditComponent;
    }(YetaWF.ComponentBase));
    YetaWF_ComponentsHTML.DateEditComponent = DateEditComponent;
    var DateGridComponent = /** @class */ (function () {
        function DateGridComponent(gridId, elem) {
            var _this = this;
            this.Grid = $YetaWF.getElementById(gridId);
            // Build a kendo date picker
            // We have to add it next to the jqgrid provided input field elem
            // We can't use the jqgrid provided element as a kendodatepicker because jqgrid gets confused and
            // uses the wrong sorting option. So we add the datepicker next to the "official" input field (which we hide)
            var dtPick = $YetaWF.createElement("input", { name: "dtpicker" });
            elem.insertAdjacentElement("afterend", dtPick);
            // Hide the jqgrid provided input element (we update the date in this hidden element)
            elem.style.display = "none";
            // init date picker
            $(dtPick).kendoDatePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateFormat,
                //sb.Append("min: sd, max: ed,");
                culture: YVolatile.Basics.Language,
                change: function (ev) {
                    var kdPicker = ev.sender;
                    var val = kdPicker.value();
                    var s = "";
                    if (val !== null) {
                        var utcDate = new Date(Date.UTC(val.getFullYear(), val.getMonth(), val.getDate(), 0, 0, 0));
                        s = utcDate.toUTCString();
                    }
                    elem.setAttribute("value", s);
                }
            });
            dtPick.addEventListener("keydown", function (event) {
                if (event.keyCode === 13)
                    _this.Grid.triggerToolbar();
            }, false);
        }
        return DateGridComponent;
    }());
    YetaWF_ComponentsHTML.DateGridComponent = DateGridComponent;
    // A <div> is being emptied. Destroy all date pickers the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBase.clearDiv(tag, DateEditComponent.SELECTOR, function (control) {
            control.destroy();
        });
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=DateEdit.js.map
