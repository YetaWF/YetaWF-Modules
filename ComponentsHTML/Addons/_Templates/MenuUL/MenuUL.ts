/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF {
    export interface IConfigs {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageConfigs;
    }
}
namespace YetaWF_ComponentsHTML {
    export interface IPackageConfigs {
        SVG_fas_caret_right: string;
    }
}

namespace YetaWF_ComponentsHTML {

    interface MenuULSetup {
        Owner: HTMLElement;
        AutoOpen: boolean;
        AutoRemove: boolean;
        AttachTo: HTMLElement | null;
        Dynamic?: boolean;
        Click?: (liElem: HTMLLIElement, target?: HTMLElement|null) => void;
        CloseOnClick?: boolean;// defaults to true
        RightAlign?: boolean;// defaults to false
    }

    interface LevelInfo {
        owningUL: HTMLUListElement;
        owningLI: HTMLLIElement;
        owningAnchor: HTMLAnchorElement;
        subUL: HTMLUListElement;
        subLI: HTMLLIElement;
    }

    export class MenuULComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_menuul";
        public static readonly SELECTOR: string = ".yt_menuul";

        private static MouseOutTimeout: number = 300;// close menu when mouse leaves

        public Setup: MenuULSetup;
        private Levels: LevelInfo[] = [];
        private CloseTimeout: number = 0;

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

            // add icons to all items with submenu
            let aSubs = $YetaWF.getElementsBySelector("li.t_hassub > a", [this.Control]);
            for (let aSub of aSubs) {
                if (!$YetaWF.elementHasClass(aSub, "t_disabled")) {
                    aSub.innerHTML += YConfigs.YetaWF_ComponentsHTML.SVG_fas_caret_right;

                    aSub.setAttribute("aria-haspopup", "true");
                    aSub.setAttribute("aria-expanded", "false");
                }
            }

            let liSubs = $YetaWF.getElementsBySelector("li > a", [this.Control]);
            $YetaWF.registerMultipleEventHandlers(liSubs, ["mouseenter"], null, (ev: Event): boolean => {
                let owningAnchor = ev.__YetaWFElem as HTMLAnchorElement;
                if ($YetaWF.elementHasClass(owningAnchor, "t_disabled"))
                    return false;
                let owningLI = $YetaWF.elementClosest(owningAnchor, "li") as HTMLLIElement;
                let owningUL = $YetaWF.elementClosest(owningAnchor, "ul") as HTMLUListElement;

                let subUL = $YetaWF.getElement1BySelectorCond("ul", [owningLI]) as HTMLUListElement;
                if (!subUL) {
                    this.scheduleCloseSublevelsStartingAt(owningUL);
                    return true;
                }
                let subLI = $YetaWF.getElement1BySelector("li", [subUL]) as HTMLLIElement;

                let levelInfo = { owningUL: owningUL, owningLI: owningLI, owningAnchor: owningAnchor, subUL: subUL, subLI: subLI };
                if (this.closeSublevelsForNewSublevel(levelInfo))
                    this.openSublevel(levelInfo);
                return false;
            });

            if (this.Setup.AutoOpen)
                this.open();
        }

        public open(): void {
            if (!this.isOpen) {

                MenuULComponent.closeMenus();

                if (this.Setup.Click) {

                    let liSubs = $YetaWF.getElementsBySelector("li > a", [this.Control]);
                    $YetaWF.registerMultipleEventHandlers(liSubs, ["click"], null, (ev: Event): boolean => {

                        let target = ev.target as HTMLElement|null;

                        let owningAnchor = ev.__YetaWFElem as HTMLAnchorElement;
                        if ($YetaWF.elementHasClass(owningAnchor, "t_disabled"))
                            return false;
                        let owningLI = $YetaWF.elementClosest(owningAnchor, "li") as HTMLLIElement;

                        if (this.Setup.CloseOnClick !== false)
                            MenuULComponent.closeMenus();
                        this.Setup.Click!(owningLI, target);
                        return false;
                    });
                }
                this.Control.style.display = "block";

                this.positionMenu();

                this.isOpen = true;
            }
        }

        private openSublevel(levelInfo: LevelInfo): void {
            let level = this.Levels.length;
            levelInfo.subUL.style.display = "";// open new sublevel

            let subUL = levelInfo.subUL;
            let owningLI = levelInfo.owningLI;

            let owningRect = owningLI.getBoundingClientRect();

            // position the sublevel
            switch (level) {
                case 0: // really t_lvl1
                    subUL.style.left = `${owningRect.width - 3}px`;// slight overlap
                    subUL.style.top = "-3px";
                    break;
                case 1: // really t_lvl2
                    subUL.style.left = `${owningRect.width - 3}px`;// slight overlap
                    subUL.style.top = "-3px";
                    break;
                default:
                    throw "Too many menu levels";
            }

            this.clearPath();
            this.Levels.push(levelInfo);
            this.setPath();
        }

        private positionMenu(): void {
            if (this.Setup.AttachTo) {
                if (this.Setup.RightAlign === true)
                    $YetaWF.positionRightAlignedBelow(this.Setup.AttachTo, this.Control);
                else
                    $YetaWF.positionLeftAlignedBelow(this.Setup.AttachTo, this.Control);
            }
        }

        private CloseSublevelsTimout: number = 0;

        private scheduleCloseSublevelsStartingAt(newOwningUL: HTMLUListElement): boolean {
            let closing = false;// defines whether any sublevels are to be closed
            if (!this.CloseSublevelsTimout) {
                for (let levelInfo of this.Levels) {
                    if (!closing) {
                        if (levelInfo.owningUL === newOwningUL)
                            closing = true;
                    }
                }
                if (closing) {
                    this.CloseSublevelsTimout = setTimeout((): void => {
                        this.closeSublevelsStartingAt(newOwningUL);
                    }, MenuULComponent.MouseOutTimeout);
                }
            }
            return closing;
        }

        private closeSublevelsStartingAt(newOwningUL: HTMLUListElement): boolean {
            let newLevels: LevelInfo[] = [];
            let closing = false;

            clearTimeout(this.CloseSublevelsTimout);
            this.CloseSublevelsTimout = 0;

            for (let levelInfo of this.Levels) {
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
        }

        private closeSublevelsForNewSublevel(newLevel: LevelInfo): boolean {
            let newLevels: LevelInfo[] = [];
            let closing = false;

            clearTimeout(this.CloseSublevelsTimout);
            this.CloseSublevelsTimout = 0;

            for (let levelInfo of this.Levels) {
                if (!closing) {
                    if (levelInfo.owningUL === newLevel.owningUL) {
                        if (levelInfo.subUL === newLevel.subUL) // the new sublevel is already open
                            return false;
                        closing = true;
                    } else
                        newLevels.push(levelInfo);
                }
                if (closing)
                    levelInfo.subUL.style.display = "none";
            }
            this.clearPath();
            this.Levels = newLevels;
            this.setPath();
            return true; // we closed all necessary sublevels
        }

        public handleMouseMove(cursorX: number, cursorY: number): boolean {

            if (this.Levels.length > 0) {
                let rect = this.Levels[0].owningLI.getBoundingClientRect();
                if (rect.left <= cursorX && cursorX < rect.right && rect.top <= cursorY && cursorY < rect.bottom) {
                    this.killTimeout();
                    return true;
                }
                for (let levelInfo of this.Levels) {
                    rect = levelInfo.subUL.getBoundingClientRect();
                    if (rect.left <= cursorX && cursorX < rect.right && rect.top <= cursorY && cursorY < rect.bottom) {
                        this.killTimeout();
                        return true;
                    }
                }
                this.startTimeout();
            }
            return true;
        }
        private killTimeout():void {
            if (this.CloseTimeout) {
                clearTimeout(this.CloseTimeout);
                this.CloseTimeout = 0;
            }
        }
        private startTimeout():void {
            if (!this.CloseTimeout) {
                this.CloseTimeout = setTimeout((): void => {
                    for (let levelInfo of this.Levels) {
                        levelInfo.subUL.style.display = "none";
                    }
                    this.clearPath();
                    this.Levels = [];
                }, MenuULComponent.MouseOutTimeout);
            }
        }

        private setPath(): void {
            for (let levelInfo of this.Levels) {
                $YetaWF.elementAddClass(levelInfo.owningUL, "t_path");
                $YetaWF.elementAddClass(levelInfo.owningLI, "t_path");
                $YetaWF.elementAddClass(levelInfo.owningAnchor, "t_path");
            }
        }
        private clearPath(): void {
            for (let levelInfo of this.Levels) {
                $YetaWF.elementRemoveClass(levelInfo.owningUL, "t_path");
                $YetaWF.elementRemoveClass(levelInfo.owningLI, "t_path");
                $YetaWF.elementRemoveClass(levelInfo.owningAnchor, "t_path");
            }
        }

        // API

        public close(): boolean {

            if (this.isOpen) {

                this.isOpen = false;

                if (this.Setup.AutoRemove) {
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

    $YetaWF.registerEventHandlerBody("mousemove", null, (ev: MouseEvent): boolean => {
        let controls: MenuULComponent[] = YetaWF.ComponentBaseDataImpl.getControls(MenuULComponent.SELECTOR);
        for (let control of controls)
            control.handleMouseMove(ev.clientX, ev.clientY);
        return true;
    });
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
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        MenuULComponent.closeMenus();
        return true;
    });
}

