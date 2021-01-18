/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface MenuSetup {
        SmallMenuMaxWidth: number;
        HoverDelay: number;
    }

    interface LevelInfo {
        owningUL: HTMLUListElement;
        owningLI: HTMLLIElement;
        owningAnchor: HTMLAnchorElement;
        subUL: HTMLUListElement;
        subLI: HTMLLIElement;
    }

    export class MenuComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_menu";
        public static readonly SELECTOR: string = ".yt_menu.t_display";

        private Setup: MenuSetup;
        private Levels: LevelInfo[] = [];
        private CloseTimeout: number = 0;

        constructor(controlId: string, setup: MenuSetup) {
            super(controlId, MenuComponent.TEMPLATE, MenuComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            this.Setup = setup;

            this.updateSize();

            // update aria info
            let aSubs = $YetaWF.getElementsBySelector("li.t_hassub > a", [this.Control]);
            for (let aSub of aSubs) {
                aSub.setAttribute("aria-haspopup", "true");
                aSub.setAttribute("aria-expanded", "false");
            }

            let liSubs = $YetaWF.getElementsBySelector("li > a", [this.Control]);
            $YetaWF.registerMultipleEventHandlers(liSubs, ["mouseenter"], null, (ev: Event): boolean => {

                if (this.isSmall) return true;

                let owningAnchor = ev.__YetaWFElem as HTMLAnchorElement;
                let owningLI = $YetaWF.elementClosest(owningAnchor, "li") as HTMLLIElement;
                if ($YetaWF.elementHasClass(owningLI, "t_megamenu_content")) return true;//we're within a megamenu (can't have menus within megamenu)
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
            $YetaWF.registerMultipleEventHandlers(liSubs, ["click"], null, (ev: Event): boolean => {

                if (!this.isSmall) return true;

                let owningAnchor = ev.__YetaWFElem as HTMLAnchorElement;
                let owningLI = $YetaWF.elementClosest(owningAnchor, "li") as HTMLLIElement;
                if ($YetaWF.elementHasClass(owningLI, "t_megamenu_content")) return true;//we're within a megamenu (can't have menus within megamenu)
                let owningUL = $YetaWF.elementClosest(owningAnchor, "ul") as HTMLUListElement;

                let subUL = $YetaWF.getElement1BySelectorCond("ul", [owningLI]) as HTMLUListElement;
                if (!subUL) {
                    this.closeSublevelsStartingAt(owningUL);
                    return true;
                }
                let subLI = $YetaWF.getElement1BySelector("li", [subUL]) as HTMLLIElement;

                let levelInfo = { owningUL: owningUL, owningLI: owningLI, owningAnchor: owningAnchor, subUL: subUL, subLI: subLI };
                if (this.closeSublevelsForNewSublevel(levelInfo))
                    this.openSublevel(levelInfo);
                else
                    return true; // allow anchor processing
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "click", "li > a > svg", (ev: Event): boolean => {

                if (!this.isSmall) return true;

                let svg = ev.__YetaWFElem;
                let owningAnchor = svg.parentElement! as HTMLAnchorElement;
                let owningLI = $YetaWF.elementClosest(owningAnchor, "li") as HTMLLIElement;
                let owningUL = $YetaWF.elementClosest(owningAnchor, "ul") as HTMLUListElement;

                let subUL = $YetaWF.getElement1BySelectorCond("ul", [owningLI]) as HTMLUListElement;
                if (!subUL) {
                    this.closeSublevelsStartingAt(owningUL);
                    return true;
                }
                let subLI = $YetaWF.getElement1BySelector("li", [subUL]) as HTMLLIElement;

                let levelInfo = { owningUL: owningUL, owningLI: owningLI, owningAnchor: owningAnchor, subUL: subUL, subLI: subLI };
                if (this.closeSublevelsForNewSublevel(levelInfo))
                    this.openSublevel(levelInfo);
                else
                    this.closeSublevelsStartingAt(owningUL);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers(liSubs, ["keydown"], null, (ev: Event): boolean => {

                let key = (ev as KeyboardEvent).key;
                if (key === "Enter") {
                    let owningAnchor = ev.__YetaWFElem as HTMLAnchorElement;
                    let owningLI = $YetaWF.elementClosestCond(owningAnchor, "li") as HTMLLIElement;
                    if (!owningLI) return true;
                    let owningUL = $YetaWF.elementClosestCond(owningAnchor, "ul") as HTMLUListElement;
                    if (!owningUL) return true;

                    let subUL = $YetaWF.getElement1BySelectorCond("ul", [owningLI]) as HTMLUListElement;
                    if (!subUL) return true; // no submenus
                    let subLI = $YetaWF.getElement1BySelector("li", [subUL]) as HTMLLIElement;

                    // we're in a menu and someone hit Enter on an anchor
                    let levelInfo = { owningUL: owningUL, owningLI: owningLI, owningAnchor: owningAnchor, subUL: subUL, subLI: subLI };
                    if (this.closeSublevelsForNewSublevel(levelInfo))
                        this.openSublevel(levelInfo);
                    else
                        this.closeSublevelsStartingAt(owningUL);
                    return false;
                }
                return true;
            });
        }

        private openSublevel(levelInfo: LevelInfo): void {
            let level = this.Levels.length;
            levelInfo.subUL.style.display = "";// open new sublevel

            let subUL = levelInfo.subUL;
            let owningLI = levelInfo.owningLI;

            let owningRect = owningLI.getBoundingClientRect();

            if (this.isSmall) {


            } else {
                // position the sublevel
                switch (level) {
                    case 0:
                        subUL.style.left = "0";
                        subUL.style.top = `${owningRect.height}px`;
                        break;
                    case 1:
                        subUL.style.left = `${owningRect.width - 3}px`;// slight overlap
                        subUL.style.top = "-3px";
                        break;
                    case 2:
                        subUL.style.left = `${owningRect.width - 3}px`;// slight overlap
                        subUL.style.top = "-3px";
                        break;
                    default:
                        throw "Too many menu levels";
                }
            }
            this.clearPath();
            this.Levels.push(levelInfo);
            this.setPath();
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
                    }, this.Setup.HoverDelay || 1);
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

            if (this.isSmall) return true;

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
                }, this.Setup.HoverDelay || 1);
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

        public updateSize(): void {
            if (this.isSmall) {
                if (!$YetaWF.elementHasClass(this.Control, "t_small")) {
                    $YetaWF.elementRemoveClasses(this.Control, ["t_large", "t_small"]);
                    $YetaWF.elementAddClass(this.Control, "t_small");
                    this.hide();
                    this.closeAll();
                }
            } else {
                if (!$YetaWF.elementHasClass(this.Control, "t_large")) {
                    $YetaWF.elementRemoveClasses(this.Control, ["t_large", "t_small"]);
                    $YetaWF.elementAddClass(this.Control, "t_large");
                    this.show();
                    this.closeAll();
                }
            }
        }

        // API

        public closeAll(): void {
            for (let levelInfo of this.Levels) {
                levelInfo.subUL.style.display = "none";
            }
            this.clearPath();
            this.Levels = [];

            if (this.isSmall)
                this.hide();
        }

        public static closeAllMenus(): void {
            let controls: MenuComponent[] = YetaWF.ComponentBaseDataImpl.getControls(MenuComponent.SELECTOR);
            for (let control of controls) {
                control.closeAll();
            }
        }

        public get isSmall(): boolean {
            let small = false;
            if (this.Setup.SmallMenuMaxWidth !== 0) {
                if (window.outerWidth <= this.Setup.SmallMenuMaxWidth)
                    small = true;
            } else {
                small = false;
            }
            return small;
        }

        public get isShown(): boolean {
            return this.Control.style.display !== "none";
        }

        public show(): void {
            this.Control.style.display = "";
        }
        public hide(): void {
            this.Control.style.display = "none";
        }

    }
    $YetaWF.registerEventHandlerBody("mousemove", null, (ev: MouseEvent): boolean => {
        let controls: MenuComponent[] = YetaWF.ComponentBaseDataImpl.getControls(MenuComponent.SELECTOR);
        for (let control of controls) {
            control.handleMouseMove(ev.clientX, ev.clientY);
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        let menus = YetaWF.ComponentBaseDataImpl.getControls<MenuComponent>(MenuComponent.SELECTOR, [ev.detail.container]);
        for (let menu of menus) {
            menu.updateSize();
        }
        return true;
    });
    // handle new content
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        let menus = YetaWF.ComponentBaseDataImpl.getControls<MenuComponent>(MenuComponent.SELECTOR, ev.detail.containers);
        for (let menu of menus) {
            menu.closeAll();
        }
        return true;
    });

    // Handle Escape key to close any open menus
    $YetaWF.registerEventHandlerBody("keydown", null, (ev: KeyboardEvent): boolean => {
        if (ev.key !== "Escape") return true;
        let menus = YetaWF.ComponentBaseDataImpl.getControls<MenuComponent>(MenuComponent.SELECTOR);
        for (let menu of menus) {
            menu.closeAll();
        }
        return true;
    });
}
