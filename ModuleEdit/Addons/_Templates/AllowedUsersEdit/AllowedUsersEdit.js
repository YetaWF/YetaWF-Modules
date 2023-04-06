"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */
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
var YetaWF_ModuleEdit;
(function (YetaWF_ModuleEdit) {
    var AllowedUsersEditComponent = /** @class */ (function (_super) {
        __extends(AllowedUsersEditComponent, _super);
        function AllowedUsersEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, AllowedUsersEditComponent.TEMPLATE, AllowedUsersEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.AddCounter = 0;
            _this.Setup = setup;
            _this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(_this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            _this.GridAll = YetaWF.ComponentBaseDataImpl.getControlById(_this.Setup.GridAllId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            _this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [_this.Control]);
            _this.inputUserName = $YetaWF.getElement1BySelector("input[name$='.NewValue']", [_this.Control]);
            $YetaWF.registerEventHandler(_this.buttonAdd, "click", null, function (ev) {
                if ($YetaWF.isLoading)
                    return true;
                var uri = $YetaWF.parseUrl(_this.Setup.AddUrl);
                var query = {
                    NewUser: _this.inputUserName.value,
                    FieldPrefix: _this.Grid.FieldName,
                };
                var formJson = $YetaWF.Forms.getJSONInfo(_this.Control, { UniqueIdPrefix: "".concat(_this.ControlId, "ls"), UniqueIdPrefixCounter: 0, UniqueIdCounter: ++_this.AddCounter });
                var data = {
                    GridData: _this.Grid.StaticData,
                    __ModuleGuid: formJson.ModuleGuid,
                    __UniqueIdCounters: formJson.UniqueIdCounters,
                };
                if (_this.Grid.ExtraData)
                    uri.addSearchSimpleObject(_this.Grid.ExtraData);
                $YetaWF.postJSON(uri, formJson, query, data, function (success, partial) {
                    if (success) {
                        _this.Grid.AddRecord(partial.TR, partial.StaticData);
                        _this.inputUserName.value = "";
                        _this.toggleButton();
                    }
                });
                return false;
            });
            $YetaWF.handleInputReturnKeyForButton(_this.inputUserName, _this.buttonAdd);
            $YetaWF.registerMultipleEventHandlers([_this.inputUserName], ["input", "change", "click", "keyup", "paste"], null, function (ev) { _this.toggleButton(); return true; });
            _this.GridAll.Control.addEventListener(YetaWF_ComponentsHTML.Grid.EVENTSELECT, function (evt) {
                var index = _this.GridAll.SelectedIndex();
                if (index < 0)
                    return;
                var td = $YetaWF.getElement1BySelector("td", [_this.GridAll.GetTR(index)]);
                var name = td.innerText.trim();
                _this.inputUserName.value = name;
                _this.toggleButton();
            });
            return _this;
        }
        AllowedUsersEditComponent.prototype.toggleButton = function () {
            var s = this.inputUserName.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        };
        AllowedUsersEditComponent.TEMPLATE = "yt_yetawf_moduleedit_allowedusers";
        AllowedUsersEditComponent.SELECTOR = ".yt_yetawf_moduleedit_allowedusers.t_edit";
        return AllowedUsersEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ModuleEdit.AllowedUsersEditComponent = AllowedUsersEditComponent;
})(YetaWF_ModuleEdit || (YetaWF_ModuleEdit = {}));

//# sourceMappingURL=AllowedUsersEdit.js.map
