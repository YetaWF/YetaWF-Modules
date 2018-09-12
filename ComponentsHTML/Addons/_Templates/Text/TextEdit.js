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
    var TextEditComponent = /** @class */ (function (_super) {
        __extends(TextEditComponent, _super);
        function TextEditComponent(controlId) {
            var _this = _super.call(this, controlId) || this;
            _this.InputText = null;
            _this.ToolTips = toolTips;
            $YetaWF.addObjectDataById(controlId, _this);
            _this.updateWidth();
            return _this;
        }
        TextEditComponent.prototype.updateWidth = function () {
            var w = this.Control.clientWidth;
            if (w > 0 && this.KendoText == null) {
                var thisObj = this;
                $(this.Control).kendoText({
                    // tslint:disable-next-line:only-arrow-functions
                    change: function () {
                        var event = document.createEvent("Event");
                        event.initEvent("Text_change", true, true);
                        thisObj.Control.dispatchEvent(event);
                        FormsSupport.validateElement(thisObj.Control);
                    }
                });
                this.KendoText = $(this.Control).data("kendoText");
                var avgw = Number($YetaWF.getAttribute(this.Control, "data-charavgw"));
                var container = $YetaWF.elementClosest(this.Control, ".k-widget.yt_Text,.k-widget.yt_Text_base,.k-widget.yt_enum");
                $(container).width(w + 3 * avgw);
            }
        };
        Object.defineProperty(TextEditComponent.prototype, "value", {
            get: function () {
                return this.Control.value;
            },
            set: function (val) {
                if (this.KendoText == null) {
                    this.Control.value = val;
                }
                else {
                    this.KendoText.value(val);
                    if (this.KendoText.select() < 0)
                        this.KendoText.select(0);
                }
            },
            enumerable: true,
            configurable: true
        });
        // retrieve the tooltip for the nth item (index) in the dropdown list
        TextEditComponent.prototype.getToolTip = function (index) {
            if (!this.ToolTips)
                return null;
            if (index < 0 || index >= this.ToolTips.length)
                return null;
            return this.ToolTips[index];
        };
        TextEditComponent.prototype.clear = function () {
            if (this.KendoText == null) {
                this.Control.selectedIndex = 0;
            }
            else {
                this.KendoText.select(0);
            }
        };
        TextEditComponent.prototype.enable = function (enabled) {
            if (this.KendoText == null) {
                $YetaWF.elementEnableToggle(this.Control, enabled);
            }
            else {
                this.KendoText.enable(enabled);
            }
        };
        TextEditComponent.prototype.destroy = function () {
            if (this.KendoText)
                this.KendoText.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        };
        TextEditComponent.prototype.ajaxUpdate = function (data, ajaxUrl, onSuccess, onFailure) {
            var _this = this;
            var uri = $YetaWF.parseUrl(ajaxUrl);
            uri.addSearchSimpleObject(data);
            var request = new XMLHttpRequest();
            request.open("POST", uri.toUrl());
            request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            request.onreadystatechange = function (ev) {
                if (request.readyState === 4 /*DONE*/) {
                    $YetaWF.setLoading(false);
                    var retVal = $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, _this.Control, undefined, undefined, function (data) {
                        // $(this.Control).val(null);
                        var thisObj = _this;
                        $(_this.Control).kendoText({
                            dataTextField: "t",
                            dataValueField: "v",
                            dataSource: data.data,
                            // tslint:disable-next-line:only-arrow-functions
                            change: function () {
                                var event = document.createEvent("Event");
                                event.initEvent("Text_change", true, true);
                                thisObj.Control.dispatchEvent(event);
                                FormsSupport.validateElement(thisObj.Control);
                            }
                        });
                        _this.ToolTips = data.tooltips;
                        _this.KendoText = $(_this.Control).data("kendoText");
                        if (onSuccess) {
                            onSuccess(data);
                        }
                        else {
                            _this.value = "";
                            $(_this.Control).trigger("change");
                            var event = document.createEvent("Event");
                            event.initEvent("Text_change", true, true);
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
            request.send();
        };
        TextEditComponent.getControlFromTag = function (elem) { return _super.getControlBaseFromTag.call(this, elem, TextEditComponent.SELECTOR); };
        TextEditComponent.getControlFromSelector = function (selector, tags) { return _super.getControlBaseFromSelector.call(this, selector, TextEditComponent.SELECTOR, tags); };
        TextEditComponent.SELECTOR = "input.k-input";
        return TextEditComponent;
    }(YetaWF.ComponentBase));
    YetaWF_ComponentsHTML.TextEditComponent = TextEditComponent;
    // We need to delay initialization until divs become visible so we can calculate the dropdown width
    $YetaWF.registerActivateDiv(function (tag) {
        var ctls = $YetaWF.getElementsBySelector(TextEditComponent.SELECTOR, [tag]);
        for (var _i = 0, ctls_1 = ctls; _i < ctls_1.length; _i++) {
            var ctl = ctls_1[_i];
            var control = TextEditComponent.getControlFromTag(ctl);
            control.updateWidth();
        }
    });
    // A <div> is being emptied. Destroy all Texts the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBase.clearDiv(tag, TextEditComponent.SELECTOR, function (control) {
            control.destroy();
        });
    });
    // handle submit/apply
    $YetaWF.registerCustomEventHandlerDocument("Text_change", ".ysubmitonchange .k-dropdown select.yt_Text_base", function (ev) {
        $YetaWF.Forms.submitOnChange(ev.target);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument("Text_change", ".yapplyonchange .k-dropdown select.yt_Text_base", function (ev) {
        $YetaWF.Forms.applyOnChange(ev.target);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument("Text_change", ".yreloadonchange .k-dropdown select.yt_Text_base", function (ev) {
        $YetaWF.Forms.reloadOnChange(ev.target);
        return false;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
