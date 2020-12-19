"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */
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
var YetaWF_Scheduler;
(function (YetaWF_Scheduler) {
    var EventEditComponent = /** @class */ (function (_super) {
        __extends(EventEditComponent, _super);
        function EventEditComponent(controlId) {
            var _this = _super.call(this, controlId, EventEditComponent.TEMPLATE, EventEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: function (control) {
                    return control.DropDown.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.DropDown.enable(enable);
                },
            }) || this;
            _this.DropDown = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.DropDown']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [_this.Control]);
            _this.Name = $YetaWF.getElement1BySelector("input[name$='.Name']", [_this.Control]);
            _this.ImplementingAssembly = $YetaWF.getElement1BySelector("input[name$='.ImplementingAssembly']", [_this.Control]);
            _this.ImplementingType = $YetaWF.getElement1BySelector("input[name$='.ImplementingType']", [_this.Control]);
            _this.ElemImplementingAssembly = $YetaWF.getElement1BySelector(".t_implasm", [_this.Control]);
            _this.ElemImplementingType = $YetaWF.getElement1BySelector(".t_impltype", [_this.Control]);
            _this.ElemDescription = $YetaWF.getElement1BySelector(".t_description", [_this.Control]);
            _this.update();
            _this.DropDown.Control.addEventListener("dropdownlist_change", function (evt) {
                _this.update();
            });
            return _this;
        }
        EventEditComponent.prototype.update = function () {
            var val = this.DropDown.value;
            var args = val.split(",");
            this.Name.value = args[0];
            this.ImplementingAssembly.value = args[2];
            this.ImplementingType.value = args[1];
            this.ElemImplementingAssembly.innerText = args[2];
            this.ElemImplementingType.innerText = args[1];
            var tip = this.DropDown.getToolTip(this.DropDown.selectedIndex);
            this.ElemDescription.innerText = tip || "";
        };
        EventEditComponent.TEMPLATE = "yt_yetawf_scheduler_event";
        EventEditComponent.SELECTOR = ".yt_yetawf_scheduler_event.t_edit";
        return EventEditComponent;
    }(YetaWF.ComponentBaseNoDataImpl));
    YetaWF_Scheduler.EventEditComponent = EventEditComponent;
})(YetaWF_Scheduler || (YetaWF_Scheduler = {}));

//# sourceMappingURL=EventEdit.js.map
