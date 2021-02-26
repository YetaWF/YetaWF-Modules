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
    var SendSelectEnum;
    (function (SendSelectEnum) {
        SendSelectEnum[SendSelectEnum["No"] = 0] = "No";
        SendSelectEnum[SendSelectEnum["Yes"] = 1] = "Yes";
        SendSelectEnum[SendSelectEnum["ChangeSinceOpen"] = 2] = "ChangeSinceOpen";
    })(SendSelectEnum || (SendSelectEnum = {}));
    var DropDownSearchEditComponent = /** @class */ (function (_super) {
        __extends(DropDownSearchEditComponent, _super);
        function DropDownSearchEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, DropDownSearchEditComponent.TEMPLATE, DropDownSearchEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: DropDownSearchEditComponent.EVENTCHANGE,
                GetValue: function (control) {
                    return control.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, function (tag, control) {
                control.closePopup(SendSelectEnum.No);
                control.clearInputTimeout();
            }) || this;
            _this.InputTimeout = 0;
            _this.LastValue = "";
            _this.Popup = null;
            _this.Enabled = true;
            _this.Focused = false;
            _this.SelectedIndex = -1;
            _this.MouseSelectedIndex = -1;
            _this.Setup = setup;
            _this.Input = $YetaWF.getElement1BySelector("input[type='text']", [_this.Control]);
            _this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [_this.Control]);
            $YetaWF.registerEventHandler(_this.Input, "input", null, function (ev) {
                _this.clearInputTimeout();
                _this.InputTimeout = setTimeout(function () {
                    _this.Hidden.value = "";
                    _this.updateDropDown();
                }, DropDownSearchEditComponent.INPUTWAIT);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "mouseenter", null, function (ev) {
                if (_this.Enabled) {
                    $YetaWF.elementRemoveClass(_this.Control, "t_hover");
                    $YetaWF.elementAddClass(_this.Control, "t_hover");
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "mouseleave", null, function (ev) {
                if (_this.Enabled)
                    $YetaWF.elementRemoveClass(_this.Control, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "focusin", null, function (ev) {
                if (_this.Enabled) {
                    $YetaWF.elementRemoveClass(_this.Control, "t_focused");
                    $YetaWF.elementAddClass(_this.Control, "t_focused");
                    _this.Focused = true;
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "focusout", null, function (ev) {
                $YetaWF.elementRemoveClass(_this.Control, "t_focused");
                if (_this.Hidden.value === "")
                    _this.Input.value = "";
                _this.Focused = false;
                _this.closePopup(SendSelectEnum.ChangeSinceOpen);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "keydown", null, function (ev) {
                if (_this.Enabled) {
                    var key = ev.key;
                    if (ev.altKey) {
                        if (_this.Popup) {
                            if (key === "ArrowUp" || key === "ArrowLeft") {
                                _this.closePopup(SendSelectEnum.Yes);
                                return false;
                            }
                        }
                        else {
                            if (key === "ArrowDown" || key === "ArrowRight") {
                                _this.updateDropDown();
                                return false;
                            }
                        }
                    }
                    else {
                        if (_this.Popup) {
                            if (key === "ArrowDown" || key === "ArrowRight") {
                                _this.setSelectedIndex(_this.SelectedIndex + 1);
                                return false;
                            }
                            else if (key === "ArrowUp" || key === "ArrowLeft") {
                                if (_this.SelectedIndex < 0)
                                    _this.setSelectedIndex(_this.totalItems - 1);
                                else
                                    _this.setSelectedIndex(_this.SelectedIndex - 1);
                                return false;
                            }
                            else if (key === "Home") {
                                _this.setSelectedIndex(0);
                                return false;
                            }
                            else if (key === "End") {
                                _this.setSelectedIndex(_this.totalItems - 1);
                                return false;
                            }
                            else if (key === "Escape") {
                                _this.closePopup(SendSelectEnum.No);
                                return false;
                            }
                            else if (key === "Tab") {
                                _this.closePopup(SendSelectEnum.ChangeSinceOpen);
                            }
                            else if (key === "Enter") {
                                _this.closePopup(SendSelectEnum.ChangeSinceOpen);
                                return false;
                            }
                        }
                    }
                }
                return true;
            });
            return _this;
        }
        DropDownSearchEditComponent.prototype.updateDropDown = function () {
            var _this = this;
            var value = this.Input.value;
            if (this.Popup) {
                if (value === this.LastValue)
                    return;
            }
            var uri = $YetaWF.parseUrl(this.Setup.AjaxUrl);
            uri.addFormInfo(this.Control);
            uri.addSearch("Search", value);
            $YetaWF.post(uri.toUrl(), uri.toFormData(), function (success, data) {
                _this.LastValue = value;
                _this.openPopup(JSON.parse(data));
            });
        };
        DropDownSearchEditComponent.prototype.clearInputTimeout = function () {
            if (this.InputTimeout)
                clearTimeout(this.InputTimeout);
            this.InputTimeout = 0;
        };
        DropDownSearchEditComponent.prototype.setSelectedIndex = function (index) {
            if (!this.Popup)
                return;
            var total = this.totalItems;
            if (index < 0 || index >= total)
                return;
            this.clearSelectedPopupItem();
            this.selectPopupItem(index);
        };
        Object.defineProperty(DropDownSearchEditComponent.prototype, "totalItems", {
            get: function () {
                if (!this.Popup)
                    return 0;
                var lis = $YetaWF.getElementsBySelector("ul li", [this.Popup]);
                return lis.length;
            },
            enumerable: false,
            configurable: true
        });
        DropDownSearchEditComponent.prototype.sendChangeEvent = function () {
            $YetaWF.sendCustomEvent(this.Control, DropDownSearchEditComponent.EVENTCHANGE);
            FormsSupport.validateElement(this.Hidden);
        };
        DropDownSearchEditComponent.prototype.openPopup = function (list) {
            var _this = this;
            this.SelectedIndex = -1;
            if (this.Popup)
                this.closePopup(SendSelectEnum.No);
            if (list.length === 0)
                return;
            DropDownSearchEditComponent.closeDropdowns();
            this.Popup =
                $YetaWF.createElement("div", { id: DropDownSearchEditComponent.POPUPID, "data-owner": this.ControlId, "aria-hidden": "false" },
                    $YetaWF.createElement("div", { class: "t_container", "data-role": "popup", "aria-hidden": "false" },
                        $YetaWF.createElement("div", { class: "t_scroller", unselectable: "on" },
                            $YetaWF.createElement("ul", { unselectable: "on", class: "t_list", tabindex: "-1", "aria-hidden": "true", "aria-live": "off", "data-role": "staticlist", role: "listbox" }))));
            var ul = $YetaWF.getElement1BySelector("ul", [this.Popup]);
            var len = list.length;
            for (var i = 0; i < len; ++i) {
                var o = list[i];
                var tt = o.Tooltip;
                var li = $YetaWF.createElement("li", { tabindex: "-1", role: "option", unselectable: "on", class: "t_item", "data-index": i, "data-value": o.Value, "data-tooltip": tt }, o.Text);
                ul.appendChild(li);
            }
            var style = window.getComputedStyle(this.Control);
            this.Popup.style.font = style.font;
            this.Popup.style.fontStyle = style.fontStyle;
            this.Popup.style.fontWeight = style.fontWeight;
            this.Popup.style.fontSize = style.fontSize;
            document.body.appendChild(this.Popup);
            this.Control.setAttribute("aria-expanded", "true");
            this.positionPopup(this.Popup);
            $YetaWF.registerEventHandler(this.Popup, "mousedown", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                var index = Number($YetaWF.getAttribute(li, "data-index"));
                _this.MouseSelectedIndex = index;
                return false;
            });
            $YetaWF.registerEventHandler(this.Popup, "mouseup", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                var index = Number($YetaWF.getAttribute(li, "data-index"));
                if (_this.MouseSelectedIndex === index) {
                    _this.setSelectedIndex(index);
                    _this.closePopup(SendSelectEnum.Yes);
                }
                else {
                    _this.MouseSelectedIndex = -1;
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.Popup, "mouseover", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                $YetaWF.elementAddClass(li, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.Popup, "mouseout", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                return true;
            });
        };
        DropDownSearchEditComponent.prototype.closePopup = function (sendEvent) {
            ToolTipsHTMLHelper.removeTooltips();
            if (this.Popup) {
                if (sendEvent === SendSelectEnum.Yes || sendEvent === SendSelectEnum.ChangeSinceOpen) {
                    if (this.SelectedIndex !== -1) {
                        var lis = $YetaWF.getElementsBySelector("ul li", [this.Popup]);
                        var value = $YetaWF.getAttribute(lis[this.SelectedIndex], "data-value");
                        this.Input.value = lis[this.SelectedIndex].innerText;
                        this.Hidden.value = value;
                    }
                    else {
                        this.Input.value = "";
                        this.Hidden.value = "";
                    }
                    this.SelectedIndex = -1;
                    this.sendChangeEvent();
                }
                this.Popup.remove();
                this.Popup = null;
                this.Control.setAttribute("aria-expanded", "false");
            }
            else {
                if (this.Hidden.value === "")
                    this.Input.value = "";
                this.sendChangeEvent();
            }
        };
        DropDownSearchEditComponent.prototype.positionPopup = function (popup) {
            if (!popup)
                return;
            var ownerId = $YetaWF.getAttribute(popup, "data-owner");
            var control = DropDownSearchEditComponent.getControlById(ownerId, DropDownSearchEditComponent.SELECTOR);
            var scroller = $YetaWF.getElement1BySelector(".t_scroller", [popup]);
            var controlRect = control.Control.getBoundingClientRect();
            var desiredHeight = control.Setup.DropDownHeightFactor * DropDownSearchEditComponent.DEFAULTHEIGHT;
            var desiredWidth = control.Setup.DropDownWidthFactor * controlRect.width;
            scroller.style.maxHeight = desiredHeight + "px";
            popup.style.width = desiredWidth + "px";
            $YetaWF.positionLeftAlignedBelow(this.Control, popup);
        };
        DropDownSearchEditComponent.prototype.selectPopupItem = function (index) {
            this.SelectedIndex = index;
            if (!this.Popup)
                return;
            var ul = $YetaWF.getElement1BySelector("ul", [this.Popup]);
            var li = ul.children[index];
            $YetaWF.elementRemoveClasses(li, ["t_selected", "t_focused"]);
            $YetaWF.elementAddClass(li, "t_selected");
            var ariaId = $YetaWF.getAttribute(this.Control, "aria-activedescendant");
            li.id = ariaId;
            $YetaWF.setAttribute(li, "aria-selected", "true");
            if (this.Focused)
                $YetaWF.elementAddClass(li, "t_focused");
            var scroller = $YetaWF.getElement1BySelector(".t_scroller", [this.Popup]);
            var rectElem = li.getBoundingClientRect();
            var rectContainer = scroller.getBoundingClientRect();
            if (rectElem.bottom > rectContainer.bottom)
                li.scrollIntoView({ behavior: "auto", block: "nearest", inline: "end" });
            if (rectElem.top < rectContainer.top)
                li.scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" });
        };
        DropDownSearchEditComponent.prototype.clearSelectedPopupItem = function () {
            if (this.Popup) {
                var lis = $YetaWF.getElementsBySelector("ul li.t_selected", [this.Popup]);
                for (var _i = 0, lis_1 = lis; _i < lis_1.length; _i++) {
                    var li = lis_1[_i];
                    $YetaWF.elementRemoveClasses(li, ["t_selected", "t_focused"]);
                    $YetaWF.setAttribute(li, "aria-selected", "false");
                    li.id = "";
                }
            }
        };
        DropDownSearchEditComponent.closeDropdowns = function () {
            var popup = $YetaWF.getElementByIdCond(DropDownSearchEditComponent.POPUPID);
            if (!popup)
                return;
            var ownerId = $YetaWF.getAttribute(popup, "data-owner");
            var control = DropDownSearchEditComponent.getControlById(ownerId, DropDownSearchEditComponent.SELECTOR);
            control.closePopup(SendSelectEnum.No);
        };
        Object.defineProperty(DropDownSearchEditComponent.prototype, "value", {
            // API
            get: function () {
                return this.Hidden.value;
            },
            set: function (val) {
                this.Hidden.value = val;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DropDownSearchEditComponent.prototype, "text", {
            get: function () {
                return this.Input.value;
            },
            set: function (val) {
                this.Input.value = val;
            },
            enumerable: false,
            configurable: true
        });
        DropDownSearchEditComponent.prototype.clear = function () {
            this.closePopup(SendSelectEnum.No);
            this.Input.value = "";
            this.Hidden.value = "";
        };
        Object.defineProperty(DropDownSearchEditComponent.prototype, "enabled", {
            get: function () {
                return this.Enabled;
            },
            enumerable: false,
            configurable: true
        });
        DropDownSearchEditComponent.prototype.enable = function (enabled) {
            this.closePopup(SendSelectEnum.No);
            $YetaWF.elementEnableToggle(this.Input, enabled);
            $YetaWF.elementRemoveClass(this.Control, "t_disabled");
            if (!enabled)
                $YetaWF.elementAddClass(this.Control, "t_disabled");
            this.Enabled = enabled;
        };
        DropDownSearchEditComponent.TEMPLATE = "yt_dropdownsearch_base";
        DropDownSearchEditComponent.SELECTOR = ".yt_dropdownsearch_base.t_edit";
        DropDownSearchEditComponent.EVENTCHANGE = "dropdownsearch_change";
        DropDownSearchEditComponent.INPUTWAIT = 300;
        DropDownSearchEditComponent.POPUPID = "yDSPopup";
        DropDownSearchEditComponent.DEFAULTHEIGHT = 200;
        return DropDownSearchEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.DropDownSearchEditComponent = DropDownSearchEditComponent;
    // close dropdown when clicking outside
    $YetaWF.registerEventHandlerBody("click", null, function (ev) {
        DropDownSearchEditComponent.closeDropdowns();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, function (ev) {
        DropDownSearchEditComponent.closeDropdowns();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        DropDownSearchEditComponent.closeDropdowns();
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=DropDownSearchEdit.js.map
