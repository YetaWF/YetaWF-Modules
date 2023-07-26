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
    var OrientationEnum;
    (function (OrientationEnum) {
        OrientationEnum[OrientationEnum["Horizontal"] = 0] = "Horizontal";
        OrientationEnum[OrientationEnum["Vertical"] = 1] = "Vertical";
    })(OrientationEnum || (OrientationEnum = {}));
    var HorizontalAlignEnum;
    (function (HorizontalAlignEnum) {
        HorizontalAlignEnum[HorizontalAlignEnum["Right"] = 0] = "Right";
        HorizontalAlignEnum[HorizontalAlignEnum["Left"] = 1] = "Left";
    })(HorizontalAlignEnum || (HorizontalAlignEnum = {}));
    var MenuComponent = /** @class */ (function (_super) {
        __extends(MenuComponent, _super);
        function MenuComponent(controlId, setup) {
            var _this = _super.call(this, controlId, MenuComponent.TEMPLATE, MenuComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.CloseTimeout = 0;
            _this.MenuRects = [];
            _this.CloseSublevelsTimout = 0;
            _this.Setup = setup;
            _this.updateSize();
            // update aria info
            var aSubs = $YetaWF.getElementsBySelector("li.t_menu.t_hassub > a", [_this.Control]);
            for (var _i = 0, aSubs_1 = aSubs; _i < aSubs_1.length; _i++) {
                var aSub = aSubs_1[_i];
                aSub.setAttribute("aria-haspopup", "true");
                aSub.setAttribute("aria-expanded", "false");
            }
            var liSubs = $YetaWF.getElementsBySelector("li.t_menu > a", [_this.Control]);
            $YetaWF.registerMultipleEventHandlers(liSubs, ["mouseenter"], null, function (ev) {
                if (_this.isVertical || _this.isSmall)
                    return true;
                var owningAnchor = ev.__YetaWFElem;
                var owningLI = $YetaWF.elementClosest(owningAnchor, "li");
                if ($YetaWF.elementHasClass(owningLI, "t_megamenu_content"))
                    return true; //we're within a megamenu (can't have menus within megamenu)
                var subUL = $YetaWF.getElement1BySelectorCond("ul.t_menu", [owningLI]);
                if (!subUL) {
                    _this.scheduleCloseSublevelsStartingAt(owningAnchor);
                    return true;
                }
                _this.closeSublevelsStartingAt(owningAnchor);
                _this.openSublevel(owningAnchor);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers(liSubs, ["click"], null, function (ev) {
                if (_this.isSmall)
                    return true; // allow anchor processing
                var owningAnchor = ev.__YetaWFElem;
                var owningLI = $YetaWF.elementClosest(owningAnchor, "li");
                if ($YetaWF.elementHasClass(owningLI, "t_megamenu_content"))
                    return true; //we're within a megamenu (can't have menus within megamenu)
                var subUL = $YetaWF.getElement1BySelectorCond("ul", [owningLI]);
                var isOpen = subUL && subUL.style.display === "";
                _this.closeSublevelsStartingAt(owningAnchor);
                if (isOpen)
                    _this.closeLevel(owningAnchor);
                else
                    _this.openSublevel(owningAnchor);
                return true; // allow anchor processing
            });
            $YetaWF.registerEventHandler(_this.Control, "click", "li.t_menu > a > svg", function (ev) {
                if (!_this.isSmall)
                    return true;
                var svg = ev.__YetaWFElem;
                var owningAnchor = svg.parentElement;
                _this.closeSublevelsStartingAt(owningAnchor, owningAnchor);
                _this.openSublevel(owningAnchor);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers(liSubs, ["keydown"], null, function (ev) {
                var key = ev.key;
                if (key === "Enter") {
                    var owningAnchor = ev.__YetaWFElem;
                    // we're in a menu and someone hit Enter on an anchor
                    _this.closeSublevelsStartingAt(owningAnchor, owningAnchor);
                    _this.openSublevel(owningAnchor);
                }
                return true;
            });
            return _this;
        }
        MenuComponent.prototype.openSublevel = function (owningAnchor) {
            var level = this.getLevel(owningAnchor);
            this.openLevel(owningAnchor);
            if (this.isVertical || this.isSmall) {
            }
            else {
                // position the sublevel
                var subUL = owningAnchor.nextElementSibling;
                if (!subUL)
                    return;
                var owningLI = $YetaWF.elementClosest(owningAnchor, "li");
                var owningRect = owningLI.getBoundingClientRect();
                switch (level) {
                    case 0:
                        if (this.Setup.HorizontalAlign === HorizontalAlignEnum.Right)
                            subUL.style.left = "0";
                        else
                            subUL.style.right = "0";
                        subUL.style.top = "".concat(owningRect.height, "px");
                        break;
                    case 1:
                        if (this.Setup.HorizontalAlign === HorizontalAlignEnum.Right)
                            subUL.style.left = "".concat(owningRect.width - 3, "px"); // slight overlap
                        else
                            subUL.style.right = "".concat(owningRect.width - 3, "px"); // slight overlap
                        subUL.style.top = "-3px";
                        break;
                    case 2:
                        if (this.Setup.HorizontalAlign === HorizontalAlignEnum.Right)
                            subUL.style.left = "".concat(owningRect.width - 3, "px"); // slight overlap
                        else
                            subUL.style.right = "".concat(owningRect.width - 3, "px"); // slight overlap
                        subUL.style.top = "-3px";
                        break;
                    default:
                        throw "Too many menu levels";
                }
            }
        };
        MenuComponent.prototype.scheduleCloseSublevelsStartingAt = function (owningAnchor) {
            var _this = this;
            this.clearScheduleCloseSublevel();
            this.CloseSublevelsTimout = setTimeout(function () {
                _this.closeSublevelsStartingAt(owningAnchor);
            }, this.Setup.HoverDelay || 1);
        };
        MenuComponent.prototype.clearScheduleCloseSublevel = function () {
            clearTimeout(this.CloseSublevelsTimout);
            this.CloseSublevelsTimout = 0;
        };
        MenuComponent.prototype.closeSublevelsStartingAt = function (anchor, exceptAnchor) {
            this.clearScheduleCloseSublevel();
            var owningUL = $YetaWF.elementClosest(anchor, "ul");
            var owningAnchor = owningUL.previousElementSibling;
            if (owningAnchor && owningAnchor.tagName === "A") {
                // sublevel
                this.internalCloseSublevelsStartingAt(owningAnchor, exceptAnchor);
            }
            else {
                // main level
                var anchors = $YetaWF.getElementsBySelector("ul.".concat(MenuComponent.TEMPLATE, " > li > a"), [this.Control]); // all top level anchors
                for (var _i = 0, anchors_1 = anchors; _i < anchors_1.length; _i++) {
                    var a = anchors_1[_i];
                    this.internalCloseSublevelsStartingAt(a, exceptAnchor);
                    this.closeLevel(a);
                }
            }
        };
        MenuComponent.prototype.internalCloseSublevelsStartingAt = function (owningAnchor, exceptAnchor) {
            var subUL = owningAnchor.nextElementSibling;
            if (!subUL)
                return;
            var subLIs = $YetaWF.getChildElementsByTag("li", subUL);
            for (var _i = 0, subLIs_1 = subLIs; _i < subLIs_1.length; _i++) {
                var subLI = subLIs_1[_i];
                var anchor = $YetaWF.getChildElement1ByTagCond("a", subLI);
                if (anchor && anchor != exceptAnchor) {
                    var subUL_1 = owningAnchor.nextElementSibling;
                    if (subUL_1)
                        this.internalCloseSublevelsStartingAt(anchor, exceptAnchor);
                    this.closeLevel(anchor);
                }
            }
        };
        MenuComponent.prototype.getLevel = function (owningAnchor) {
            var level = 0;
            var elem = owningAnchor;
            while (elem) {
                elem = $YetaWF.elementClosestCond(elem.parentElement, "ul");
                if (!elem || $YetaWF.elementHasClass(elem, MenuComponent.TEMPLATE))
                    return level;
                ++level;
            }
            return level;
        };
        MenuComponent.prototype.openLevel = function (owningAnchor) {
            var _this = this;
            owningAnchor.setAttribute("aria-expanded", "true");
            var subUL = owningAnchor.nextElementSibling;
            if (!subUL)
                return;
            if (this.isVertical || this.isSmall) {
                $YetaWF.animateHeight(subUL, true, function () {
                    subUL.style.height = "auto"; // height to auto, so submenus can expand
                    _this.MenuRects.push({ Anchor: owningAnchor, Rect: subUL.getBoundingClientRect(), });
                });
            }
            else {
                subUL.style.display = ""; // show
                this.MenuRects.push({ Anchor: owningAnchor, Rect: subUL.getBoundingClientRect(), });
            }
        };
        MenuComponent.prototype.closeLevel = function (owningAnchor) {
            var subUL = owningAnchor.nextElementSibling;
            if (subUL) {
                owningAnchor.setAttribute("aria-expanded", "false");
                if (this.isVertical || this.isSmall) {
                    $YetaWF.animateHeight(subUL, false, function () {
                        subUL.style.display = "none"; // hide
                    });
                }
                else {
                    subUL.style.display = "none"; // hide
                }
                this.MenuRects = this.MenuRects.filter(function (v, index) {
                    return v.Anchor !== owningAnchor;
                });
            }
        };
        MenuComponent.prototype.handleMouseMove = function (cursorX, cursorY) {
            if (this.isVertical || this.isSmall)
                return true;
            var mainRect = this.Control.getBoundingClientRect();
            if (mainRect.left <= cursorX && cursorX < mainRect.right && mainRect.top <= cursorY && cursorY < mainRect.bottom) {
                this.killTimeout();
                return true;
            }
            for (var _i = 0, _a = this.MenuRects; _i < _a.length; _i++) {
                var menuRect = _a[_i];
                var rect = menuRect.Rect;
                if (rect.left <= cursorX && cursorX < rect.right && rect.top <= cursorY && cursorY < rect.bottom) {
                    this.killTimeout();
                    return true;
                }
            }
            this.startTimeout();
            return true;
        };
        MenuComponent.prototype.killTimeout = function () {
            if (this.CloseTimeout) {
                clearTimeout(this.CloseTimeout);
                this.CloseTimeout = 0;
            }
        };
        MenuComponent.prototype.startTimeout = function () {
            var _this = this;
            if (!this.CloseTimeout) {
                this.CloseTimeout = setTimeout(function () {
                    _this.closeAll();
                }, this.Setup.HoverDelay || 1);
            }
        };
        MenuComponent.prototype.clearPath = function () {
            var anchors = $YetaWF.getElementsBySelector("ul.t_menu > li > a", [this.Control]);
            for (var _i = 0, anchors_2 = anchors; _i < anchors_2.length; _i++) {
                var a = anchors_2[_i];
                $YetaWF.elementRemoveClass(a, "t_path");
                $YetaWF.elementRemoveClass($YetaWF.elementClosest(a, "li"), "t_path");
                $YetaWF.elementRemoveClass($YetaWF.elementClosest(a, "ul"), "t_path");
            }
        };
        MenuComponent.prototype.updateSize = function () {
            if (this.isSmall) {
                if (!$YetaWF.elementHasClass(this.Control, "t_small")) {
                    $YetaWF.elementRemoveClasses(this.Control, ["t_large", "t_small", "t_horizontal", "t_vertical"]);
                    $YetaWF.elementAddClass(this.Control, "t_small");
                    this.Control.style.width = "";
                    this.hide();
                    this.closeAll();
                }
            }
            else {
                if (!$YetaWF.elementHasClass(this.Control, "t_large")) {
                    $YetaWF.elementRemoveClasses(this.Control, ["t_large", "t_small", "t_horizontal", "t_vertical"]);
                    $YetaWF.elementAddClass(this.Control, "t_large");
                    if (this.isVertical) {
                        $YetaWF.elementAddClass(this.Control, "t_vertical");
                        this.Control.style.width = "".concat(this.Setup.VerticalWidth, "px");
                    }
                    else {
                        $YetaWF.elementAddClass(this.Control, "t_horizontal");
                        this.Control.style.width = "";
                    }
                    this.show();
                    this.closeAll();
                }
            }
        };
        // API
        MenuComponent.prototype.closeAll = function () {
            this.MenuRects = [];
            this.clearScheduleCloseSublevel();
            if (this.isVertical)
                return;
            var anchors = $YetaWF.getElementsBySelector("ul.".concat(MenuComponent.TEMPLATE, " > li > a"), [this.Control]); // all top level anchors
            for (var _i = 0, anchors_3 = anchors; _i < anchors_3.length; _i++) {
                var anchor = anchors_3[_i];
                this.closeSublevelsStartingAt(anchor);
                this.closeLevel(anchor);
            }
            if (this.isSmall)
                this.hide();
        };
        MenuComponent.closeAllMenus = function () {
            var controls = YetaWF.ComponentBaseDataImpl.getControls(MenuComponent.SELECTOR);
            for (var _i = 0, controls_1 = controls; _i < controls_1.length; _i++) {
                var control = controls_1[_i];
                control.closeAll();
            }
        };
        Object.defineProperty(MenuComponent.prototype, "isShown", {
            get: function () {
                return this.Control.style.display !== "none";
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(MenuComponent.prototype, "isSmall", {
            get: function () {
                var small = false;
                if (this.Setup.SmallMenuMaxWidth !== 0) {
                    if (window.innerWidth <= this.Setup.SmallMenuMaxWidth)
                        small = true;
                }
                else {
                    small = false;
                }
                return small;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(MenuComponent.prototype, "isHorizontal", {
            get: function () {
                return !this.isSmall && this.Setup.Orientation == OrientationEnum.Horizontal;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(MenuComponent.prototype, "isVertical", {
            get: function () {
                return !this.isSmall && this.Setup.Orientation == OrientationEnum.Vertical;
            },
            enumerable: false,
            configurable: true
        });
        MenuComponent.prototype.show = function () {
            this.Control.style.display = "";
        };
        MenuComponent.prototype.hide = function () {
            this.Control.style.display = "none";
        };
        MenuComponent.prototype.selectEntry = function (path) {
            this.clearPath();
            var subUL = this.Control;
            return this.selectSublevelEntry(this.normalizePath(path), subUL);
        };
        MenuComponent.prototype.selectSublevelEntry = function (path, subUL) {
            var result = false;
            var subLI = $YetaWF.getChildElement1ByTagCond("li", subUL);
            while (!!subLI) {
                var anchor = $YetaWF.getChildElement1ByTagCond("a", subLI);
                if (anchor) {
                    if (this.normalizePath(anchor.href) === path) {
                        $YetaWF.elementToggleClass(subLI, "t_selected", true);
                        $YetaWF.elementToggleClass(subLI, "t_path", true);
                        $YetaWF.elementToggleClass(subUL, "t_path", true);
                        $YetaWF.elementToggleClass(anchor, "t_path", true);
                        result = true;
                    }
                    else {
                        $YetaWF.elementRemoveClasses(subLI, ["t_selected"]);
                    }
                    var subSubUL = $YetaWF.getChildElement1ByTagCond("ul", subLI);
                    if (subSubUL) {
                        if (this.selectSublevelEntry(path, subSubUL)) {
                            $YetaWF.elementToggleClass(subLI, "t_path", true);
                            $YetaWF.elementToggleClass(subUL, "t_path", true);
                            $YetaWF.elementToggleClass(anchor, "t_path", true);
                            if (this.isVertical)
                                subSubUL.style.display = ""; // expand it (no transition)
                            result = true;
                        }
                    }
                }
                subLI = subLI.nextElementSibling;
            }
            return result;
        };
        MenuComponent.prototype.normalizePath = function (path) {
            var i = path.indexOf("?");
            if (i > 0)
                path = path.substring(0, i);
            return path.toLowerCase();
        };
        MenuComponent.TEMPLATE = "yt_menu";
        MenuComponent.SELECTOR = ".yt_menu.t_display";
        return MenuComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.MenuComponent = MenuComponent;
    $YetaWF.registerEventHandlerBody("mousemove", null, function (ev) {
        var controls = YetaWF.ComponentBaseDataImpl.getControls(MenuComponent.SELECTOR);
        for (var _i = 0, controls_2 = controls; _i < controls_2.length; _i++) {
            var control = controls_2[_i];
            control.handleMouseMove(ev.clientX, ev.clientY);
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        var menus = YetaWF.ComponentBaseDataImpl.getControls(MenuComponent.SELECTOR, [ev.detail.container]);
        for (var _i = 0, menus_1 = menus; _i < menus_1.length; _i++) {
            var menu = menus_1[_i];
            menu.updateSize();
        }
        return true;
    });
    // handle new content
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        var menus = YetaWF.ComponentBaseDataImpl.getControls(MenuComponent.SELECTOR);
        for (var _i = 0, menus_2 = menus; _i < menus_2.length; _i++) {
            var menu = menus_2[_i];
            menu.closeAll();
            menu.selectEntry(window.location.href);
        }
        return true;
    });
    // Handle Escape key to close any open menus
    $YetaWF.registerEventHandlerBody("keydown", null, function (ev) {
        if (ev.key !== "Escape")
            return true;
        var menus = YetaWF.ComponentBaseDataImpl.getControls(MenuComponent.SELECTOR);
        for (var _i = 0, menus_3 = menus; _i < menus_3.length; _i++) {
            var menu = menus_3[_i];
            menu.closeAll();
        }
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Menu.js.map
