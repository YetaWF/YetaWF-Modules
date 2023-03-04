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
    var SendSelectEnum;
    (function (SendSelectEnum) {
        SendSelectEnum[SendSelectEnum["No"] = 0] = "No";
        SendSelectEnum[SendSelectEnum["Yes"] = 1] = "Yes";
        SendSelectEnum[SendSelectEnum["ChangeSinceOpen"] = 2] = "ChangeSinceOpen";
    })(SendSelectEnum || (SendSelectEnum = {}));
    var DropDownListEditComponent = /** @class */ (function (_super) {
        __extends(DropDownListEditComponent, _super);
        function DropDownListEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, DropDownListEditComponent.TEMPLATE, DropDownListEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: DropDownListEditComponent.EVENTCHANGE,
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
            }) || this;
            _this.Popup = null;
            _this.Enabled = true;
            _this.Focused = false;
            _this.DropDownWidth = 0;
            _this.IndexOnOpen = -1;
            _this.MouseSelectedIndex = -1;
            _this.Setup = setup;
            _this.Input = $YetaWF.getElement1BySelector(".t_input", [_this.Control]);
            _this.Select = $YetaWF.getElement1BySelector("select", [_this.Control]);
            _this.Container = $YetaWF.getElement1BySelector(".t_container", [_this.Control]);
            _this.Enabled = !$YetaWF.elementHasClass(_this.Container, "t_disabled");
            _this.optionsUpdated();
            $YetaWF.registerEventHandler(_this.Container, "mouseenter", null, function (ev) {
                if (_this.Enabled) {
                    $YetaWF.elementRemoveClass(_this.Container, "t_hover");
                    $YetaWF.elementAddClass(_this.Container, "t_hover");
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Container, "mouseleave", null, function (ev) {
                if (_this.Enabled)
                    $YetaWF.elementRemoveClass(_this.Container, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(_this.Container, "click", null, function (ev) {
                if (_this.Enabled)
                    _this.openPopup();
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "focusin", null, function (ev) {
                if (_this.Enabled) {
                    $YetaWF.elementRemoveClass(_this.Container, "t_focused");
                    $YetaWF.elementAddClass(_this.Container, "t_focused");
                    _this.Focused = true;
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "focusout", null, function (ev) {
                $YetaWF.elementRemoveClass(_this.Container, "t_focused");
                _this.Focused = false;
                _this.closePopup(SendSelectEnum.ChangeSinceOpen);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "keydown", null, function (ev) {
                if (_this.Enabled) {
                    var key = ev.key;
                    if (ev.altKey) {
                        if (key === "ArrowDown" || key === "Down" || key === "ArrowRight" || key === "Right") {
                            _this.openPopup();
                            return false;
                        }
                        if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                            _this.closePopup(SendSelectEnum.Yes);
                            return false;
                        }
                    }
                    else {
                        if (key === "ArrowDown" || key === "Down" || key === "ArrowRight" || key === "Right") {
                            _this.setSelectedIndex(_this.selectedIndex + 1);
                            return false;
                        }
                        else if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                            _this.setSelectedIndex(_this.selectedIndex - 1);
                            return false;
                        }
                        else if (key === "Home") {
                            _this.setSelectedIndex(0);
                            return false;
                        }
                        else if (key === "End") {
                            var total = _this.totalItems;
                            _this.setSelectedIndex(total - 1);
                            return false;
                        }
                        else if (key === "Escape") {
                            if (_this.isOpen) {
                                _this.closePopup(SendSelectEnum.No);
                                return false;
                            }
                            return true;
                        }
                        else if (key === "Tab") {
                            _this.closePopup(SendSelectEnum.ChangeSinceOpen);
                            return true;
                        }
                        else if (key === "Enter") {
                            if (_this.Popup) {
                                _this.closePopup(SendSelectEnum.ChangeSinceOpen);
                                return false;
                            }
                        }
                        else if (key.length === 1) {
                            // find an entry starting with the character pressed
                            var opts = _this.Select.options;
                            var len = opts.length;
                            for (var i = _this.selectedIndex + 1; i < len; ++i) {
                                key = key.toLowerCase();
                                if (opts[i].text.toLowerCase().startsWith(key)) {
                                    _this.setSelectedIndex(i);
                                    return true;
                                }
                            }
                            if (_this.selectedIndex > 0) {
                                var end = _this.selectedIndex;
                                for (var i = 0; i < end; ++i) {
                                    key = key.toLowerCase();
                                    if (opts[i].text.toLowerCase().startsWith(key)) {
                                        _this.setSelectedIndex(i);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            });
            return _this;
        }
        Object.defineProperty(DropDownListEditComponent.prototype, "value", {
            get: function () {
                return this.Select.value;
            },
            set: function (val) {
                this.Select.value = val;
                this.clearSelectedPopupItem();
                this.selectPopupItem();
                if (this.selectedIndex >= 0)
                    this.Input.innerText = this.Select.options[this.selectedIndex].text;
                else
                    this.Input.innerHTML = "&zwj;";
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DropDownListEditComponent.prototype, "selectedIndex", {
            get: function () {
                return this.Select.selectedIndex;
            },
            enumerable: false,
            configurable: true
        });
        DropDownListEditComponent.prototype.setSelectedIndex = function (index, event) {
            var total = this.totalItems;
            if (index < 0 || index >= total)
                return;
            this.Select.selectedIndex = index;
            this.Select.options[index].selected = true;
            this.clearSelectedPopupItem();
            this.selectPopupItem();
            this.Input.innerText = this.Select.options[index].text;
            if (event !== false && !this.isOpen)
                this.sendChangeEvent();
        };
        Object.defineProperty(DropDownListEditComponent.prototype, "totalItems", {
            get: function () {
                return this.Select.options.length;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DropDownListEditComponent.prototype, "isOpen", {
            get: function () {
                return this.Popup != null;
            },
            enumerable: false,
            configurable: true
        });
        DropDownListEditComponent.prototype.setOptionsHTML = function (optionsHTML) {
            this.Select.innerHTML = optionsHTML;
            this.optionsUpdated();
            this.setSelectedIndex(0);
        };
        // retrieve the tooltip for the nth item (index) in the dropdown list
        DropDownListEditComponent.prototype.getToolTip = function (index) {
            var total = this.totalItems;
            if (index < 0 || index >= total)
                return null;
            var opt = this.Select.options[index];
            var tt = $YetaWF.getAttributeCond(opt, YConfigs.Basics.CssTooltip);
            return tt;
        };
        DropDownListEditComponent.prototype.clear = function () {
            this.closePopup(SendSelectEnum.No);
            this.setSelectedIndex(0, false);
        };
        DropDownListEditComponent.prototype.enable = function (enabled) {
            this.closePopup(SendSelectEnum.No);
            $YetaWF.elementEnableToggle(this.Select, enabled);
            $YetaWF.elementEnableToggle(this.Container, enabled);
            $YetaWF.elementRemoveClass(this.Container, "t_disabled");
            this.Control.removeAttribute("tabindex");
            if (!enabled) {
                $YetaWF.elementAddClass(this.Container, "t_disabled");
                $YetaWF.setAttribute(this.Control, "aria-disabled", "true");
            }
            else {
                $YetaWF.setAttribute(this.Control, "tabindex", "0");
                $YetaWF.setAttribute(this.Control, "aria-disabled", "false");
            }
            this.Enabled = enabled;
        };
        DropDownListEditComponent.prototype.sendChangeEvent = function () {
            $YetaWF.sendCustomEvent(this.Control, DropDownListEditComponent.EVENTCHANGE);
            FormsSupport.validateElement(this.Select);
        };
        DropDownListEditComponent.prototype.optionsUpdated = function () {
            this.DropDownWidth = this.calcMaxStringLength();
            if (this.Setup.AdjustWidth) {
                this.Control.style.width = "".concat(this.DropDownWidth, "px");
                //} else {
                //    this.Control.style.minWidth = `${this.DropDownWidth}px`;
            }
        };
        DropDownListEditComponent.prototype.openPopup = function () {
            var _this = this;
            if (this.Popup) {
                this.closePopup(SendSelectEnum.No);
                return;
            }
            this.IndexOnOpen = this.selectedIndex;
            DropDownListEditComponent.closeDropdowns();
            this.Popup =
                $YetaWF.createElement("div", { id: DropDownListEditComponent.POPUPID, "data-owner": this.ControlId, "aria-hidden": "false" },
                    $YetaWF.createElement("div", { class: "t_container", "data-role": "popup", "aria-hidden": "false" },
                        $YetaWF.createElement("div", { class: "t_scroller", unselectable: "on" },
                            $YetaWF.createElement("ul", { unselectable: "on", class: "t_list", tabindex: "-1", "aria-hidden": "true", "aria-live": "off", "data-role": "staticlist", role: "listbox" }))));
            var ul = $YetaWF.getElement1BySelector("ul", [this.Popup]);
            var opts = this.Select.options;
            var len = opts.length;
            var html = "";
            for (var i = 0; i < len; ++i) {
                var o = opts[i];
                var tt = o.getAttribute(YConfigs.Basics.CssTooltip);
                if (tt)
                    tt = " ".concat(YConfigs.Basics.CssTooltip, "=\"").concat(tt, "\"");
                else
                    tt = "";
                html += "<li tabindex=\"-1\" role=\"option\" unselectable=\"on\" class=\"t_item\" data-index=\"".concat(i, "\"").concat(tt, ">").concat(o.innerHTML, "</li>");
            }
            ul.innerHTML = html;
            var style = window.getComputedStyle(this.Control);
            this.Popup.style.font = style.font;
            this.Popup.style.fontStyle = style.fontStyle;
            this.Popup.style.fontWeight = style.fontWeight;
            this.Popup.style.fontSize = style.fontSize;
            DropDownListEditComponent.positionPopup(this.Popup);
            document.body.appendChild(this.Popup);
            this.selectPopupItem();
            this.Control.setAttribute("aria-expanded", "true");
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
        DropDownListEditComponent.prototype.closePopup = function (sendEvent) {
            ToolTipsHTMLHelper.removeTooltips();
            if (!this.Popup) {
                if (sendEvent === SendSelectEnum.ChangeSinceOpen && this.IndexOnOpen !== -1 && this.selectedIndex !== this.IndexOnOpen) {
                    this.IndexOnOpen = -1;
                    this.sendChangeEvent();
                }
            }
            else {
                this.Popup.remove();
                this.Popup = null;
                this.Control.setAttribute("aria-expanded", "false");
                if (sendEvent === SendSelectEnum.Yes) {
                    this.IndexOnOpen = -1;
                    this.sendChangeEvent();
                }
                else if (sendEvent === SendSelectEnum.ChangeSinceOpen && this.IndexOnOpen !== -1 && this.IndexOnOpen !== this.selectedIndex) {
                    this.IndexOnOpen = -1;
                    this.sendChangeEvent();
                }
            }
        };
        DropDownListEditComponent.positionPopup = function (popup) {
            if (!popup)
                return;
            var ownerId = $YetaWF.getAttribute(popup, "data-owner");
            var control = DropDownListEditComponent.getControlById(ownerId, DropDownListEditComponent.SELECTOR);
            var scroller = $YetaWF.getElement1BySelector(".t_scroller", [popup]);
            // resize to fit
            var controlRect = control.Control.getBoundingClientRect();
            var desiredHeight = control.Setup.DropDownHeightFactor * DropDownListEditComponent.DEFAULTHEIGHT;
            var desiredWidth = control.Setup.DropDownWidthFactor * controlRect.width;
            var bottomAvailable = window.innerHeight - controlRect.bottom;
            var topAvailable = controlRect.top;
            var top = 0, bottom = 0;
            // Top/bottom position and height calculation
            var useTop = true;
            if (bottomAvailable < desiredHeight && topAvailable > bottomAvailable) {
                useTop = false;
                bottom = window.innerHeight - controlRect.top;
                if (topAvailable < desiredHeight)
                    desiredHeight = topAvailable;
            }
            else {
                top = controlRect.bottom;
                if (bottomAvailable < desiredHeight)
                    bottomAvailable = desiredHeight;
            }
            // Left/Width calculation
            var left = 0, right = 0;
            var useLeft = true;
            if (desiredWidth > window.innerWidth) {
                useLeft = false;
                left = 0;
                desiredWidth = window.innerWidth;
            }
            else {
                if (controlRect.left + desiredWidth > window.innerWidth) {
                    useLeft = false;
                    right = 0;
                }
                else if (controlRect.left < 0) {
                    left = 0;
                }
                else {
                    left = controlRect.left;
                }
            }
            // set left, top, width on #yDDPopup
            if (useTop) {
                popup.style.top = "".concat(top + window.pageYOffset, "px");
            }
            else {
                popup.style.bottom = "".concat(bottom - window.pageYOffset, "px");
            }
            if (useLeft) {
                popup.style.left = "".concat(left + window.pageXOffset, "px");
            }
            else {
                popup.style.right = "".concat(right - window.pageXOffset, "px");
            }
            popup.style.width = "".concat(desiredWidth, "px");
            scroller.style.maxHeight = "".concat(desiredHeight, "px");
        };
        DropDownListEditComponent.prototype.selectPopupItem = function () {
            var index = this.Select.selectedIndex;
            if (index < 0)
                return;
            if (this.Popup) {
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
            }
        };
        DropDownListEditComponent.prototype.clearSelectedPopupItem = function () {
            if (this.Popup) {
                var lis = $YetaWF.getElementsBySelector("ul li.t_selected", [this.Popup]);
                for (var _i = 0, lis_1 = lis; _i < lis_1.length; _i++) {
                    var li = lis_1[_i];
                    $YetaWF.elementRemoveClasses(li, ["t_selected", "t_focused"]);
                    $YetaWF.setAttribute(li, "aria-selected", "false");
                    li.id = "";
                }
            }
            this.Input.innerHTML = "&zwj;";
        };
        DropDownListEditComponent.closeDropdowns = function () {
            var popup = $YetaWF.getElementByIdCond(DropDownListEditComponent.POPUPID);
            if (!popup)
                return;
            var ownerId = $YetaWF.getAttribute(popup, "data-owner");
            var control = DropDownListEditComponent.getControlById(ownerId, DropDownListEditComponent.SELECTOR);
            control.closePopup(SendSelectEnum.No);
        };
        DropDownListEditComponent.prototype.calcMaxStringLength = function () {
            var elem = $YetaWF.createElement("div", { style: "position:absolute;visibility:hidden;white-space:nowrap" });
            document.body.appendChild(elem);
            // copy font attributes
            var style = window.getComputedStyle(this.Control);
            elem.style.font = style.font;
            elem.style.fontStyle = style.fontStyle;
            elem.style.fontWeight = style.fontWeight;
            elem.style.fontSize = style.fontSize;
            var width = 0;
            var opts = this.Select.options;
            var len = opts.length;
            for (var i = 0; i < len; ++i) {
                elem.innerHTML = opts[i].innerHTML;
                var w = elem.clientWidth;
                if (w > width)
                    width = w;
            }
            // extra for dropdown selector
            elem.innerText = "MMM"; // 3 characters
            width += elem.clientWidth;
            width += 4; // fudge factor
            elem.remove();
            return width;
        };
        DropDownListEditComponent.prototype.updateContents = function (data, url, onSuccess, onFailure) {
            var _this = this;
            this.closePopup(SendSelectEnum.No);
            var uri = $YetaWF.parseUrl(url);
            uri.addSearchSimpleObject(data);
            var formJson = $YetaWF.Forms.getJSONInfo(this.Control);
            $YetaWF.postJSON(uri, formJson, null, null, function (success, data) {
                if (success) {
                    _this.setOptionsHTML(data.OptionsHTML);
                    if (onSuccess)
                        onSuccess(data);
                }
                else {
                    if (onFailure)
                        onFailure();
                }
            });
        };
        DropDownListEditComponent.TEMPLATE = "yt_dropdownlist_base";
        DropDownListEditComponent.SELECTOR = ".yt_dropdownlist_base.t_edit";
        DropDownListEditComponent.EVENTCHANGE = "dropdownlist_change";
        DropDownListEditComponent.POPUPID = "yDDPopup";
        DropDownListEditComponent.DEFAULTHEIGHT = 200;
        return DropDownListEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.DropDownListEditComponent = DropDownListEditComponent;
    // handle submit/apply
    $YetaWF.registerCustomEventHandlerDocument(DropDownListEditComponent.EVENTCHANGE, ".ysubmitonchange ".concat(DropDownListEditComponent.SELECTOR), function (ev) {
        $YetaWF.Forms.submitOnChange(ev.target);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument(DropDownListEditComponent.EVENTCHANGE, ".yapplyonchange ".concat(DropDownListEditComponent.SELECTOR), function (ev) {
        $YetaWF.Forms.applyOnChange(ev.target);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument(DropDownListEditComponent.EVENTCHANGE, ".yreloadonchange ".concat(DropDownListEditComponent.SELECTOR), function (ev) {
        $YetaWF.Forms.reloadOnChange(ev.target);
        return false;
    });
    // close dropdown when clicking outside
    $YetaWF.registerEventHandlerBody("click", null, function (ev) {
        DropDownListEditComponent.closeDropdowns();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, function (ev) {
        DropDownListEditComponent.closeDropdowns();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        DropDownListEditComponent.closeDropdowns();
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=DropDownListEdit.js.map
