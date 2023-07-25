/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface MenuSetup {
        Orientation: OrientationEnum;
        VerticalWidth: number;
        SmallMenuMaxWidth: number;
        HoverDelay: number;
    }

    enum OrientationEnum {
        Horizontal = 0,
        Vertical = 1,
    }

    interface MenuRect {
        Anchor: HTMLAnchorElement;
        Rect: DOMRect;
    }

    export class MenuComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_menu";
        public static readonly SELECTOR: string = ".yt_menu.t_display";

        private Setup: MenuSetup;
        private CloseTimeout: number = 0;
        private MenuRects: MenuRect[] = [];

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
            let aSubs = $YetaWF.getElementsBySelector("li.t_menu.t_hassub > a", [this.Control]);
            for (let aSub of aSubs) {
                aSub.setAttribute("aria-haspopup", "true");
                aSub.setAttribute("aria-expanded", "false");
            }

            let liSubs = $YetaWF.getElementsBySelector("li.t_menu > a", [this.Control]);

            $YetaWF.registerMultipleEventHandlers(liSubs, ["mouseenter"], null, (ev: Event): boolean => {
                if (this.isVertical || this.isSmall) return true;

                let owningAnchor = ev.__YetaWFElem as HTMLAnchorElement;
                let owningLI = $YetaWF.elementClosest(owningAnchor, "li") as HTMLLIElement;
                if ($YetaWF.elementHasClass(owningLI, "t_megamenu_content")) return true;//we're within a megamenu (can't have menus within megamenu)

                let subUL = $YetaWF.getElement1BySelectorCond("ul.t_menu", [owningLI]) as HTMLUListElement;
                if (!subUL) {
                    this.scheduleCloseSublevelsStartingAt(owningAnchor);
                    return true;
                }
                this.closeSublevelsStartingAt(owningAnchor);
                this.openSublevel(owningAnchor);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers(liSubs, ["click"], null, (ev: Event): boolean => {

                if (this.isSmall) return true;// allow anchor processing

                let owningAnchor = ev.__YetaWFElem as HTMLAnchorElement;
                let owningLI = $YetaWF.elementClosest(owningAnchor, "li") as HTMLLIElement;
                if ($YetaWF.elementHasClass(owningLI, "t_megamenu_content")) return true;//we're within a megamenu (can't have menus within megamenu)

                let subUL = $YetaWF.getElement1BySelectorCond("ul", [owningLI]) as HTMLUListElement;
                const isOpen = subUL && subUL.style.display === "";
                this.closeSublevelsStartingAt(owningAnchor);
                if (isOpen)
                    this.closeLevel(owningAnchor);
                else
                    this.openSublevel(owningAnchor);
                return true; // allow anchor processing
            });
            $YetaWF.registerEventHandler(this.Control, "click", "li.t_menu > a > svg", (ev: Event): boolean => {

                if (!this.isSmall) return true;

                let svg = ev.__YetaWFElem;
                let owningAnchor = svg.parentElement! as HTMLAnchorElement;

                this.closeSublevelsStartingAt(owningAnchor, owningAnchor);
                this.openSublevel(owningAnchor);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers(liSubs, ["keydown"], null, (ev: Event): boolean => {

                let key = (ev as KeyboardEvent).key;
                if (key === "Enter") {
                    let owningAnchor = ev.__YetaWFElem as HTMLAnchorElement;
                    // we're in a menu and someone hit Enter on an anchor
                    this.closeSublevelsStartingAt(owningAnchor, owningAnchor);
                    this.openSublevel(owningAnchor);
                }
                return true;
            });
        }

        private openSublevel(owningAnchor: HTMLAnchorElement): void {
            const level = this.getLevel(owningAnchor);
            this.openLevel(owningAnchor);

            if (this.isVertical || this.isSmall) {

            } else {
                // position the sublevel
                const subUL = owningAnchor.nextElementSibling as HTMLUListElement;
                if (!subUL) return;
                const owningLI = $YetaWF.elementClosest(owningAnchor, "li");
                const owningRect = owningLI.getBoundingClientRect();
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
        }

        private CloseSublevelsTimout: number = 0;

        private scheduleCloseSublevelsStartingAt(owningAnchor: HTMLAnchorElement): void {
            this.clearScheduleCloseSublevel();
            this.CloseSublevelsTimout = setTimeout((): void => {
                this.closeSublevelsStartingAt(owningAnchor);
            }, this.Setup.HoverDelay || 1);
        }
        private clearScheduleCloseSublevel(): void {
            clearTimeout(this.CloseSublevelsTimout);
            this.CloseSublevelsTimout = 0;
        }

        private closeSublevelsStartingAt(anchor: HTMLAnchorElement, exceptAnchor?: HTMLAnchorElement): void {
            this.clearScheduleCloseSublevel();

            const owningUL = $YetaWF.elementClosest(anchor, "ul");
            const owningAnchor = owningUL.previousElementSibling as HTMLAnchorElement|null;
            if (owningAnchor && owningAnchor.tagName === "A") {
                // sublevel
                this.internalCloseSublevelsStartingAt(owningAnchor, exceptAnchor);
            } else {
                // main level
                const anchors = $YetaWF.getElementsBySelector(`ul.${MenuComponent.TEMPLATE} > li > a`, [this.Control]) as HTMLAnchorElement[];// all top level anchors
                for (const a of anchors) {
                    this.internalCloseSublevelsStartingAt(a, exceptAnchor);
                    this.closeLevel(a);
                }
            }
        }

        private internalCloseSublevelsStartingAt(owningAnchor: HTMLAnchorElement, exceptAnchor?: HTMLAnchorElement): void {
            const subUL = owningAnchor.nextElementSibling as HTMLUListElement|null;
            if (!subUL) return;
            const subLIs = $YetaWF.getChildElementsByTag("li", subUL);
            for (const subLI of subLIs) {
                const anchor = $YetaWF.getChildElement1ByTagCond("a", subLI) as HTMLAnchorElement|null;
                if (anchor && anchor != exceptAnchor) {
                    const subUL = owningAnchor.nextElementSibling as HTMLUListElement|null;
                    if (subUL)
                        this.internalCloseSublevelsStartingAt(anchor, exceptAnchor);
                    this.closeLevel(anchor);
                }
            }
        }

        private getLevel(owningAnchor: HTMLAnchorElement): number {
            let level = 0;
            let elem: HTMLElement|null = owningAnchor;
            while (elem) {
                elem = $YetaWF.elementClosestCond(elem.parentElement as HTMLElement, "ul");
                if (!elem || $YetaWF.elementHasClass(elem, MenuComponent.TEMPLATE))
                    return level;
                ++level;
            }
            return level;
        }

        private openLevel(owningAnchor: HTMLAnchorElement): void {
            owningAnchor.setAttribute("aria-expanded", "true");
            const subUL = owningAnchor.nextElementSibling as HTMLUListElement;
            if (!subUL) return;
            if (this.isVertical || this.isSmall) {
                $YetaWF.animateHeight(subUL, true, (): void => {
                    subUL.style.height = "auto";// height to auto, so submenus can expand
                    this.MenuRects.push({ Anchor: owningAnchor, Rect: subUL.getBoundingClientRect(), });
                });
            } else {
                subUL.style.display = "";// show
                this.MenuRects.push({ Anchor: owningAnchor, Rect: subUL.getBoundingClientRect(), });
            }
        }

        private closeLevel(owningAnchor: HTMLAnchorElement): void {
            const subUL = owningAnchor.nextElementSibling as HTMLUListElement|null;
            if (subUL) {
                owningAnchor.setAttribute("aria-expanded", "false");
                if (this.isVertical || this.isSmall) {
                    $YetaWF.animateHeight(subUL, false, (): void => {
                        subUL.style.display = "none";// hide
                    });
                } else {
                    subUL.style.display = "none";// hide
                }
                this.MenuRects = this.MenuRects.filter((v: MenuRect, index: number): boolean => {
                    return v.Anchor !== owningAnchor;
                });
            }
        }

        public handleMouseMove(cursorX: number, cursorY: number): boolean {

            if (this.isVertical || this.isSmall) return true;

            const mainRect = this.Control.getBoundingClientRect();
            if (mainRect.left <= cursorX && cursorX < mainRect.right && mainRect.top <= cursorY && cursorY < mainRect.bottom) {
                this.killTimeout();
                return true;
            }
            for (const menuRect of this.MenuRects) {
                const rect = menuRect.Rect;
                if (rect.left <= cursorX && cursorX < rect.right && rect.top <= cursorY && cursorY < rect.bottom) {
                    this.killTimeout();
                    return true;
                }
            }
            this.startTimeout();
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
                    this.closeAll();
                }, this.Setup.HoverDelay || 1);
            }
        }

        private clearPath(): void {
            const anchors = $YetaWF.getElementsBySelector("ul.t_menu > li > a", [this.Control]);
            for (const a of anchors) {
                $YetaWF.elementRemoveClass(a, "t_path");
                $YetaWF.elementRemoveClass($YetaWF.elementClosest(a, "li"), "t_path");
                $YetaWF.elementRemoveClass($YetaWF.elementClosest(a, "ul"), "t_path");
            }
        }

        public updateSize(): void {
            if (this.isSmall) {
                if (!$YetaWF.elementHasClass(this.Control, "t_small")) {
                    $YetaWF.elementRemoveClasses(this.Control, ["t_large", "t_small", "t_horizontal", "t_vertical"]);
                    $YetaWF.elementAddClass(this.Control, "t_small");
                    this.Control.style.width = "";
                    this.hide();
                    this.closeAll();
                }
            } else {
                if (!$YetaWF.elementHasClass(this.Control, "t_large")) {
                    $YetaWF.elementRemoveClasses(this.Control, ["t_large", "t_small", "t_horizontal", "t_vertical"]);
                    $YetaWF.elementAddClass(this.Control, "t_large");
                    if (this.isVertical) {
                        $YetaWF.elementAddClass(this.Control, "t_vertical");
                        this.Control.style.width = `${this.Setup.VerticalWidth}px`;
                    } else {
                        $YetaWF.elementAddClass(this.Control, "t_horizontal");
                        this.Control.style.width = "";
                    }
                    this.show();
                    this.closeAll();
                }
            }
        }

        // API

        public closeAll(): void {
            this.MenuRects = [];
            this.clearScheduleCloseSublevel();
            if (this.isVertical) return;
            const anchors = $YetaWF.getElementsBySelector(`ul.${MenuComponent.TEMPLATE} > li > a`, [this.Control]) as HTMLAnchorElement[];// all top level anchors
            for (const anchor of anchors) {
                this.closeSublevelsStartingAt(anchor);
            }
            if (this.isSmall)
                this.hide();
        }

        public static closeAllMenus(): void {
            let controls: MenuComponent[] = YetaWF.ComponentBaseDataImpl.getControls(MenuComponent.SELECTOR);
            for (let control of controls) {
                control.closeAll();
            }
        }

        public get isShown(): boolean {
            return this.Control.style.display !== "none";
        }

        public get isSmall(): boolean {
            let small = false;
            if (this.Setup.SmallMenuMaxWidth !== 0) {
                if (window.innerWidth <= this.Setup.SmallMenuMaxWidth)
                    small = true;
            } else {
                small = false;
            }
            return small;
        }
        public get isHorizontal(): boolean {
            return !this.isSmall && this.Setup.Orientation == OrientationEnum.Horizontal;
        }
        public get isVertical(): boolean {
            return !this.isSmall && this.Setup.Orientation == OrientationEnum.Vertical;
        }

        public show(): void {
            this.Control.style.display = "";
        }
        public hide(): void {
            this.Control.style.display = "none";
        }

        public selectEntry(path: string): boolean {
            this.clearPath();
            const subUL = this.Control as HTMLUListElement;
            return this.selectSublevelEntry(this.normalizePath(path), subUL);
        }
        private selectSublevelEntry(path: string, subUL: HTMLUListElement): boolean {
            let result = false;
            let subLI = $YetaWF.getChildElement1ByTagCond("li", subUL) as HTMLLIElement|null;
            while (!!subLI) {
                let anchor = $YetaWF.getChildElement1ByTagCond("a", subLI) as HTMLAnchorElement|null;
                if (anchor) {
                    if (this.normalizePath(anchor.href) === path) {
                        $YetaWF.elementToggleClass(subLI, "t_selected", true);
                        $YetaWF.elementToggleClass(subLI, "t_path", true);
                        $YetaWF.elementToggleClass(subUL, "t_path", true);
                        $YetaWF.elementToggleClass(anchor, "t_path", true);
                        result = true;
                    } else {
                        $YetaWF.elementRemoveClasses(subLI, ["t_selected"]);
                    }
                    const subSubUL = $YetaWF.getChildElement1ByTagCond("ul", subLI) as HTMLUListElement;
                    if (subSubUL) {
                        if (this.selectSublevelEntry(path, subSubUL)) {
                            $YetaWF.elementToggleClass(subLI, "t_path", true);
                            $YetaWF.elementToggleClass(subUL, "t_path", true);
                            $YetaWF.elementToggleClass(anchor, "t_path", true);
                            if (this.isVertical)
                                subSubUL.style.display = "";// expand it (no transition)
                            result = true;
                        }
                    }
                }
                subLI = subLI.nextElementSibling as HTMLLIElement;
            }
            return result;
        }
        private normalizePath(path: string): string {
            const i = path.indexOf("?");
            if (i > 0)
                path = path.substring(0, i);
            return path.toLowerCase();
        }

    }

    $YetaWF.registerEventHandlerBody("mousemove", null, (ev: MouseEvent): boolean => {
        // let controls: MenuComponent[] = YetaWF.ComponentBaseDataImpl.getControls(MenuComponent.SELECTOR);
        // for (let control of controls) {
        //     control.handleMouseMove(ev.clientX, ev.clientY);
        // }
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
        let menus = YetaWF.ComponentBaseDataImpl.getControls<MenuComponent>(MenuComponent.SELECTOR);
        for (let menu of menus) {
            menu.closeAll();
            menu.selectEntry(window.location.href);
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
