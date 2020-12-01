/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/// <reference types="kendo-ui" />

namespace YetaWF_ComponentsHTML {

    interface MenuULSetup {
        Owner: HTMLElement;
        AutoOpen: boolean;
        AutoRemove: boolean;
        AttachTo: HTMLElement | null;
        Dynamic?: boolean;
        Click?: (liElem: HTMLLIElement) => void;
    }

    export class MenuULComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_menuul";
        public static readonly SELECTOR: string = ".yt_menuul";

        public Setup: MenuULSetup;

        private isOpen: boolean = false;

        constructor(controlId: string, setup: MenuULSetup) {
            super(controlId, MenuULComponent.TEMPLATE, MenuULComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }, false, (tag: HTMLElement, control: MenuULComponent): void => {
                control.close();
            });

            this.Setup = setup;

            if (!$YetaWF.elementHasClass(this.Control, "yt_menuul"))
                $YetaWF.elementAddClass(this.Control, "yt_menuul");
            if (this.Setup.Dynamic)
                $YetaWF.elementAddClass(this.Control, "t_dynamic");

            if (this.Setup.AutoOpen)
                this.open();
        }

        public open(): void {
            if (!this.isOpen) {

                MenuULComponent.closeMenus();

                let $menu = $(this.Control);
                $menu.kendoMenu({
                    orientation: "vertical"
                });

                let menu = $menu.data("kendoMenu");
                if (this.Setup.Click) {
                    let me = this;
                    menu.setOptions({
                        // eslint-disable-next-line prefer-arrow/prefer-arrow-functions
                        select: function(ev: any): void {
                            MenuULComponent.closeMenus();
                            me.Setup.Click!(ev.item);
                        }
                    });
                }
                this.Control.style.display = "block";

                this.positionMenu();

                this.isOpen = true;
            }
        }

        private positionMenu(): void {
            if (this.Setup.AttachTo)
                $YetaWF.positionLeftAlignedBelow(this.Setup.AttachTo, this.Control);
        }

        public close(): boolean {

            if (this.isOpen) {

                let $menu = $(this.Control);
                $menu.hide();
                let menu = $menu.data("kendoMenu");
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
        }

        public static closeMenus(): boolean {
            let menus = YetaWF.ComponentBaseDataImpl.getControls<MenuULComponent>(MenuULComponent.SELECTOR);
            let closed : boolean = false;
            for (let menu of menus)
                closed = menu.close() || closed;
            return closed;
        }

        public static getOwnerFromTag(tag: HTMLElement): HTMLElement | null {
            let menu = YetaWF.ComponentBaseDataImpl.getControlFromTagCond<MenuULComponent>(tag, MenuULComponent.SELECTOR);
            if (menu)
                return menu.Setup.Owner;
            return null;
        }
    }

    // Handle clicks elsewhere so we can close the menus
    $YetaWF.registerMultipleEventHandlersBody(["mousedown"], MenuULComponent.SELECTOR, (ev: Event): boolean => {
        // prevent event from reaching body
        return false;
    });
    $YetaWF.registerMultipleEventHandlersBody(["click", "mousedown"], null, (ev: Event): boolean => {
        // delay closing to handle the event
        setTimeout(():void => {
            MenuULComponent.closeMenus();
        }, 300);
        return true;
    });

    // Handle Escape key to close any open menus
    $YetaWF.registerEventHandlerBody("keydown", null, (ev: KeyboardEvent): boolean => {
        if (ev.key !== "Escape") return true;
        MenuULComponent.closeMenus();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: CustomEvent<YetaWF.DetailsEventContainerScroll>): boolean => {
        MenuULComponent.closeMenus();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        MenuULComponent.closeMenus();
        return true;
    });

    // last chance - handle a new page (UPS) and close open menus
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: Event): boolean => {
        MenuULComponent.closeMenus();
        return true;
    });
}

