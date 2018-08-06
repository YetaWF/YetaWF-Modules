"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var DayTimeRangeComponent = /** @class */ (function () {
        function DayTimeRangeComponent(controlId) {
            var _this = this;
            this.NoSubmit = YConfigs.Forms.CssFormNoSubmit + " " + YConfigs.Forms.CssFormNoValidate;
            this.Control = $YetaWF.getElementById(controlId);
            this.Additional = $YetaWF.getElement1BySelector("input[name$='.Additional']", [this.Control]);
            this.AddDiv = $YetaWF.getElement1BySelector(".t_add", [this.Control]);
            this.Closed = $YetaWF.getElement1BySelector("input[name$='.Closed']", [this.Control]);
            this.StartDiv = $YetaWF.getElement1BySelector(".t_from", [this.Control]);
            this.EndDiv = $YetaWF.getElement1BySelector(".t_to", [this.Control]);
            this.Start2Div = $YetaWF.getElement1BySelector(".t_from2", [this.Control]);
            this.End2Div = $YetaWF.getElement1BySelector(".t_to2", [this.Control]);
            this.Start = $YetaWF.getElement1BySelector("input[name$='.Start']", [this.Control]);
            this.End = $YetaWF.getElement1BySelector("input[name$='.End']", [this.Control]);
            this.Start2 = $YetaWF.getElement1BySelector("input[name$='.Start2']", [this.Control]);
            this.End2 = $YetaWF.getElement1BySelector("input[name$='.End2']", [this.Control]);
            this.toggleRanges();
            $YetaWF.registerEventHandler(this.Additional, "change", null, function (ev) {
                _this.toggleRanges();
                return false;
            });
            $YetaWF.registerEventHandler(this.Closed, "change", null, function (ev) {
                _this.toggleRanges();
                return false;
            });
        }
        DayTimeRangeComponent.prototype.toggleRanges = function () {
            if (this.Closed.checked) {
                this.StartDiv.style.display = "none";
                this.EndDiv.style.display = "none";
                this.Start2Div.style.display = "none";
                this.End2Div.style.display = "none";
                this.AddDiv.style.display = "none";
                $YetaWF.elementAddClasses(this.Start, this.NoSubmit);
                $YetaWF.elementAddClasses(this.End, this.NoSubmit);
                $YetaWF.elementAddClasses(this.Start2, this.NoSubmit);
                $YetaWF.elementAddClasses(this.End2, this.NoSubmit);
            }
            else {
                this.StartDiv.style.display = "";
                this.EndDiv.style.display = "";
                this.AddDiv.style.display = "";
                $YetaWF.elementRemoveClasses(this.Start, this.NoSubmit);
                $YetaWF.elementRemoveClasses(this.End, this.NoSubmit);
                if (this.Additional.checked) {
                    this.Start2Div.style.display = "";
                    this.End2Div.style.display = "";
                    $YetaWF.elementRemoveClasses(this.Start2, this.NoSubmit);
                    $YetaWF.elementRemoveClasses(this.End2, this.NoSubmit);
                }
                else {
                    this.Start2Div.style.display = "none";
                    this.End2Div.style.display = "none";
                    $YetaWF.elementAddClasses(this.Start2, this.NoSubmit);
                    $YetaWF.elementAddClasses(this.End2, this.NoSubmit);
                }
            }
        };
        return DayTimeRangeComponent;
    }());
    YetaWF_ComponentsHTML.DayTimeRangeComponent = DayTimeRangeComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
$.validator.unobtrusive.adapters.add("daytimerangeto", function (options) {
    options.rules['daytimerangeto'] = {};
    options.messages['daytimerangeto'] = options.message;
});
$.validator.addMethod("daytimerangeto", function (value, element, parameters) {
    if ($YetaWF.elementHasClass(element, YConfigs.Forms.CssFormNoValidate))
        return true;
    var elem = element;
    var isRange1 = $YetaWF.elementClosestCond(elem, ".t_to") != null;
    var control = $YetaWF.elementClosestCond(elem, ".yt_daytimerange");
    if (control == null)
        return false;
    var fromRange;
    if (isRange1)
        fromRange = $YetaWF.getElement1BySelector("input[name$='.Start']", [control]);
    else
        fromRange = $YetaWF.getElement1BySelector("input[name$='.Start2']", [control]);
    try {
        var dtTo = new Date(elem.value);
        var dtFrom = new Date(fromRange.value);
        if (dtTo >= dtFrom)
            return true;
    }
    finally { }
    return false;
});
$.validator.unobtrusive.adapters.add("daytimerangefrom2", function (options) {
    options.rules['daytimerangefrom2'] = {};
    options.messages['daytimerangefrom2'] = options.message;
});
$.validator.addMethod("daytimerangefrom2", function (value, element, parameters) {
    if ($YetaWF.elementHasClass(element, YConfigs.Forms.CssFormNoValidate))
        return true;
    var elem = element;
    var control = $YetaWF.elementClosestCond(elem, ".yt_daytimerange");
    if (control == null)
        return false;
    var endRange1 = $YetaWF.getElement1BySelector("input[name$='.End']", [control]);
    try {
        var dtFrom2 = new Date(elem.value);
        var dtTo1 = new Date(endRange1.value);
        if (dtFrom2 >= dtTo1)
            return true;
    }
    finally { }
    return false;
});

//# sourceMappingURL=DayTimeRange.js.map
