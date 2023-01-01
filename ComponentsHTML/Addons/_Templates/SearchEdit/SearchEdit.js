"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
    var SearchEditComponent = /** @class */ (function (_super) {
        __extends(SearchEditComponent, _super);
        function SearchEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, SearchEditComponent.TEMPLATE, SearchEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: SearchEditComponent.EVENTCHANGE,
                GetValue: function (control) {
                    return control.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, function (tag, control) {
                control.stopAutoClick();
            }) || this;
            _this.AutoTimeout = 0;
            _this.Setup = setup;
            _this.InputControl = _this.Control;
            _this.Container = $YetaWF.elementClosest(_this.Control, ".yt_search_container");
            $YetaWF.registerMultipleEventHandlers([_this.InputControl], ["input"], null, function (ev) {
                _this.sendChangeEvent();
                _this.startAutoClick();
                return true;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "keydown", null, function (ev) {
                var key = ev.key;
                switch (key) {
                    case "Enter":
                        _this.sendClickEvent();
                        return false;
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Container, "click", ".t_search", function (ev) {
                _this.InputControl.focus();
                _this.sendClickEvent();
                return false;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "focusin", null, function (ev) {
                if (_this.enabled) {
                    $YetaWF.elementRemoveClass(_this.Container, "t_focused");
                    $YetaWF.elementAddClass(_this.Container, "t_focused");
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "focusout", null, function (ev) {
                $YetaWF.elementRemoveClass(_this.Container, "t_focused");
                return true;
            });
            return _this;
        }
        SearchEditComponent.prototype.startAutoClick = function () {
            var _this = this;
            this.stopAutoClick();
            if (this.Setup.AutoClickDelay) {
                this.AutoTimeout = setTimeout(function () {
                    _this.sendClickEvent();
                }, this.Setup.AutoClickDelay);
            }
        };
        SearchEditComponent.prototype.stopAutoClick = function () {
            if (this.AutoTimeout) {
                clearInterval(this.AutoTimeout);
                this.AutoTimeout = 0;
            }
        };
        // events
        SearchEditComponent.prototype.sendChangeEvent = function () {
            $YetaWF.sendCustomEvent(this.Control, SearchEditComponent.EVENTCHANGE);
            FormsSupport.validateElement(this.Control);
        };
        SearchEditComponent.prototype.sendClickEvent = function () {
            $YetaWF.sendCustomEvent(this.Control, SearchEditComponent.EVENTCLICK);
        };
        Object.defineProperty(SearchEditComponent.prototype, "value", {
            // API
            get: function () {
                return this.InputControl.value;
            },
            set: function (val) {
                this.InputControl.value = val;
            },
            enumerable: false,
            configurable: true
        });
        SearchEditComponent.prototype.clear = function () {
            this.InputControl.value = "";
        };
        SearchEditComponent.prototype.enable = function (enabled) {
            $YetaWF.elementEnableToggle(this.InputControl, enabled);
            $YetaWF.elementRemoveClass(this.Container, "t_disabled");
            if (!enabled)
                $YetaWF.elementAddClass(this.Container, "t_disabled");
        };
        Object.defineProperty(SearchEditComponent.prototype, "enabled", {
            get: function () {
                return !$YetaWF.elementHasClass(this.Container, "t_disabled");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(SearchEditComponent.prototype, "focused", {
            get: function () {
                return $YetaWF.elementHasClass(this.Container, "t_focused");
            },
            enumerable: false,
            configurable: true
        });
        SearchEditComponent.TEMPLATE = "yt_search";
        SearchEditComponent.SELECTOR = ".yt_search.t_edit";
        SearchEditComponent.EVENTCHANGE = "search_change";
        SearchEditComponent.EVENTCLICK = "search_click";
        return SearchEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.SearchEditComponent = SearchEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=SearchEdit.js.map
