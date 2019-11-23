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
    var DayTimeRangeEditComponent = /** @class */ (function (_super) {
        __extends(DayTimeRangeEditComponent, _super);
        function DayTimeRangeEditComponent(controlId /*, setup: DayTimeRangeEditSetup*/) {
            var _this = _super.call(this, controlId, DayTimeRangeEditComponent.TEMPLATE, DayTimeRangeEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: function (control, enable, clearOnDisable) {
                    //control.enable(enable);
                    //if (!enable && clearOnDisable)
                    //    control.clear();
                },
            }) || this;
            _this.NoSubmit = "yform-nosubmit yform-novalidate";
            _this.Additional = $YetaWF.getElement1BySelector("input[name$='.Additional']", [_this.Control]);
            _this.AddDiv = $YetaWF.getElement1BySelector(".t_add", [_this.Control]);
            _this.Closed = $YetaWF.getElement1BySelector("input[name$='.Closed']", [_this.Control]);
            _this.ClosedDiv = $YetaWF.getElement1BySelector(".t_closed", [_this.Control]);
            _this.StartDiv = $YetaWF.getElement1BySelector(".t_from", [_this.Control]);
            _this.EndDiv = $YetaWF.getElement1BySelector(".t_to", [_this.Control]);
            _this.Start2Div = $YetaWF.getElement1BySelector(".t_from2", [_this.Control]);
            _this.End2Div = $YetaWF.getElement1BySelector(".t_to2", [_this.Control]);
            _this.Start = $YetaWF.getElement1BySelector("input[name$='.Start']", [_this.Control]);
            _this.End = $YetaWF.getElement1BySelector("input[name$='.End']", [_this.Control]);
            _this.Start2 = $YetaWF.getElement1BySelector("input[name$='.Start2']", [_this.Control]);
            _this.End2 = $YetaWF.getElement1BySelector("input[name$='.End2']", [_this.Control]);
            _this.toggleRanges();
            $YetaWF.registerEventHandler(_this.Additional, "change", null, function (ev) {
                _this.toggleRanges();
                return false;
            });
            $YetaWF.registerEventHandler(_this.Closed, "change", null, function (ev) {
                _this.toggleRanges();
                return false;
            });
            return _this;
        }
        DayTimeRangeEditComponent.prototype.toggleRanges = function () {
            if (this.Closed.checked) {
                this.StartDiv.style.display = "none";
                this.EndDiv.style.display = "none";
                this.Start2Div.style.display = "none";
                this.End2Div.style.display = "none";
                this.AddDiv.style.display = "none";
                $YetaWF.elementAddClassList(this.Start, this.NoSubmit);
                $YetaWF.elementAddClassList(this.End, this.NoSubmit);
                $YetaWF.elementAddClassList(this.Start2, this.NoSubmit);
                $YetaWF.elementAddClassList(this.End2, this.NoSubmit);
            }
            else {
                this.StartDiv.style.display = "";
                this.EndDiv.style.display = "";
                this.AddDiv.style.display = "";
                $YetaWF.elementRemoveClassList(this.Start, this.NoSubmit);
                $YetaWF.elementRemoveClassList(this.End, this.NoSubmit);
                if (this.Additional.checked) {
                    this.Start2Div.style.display = "";
                    this.End2Div.style.display = "";
                    $YetaWF.elementRemoveClassList(this.Start2, this.NoSubmit);
                    $YetaWF.elementRemoveClassList(this.End2, this.NoSubmit);
                    this.ClosedDiv.style.display = "none";
                }
                else {
                    this.Start2Div.style.display = "none";
                    this.End2Div.style.display = "none";
                    $YetaWF.elementAddClassList(this.Start2, this.NoSubmit);
                    $YetaWF.elementAddClassList(this.End2, this.NoSubmit);
                    this.ClosedDiv.style.display = "";
                }
            }
        };
        DayTimeRangeEditComponent.TEMPLATE = "yt_daytimerange";
        DayTimeRangeEditComponent.SELECTOR = ".yt_daytimerange.t_edit";
        return DayTimeRangeEditComponent;
    }(YetaWF.ComponentBaseNoDataImpl));
    YetaWF_ComponentsHTML.DayTimeRangeEditComponent = DayTimeRangeEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
YetaWF_ComponentsHTML_Validation.registerValidator("daytimerangeto", function (form, elem, val) {
    var value = elem.value;
    if (!value)
        return true;
    var isRange1 = $YetaWF.elementClosestCond(elem, ".t_to") != null;
    var control = $YetaWF.elementClosestCond(elem, ".yt_daytimerange");
    if (!control)
        return false;
    var fromRange;
    if (isRange1)
        fromRange = $YetaWF.getElement1BySelector("input[name$='.Start']", [control]);
    else
        fromRange = $YetaWF.getElement1BySelector("input[name$='.Start2']", [control]);
    try {
        var dtTo = new Date(value);
        var dtFrom = new Date(fromRange.value);
        if (dtTo >= dtFrom)
            return true;
    }
    finally { }
    return false;
});
YetaWF_ComponentsHTML_Validation.registerValidator("daytimerangefrom2", function (form, elem, val) {
    var value = elem.value;
    if (!value)
        return true;
    var control = $YetaWF.elementClosestCond(elem, ".yt_daytimerange");
    if (!control)
        return false;
    var endRange1 = $YetaWF.getElement1BySelector("input[name$='.End']", [control]);
    try {
        var dtFrom2 = new Date(value);
        var dtTo1 = new Date(endRange1.value);
        if (dtFrom2 >= dtTo1)
            return true;
    }
    finally { }
    return false;
});

//# sourceMappingURL=DayTimeRangeEdit.js.map
