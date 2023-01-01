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
    var MenuComponent = /** @class */ (function (_super) {
        __extends(MenuComponent, _super);
        function MenuComponent(controlId, setup) {
            var _this = _super.call(this, controlId, MenuComponent.TEMPLATE, MenuComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.Levels = [];
            _this.CloseTimeout = 0;
            _this.CloseSublevelsTimout = 0;
            _this.Setup = setup;
            _this.updateSize();
            // update aria info
            var aSubs = $YetaWF.getElementsBySelector("li.t_hassub > a", [_this.Control]);
            for (var _i = 0, aSubs_1 = aSubs; _i < aSubs_1.length; _i++) {
                var aSub = aSubs_1[_i];
                aSub.setAttribute("aria-haspopup", "true");
                aSub.setAttribute("aria-expanded", "false");
            }
            var liSubs = $YetaWF.getElementsBySelector("li > a", [_this.Control]);
            $YetaWF.registerMultipleEventHandlers(liSubs, ["mouseenter"], null, function (ev) {
                if (_this.isSmall)
                    return true;
                var owningAnchor = ev.__YetaWFElem;
                var owningLI = $YetaWF.elementClosest(owningAnchor, "li");
                if ($YetaWF.elementHasClass(owningLI, "t_megamenu_content"))
                    return true; //we're within a megamenu (can't have menus within megamenu)
                var owningUL = $YetaWF.elementClosest(owningAnchor, "ul");
                var subUL = $YetaWF.getElement1BySelectorCond("ul", [owningLI]);
                if (!subUL) {
                    _this.scheduleCloseSublevelsStartingAt(owningUL);
                    return true;
                }
                var subLI = $YetaWF.getElement1BySelector("li", [subUL]);
                var levelInfo = { owningUL: owningUL, owningLI: owningLI, owningAnchor: owningAnchor, subUL: subUL, subLI: subLI };
                if (_this.closeSublevelsForNewSublevel(levelInfo))
                    _this.openSublevel(levelInfo);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers(liSubs, ["click"], null, function (ev) {
                if (!_this.isSmall)
                    return true;
                var owningAnchor = ev.__YetaWFElem;
                var owningLI = $YetaWF.elementClosest(owningAnchor, "li");
                if ($YetaWF.elementHasClass(owningLI, "t_megamenu_content"))
                    return true; //we're within a megamenu (can't have menus within megamenu)
                var owningUL = $YetaWF.elementClosest(owningAnchor, "ul");
                var subUL = $YetaWF.getElement1BySelectorCond("ul", [owningLI]);
                if (!subUL) {
                    _this.closeSublevelsStartingAt(owningUL);
                    return true;
                }
                var subLI = $YetaWF.getElement1BySelector("li", [subUL]);
                var levelInfo = { owningUL: owningUL, owningLI: owningLI, owningAnchor: owningAnchor, subUL: subUL, subLI: subLI };
                if (_this.closeSublevelsForNewSublevel(levelInfo))
                    _this.openSublevel(levelInfo);
                else
                    return true; // allow anchor processing
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "click", "li > a > svg", function (ev) {
                if (!_this.isSmall)
                    return true;
                var svg = ev.__YetaWFElem;
                var owningAnchor = svg.parentElement;
                var owningLI = $YetaWF.elementClosest(owningAnchor, "li");
                var owningUL = $YetaWF.elementClosest(owningAnchor, "ul");
                var subUL = $YetaWF.getElement1BySelectorCond("ul", [owningLI]);
                if (!subUL) {
                    _this.closeSublevelsStartingAt(owningUL);
                    return true;
                }
                var subLI = $YetaWF.getElement1BySelector("li", [subUL]);
                var levelInfo = { owningUL: owningUL, owningLI: owningLI, owningAnchor: owningAnchor, subUL: subUL, subLI: subLI };
                if (_this.closeSublevelsForNewSublevel(levelInfo))
                    _this.openSublevel(levelInfo);
                else
                    _this.closeSublevelsStartingAt(owningUL);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers(liSubs, ["keydown"], null, function (ev) {
                var key = ev.key;
                if (key === "Enter") {
                    var owningAnchor = ev.__YetaWFElem;
                    var owningLI = $YetaWF.elementClosestCond(owningAnchor, "li");
                    if (!owningLI)
                        return true;
                    var owningUL = $YetaWF.elementClosestCond(owningAnchor, "ul");
                    if (!owningUL)
                        return true;
                    var subUL = $YetaWF.getElement1BySelectorCond("ul", [owningLI]);
                    if (!subUL)
                        return true; // no submenus
                    var subLI = $YetaWF.getElement1BySelector("li", [subUL]);
                    // we're in a menu and someone hit Enter on an anchor
                    var levelInfo = { owningUL: owningUL, owningLI: owningLI, owningAnchor: owningAnchor, subUL: subUL, subLI: subLI };
                    if (_this.closeSublevelsForNewSublevel(levelInfo))
                        _this.openSublevel(levelInfo);
                    else
                        _this.closeSublevelsStartingAt(owningUL);
                    return false;
                }
                return true;
            });
            return _this;
        }
        MenuComponent.prototype.openSublevel = function (levelInfo) {
            var level = this.Levels.length;
            levelInfo.subUL.style.display = ""; // open new sublevel
            var subUL = levelInfo.subUL;
            var owningLI = levelInfo.owningLI;
            var owningRect = owningLI.getBoundingClientRect();
            if (this.isSmall) {
            }
            else {
                // position the sublevel
                switch (level) {
                    case 0:
                        subUL.style.left = "0";
                        subUL.style.top = "".concat(owningRect.height, "px");
                        break;
                    case 1:
                        subUL.style.left = "".concat(owningRect.width - 3, "px"); // slight overlap
                        subUL.style.top = "-3px";
                        break;
                    case 2:
                        subUL.style.left = "".concat(owningRect.width - 3, "px"); // slight overlap
                        subUL.style.top = "-3px";
                        break;
                    default:
                        throw "Too many menu levels";
                }
            }
            this.clearPath();
            this.Levels.push(levelInfo);
            this.setPath();
        };
        MenuComponent.prototype.scheduleCloseSublevelsStartingAt = function (newOwningUL) {
            var _this = this;
            var closing = false; // defines whether any sublevels are to be closed
            if (!this.CloseSublevelsTimout) {
                for (var _i = 0, _a = this.Levels; _i < _a.length; _i++) {
                    var levelInfo = _a[_i];
                    if (!closing) {
                        if (levelInfo.owningUL === newOwningUL)
                            closing = true;
                    }
                }
                if (closing) {
                    this.CloseSublevelsTimout = setTimeout(function () {
                        _this.closeSublevelsStartingAt(newOwningUL);
                    }, this.Setup.HoverDelay || 1);
                }
            }
            return closing;
        };
        MenuComponent.prototype.closeSublevelsStartingAt = function (newOwningUL) {
            var newLevels = [];
            var closing = false;
            clearTimeout(this.CloseSublevelsTimout);
            this.CloseSublevelsTimout = 0;
            for (var _i = 0, _a = this.Levels; _i < _a.length; _i++) {
                var levelInfo = _a[_i];
                if (!closing) {
                    if (levelInfo.owningUL === newOwningUL)
                        closing = true;
                    else
                        newLevels.push(levelInfo);
                }
                if (closing)
                    levelInfo.subUL.style.display = "none";
            }
            this.clearPath();
            this.Levels = newLevels;
            this.setPath();
            return closing; // returns whether any sublevels were closed
        };
        MenuComponent.prototype.closeSublevelsForNewSublevel = function (newLevel) {
            var newLevels = [];
            var closing = false;
            clearTimeout(this.CloseSublevelsTimout);
            this.CloseSublevelsTimout = 0;
            for (var _i = 0, _a = this.Levels; _i < _a.length; _i++) {
                var levelInfo = _a[_i];
                if (!closing) {
                    if (levelInfo.owningUL === newLevel.owningUL) {
                        if (levelInfo.subUL === newLevel.subUL) // the new sublevel is already open
                            return false;
                        closing = true;
                    }
                    else
                        newLevels.push(levelInfo);
                }
                if (closing)
                    levelInfo.subUL.style.display = "none";
            }
            this.clearPath();
            this.Levels = newLevels;
            this.setPath();
            return true; // we closed all necessary sublevels
        };
        MenuComponent.prototype.handleMouseMove = function (cursorX, cursorY) {
            if (this.isSmall)
                return true;
            if (this.Levels.length > 0) {
                var rect = this.Levels[0].owningLI.getBoundingClientRect();
                if (rect.left <= cursorX && cursorX < rect.right && rect.top <= cursorY && cursorY < rect.bottom) {
                    this.killTimeout();
                    return true;
                }
                for (var _i = 0, _a = this.Levels; _i < _a.length; _i++) {
                    var levelInfo = _a[_i];
                    rect = levelInfo.subUL.getBoundingClientRect();
                    if (rect.left <= cursorX && cursorX < rect.right && rect.top <= cursorY && cursorY < rect.bottom) {
                        this.killTimeout();
                        return true;
                    }
                }
                this.startTimeout();
            }
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
                    for (var _i = 0, _a = _this.Levels; _i < _a.length; _i++) {
                        var levelInfo = _a[_i];
                        levelInfo.subUL.style.display = "none";
                    }
                    _this.clearPath();
                    _this.Levels = [];
                }, this.Setup.HoverDelay || 1);
            }
        };
        MenuComponent.prototype.setPath = function () {
            for (var _i = 0, _a = this.Levels; _i < _a.length; _i++) {
                var levelInfo = _a[_i];
                $YetaWF.elementAddClass(levelInfo.owningUL, "t_path");
                $YetaWF.elementAddClass(levelInfo.owningLI, "t_path");
                $YetaWF.elementAddClass(levelInfo.owningAnchor, "t_path");
            }
        };
        MenuComponent.prototype.clearPath = function () {
            for (var _i = 0, _a = this.Levels; _i < _a.length; _i++) {
                var levelInfo = _a[_i];
                $YetaWF.elementRemoveClass(levelInfo.owningUL, "t_path");
                $YetaWF.elementRemoveClass(levelInfo.owningLI, "t_path");
                $YetaWF.elementRemoveClass(levelInfo.owningAnchor, "t_path");
            }
        };
        MenuComponent.prototype.updateSize = function () {
            if (this.isSmall) {
                if (!$YetaWF.elementHasClass(this.Control, "t_small")) {
                    $YetaWF.elementRemoveClasses(this.Control, ["t_large", "t_small"]);
                    $YetaWF.elementAddClass(this.Control, "t_small");
                    this.hide();
                    this.closeAll();
                }
            }
            else {
                if (!$YetaWF.elementHasClass(this.Control, "t_large")) {
                    $YetaWF.elementRemoveClasses(this.Control, ["t_large", "t_small"]);
                    $YetaWF.elementAddClass(this.Control, "t_large");
                    this.show();
                    this.closeAll();
                }
            }
        };
        // API
        MenuComponent.prototype.closeAll = function () {
            for (var _i = 0, _a = this.Levels; _i < _a.length; _i++) {
                var levelInfo = _a[_i];
                levelInfo.subUL.style.display = "none";
            }
            this.clearPath();
            this.Levels = [];
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
        Object.defineProperty(MenuComponent.prototype, "isShown", {
            get: function () {
                return this.Control.style.display !== "none";
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
