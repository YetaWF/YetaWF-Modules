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
    var StyleEnum;
    (function (StyleEnum) {
        StyleEnum[StyleEnum["Bootstrap"] = 1] = "Bootstrap";
        StyleEnum[StyleEnum["Kendo"] = 2] = "Kendo";
    })(StyleEnum || (StyleEnum = {}));
    var DirectionEnum;
    (function (DirectionEnum) {
        DirectionEnum[DirectionEnum["Bottom"] = 0] = "Bottom";
        DirectionEnum[DirectionEnum["Top"] = 1] = "Top";
        DirectionEnum[DirectionEnum["Left"] = 2] = "Left";
        DirectionEnum[DirectionEnum["Right"] = 3] = "Right";
    })(DirectionEnum || (DirectionEnum = {}));
    var OrientationEnum;
    (function (OrientationEnum) {
        OrientationEnum[OrientationEnum["Horizontal"] = 0] = "Horizontal";
        OrientationEnum[OrientationEnum["Vertical"] = 1] = "Vertical";
    })(OrientationEnum || (OrientationEnum = {}));
    var MenuComponent = /** @class */ (function (_super) {
        __extends(MenuComponent, _super);
        function MenuComponent(controlId, setup) {
            var _a, _b;
            var _this = _super.call(this, controlId, MenuComponent.TEMPLATE, MenuComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.Setup = setup;
            _this.MenuDiv = $YetaWF.getElementById(_this.Setup.MenuId);
            if (_this.Setup.Style === StyleEnum.Kendo) {
                $(_this.MenuDiv).kendoMenu({
                    direction: _this.GetDirectionKendo(),
                    orientation: _this.GetOrientationKendo(),
                    popupCollision: (_a = _this.Setup.PopupCollision) !== null && _a !== void 0 ? _a : "fit flip",
                    hoverDelay: (_b = _this.Setup.HoverDelay) !== null && _b !== void 0 ? _b : 0,
                });
            }
            return _this;
        }
        MenuComponent.prototype.GetDirectionKendo = function () {
            switch (this.Setup.Direction) {
                default: return "default";
                case DirectionEnum.Top: return "top";
                case DirectionEnum.Bottom: return "bottom";
                case DirectionEnum.Left: return "left";
                case DirectionEnum.Right: return "right";
            }
        };
        MenuComponent.prototype.GetOrientationKendo = function () {
            switch (this.Setup.Orientation) {
                default:
                case OrientationEnum.Horizontal: return "horizontal";
                case OrientationEnum.Vertical: return "vertical";
            }
        };
        MenuComponent.TEMPLATE = "yt_menu";
        MenuComponent.SELECTOR = "yt_menu.t_display";
        return MenuComponent;
    }(YetaWF.ComponentBaseNoDataImpl));
    YetaWF_ComponentsHTML.MenuComponent = MenuComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Menu.js.map
