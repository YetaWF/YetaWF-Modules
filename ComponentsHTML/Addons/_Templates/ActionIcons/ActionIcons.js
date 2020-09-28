"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ActionIconsComponent = /** @class */ (function (_super) {
        __extends(ActionIconsComponent, _super);
        function ActionIconsComponent(controlId, setup) {
            var _this = _super.call(this, controlId, ActionIconsComponent.TEMPLATE, ActionIconsComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }, false, function (tag, control) {
                ActionIconsComponent.closeMenusGiven([control.MenuControl]);
                var menu = $(control.MenuControl).data("kendoMenu");
                menu.destroy();
                var btn = $(control.Control).data("kendoButton");
                btn.destroy();
                //var list = $YetaWF.getElementsBySelector("ul.yGridActionMenu", [control.Control]);
                //for (let el of list) {
                //    var menu = $(el).data("kendoMenu");
                //    if (!menu) throw "No kendo object found";/*DEBUG*/
                //    menu.destroy();
                //}
            }) || this;
            _this.MenuControl = $YetaWF.getElementById(setup.MenuId);
            var $btn = $(_this.Control).kendoButton(); // kendo use
            $btn.on("click", function (ev) {
                var vis = $YetaWF.isVisible(_this.MenuControl);
                ActionIconsComponent.closeMenus();
                if (!vis)
                    _this.openMenu();
                ev.preventDefault();
                return false;
            });
            $(_this.MenuControl).kendoMenu({
                orientation: "vertical"
            }).hide();
            return _this;
        }
        ActionIconsComponent.prototype.openMenu = function () {
            ActionIconsComponent.closeMenus();
            var $idMenu = $(this.MenuControl);
            $idMenu.appendTo($("body"));
            $idMenu.show();
            ++ActionIconsComponent.menusOpen;
            this.positionMenu();
        };
        ActionIconsComponent.prototype.positionMenu = function () {
            $YetaWF.positionLeftAlignedBelow(this.Control, this.MenuControl);
        };
        ActionIconsComponent.closeMenus = function () {
            // find all action menus after grid (there really should only be one)
            var menus = $YetaWF.getElementsBySelector(".yGridActionMenu");
            ActionIconsComponent.closeMenusGiven(menus);
        };
        ActionIconsComponent.closeMenusGiven = function (menus) {
            ActionIconsComponent.menusOpen = 0;
            for (var _i = 0, menus_1 = menus; _i < menus_1.length; _i++) {
                var menu = menus_1[_i];
                $(menu).hide();
                var idButton = $YetaWF.getAttribute(menu, "id").replace("_menu", "_btn");
                var button = $YetaWF.getElementByIdCond(idButton);
                if (button) { // there can be a case without button if we switched to a new page
                    $(menu).appendTo(button.parentElement);
                }
            }
        };
        ActionIconsComponent.TEMPLATE = "yt_actionicons";
        ActionIconsComponent.SELECTOR = ".yt_actionicons";
        ActionIconsComponent.menusOpen = 0;
        return ActionIconsComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.ActionIconsComponent = ActionIconsComponent;
    // Handle clicks elsewhere so we can close the menus
    $YetaWF.registerMultipleEventHandlersBody(["click", "mousedown"], null, function (ev) {
        var e = ev;
        if (e.which !== 1)
            return true;
        if (ActionIconsComponent.menusOpen > 0) {
            var menus = $YetaWF.getElementsBySelector(".yGridActionMenu"); // get all action menus
            menus = $YetaWF.limitToVisibleOnly(menus);
            // delay closing to handle the event
            setTimeout(function () {
                ActionIconsComponent.closeMenusGiven(menus);
            }, 300);
        }
        return true;
    });
    // Handle Escape key to close any open menus
    $YetaWF.registerEventHandlerBody("keydown", null, function (ev) {
        if (ev.key !== "Escape")
            return true;
        ActionIconsComponent.closeMenus();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, function (ev) {
        ActionIconsComponent.closeMenus();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        ActionIconsComponent.closeMenus();
        return true;
    });
    // last chance - handle a new page (UPS) and close open menus
    $YetaWF.registerNewPage(false, function (url) {
        ActionIconsComponent.closeMenus();
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=ActionIcons.js.map
