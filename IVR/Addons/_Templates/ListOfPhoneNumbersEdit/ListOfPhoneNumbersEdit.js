"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */
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
var Softelvdm_IVR;
(function (Softelvdm_IVR) {
    var ListOfPhoneNumbersEditComponent = /** @class */ (function (_super) {
        __extends(ListOfPhoneNumbersEditComponent, _super);
        function ListOfPhoneNumbersEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, ListOfPhoneNumbersEditComponent.TEMPLATE, ListOfPhoneNumbersEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.AddCounter = 0;
            _this.Setup = setup;
            _this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(_this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            _this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [_this.Control]);
            _this.inputPhoneNumber = $YetaWF.getElement1BySelector("input[name$='.NewPhoneNumber']", [_this.Control]);
            $YetaWF.registerEventHandler(_this.buttonAdd, "click", null, function (ev) {
                if ($YetaWF.isLoading)
                    return true;
                var uri = $YetaWF.parseUrl(_this.Setup.AddUrl);
                var query = {
                    NewPhoneNumber: _this.inputPhoneNumber.value,
                    SMS: false,
                    FieldPrefix: _this.Grid.FieldName,
                };
                var data = {
                    GridData: _this.Grid.StaticData
                };
                if (_this.Grid.ExtraData)
                    uri.addSearchSimpleObject(_this.Grid.ExtraData);
                var formJson = $YetaWF.Forms.getJSONInfo(_this.Control, { UniqueIdPrefix: "".concat(_this.ControlId, "ls"), UniqueIdPrefixCounter: 0, UniqueIdCounter: ++_this.AddCounter });
                $YetaWF.postJSON(uri, formJson, query, data, function (success, partial) {
                    if (success) {
                        _this.Grid.AddRecord(partial.TR, partial.StaticData);
                        _this.inputPhoneNumber.value = "";
                        _this.toggleButton();
                    }
                });
                return false;
            });
            $YetaWF.handleInputReturnKeyForButton(_this.inputPhoneNumber, _this.buttonAdd);
            $YetaWF.registerMultipleEventHandlers([_this.inputPhoneNumber], ["input", "change", "click", "keyup", "paste"], null, function (ev) { _this.toggleButton(); return true; });
            return _this;
        }
        ListOfPhoneNumbersEditComponent.prototype.toggleButton = function () {
            var s = this.inputPhoneNumber.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        };
        ListOfPhoneNumbersEditComponent.TEMPLATE = "yt_softelvdm_ivr_listofphonenumbers";
        ListOfPhoneNumbersEditComponent.SELECTOR = ".yt_softelvdm_ivr_listofphonenumbers.t_edit";
        return ListOfPhoneNumbersEditComponent;
    }(YetaWF.ComponentBaseNoDataImpl));
    Softelvdm_IVR.ListOfPhoneNumbersEditComponent = ListOfPhoneNumbersEditComponent;
})(Softelvdm_IVR || (Softelvdm_IVR = {}));

//# sourceMappingURL=ListOfPhoneNumbersEdit.js.map
