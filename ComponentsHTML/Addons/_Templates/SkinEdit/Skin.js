"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var SkinEditComponent = /** @class */ (function (_super) {
        __extends(SkinEditComponent, _super);
        function SkinEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, SkinEditComponent.TEMPLATE, SkinEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: function (control) { return null; },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                },
            }) || this;
            _this.Setup = setup;
            _this.SelectCollection = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.Collection']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [_this.Control]);
            _this.SelectPageFile = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.PageFileName']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [_this.Control]);
            _this.SelectPopupFile = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.PopupFileName']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [_this.Control]);
            _this.SelectCollection.Control.addEventListener("dropdownlist_change", function (evt) {
                var data = { SkinCollection: _this.SelectCollection.value };
                $YetaWF.setLoading(true);
                var uri = $YetaWF.parseUrl(_this.Setup.AjaxUrl);
                uri.addSearchSimpleObject(data);
                var request = new XMLHttpRequest();
                request.open("POST", uri.toUrl());
                request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                request.onreadystatechange = function (ev) {
                    if (request.readyState === 4 /*DONE*/) {
                        $YetaWF.setLoading(false);
                        $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, _this.Control, undefined, function (data) {
                            var lists = JSON.parse(data);
                            _this.SelectPageFile.setOptionsHTML(lists.PagesHTML);
                            _this.SelectPopupFile.setOptionsHTML(lists.PopupsHTML);
                        });
                    }
                };
                request.send(uri.toFormData());
            });
            return _this;
        }
        SkinEditComponent.prototype.enable = function (enabled) {
            this.SelectCollection.enable(enabled);
            this.SelectPageFile.enable(enabled);
            this.SelectPopupFile.enable(enabled);
        };
        SkinEditComponent.TEMPLATE = "yt_skin";
        SkinEditComponent.SELECTOR = ".yt_skin.t_edit";
        return SkinEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.SkinEditComponent = SkinEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Skin.js.map
