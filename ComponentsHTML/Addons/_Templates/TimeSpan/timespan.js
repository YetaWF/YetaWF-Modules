"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    //interface Setup {
    //}
    var TimeSpanEditComponent = /** @class */ (function () {
        function TimeSpanEditComponent(controlId /*, setup: Setup*/) {
            var _this = this;
            this.Control = $YetaWF.getElementById(controlId);
            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]);
            this.InputDays = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond("input[name$='Days']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputHours = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond("input[name$='Hours']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputMins = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond("input[name$='Minutes']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputSecs = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond("input[name$='Seconds']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Control]);
            // capture changes in all edit controls
            if (this.InputDays) {
                this.InputDays.Control.addEventListener("intvalue_change", function (evt) {
                    _this.updateValue();
                });
            }
            if (this.InputHours) {
                this.InputHours.Control.addEventListener("intvalue_change", function (evt) {
                    _this.updateValue();
                });
            }
            if (this.InputMins) {
                this.InputMins.Control.addEventListener("intvalue_change", function (evt) {
                    _this.updateValue();
                });
            }
            if (this.InputSecs) {
                this.InputSecs.Control.addEventListener("intvalue_change", function (evt) {
                    _this.updateValue();
                });
            }
        }
        TimeSpanEditComponent.prototype.updateValue = function () {
            if (this.InputDays && this.InputHours && this.InputMins && this.InputSecs) {
                this.Hidden.value = this.InputDays.value + "." + this.InputHours.value + ":" + this.InputMins.value + ":" + this.InputSecs.value;
            }
            else if (this.InputDays && this.InputHours && this.InputMins) {
                this.Hidden.value = this.InputDays.value + "." + this.InputHours.value + ":" + this.InputMins.value;
            }
            if (this.InputHours && this.InputMins && this.InputSecs) {
                this.Hidden.value = this.InputHours.value + ":" + this.InputMins.value + ":" + this.InputSecs.value;
            }
            else if (this.InputHours && this.InputMins) {
                this.Hidden.value = this.InputHours.value + ":" + this.InputMins.value;
            }
        };
        return TimeSpanEditComponent;
    }());
    YetaWF_ComponentsHTML.TimeSpanEditComponent = TimeSpanEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
