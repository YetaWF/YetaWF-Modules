"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */
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
var YetaWF_DevTests;
(function (YetaWF_DevTests) {
    var ListOfEmailAddressesEditComponent = /** @class */ (function (_super) {
        __extends(ListOfEmailAddressesEditComponent, _super);
        function ListOfEmailAddressesEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, ListOfEmailAddressesEditComponent.TEMPLATE, ListOfEmailAddressesEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.ReloadInProgress = false;
            _this.AddCounter = 0;
            _this.Setup = setup;
            _this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(_this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            _this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [_this.Control]);
            _this.inputEmail = $YetaWF.getElement1BySelector("input[name$='.NewValue']", [_this.Control]);
            $YetaWF.registerEventHandler(_this.buttonAdd, "click", null, function (ev) {
                if (_this.ReloadInProgress)
                    return true;
                _this.ReloadInProgress = true;
                var uri = $YetaWF.parseUrl(_this.Setup.AddUrl);
                uri.addFormInfo(_this.Control);
                var uniqueIdCounters = { UniqueIdPrefix: "".concat(_this.ControlId, "ls"), UniqueIdPrefixCounter: 0, UniqueIdCounter: ++_this.AddCounter };
                uri.addSearch(YConfigs.Forms.UniqueIdCounters, JSON.stringify(uniqueIdCounters));
                uri.addSearch("newEmailAddress", _this.inputEmail.value.trim());
                uri.addSearch("fieldPrefix", _this.Grid.FieldName);
                uri.addSearch("data", JSON.stringify(_this.Grid.StaticData));
                if (_this.Grid.ExtraData)
                    uri.addSearchSimpleObject(_this.Grid.ExtraData);
                $YetaWF.post(_this.Setup.AddUrl, uri.toFormData(), function (success, partial) {
                    _this.ReloadInProgress = false;
                    if (success) {
                        _this.Grid.AddRecord(partial.TR, partial.StaticData);
                        _this.inputEmail.value = "";
                    }
                });
                return false;
            });
            $YetaWF.handleInputReturnKeyForButton(_this.inputEmail, _this.buttonAdd);
            $YetaWF.registerMultipleEventHandlers([_this.inputEmail], ["input", "change", "click", "keyup", "paste"], null, function (ev) { _this.toggleButton(); return true; });
            return _this;
        }
        ListOfEmailAddressesEditComponent.prototype.toggleButton = function () {
            var s = this.inputEmail.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        };
        ListOfEmailAddressesEditComponent.TEMPLATE = "yt_yetawf_devtests_listofemailaddresses";
        ListOfEmailAddressesEditComponent.SELECTOR = ".yt_yetawf_devtests_listofemailaddresses.t_edit";
        return ListOfEmailAddressesEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_DevTests.ListOfEmailAddressesEditComponent = ListOfEmailAddressesEditComponent;
})(YetaWF_DevTests || (YetaWF_DevTests = {}));

//# sourceMappingURL=ListOfEmailAddressesEdit.js.map
