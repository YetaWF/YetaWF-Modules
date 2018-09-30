"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
var YetaWF_DevTests;
(function (YetaWF_DevTests) {
    var ListOfEmailAddressesEditComponent = /** @class */ (function (_super) {
        __extends(ListOfEmailAddressesEditComponent, _super);
        function ListOfEmailAddressesEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId) || this;
            _this.ReloadInProgress = false;
            _this.AddCounter = 0;
            _this.Setup = setup;
            _this.Grid = Softelvdm_Grid.Grid.getControlById(_this.Setup.GridId);
            _this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [_this.Control]);
            _this.inputEmail = $YetaWF.getElement1BySelector("input[name$='.NewValue']", [_this.Control]);
            $YetaWF.registerEventHandler(_this.buttonAdd, "click", null, function (ev) {
                if (_this.ReloadInProgress)
                    return true;
                _this.ReloadInProgress = true;
                $YetaWF.setLoading(true);
                var uri = $YetaWF.parseUrl(_this.Setup.AddUrl);
                uri.addFormInfo(_this.Control, ++_this.AddCounter);
                uri.addSearch("newEmailAddress", _this.inputEmail.value.trim());
                uri.addSearch("fieldPrefix", _this.Grid.FieldName);
                uri.addSearch("data", JSON.stringify(_this.Grid.StaticData));
                if (_this.Grid.ExtraData)
                    uri.addSearchSimpleObject(_this.Grid.ExtraData);
                var request = new XMLHttpRequest();
                request.open("POST", _this.Setup.AddUrl, true);
                request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                request.onreadystatechange = function (ev) {
                    if (request.readyState === 4 /*DONE*/) {
                        _this.ReloadInProgress = false;
                        $YetaWF.setLoading(false);
                        $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, undefined, undefined, function (result) {
                            var partial = JSON.parse(request.responseText);
                            _this.ReloadInProgress = false;
                            $YetaWF.setLoading(false);
                            _this.Grid.AddRecord(partial.TR, partial.StaticData);
                            _this.inputEmail.value = "";
                        });
                    }
                };
                request.send(uri.toFormData());
                return false;
            });
            $YetaWF.registerMultipleEventHandlers(_this.inputEmail, ["input", "change", "click", "keyup", "paste"], null, function (ev) { _this.toggleButton(); return true; });
            return _this;
        }
        ListOfEmailAddressesEditComponent.prototype.toggleButton = function () {
            var s = this.inputEmail.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        };
        return ListOfEmailAddressesEditComponent;
    }(YetaWF.ComponentBase));
    YetaWF_DevTests.ListOfEmailAddressesEditComponent = ListOfEmailAddressesEditComponent;
})(YetaWF_DevTests || (YetaWF_DevTests = {}));

//# sourceMappingURL=ListOfEmailAddressesEdit.js.map
