"use strict";
/* Copyright � 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */
var YetaWF_Scheduler;
(function (YetaWF_Scheduler) {
    var EventEditComponent = /** @class */ (function () {
        function EventEditComponent(controlId) {
            var _this = this;
            this.Control = $YetaWF.getElementById(controlId);
            this.DropDown = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("select[name$='.DropDown']", [this.Control]);
            this.Name = $YetaWF.getElement1BySelector("input[name$='.Name']", [this.Control]);
            this.ImplementingAssembly = $YetaWF.getElement1BySelector("input[name$='.ImplementingAssembly']", [this.Control]);
            this.ImplementingType = $YetaWF.getElement1BySelector("input[name$='.ImplementingType']", [this.Control]);
            this.ElemImplementingAssembly = $YetaWF.getElement1BySelector(".t_implasm", [this.Control]);
            this.ElemImplementingType = $YetaWF.getElement1BySelector(".t_impltype", [this.Control]);
            this.ElemDescription = $YetaWF.getElement1BySelector(".t_description", [this.Control]);
            this.DropDown.Control.addEventListener("dropdownlist_change", function (evt) {
                var args = _this.DropDown.value.split(",");
                _this.Name.value = args[0];
                _this.ImplementingAssembly.value = args[2];
                _this.ImplementingType.value = args[1];
                _this.ElemImplementingAssembly.innerText = args[2];
                _this.ElemImplementingType.innerText = args[1];
                var tip = _this.DropDown.getToolTip(_this.DropDown.selectedIndex);
                _this.ElemDescription.innerText = tip || "";
            });
        }
        return EventEditComponent;
    }());
    YetaWF_Scheduler.EventEditComponent = EventEditComponent;
})(YetaWF_Scheduler || (YetaWF_Scheduler = {}));

//# sourceMappingURL=EventEdit.js.map
