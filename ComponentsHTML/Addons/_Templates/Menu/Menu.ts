/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/// <reference types="kendo-ui" />

namespace YetaWF_ComponentsHTML {

    interface MenuSetup {
        MenuId: string;
        Style: StyleEnum;
        Direction: DirectionEnum;
        Orientation: OrientationEnum;
        PopupCollision?: string;
        HoverDelay?: number;
    }
    enum StyleEnum {
        Bootstrap = 1,
        Kendo = 2,
    }
    enum DirectionEnum {
        Bottom = 0,
        Top = 1,
        Left = 2,
        Right = 3,
    }
    enum OrientationEnum {
        Horizontal = 0,
        Vertical = 1,
    }

    export class MenuComponent extends YetaWF.ComponentBaseNoDataImpl {

        public static readonly TEMPLATE: string = "yt_menu";
        public static readonly SELECTOR: string = "yt_menu.t_display";

        private Setup: MenuSetup;
        private MenuDiv: HTMLDivElement;

        constructor(controlId: string, setup: MenuSetup) {
            super(controlId, MenuComponent.TEMPLATE, MenuComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });

            this.Setup = setup;
            this.MenuDiv = $YetaWF.getElementById(this.Setup.MenuId) as HTMLDivElement;

            if (this.Setup.Style === StyleEnum.Kendo) {
                $(this.MenuDiv).kendoMenu({
                    direction: this.GetDirectionKendo(),
                    orientation: this.GetOrientationKendo(),
                    popupCollision: this.Setup.PopupCollision ?? "fit flip",
                    hoverDelay: this.Setup.HoverDelay ?? 0,
                });
            }
        }

        private GetDirectionKendo(): string {
            switch (this.Setup.Direction) {
                default: return "default";
                case DirectionEnum.Top: return "top";
                case DirectionEnum.Bottom: return "bottom";
                case DirectionEnum.Left: return "left";
                case DirectionEnum.Right: return "right";
            }
        }
        private GetOrientationKendo(): string {
            switch (this.Setup.Orientation) {
                default:
                case OrientationEnum.Horizontal: return "horizontal";
                case OrientationEnum.Vertical: return "vertical";
            }
        }
    }
}
