"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var CheckListMenuEditComponent = /** @class */ (function (_super) {
        __extends(CheckListMenuEditComponent, _super);
        function CheckListMenuEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, CheckListMenuEditComponent.TEMPLATE, CheckListMenuEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            //this.Setup = setup;
            _this.Button = $YetaWF.getElement1BySelector("button", [_this.Control]);
            _this.Menu = $YetaWF.getElement1BySelector("ul", [_this.Control]);
            $YetaWF.registerEventHandler(_this.Button, "mousedown", null, function (ev) {
                if (!YetaWF_ComponentsHTML.MenuULComponent.closeMenus())
                    _this.openMenu();
                return false;
            });
            $YetaWF.registerEventHandler(_this.Button, "click", null, function (ev) {
                return false;
            });
            return _this;
        }
        CheckListMenuEditComponent.prototype.openMenu = function () {
            var _this = this;
            var menu = this.Menu.cloneNode(true);
            menu.id = "".concat(this.Menu.id, "_live");
            // update checkboxes from hidden fields
            var lis = $YetaWF.getElementsBySelector("li", [menu]);
            for (var _i = 0, lis_1 = lis; _i < lis_1.length; _i++) {
                var li = lis_1[_i];
                var name_1 = $YetaWF.getAttribute(li, "data-name");
                var input = $YetaWF.getElement1BySelector("input[name$='.Value'][data-name='".concat(name_1, "']"), [this.Control]);
                var check = $YetaWF.getElement1BySelector("li[data-name='".concat(name_1, "'] input[type=\"checkbox\"]"), [menu]);
                check.checked = input.value === "True";
            }
            document.body.appendChild(menu);
            new YetaWF_ComponentsHTML.MenuULComponent(menu.id, {
                "Owner": this.Menu,
                "AutoOpen": true,
                "AutoRemove": true,
                "AttachTo": this.Button,
                "Dynamic": true,
                "RightAlign": true,
                "CloseOnClick": false,
                "Click": function (liElem, target) {
                    setTimeout(function () {
                        var check = $YetaWF.getElement1BySelector("input", [liElem]);
                        check.checked = !check.checked;
                        // update hidden field
                        var name = $YetaWF.getAttribute(liElem, "data-name");
                        var input = $YetaWF.getElement1BySelector("input[name$='.Value'][data-name='".concat(name, "']"), [_this.Control]);
                        input.value = check.checked ? "True" : "False";
                        _this.sendChangeEvent();
                    }, 1);
                }
            });
        };
        CheckListMenuEditComponent.prototype.sendChangeEvent = function () {
            $YetaWF.sendCustomEvent(this.Control, CheckListMenuEditComponent.EVENTCHANGE);
        };
        // API
        CheckListMenuEditComponent.prototype.getValueEntries = function () {
            var entries = [];
            var lis = $YetaWF.getElementsBySelector("li", [this.Menu]);
            for (var _i = 0, lis_2 = lis; _i < lis_2.length; _i++) {
                var li = lis_2[_i];
                var name_2 = $YetaWF.getAttribute(li, "data-name");
                var input = $YetaWF.getElement1BySelector("input[name$='.Value'][data-name='".concat(name_2, "']"), [this.Control]);
                entries.push({ Name: name_2, Checked: input.value === "True" });
            }
            return entries;
        };
        CheckListMenuEditComponent.prototype.replaceValues = function (values) {
            var lis = $YetaWF.getElementsBySelector("li", [this.Menu]);
            if (lis.length !== values.length)
                throw "replaceValues expected ".concat(lis.length, " values, received ").concat(values.length);
            var valIndex = 0;
            for (var _i = 0, lis_3 = lis; _i < lis_3.length; _i++) {
                var li = lis_3[_i];
                var name_3 = $YetaWF.getAttribute(li, "data-name");
                var input = $YetaWF.getElement1BySelector("input[name$='.Value'][data-name='".concat(name_3, "']"), [this.Control]);
                input.value = values[valIndex] ? "True" : "False";
                ++valIndex;
            }
        };
        CheckListMenuEditComponent.TEMPLATE = "yt_checklistmenu";
        CheckListMenuEditComponent.SELECTOR = ".yt_checklistmenu.t_edit";
        CheckListMenuEditComponent.EVENTCHANGE = "checklistmenu_change";
        return CheckListMenuEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.CheckListMenuEditComponent = CheckListMenuEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=CheckListMenuEdit.js.map
