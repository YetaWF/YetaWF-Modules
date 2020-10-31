/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface ActionIconsSetup {
        MenuId: string;
    }

    export class ActionIconsComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_actionicons";
        public static readonly SELECTOR: string = ".yt_actionicons";

        private MenuControl: HTMLDivElement;
        private ButtonControl: DropDownButtonComponent;

        public static menusOpen: number = 0;

        constructor(controlId: string, setup: ActionIconsSetup) {
            super(controlId, ActionIconsComponent.TEMPLATE, ActionIconsComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }, false, (tag: HTMLElement, control: ActionIconsComponent): void => {
                ActionIconsComponent.closeMenusGiven([control.MenuControl]);
                let menu = $(control.MenuControl).data("kendoMenu");
                menu.destroy();
                //var list = $YetaWF.getElementsBySelector("ul.yGridActionMenu", [control.Control]);
                //for (let el of list) {
                //    var menu = $(el).data("kendoMenu");
                //    if (!menu) throw "No kendo object found";/*DEBUG*/
                //    menu.destroy();
                //}
            });

            this.MenuControl = $YetaWF.getElementById(setup.MenuId) as HTMLDivElement;
            this.ButtonControl = DropDownButtonComponent.getControlFromSelector("button", DropDownButtonComponent.SELECTOR, [this.Control]);

            $YetaWF.registerCustomEventHandler(this.ButtonControl, DropDownButtonComponent.CLICKEDEVENT, (ev: CustomEvent):void => {
                let vis = $YetaWF.isVisible(this.MenuControl);
                ActionIconsComponent.closeMenus();
                if (!vis)
                    this.openMenu();
            });

            $(this.MenuControl).kendoMenu({
                orientation: "vertical"
            }).hide();
        }

        private openMenu(): void {
            ActionIconsComponent.closeMenus();
            var $idMenu = $(this.MenuControl);
            $idMenu.appendTo($("body"));
            $idMenu.show();
            ++ActionIconsComponent.menusOpen;
            this.positionMenu();
        }
        private positionMenu(): void {
            $YetaWF.positionLeftAlignedBelow(this.ButtonControl.Control, this.MenuControl);
        }

        public static closeMenus(): void {
            // find all action menus after grid (there really should only be one)
            var menus = $YetaWF.getElementsBySelector(".yGridActionMenu");
            ActionIconsComponent.closeMenusGiven(menus);
        }
        public static closeMenusGiven(menus: HTMLElement[]): void {
            ActionIconsComponent.menusOpen = 0;
            for (let menu of menus) {
                $(menu).hide();
                var idButton = $YetaWF.getAttribute(menu, "id").replace("_menu", "_btn");
                var button = $YetaWF.getElementByIdCond(idButton);
                if (button) { // there can be a case without button if we switched to a new page
                    $(menu).appendTo(button.parentElement as HTMLElement);
                }
            }
        }
    }

    // Handle clicks elsewhere so we can close the menus
    $YetaWF.registerMultipleEventHandlersBody(["click", "mousedown"], null, (ev: Event): boolean => {
        var e = ev as MouseEvent;
        if (e.which !== 1) return true;
        if (ActionIconsComponent.menusOpen > 0) {
            var menus = $YetaWF.getElementsBySelector(".yGridActionMenu");// get all action menus
            menus = $YetaWF.limitToVisibleOnly(menus);
            // delay closing to handle the event
            setTimeout(():void => {
                ActionIconsComponent.closeMenusGiven(menus);
            }, 300);
        }
        return true;
    });
    // Handle Escape key to close any open menus
    $YetaWF.registerEventHandlerBody("keydown", null, (ev: KeyboardEvent): boolean => {
        if (ev.key !== "Escape") return true;
        ActionIconsComponent.closeMenus();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: CustomEvent<YetaWF.DetailsEventContainerScroll>): boolean => {
        ActionIconsComponent.closeMenus();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        ActionIconsComponent.closeMenus();
        return true;
    });

    // last chance - handle a new page (UPS) and close open menus
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: Event): boolean => {
        ActionIconsComponent.closeMenus();
        return true;
    });
}

