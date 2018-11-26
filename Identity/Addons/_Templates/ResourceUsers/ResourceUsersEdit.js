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
    var ResourceUsersEditComponent = /** @class */ (function (_super) {
        __extends(ResourceUsersEditComponent, _super);
        function ResourceUsersEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId) || this;
            _this.ReloadInProgress = false;
            _this.AddCounter = 0;
            _this.Setup = setup;
            _this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(_this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            _this.GridAll = YetaWF.ComponentBaseDataImpl.getControlById(_this.Setup.GridAllId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            _this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [_this.Control]);
            _this.inputUserName = $YetaWF.getElement1BySelector("input[name$='.NewValue']", [_this.Control]);
            $YetaWF.registerEventHandler(_this.buttonAdd, "click", null, function (ev) {
                if (_this.ReloadInProgress)
                    return true;
                _this.ReloadInProgress = true;
                $YetaWF.setLoading(true);
                var uri = $YetaWF.parseUrl(_this.Setup.AddUrl);
                uri.addFormInfo(_this.Control, ++_this.AddCounter);
                uri.addSearch("newUser", _this.inputUserName.value);
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
                        });
                    }
                };
                request.send(uri.toFormData());
                return false;
            });
            $YetaWF.registerMultipleEventHandlers(_this.inputUserName, ["input", "change", "click", "keyup", "paste"], null, function (ev) { _this.toggleButton(); return true; });
            _this.GridAll.Control.addEventListener("grid_selectionchange", function (evt) {
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
        ResourceUsersEditComponent.prototype.toggleButton = function () {
            var s = this.inputUserName.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        };
        return ResourceUsersEditComponent;
    }(YetaWF.ComponentBaseImpl));
    YetaWF_Identity.ResourceUsersEditComponent = ResourceUsersEditComponent;
})(YetaWF_Identity || (YetaWF_Identity = {}));

//# sourceMappingURL=ResourceUsersEdit.js.map
