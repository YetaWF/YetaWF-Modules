/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */
var YetaWF_CoreComponentsHTML;
(function (YetaWF_CoreComponentsHTML) {
    var DateTime;
    (function (DateTime) {
        var Template = /** @class */ (function () {
            function Template() {
            }
            Template.prototype.getGrid = function (ctrlId) {
                var el = document.getElementById(ctrlId);
                if (el == null)
                    throw "Grid element " + ctrlId + " not found"; /*DEBUG*/
                return el;
            };
            Template.prototype.getControl = function (ctrlId) {
                var el = document.getElementById(ctrlId);
                if (el == null)
                    throw "Element " + ctrlId + " not found"; /*DEBUG*/
                return el;
            };
            Template.prototype.getHidden = function (ctrl) {
                var hidden = ctrl.querySelector("input[type=\"hidden\"]");
                if (hidden == null)
                    throw "Couldn't find hidden field"; /*DEBUG*/
                return hidden;
            };
            Template.prototype.setHidden = function (hidden, dateVal) {
                var s = "";
                if (dateVal != null) {
                    s = dateVal.toUTCString();
                }
                hidden.setAttribute("value", s);
            };
            Template.prototype.setHiddenText = function (hidden, dateVal) {
                hidden.setAttribute("value", dateVal ? dateVal : "");
            };
            Template.prototype.getDate = function (ctrl) {
                var date = ctrl.querySelector("input[name=\"dtpicker\"]");
                if (date == null)
                    throw "Couldn't find datetime field"; /*DEBUG*/
                return date;
            };
            /**
             * Initializes one instance of a Date template control.
             * @param ctrlId - The HTML id of the date template control.
             */
            Template.prototype.init = function (ctrlId) {
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
                $(date).kendoDateTimePicker({
                    animation: false,
                    format: YVolatile.DateTime.DateTimeFormat,
                    min: sd, max: ed,
                    culture: YConfigs.Basics.Language,
                    change: function (ev) {
                        var kdPicker = ev.sender;
                        var val = kdPicker.value();
                        if (val == null)
                            thisObj.setHiddenText(hidden, kdPicker.element.val());
                        else
                            thisObj.setHidden(hidden, val);
                        YetaWF_Core.Forms.ValidateElement(hidden);
                    }
                });
                var kdPicker = $(date).data("kendoDateTimePicker");
                this.setHidden(hidden, kdPicker.value());
                function changeHandler(event) {
                    var val = kdPicker.value();
                    if (val == null)
                        thisObj.setHiddenText(hidden, event.target.value);
                    else
                        thisObj.setHidden(hidden, val);
                    YetaWF_Core.Forms.ValidateElement(hidden);
                }
                date.addEventListener("change", changeHandler, false);
            };
            /**
             * Renders a date picker in the jqGrid filter toolbar.
             * @param gridId - The id of the grid containing the date picker.
             * @param elem - The element containing the date value.
             */
            Template.prototype.renderjqGridFilter = function (gridId, elem) {
                var grid = this.getGrid(gridId);
                // Build a kendo date picker
                // We have to add it next to the jqgrid provided input field elem
                // We can't use the jqgrid provided element as a kendoDateTimePicker because jqgrid gets confused and
                // uses the wrong sorting option. So we add the datepicker next to the "official" input field (which we hide)
                var dtPick = <input name="dtpicker"/>;
                elem.insertAdjacentElement("afterend", dtPick);
                // Hide the jqgrid provided input element (we update the date in this hidden element)
                elem.style.display = "none";
                // init date picker
                $(dtPick).kendoDateTimePicker({
                    animation: false,
                    format: YVolatile.DateTime.DateTimeFormat,
                    //sb.Append("min: sd, max: ed,");
                    culture: YConfigs.Basics.Language,
                    change: function (ev) {
                        var kdPicker = ev.sender;
                        var val = kdPicker.value();
                        var s = "";
                        if (val !== null) {
                            s = val.toUTCString();
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
            return Template;
        }());
        DateTime.Template = Template;
        // A <div> is being emptied. Destroy all date/time pickers the <div> may contain.
        YetaWF_Basics.addClearDiv(function (tag) {
            var list = tag.querySelectorAll(".yt_datetime.t_edit input[name=\"dtpicker\"]");
            var len = list.length;
            for (var i = 0; i < len; ++i) {
                var el = list[i];
                var datepicker = $(el).data("kendoDateTimePicker");
                if (!datepicker)
                    throw "No kendo object found"; /*DEBUG*/
                datepicker.destroy();
            }
        });
    })(DateTime = YetaWF_CoreComponentsHTML.DateTime || (YetaWF_CoreComponentsHTML.DateTime = {}));
})(YetaWF_CoreComponentsHTML || (YetaWF_CoreComponentsHTML = {}));