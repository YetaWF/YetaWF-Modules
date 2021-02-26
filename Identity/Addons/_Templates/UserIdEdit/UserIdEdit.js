"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_Identity;
(function (YetaWF_Identity) {
    var UserIdEditComponent = /** @class */ (function (_super) {
        __extends(UserIdEditComponent, _super);
        function UserIdEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, UserIdEditComponent.TEMPLATE, UserIdEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: function (control) {
                    return control.inputHidden.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (clearOnDisable)
                        control.clear();
                },
            }) || this;
            _this.Setup = setup;
            _this.GridAll = YetaWF.ComponentBaseDataImpl.getControlById(_this.Setup.GridAllId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            _this.inputHidden = $YetaWF.getElementById(_this.Setup.HiddenId);
            _this.inputName = $YetaWF.getElementById(_this.Setup.NameId);
            _this.buttonClear = $YetaWF.getElement1BySelector(".t_clear", [_this.Control]);
            $YetaWF.registerEventHandler(_this.buttonClear, "click", null, function (ev) {
                _this.inputHidden.value = "0";
                _this.inputName.value = "";
                FormsSupport.validateElement(_this.inputHidden, function (value) {
                    return (value && value !== "0");
                });
                return false;
            });
            _this.GridAll.Control.addEventListener("grid_selectionchange", function (evt) {
                var index = _this.GridAll.SelectedIndex();
                if (index < 0)
                    return;
                var tr = _this.GridAll.GetTR(index);
                var tdName = tr.children[0];
                var inputUserId = $YetaWF.getElement1BySelector("input[name$='.UserId']", [tdName]);
                var name = tdName.innerText.trim();
                _this.inputName.value = name;
                _this.inputHidden.value = inputUserId.value;
                FormsSupport.validateElement(_this.inputHidden, function (value) {
                    return (value && value !== "0");
                });
            });
            return _this;
        }
        UserIdEditComponent.prototype.enable = function (enabled) {
            $YetaWF.elementEnableToggle(this.inputHidden, enabled);
            this.GridAll.enable(enabled);
        };
        UserIdEditComponent.prototype.clear = function () {
            this.inputName.value = "";
            this.inputHidden.value = "0";
            this.GridAll.ClearSelection();
        };
        UserIdEditComponent.TEMPLATE = "yt_yetawf_identity_userid";
        UserIdEditComponent.SELECTOR = ".yt_yetawf_identity_userid.t_large.t_edit";
        return UserIdEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_Identity.UserIdEditComponent = UserIdEditComponent;
})(YetaWF_Identity || (YetaWF_Identity = {}));

//# sourceMappingURL=UserIdEdit.js.map
