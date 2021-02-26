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
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var UrlTypeEnum;
    (function (UrlTypeEnum) {
        UrlTypeEnum[UrlTypeEnum["Local"] = 1] = "Local";
        UrlTypeEnum[UrlTypeEnum["Remote"] = 2] = "Remote";
    })(UrlTypeEnum || (UrlTypeEnum = {}));
    var UrlEditComponent = /** @class */ (function (_super) {
        __extends(UrlEditComponent, _super);
        function UrlEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, UrlEditComponent.TEMPLATE, UrlEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: UrlEditComponent.EVENTCHANGE,
                GetValue: function (control) {
                    return control.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }) || this;
            _this.selectPage = null;
            _this.inputUrl = null;
            _this.divLocal = null;
            _this.divRemote = null;
            _this.Setup = setup;
            _this.inputHidden = $YetaWF.getElement1BySelector(".t_hidden", [_this.Control]);
            _this.selectType = YetaWF.ComponentBaseDataImpl.getControlFromSelector(".yt_urltype", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [_this.Control]);
            // eslint-disable-next-line no-bitwise
            if (_this.Setup.Type & UrlTypeEnum.Local) {
                _this.selectPage = YetaWF.ComponentBaseDataImpl.getControlFromSelector(".yt_urldesignedpage", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [_this.Control]);
                _this.divLocal = $YetaWF.getElement1BySelector(".t_local", [_this.Control]);
            }
            // eslint-disable-next-line no-bitwise
            if (_this.Setup.Type & UrlTypeEnum.Remote) {
                _this.inputUrl = $YetaWF.getElement1BySelector(".yt_urlremotepage", [_this.Control]);
                _this.divRemote = $YetaWF.getElement1BySelector(".t_remote", [_this.Control]);
            }
            _this.aLink = $YetaWF.getElement1BySelector(".t_link a", [_this.Control]);
            _this.value = _this.Setup.Url;
            if (!_this.inputUrl || !_this.selectPage)
                _this.selectType.enable(false);
            _this.selectType.Control.addEventListener(YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, function (evt) {
                _this.updateStatus();
                _this.sendEvent();
            });
            if (_this.selectPage) {
                _this.selectPage.Control.addEventListener(YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, function (evt) {
                    _this.updateStatus();
                    _this.sendEvent();
                });
            }
            if (_this.inputUrl) {
                $YetaWF.registerMultipleEventHandlers([_this.inputUrl], ["input", "change", "click", "keyup", "paste"], null, function (ev) {
                    _this.updateStatus();
                    _this.sendEvent();
                    return true;
                });
            }
            return _this;
        }
        UrlEditComponent.prototype.updateStatus = function () {
            var tp = Number(this.selectType.value);
            switch (tp) {
                case UrlTypeEnum.Local:
                    if (this.divLocal)
                        this.divLocal.style.display = "";
                    if (this.divRemote)
                        this.divRemote.style.display = "none";
                    if (this.selectPage)
                        this.inputHidden.value = this.selectPage.value.trim();
                    break;
                case UrlTypeEnum.Remote:
                    if (this.divLocal)
                        this.divLocal.style.display = "none";
                    if (this.divRemote)
                        this.divRemote.style.display = "";
                    if (this.inputUrl)
                        this.inputHidden.value = this.inputUrl.value.trim();
                    break;
            }
            var url = this.inputHidden.value.trim();
            if (url && url.length > 0) {
                if (tp === UrlTypeEnum.Local) {
                    var uri = $YetaWF.parseUrl(url);
                    uri.removeSearch(YConfigs.Basics.Link_NoEditMode);
                    uri.addSearch(YConfigs.Basics.Link_NoEditMode, "y");
                    this.aLink.href = uri.toUrl();
                }
                else {
                    this.aLink.href = url;
                }
                this.aLink.style.display = "";
            }
            else {
                this.aLink.href = "";
                this.aLink.style.display = "none";
            }
        };
        UrlEditComponent.prototype.sendEvent = function () {
            FormsSupport.validateElement(this.inputHidden);
            var event = document.createEvent("Event");
            event.initEvent(UrlEditComponent.EVENTCHANGE, true, true);
            this.Control.dispatchEvent(event);
        };
        Object.defineProperty(UrlEditComponent.prototype, "value", {
            // API
            get: function () {
                return this.inputHidden.value;
            },
            set: function (url) {
                var sel = Number(this.selectType.value); // current selection
                if (this.Setup.Type === UrlTypeEnum.Local + UrlTypeEnum.Remote && this.selectPage) {
                    if (url != null && (url.startsWith("//") || url.startsWith("http"))) {
                        // remote
                        if (this.inputUrl)
                            sel = UrlTypeEnum.Remote;
                    }
                    else {
                        // try local
                        this.selectPage.value = url;
                        if (this.selectPage.value !== url)
                            sel = UrlTypeEnum.Remote; // have to use remote as there was no match in the designed pages
                        else
                            sel = UrlTypeEnum.Local;
                    }
                }
                else if (this.Setup.Type === UrlTypeEnum.Local && this.selectPage) {
                    sel = UrlTypeEnum.Local;
                }
                else {
                    sel = UrlTypeEnum.Remote;
                }
                this.inputHidden.value = url;
                this.selectType.value = sel.toString();
                if (sel === UrlTypeEnum.Local && this.selectPage) {
                    this.selectPage.value = url;
                }
                else if (sel === UrlTypeEnum.Remote && this.inputUrl) {
                    this.inputUrl.value = url;
                }
                this.updateStatus();
            },
            enumerable: false,
            configurable: true
        });
        UrlEditComponent.prototype.clear = function () {
            this.value = "";
        };
        UrlEditComponent.prototype.enable = function (enabled) {
            if (this.inputUrl && this.selectPage)
                this.selectType.enable(enabled);
            if (this.selectPage)
                this.selectPage.enable(enabled);
            if (this.inputUrl)
                $YetaWF.elementEnableToggle(this.inputUrl, enabled);
        };
        UrlEditComponent.TEMPLATE = "yt_url";
        UrlEditComponent.SELECTOR = ".yt_url.t_edit";
        UrlEditComponent.EVENTCHANGE = "url_change";
        return UrlEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.UrlEditComponent = UrlEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=UrlEdit.js.map
