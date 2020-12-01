"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
            _this.isOpen = false;
            _this.Setup = setup;
            if (!$YetaWF.elementHasClass(_this.Control, "yt_menuul"))
                $YetaWF.elementAddClass(_this.Control, "yt_menuul");
            if (_this.Setup.Dynamic)
                $YetaWF.elementAddClass(_this.Control, "t_dynamic");
            if (_this.Setup.AutoOpen)
                _this.open();
            return _this;
        }
        MenuULComponent.prototype.open = function () {
            if (!this.isOpen) {
                MenuULComponent.closeMenus();
                var $menu = $(this.Control);
                $menu.kendoMenu({
                    orientation: "vertical"
                });
                var menu = $menu.data("kendoMenu");
                if (this.Setup.Click) {
                    var me_1 = this;
                    menu.setOptions({
                        // eslint-disable-next-line prefer-arrow/prefer-arrow-functions
                        select: function (ev) {
                            MenuULComponent.closeMenus();
                            me_1.Setup.Click(ev.item);
                        }
                    });
                }
                this.Control.style.display = "block";
                this.positionMenu();
                this.isOpen = true;
            }
        };
        MenuULComponent.prototype.positionMenu = function () {
            if (this.Setup.AttachTo)
                $YetaWF.positionLeftAlignedBelow(this.Setup.AttachTo, this.Control);
        };
        MenuULComponent.prototype.close = function () {
            if (this.isOpen) {
                var $menu = $(this.Control);
                $menu.hide();
                var menu = $menu.data("kendoMenu");
                menu.destroy();
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
        return MenuULComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.MenuULComponent = MenuULComponent;
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
