"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var DateComponent = /** @class */ (function () {
        function DateComponent() {
        }
        DateComponent.prototype.getGrid = function (ctrlId) {
            var el = document.getElementById(ctrlId);
            if (el == null)
                throw "Grid element " + ctrlId + " not found"; /*DEBUG*/
            return el;
        };
        DateComponent.prototype.getControl = function (ctrlId) {
            var el = document.getElementById(ctrlId);
            if (el == null)
                throw "Element " + ctrlId + " not found"; /*DEBUG*/
            return el;
        };
        DateComponent.prototype.getHidden = function (ctrl) {
            var hidden = ctrl.querySelector("input[type=\"hidden\"]");
            if (hidden == null)
                throw "Couldn't find hidden field"; /*DEBUG*/
            return hidden;
        };
        DateComponent.prototype.setHidden = function (hidden, dateVal) {
            var s = "";
            if (dateVal != null) {
                var utcDate = new Date(Date.UTC(dateVal.getFullYear(), dateVal.getMonth(), dateVal.getDate(), 0, 0, 0));
                s = utcDate.toUTCString();
            }
            hidden.setAttribute("value", s);
        };
        DateComponent.prototype.setHiddenText = function (hidden, dateVal) {
            hidden.setAttribute("value", dateVal ? dateVal : "");
        };
        DateComponent.prototype.getDate = function (ctrl) {
            var date = ctrl.querySelector("input[name=\"dtpicker\"]");
            if (date == null)
                throw "Couldn't find date field"; /*DEBUG*/
            return date;
        };
        /**
         * Initializes one instance of a Date template control.
         * @param ctrlId - The HTML id of the date template control.
         */
        DateComponent.prototype.init = function (ctrlId) {
            var thisObj = this;
            var ctrl = this.getControl(ctrlId);
            var hidden = this.getHidden(ctrl);
            var date = this.getDate(ctrl);
            var sd = new Date(1900, 1 - 1, 1);
            var d = date.getAttribute("data-min-y");
            if (d != null)
                sd = new Date(Number(date.getAttribute("data-min-y")), Number(date.getAttribute("data-min-m")) - 1, Number(date.getAttribute("data-min-d")));
            d = date.getAttribute("data-max-y");
            var ed = new Date(2199, 12 - 1, 31);
            if (d != null)
                ed = new Date(Number(date.getAttribute("data-max-y")), Number(date.getAttribute("data-max-m")) - 1, Number(date.getAttribute("data-max-d")));
            $(date).kendoDatePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateFormat,
                min: sd, max: ed,
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
            var kdPicker = $(date).data("kendoDatePicker");
            this.setHidden(hidden, kdPicker.value());
            function changeHandler(event) {
                var val = kdPicker.value();
                if (val == null)
                    thisObj.setHiddenText(hidden, event.target.value);
                else
                    thisObj.setHidden(hidden, val);
                FormsSupport.validateElement(hidden);
            }
            date.addEventListener("change", changeHandler, false);
        };
        /**
         * Renders a date picker in the jqGrid filter toolbar.
         * @param gridId - The id of the grid containing the date picker.
         * @param elem - The element containing the date value.
         */
        DateComponent.prototype.renderjqGridFilter = function (gridId, elem) {
            var grid = this.getGrid(gridId);
            // Build a kendo date picker
            // We have to add it next to the jqgrid provided input field elem
            // We can't use the jqgrid provided element as a kendodatepicker because jqgrid gets confused and
            // uses the wrong sorting option. So we add the datepicker next to the "official" input field (which we hide)
            var dtPick = YetaWF_Basics.createElement("input", { name: "dtpicker" });
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
            /**
             * Handles Return key in Date picker, part of jqGrid filter toolbar.
             * @param event
             */
            function keydownHandler(event) {
                if (event.keyCode === 13)
                    grid.triggerToolbar();
            }
            dtPick.addEventListener("keydown", keydownHandler, false);
        };
        return DateComponent;
    }());
    YetaWF_ComponentsHTML.DateComponent = DateComponent;
    // A <div> is being emptied. Destroy all date/time pickers the <div> may contain.
    YetaWF_Basics.addClearDiv(function (tag) {
        var list = tag.querySelectorAll(".yt_date.t_edit input[name=\"dtpicker\"]");
        var len = list.length;
        for (var i = 0; i < len; ++i) {
            var el = list[i];
            var datepicker = $(el).data("kendoDatePicker");
            if (!datepicker)
                throw "No kendo object found"; /*DEBUG*/
            datepicker.destroy();
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Date.js.map
