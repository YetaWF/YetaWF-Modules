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
/// <reference types="kendo-ui" />
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var MenuULComponent = /** @class */ (function (_super) {
        __extends(MenuULComponent, _super);
        function MenuULComponent(controlId, setup) {
            var _this = _super.call(this, controlId, MenuULComponent.TEMPLATE, MenuULComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }, false, function (tag, control) {
                control.close();
            }) || this;
            _this.Levels = [];
            _this.CloseTimeout = 0;
            _this.isOpen = false;
            _this.CloseSublevelsTimout = 0;
            _this.Setup = setup;
            if (!$YetaWF.elementHasClass(_this.Control, "yt_menuul"))
                $YetaWF.elementAddClass(_this.Control, "yt_menuul");
            if (_this.Setup.Dynamic)
                $YetaWF.elementAddClass(_this.Control, "t_dynamic");
            // add icons to all items with submenu
            var aSubs = $YetaWF.getElementsBySelector("li.t_hassub > a", [_this.Control]);
            for (var _i = 0, aSubs_1 = aSubs; _i < aSubs_1.length; _i++) {
                var aSub = aSubs_1[_i];
                // icon used: fa-caret-right
                aSub.innerHTML += "<svg class='t_right' aria-hidden='true' focusable='false' role='img' viewBox='0 0 192 512'><path fill='currentColor' d='M0 384.662V127.338c0-17.818 21.543-26.741 34.142-14.142l128.662 128.662c7.81 7.81 7.81 20.474 0 28.284L34.142 398.804C21.543 411.404 0 402.48 0 384.662z'></path></svg>";
                aSub.setAttribute("aria-haspopup", "true");
                aSub.setAttribute("aria-expanded", "false");
            }
            var liSubs = $YetaWF.getElementsBySelector("li > a", [_this.Control]);
            $YetaWF.registerMultipleEventHandlers(liSubs, ["mouseenter"], null, function (ev) {
                var owningAnchor = ev.__YetaWFElem;
                var owningLI = $YetaWF.elementClosest(owningAnchor, "li");
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
            if (_this.Setup.AutoOpen)
                _this.open();
            return _this;
        }
        MenuULComponent.prototype.open = function () {
            var _this = this;
            if (!this.isOpen) {
                MenuULComponent.closeMenus();
                if (this.Setup.Click) {
                    var liSubs = $YetaWF.getElementsBySelector("li > a", [this.Control]);
                    $YetaWF.registerMultipleEventHandlers(liSubs, ["click"], null, function (ev) {
                        var owningAnchor = ev.__YetaWFElem;
                        var owningLI = $YetaWF.elementClosest(owningAnchor, "li");
                        MenuULComponent.closeMenus();
                        _this.Setup.Click(owningLI);
                        return false;
                    });
                }
                this.Control.style.display = "block";
                this.positionMenu();
                this.isOpen = true;
            }
        };
        MenuULComponent.prototype.openSublevel = function (levelInfo) {
            var level = this.Levels.length;
            levelInfo.subUL.style.display = ""; // open new sublevel
            var subUL = levelInfo.subUL;
            var owningLI = levelInfo.owningLI;
            var owningRect = owningLI.getBoundingClientRect();
            // position the sublevel
            switch (level) {
                case 0: // really t_lvl1
                    subUL.style.left = owningRect.width - 3 + "px"; // slight overlap
                    subUL.style.top = "-3px";
                    break;
                case 1: // really t_lvl2
                    subUL.style.left = owningRect.width - 3 + "px"; // slight overlap
                    subUL.style.top = "-3px";
                    break;
                default:
                    throw "Too many menu levels";
            }
            this.clearPath();
            this.Levels.push(levelInfo);
            this.setPath();
        };
        MenuULComponent.prototype.positionMenu = function () {
            if (this.Setup.AttachTo)
                $YetaWF.positionLeftAlignedBelow(this.Setup.AttachTo, this.Control);
        };
        MenuULComponent.prototype.scheduleCloseSublevelsStartingAt = function (newOwningUL) {
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
                    }, MenuULComponent.MouseOutTimeout);
                }
            }
            return closing;
        };
        MenuULComponent.prototype.closeSublevelsStartingAt = function (newOwningUL) {
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
        MenuULComponent.prototype.closeSublevelsForNewSublevel = function (newLevel) {
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
        MenuULComponent.prototype.handleMouseMove = function (cursorX, cursorY) {
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
        MenuULComponent.prototype.killTimeout = function () {
            if (this.CloseTimeout) {
                clearTimeout(this.CloseTimeout);
                this.CloseTimeout = 0;
            }
        };
        MenuULComponent.prototype.startTimeout = function () {
            var _this = this;
            if (!this.CloseTimeout) {
                this.CloseTimeout = setTimeout(function () {
                    for (var _i = 0, _a = _this.Levels; _i < _a.length; _i++) {
                        var levelInfo = _a[_i];
                        levelInfo.subUL.style.display = "none";
                    }
                    _this.clearPath();
                    _this.Levels = [];
                }, MenuULComponent.MouseOutTimeout);
            }
        };
        MenuULComponent.prototype.setPath = function () {
            for (var _i = 0, _a = this.Levels; _i < _a.length; _i++) {
                var levelInfo = _a[_i];
                $YetaWF.elementAddClass(levelInfo.owningUL, "t_path");
                $YetaWF.elementAddClass(levelInfo.owningLI, "t_path");
                $YetaWF.elementAddClass(levelInfo.owningAnchor, "t_path");
            }
        };
        MenuULComponent.prototype.clearPath = function () {
            for (var _i = 0, _a = this.Levels; _i < _a.length; _i++) {
                var levelInfo = _a[_i];
                $YetaWF.elementRemoveClass(levelInfo.owningUL, "t_path");
                $YetaWF.elementRemoveClass(levelInfo.owningLI, "t_path");
                $YetaWF.elementRemoveClass(levelInfo.owningAnchor, "t_path");
            }
        };
        // API
        MenuULComponent.prototype.close = function () {
            if (this.isOpen) {
                this.isOpen = false;
                if (this.Setup.AutoRemove) {
                    this.destroy();
                    $YetaWF.processClearDiv(this.Control);
                    this.Control.remove();
                }
                return true;
            }
            return false;
        };
        MenuULComponent.closeMenus = function () {
            var menus = YetaWF.ComponentBaseDataImpl.getControls(MenuULComponent.SELECTOR);
            var closed = false;
            for (var _i = 0, menus_1 = menus; _i < menus_1.length; _i++) {
                var menu = menus_1[_i];
                closed = menu.close() || closed;
            }
            return closed;
        };
        MenuULComponent.getOwnerFromTag = function (tag) {
            var menu = YetaWF.ComponentBaseDataImpl.getControlFromTagCond(tag, MenuULComponent.SELECTOR);
            if (menu)
                return menu.Setup.Owner;
            return null;
        };
        MenuULComponent.TEMPLATE = "yt_menuul";
        MenuULComponent.SELECTOR = ".yt_menuul";
        MenuULComponent.MouseOutTimeout = 300; // close menu when mouse leaves
        return MenuULComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.MenuULComponent = MenuULComponent;
    $YetaWF.registerEventHandlerBody("mousemove", null, function (ev) {
        var controls = YetaWF.ComponentBaseDataImpl.getControls(MenuULComponent.SELECTOR);
        for (var _i = 0, controls_1 = controls; _i < controls_1.length; _i++) {
            var control = controls_1[_i];
            control.handleMouseMove(ev.clientX, ev.clientY);
        }
        return true;
    });
    // Handle clicks elsewhere so we can close the menus
    $YetaWF.registerMultipleEventHandlersBody(["mousedown"], MenuULComponent.SELECTOR, function (ev) {
        // prevent event from reaching body
        return false;
    });
    $YetaWF.registerMultipleEventHandlersBody(["click", "mousedown"], null, function (ev) {
        // delay closing to handle the event
        setTimeout(function () {
            MenuULComponent.closeMenus();
        }, 300);
        return true;
    });
    // Handle Escape key to close any open menus
    $YetaWF.registerEventHandlerBody("keydown", null, function (ev) {
        if (ev.key !== "Escape")
            return true;
        MenuULComponent.closeMenus();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, function (ev) {
        MenuULComponent.closeMenus();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        MenuULComponent.closeMenus();
        return true;
    });
    // last chance - handle a new page (UPS) and close open menus
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        MenuULComponent.closeMenus();
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=MenuUL.js.map
