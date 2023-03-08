"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */
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
var YetaWF_Pages;
(function (YetaWF_Pages) {
    var ListOfLocalPagesEditComponent = /** @class */ (function (_super) {
        __extends(ListOfLocalPagesEditComponent, _super);
        function ListOfLocalPagesEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, ListOfLocalPagesEditComponent.TEMPLATE, ListOfLocalPagesEditComponent.SELECTOR, {
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
            _this.SelectUrl = YetaWF.ComponentBaseDataImpl.getControlFromSelector("[name$='.NewValue']", YetaWF_ComponentsHTML.UrlEditComponent.SELECTOR, [_this.Control]);
            $YetaWF.registerEventHandler(_this.buttonAdd, "click", null, function (ev) {
                if ($YetaWF.isLoading)
                    return true;
                var uri = $YetaWF.parseUrl(_this.Setup.AddUrl);
                var query = {
                    NewUrl: _this.SelectUrl.value.trim(),
                    FieldPrefix: _this.Grid.FieldName,
                };
                var formJson = $YetaWF.Forms.getJSONInfo(_this.Control, { UniqueIdPrefix: "".concat(_this.ControlId, "ls"), UniqueIdPrefixCounter: 0, UniqueIdCounter: ++_this.AddCounter });
                var data = {
                    GridData: _this.Grid.StaticData,
                    __ModuleGuid: formJson.ModuleGuid,
                    __UniqueIdCounters: formJson.UniqueIdCounters,
                    __RequestVerificationToken: formJson.RequestVerificationToken,
                };
                $YetaWF.postJSON(uri, formJson, query, data, function (success, partial) {
                    if (success)
                        _this.Grid.AddRecord(partial.TR, partial.StaticData);
                });
                return false;
            });
            _this.SelectUrl.Control.addEventListener(YetaWF_ComponentsHTML.UrlEditComponent.EVENTCHANGE, function (evt) {
                _this.toggleButton();
            });
            _this.GridAll.Control.addEventListener(YetaWF_ComponentsHTML.Grid.EVENTSELECT, function (evt) {
                var index = _this.GridAll.SelectedIndex();
                if (index < 0)
                    return;
                var td = $YetaWF.getElement1BySelector("td", [_this.GridAll.GetTR(index)]);
                var url = td.innerText.trim();
                _this.SelectUrl.value = url;
                _this.toggleButton();
            });
            return _this;
        }
        ListOfLocalPagesEditComponent.prototype.toggleButton = function () {
            var s = this.SelectUrl.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        };
        ListOfLocalPagesEditComponent.TEMPLATE = "yt_yetawf_pages_listoflocalpages";
        ListOfLocalPagesEditComponent.SELECTOR = ".yt_yetawf_pages_listoflocalpages.t_edit";
        return ListOfLocalPagesEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_Pages.ListOfLocalPagesEditComponent = ListOfLocalPagesEditComponent;
})(YetaWF_Pages || (YetaWF_Pages = {}));

//# sourceMappingURL=ListOfLocalPagesEdit.js.map
