/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface ActionIconsSetup {
        MenuId: string;
    }

    export class ActionIconsComponent extends YetaWF.ComponentBase<HTMLButtonElement> {

        private MenuControl: HTMLDivElement;

        public static menusOpen: number = 0;

        constructor(controlId: string, setup: ActionIconsSetup) {
            super(controlId);

            this.MenuControl = $YetaWF.getElementById(setup.MenuId) as HTMLDivElement;

            var $btn = $(this.Control).kendoButton();
            $btn.on("click", (ev: Event): boolean => {
                var vis = $YetaWF.isVisible(this.MenuControl);
                ActionIconsComponent.closeMenus();
                if (!vis)
                    this.openMenu();
                ev.preventDefault();
                return false;
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
            $idMenu.position({
                my: "left top",
                at: "left bottom",
                of: $(this.Control),
                collision: "flip"
            });
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
        if (ev.which !== 27) return true;
        ActionIconsComponent.closeMenus();
        return true;
    });

    // last chance - handle a new page (UPS) and close open menus
    $YetaWF.registerNewPage((url:string): void => {
        ActionIconsComponent.closeMenus();
    });

    // A <div> is being emptied. Destroy all actionicons the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        //var list = tag.querySelectorAll("button.yt_actionicons");
        //var len = list.length;
        //for (var i = 0; i < len; ++i) {
        //    var el = list[i];
        //    var button = $(el).data("kendoButton");
        //    if (!button) throw "No kendo object found";/*DEBUG*/
        //    button.destroy();
        //}
        var list = $YetaWF.getElementsBySelector("ul.yGridActionMenu", [tag]);
        for (let el of list) {
            var menu = $(el).data("kendoMenu");
            if (!menu) throw "No kendo object found";/*DEBUG*/
            menu.destroy();
        }
    });
}

