"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    //interface Setup {
    //}
    var TimeSpanEditComponent = /** @class */ (function () {
        function TimeSpanEditComponent(controlId /*, setup: Setup*/) {
            var _this = this;
            this.Control = $YetaWF.getElementById(controlId);
            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]);
            this.InputDays = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name$='Days']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputHours = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name$='Hours']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputMins = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name$='Minutes']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputSecs = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name$='Seconds']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Control]);
            // capture changes in all edit controls
            this.InputDays.Control.addEventListener("intvalue_change", function (evt) {
                _this.updateValue();
            });
            this.InputHours.Control.addEventListener("intvalue_change", function (evt) {
                _this.updateValue();
            });
            this.InputMins.Control.addEventListener("intvalue_change", function (evt) {
                _this.updateValue();
            });
            this.InputSecs.Control.addEventListener("intvalue_change", function (evt) {
                _this.updateValue();
            });
        }
        TimeSpanEditComponent.prototype.updateValue = function () {
            this.Hidden.value = this.InputDays.value + "." + this.InputHours.value + ":" + this.InputMins.value + ":" + this.InputSecs.value;
        };
        return TimeSpanEditComponent;
    }());
    YetaWF_ComponentsHTML.TimeSpanEditComponent = TimeSpanEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=TimeSpan.js.map
