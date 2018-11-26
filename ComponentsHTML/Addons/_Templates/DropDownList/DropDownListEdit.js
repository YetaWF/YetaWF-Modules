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
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var DropDownListEditComponent = /** @class */ (function (_super) {
        __extends(DropDownListEditComponent, _super);
        function DropDownListEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId) || this;
            _this.KendoDropDownList = null;
            _this.Setup = setup;
            _this.updateWidth();
            return _this;
        }
        DropDownListEditComponent.prototype.updateWidth = function () {
            var w = this.Control.clientWidth;
            if (w > 0 && this.KendoDropDownList == null) {
                var thisObj = this;
                $(this.Control).kendoDropDownList({
                    autoWidth: true,
                    // tslint:disable-next-line:only-arrow-functions
                    change: function () {
                        var event = document.createEvent("Event");
                        event.initEvent("dropdownlist_change", true, true);
                        thisObj.Control.dispatchEvent(event);
                        FormsSupport.validateElement(thisObj.Control);
                    }
                });
                this.KendoDropDownList = $(this.Control).data("kendoDropDownList");
                if (this.Setup.AdjustWidth) {
                    var avgw = Number($YetaWF.getAttribute(this.Control, "data-charavgw"));
                    var container = $YetaWF.elementClosest(this.Control, ".k-widget.yt_dropdownlist,.k-widget.yt_dropdownlist_base,.k-widget.yt_enum");
                    $(container).width(w + 3 * avgw);
                }
            }
        };
        Object.defineProperty(DropDownListEditComponent.prototype, "value", {
            get: function () {
                return this.Control.value;
            },
            set: function (val) {
                if (this.KendoDropDownList == null) {
                    this.Control.value = val;
                }
                else {
                    this.KendoDropDownList.value(val);
                    if (this.KendoDropDownList.select() < 0)
                        this.KendoDropDownList.select(0);
                }
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(DropDownListEditComponent.prototype, "selectedIndex", {
            get: function () {
                if (this.KendoDropDownList == null)
                    return -1;
                return this.KendoDropDownList.select();
            },
            enumerable: true,
            configurable: true
        });
        // retrieve the tooltip for the nth item (index) in the dropdown list
        DropDownListEditComponent.prototype.getToolTip = function (index) {
            if (!this.Setup.ToolTips)
                return null;
            if (index < 0 || index >= this.Setup.ToolTips.length)
                return null;
            return this.Setup.ToolTips[index];
        };
        DropDownListEditComponent.prototype.clear = function () {
            if (this.KendoDropDownList == null) {
                this.Control.selectedIndex = 0;
            }
            else {
                this.KendoDropDownList.select(0);
            }
        };
        DropDownListEditComponent.prototype.enable = function (enabled) {
            if (this.KendoDropDownList == null) {
                $YetaWF.elementEnableToggle(this.Control, enabled);
            }
            else {
                this.KendoDropDownList.enable(enabled);
            }
        };
        DropDownListEditComponent.prototype.internalDestroy = function () {
            try {
                if (this.KendoDropDownList)
                    this.KendoDropDownList.destroy();
            }
            catch (e) { }
            this.KendoDropDownList = null;
        };
        DropDownListEditComponent.prototype.ajaxUpdate = function (data, ajaxUrl, onSuccess, onFailure) {
            var _this = this;
            var uri = $YetaWF.parseUrl(ajaxUrl);
            uri.addSearchSimpleObject(data);
            var request = new XMLHttpRequest();
            request.open("POST", ajaxUrl);
            request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            request.onreadystatechange = function (ev) {
                if (request.readyState === 4 /*DONE*/) {
                    $YetaWF.setLoading(false);
                    var retVal = $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, _this.Control, undefined, undefined, function (data) {
                        // $(this.Control).val(null);
                        var thisObj = _this;
                        $(_this.Control).kendoDropDownList({
                            dataTextField: "t",
                            dataValueField: "v",
                            dataSource: data.data,
                            autoWidth: true,
                            // tslint:disable-next-line:only-arrow-functions
                            change: function () {
                                var event = document.createEvent("Event");
                                event.initEvent("dropdownlist_change", true, true);
                                thisObj.Control.dispatchEvent(event);
                                FormsSupport.validateElement(thisObj.Control);
                            }
                        });
                        _this.Setup.ToolTips = data.tooltips;
                        _this.KendoDropDownList = $(_this.Control).data("kendoDropDownList");
                        if (onSuccess) {
                            onSuccess(data);
                        }
                        else {
                            _this.value = "";
                            $(_this.Control).trigger("change");
                            var event = document.createEvent("Event");
                            event.initEvent("dropdownlist_change", true, true);
                            _this.Control.dispatchEvent(event);
                        }
                    });
                    if (!retVal) {
                        if (onFailure)
                            onFailure(request.responseText);
                        throw "Unexpected data returned";
                    }
                }
            };
            request.send(uri.toFormData());
        };
        DropDownListEditComponent.SELECTOR = "select.yt_dropdownlist_base.t_edit.t_kendo";
        return DropDownListEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.DropDownListEditComponent = DropDownListEditComponent;
    // We need to delay initialization until divs become visible so we can calculate the dropdown width
    $YetaWF.registerActivateDiv(function (tag) {
        var ctls = $YetaWF.getElementsBySelector(DropDownListEditComponent.SELECTOR, [tag]);
        for (var _i = 0, ctls_1 = ctls; _i < ctls_1.length; _i++) {
            var ctl = ctls_1[_i];
            var control = YetaWF.ComponentBaseDataImpl.getControlFromTag(ctl, DropDownListEditComponent.SELECTOR);
            control.updateWidth();
        }
    });
    // A <div> is being emptied. Destroy all dropdownlists the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBaseDataImpl.clearDiv(tag, DropDownListEditComponent.SELECTOR, function (control) {
            control.internalDestroy();
        });
    });
    // handle submit/apply
    $YetaWF.registerCustomEventHandlerDocument("dropdownlist_change", ".ysubmitonchange .k-dropdown select.yt_dropdownlist_base", function (ev) {
        $YetaWF.Forms.submitOnChange(ev.target);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument("dropdownlist_change", ".yapplyonchange .k-dropdown select.yt_dropdownlist_base", function (ev) {
        $YetaWF.Forms.applyOnChange(ev.target);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument("dropdownlist_change", ".yreloadonchange .k-dropdown select.yt_dropdownlist_base", function (ev) {
        $YetaWF.Forms.reloadOnChange(ev.target);
        return false;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
