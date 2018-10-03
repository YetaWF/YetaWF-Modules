"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
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
            var _this = _super.call(this, controlId) || this;
            _this.Setup = setup;
            _this.GridAll = YetaWF_ComponentsHTML.Grid.getControlById(_this.Setup.GridAllId);
            _this.inputHidden = $YetaWF.getElementById(_this.Setup.HiddenId);
            _this.inputName = $YetaWF.getElementById(_this.Setup.NameId);
            _this.buttonClear = $YetaWF.getElement1BySelector(".t_clear", [_this.Control]);
            $YetaWF.registerEventHandler(_this.buttonClear, "click", null, function (ev) {
                _this.inputHidden.value = "0";
                _this.inputName.value = "";
                return false;
            });
            _this.GridAll.Control.addEventListener("grid_selectionchange", function (evt) {
                var index = _this.GridAll.SelectedIndex();
                var tr = _this.GridAll.GetTR(index);
                var tdName = tr.children[0];
                var inputUserId = $YetaWF.getElement1BySelector("input[name$='.UserId']", [tdName]);
                var name = tdName.innerText.trim();
                _this.inputName.value = name;
                _this.inputHidden.value = inputUserId.value;
            });
            return _this;
        }
        return UserIdEditComponent;
    }(YetaWF.ComponentBase));
    YetaWF_Identity.UserIdEditComponent = UserIdEditComponent;
})(YetaWF_Identity || (YetaWF_Identity = {}));

//# sourceMappingURL=UserIdEdit.js.map
