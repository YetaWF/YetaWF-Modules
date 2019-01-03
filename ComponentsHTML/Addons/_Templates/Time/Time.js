"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var TimeComponent = /** @class */ (function () {
        function TimeComponent() {
        }
        TimeComponent.prototype.getGrid = function (ctrlId) {
            var el = document.getElementById(ctrlId);
            if (el == null)
                throw "Grid element " + ctrlId + " not found"; /*DEBUG*/
            return el;
        };
        TimeComponent.prototype.getControl = function (ctrlId) {
            var el = document.getElementById(ctrlId);
            if (el == null)
                throw "Element " + ctrlId + " not found"; /*DEBUG*/
            return el;
        };
        TimeComponent.prototype.getHidden = function (ctrl) {
            var hidden = ctrl.querySelector("input[type=\"hidden\"]");
            if (hidden == null)
                throw "Couldn't find hidden field"; /*DEBUG*/
            return hidden;
        };
        TimeComponent.prototype.setHidden = function (hidden, dateVal) {
            var s = "";
            if (dateVal != null) {
                s = dateVal.toISOString();
            }
            hidden.setAttribute("value", s);
        };
        TimeComponent.prototype.setHiddenText = function (hidden, dateVal) {
            hidden.setAttribute("value", dateVal ? dateVal : "");
        };
        TimeComponent.prototype.getDate = function (ctrl) {
            var date = ctrl.querySelector("input[name=\"dtpicker\"]");
            if (date == null)
                throw "Couldn't find time field"; /*DEBUG*/
            return date;
        };
        /**
         * Initializes one instance of a Date template control.
         * @param ctrlId - The HTML id of the date template control.
         */
        TimeComponent.prototype.init = function (ctrlId) {
            var thisObj = this;
            var ctrl = this.getControl(ctrlId);
            var hidden = this.getHidden(ctrl);
            var date = this.getDate(ctrl);
            $(date).kendoTimePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.TimeFormat,
                culture: YVolatile.Basics.Language,
                change: function (ev) {
                    var kdPicker = ev.sender;
                    var val = kdPicker.value();
                    if (val == null)
                        thisObj.setHiddenText(hidden, kdPicker.element.val());
                    else
                        thisObj.setHidden(hidden, val);
                    FormsSupport.validateElement(hidden);
                }
            });
            var kdPicker = $(date).data("kendoTimePicker");
            this.setHidden(hidden, kdPicker.value());
            date.addEventListener("change", function (event) {
                var val = kdPicker.value();
                if (val == null)
                    thisObj.setHiddenText(hidden, event.target.value);
                else
                    thisObj.setHidden(hidden, val);
                FormsSupport.validateElement(hidden);
            }, false);
        };
        /**
         * Renders a time picker in the jqGrid filter toolbar.
         * @param gridId - The id of the grid containing the date picker.
         * @param elem - The element containing the date value.
         */
        TimeComponent.prototype.renderjqGridFilter = function (gridId, elem) {
            var grid = this.getGrid(gridId);
            // Build a kendo time picker
            // We have to add it next to the jqgrid provided input field elem
            // We can't use the jqgrid provided element as a kendoTimePicker because jqgrid gets confused and
            // uses the wrong sorting option. So we add the timepicker next to the "official" input field (which we hide)
            var dtPick = $YetaWF.createElement("input", { name: "dtpicker" });
            elem.insertAdjacentElement("afterend", dtPick);
            // Hide the jqgrid provided input element (we update the time in this hidden element)
            elem.style.display = "none";
            // init time picker
            $(dtPick).kendoTimePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.TimeFormat,
                culture: YVolatile.Basics.Language,
                change: function (ev) {
                    var kdPicker = ev.sender;
                    var val = kdPicker.value();
                    var s = "";
                    if (val !== null) {
                        s = val.toISOString();
                    }
                    elem.setAttribute("value", s);
                }
            });
            /**
             * Handles Return key in time picker, part of jqGrid filter toolbar.
             * @param event
             */
            dtPick.addEventListener("keydown", function (event) {
                if (event.keyCode === 13)
                    grid.triggerToolbar();
            }, false);
        };
        return TimeComponent;
    }());
    YetaWF_ComponentsHTML.TimeComponent = TimeComponent;
    // A <div> is being emptied. Destroy all time pickers the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        var list = $YetaWF.getElementsBySelector(".yt_time.t_edit input[name=\"dtpicker\"]", [tag]);
        for (var _i = 0, list_1 = list; _i < list_1.length; _i++) {
            var el = list_1[_i];
            var timepicker = $(el).data("kendoTimePicker");
            if (!timepicker)
                throw "No kendo object found"; /*DEBUG*/
            timepicker.destroy();
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Time.js.map
