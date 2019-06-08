/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

// Kendo UI menu use

namespace YetaWF_ComponentsHTML {

    //export interface IPackageLocs {
    //    GridTotals: string;
    //    GridTotal0: string;
    //    GridTotalNone: string;
    //}

    interface TreeSetup {

        StaticData: any[] | null;

        DragDrop: boolean;

        HoverCss: string;
        HighlightCss: string;
        DisabledCss: string;
        RowHighlightCss: string;
        RowDragDropHighlightCss: string;
    }

    enum TargetPositionEnum {
        on = 0,
        before = 1,
        after = 2,
    }
    export class TreeComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_tree";
        public static readonly SELECTOR: string = ".yt_tree";

        private Setup: TreeSetup;
        private DeletedRecords: number[] = [];// array of deleted record numbers

        private static DDTree: TreeComponent | null = null;
        private DDSource: HTMLLIElement | null = null;
        private DDSourceAnchor: HTMLAnchorElement | null = null;
        private DDLastTarget: HTMLLIElement | null = null;
        private DDLastTargetAnchor: HTMLAnchorElement | null = null;
        private DDTargetPosition: TargetPositionEnum = TargetPositionEnum.on;

        constructor(controlId: string, setup: TreeSetup) {
            super(controlId, TreeComponent.TEMPLATE, TreeComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            this.Setup = setup;

            $YetaWF.registerEventHandler(this.Control, "click", "a.t_entry", (ev: MouseEvent): boolean => {
                var liElem = $YetaWF.elementClosest(ev.__YetaWFElem, "li") as HTMLLIElement; // get row we're on
                this.setSelect(liElem);
                this.sendClickEvent();
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "click", "i.t_icdown", (ev: MouseEvent): boolean => {
                var li = $YetaWF.elementClosest(ev.__YetaWFElem, "li") as HTMLLIElement; // get row we're on
                setTimeout((): void => {
                    this.collapse(li);
                }, 1);
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "click", "i.t_icright", (ev: MouseEvent): boolean => {
                var li = $YetaWF.elementClosest(ev.__YetaWFElem, "li") as HTMLLIElement; // get row we're on
                setTimeout((): void => {
                    this.expand(li);
                }, 1);
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "keydown", null, (ev: KeyboardEvent): boolean => {
                var key = ev.key;
                if (key === "ArrowDown" || key === "Down") {
                    var liElem = this.getSelect();
                    if (!liElem) return false;
                    liElem = this.getNextVisibleEntry(liElem);
                    if (!liElem) return false;
                    this.setSelect(liElem, true);
                    this.sendSelectEvent();
                    return false;
                } else if (key === "ArrowUp" || key === "Up") {
                    var liElem = this.getSelect();
                    if (!liElem) return false;
                    liElem = this.getPrevVisibleEntry(liElem);
                    if (!liElem) return false;
                    this.setSelect(liElem, true);
                    this.sendSelectEvent();
                    return false;
                } else if (key === "Home") {
                    var liElem = this.getFirstVisibleItem();
                    if (!liElem) return false;
                    this.setSelect(liElem, true);
                    this.sendSelectEvent();
                    return false;
                } else if (key === "End") {
                    var liElem = this.getLastVisibleItem();
                    if (!liElem) return false;
                    this.setSelect(liElem, true);
                    this.sendSelectEvent();
                    return false;
                } else if (key === "ArrowLeft" || key === "Left") {
                    var liElem = this.getSelect();
                    if (!liElem) return false;
                    if (this.canCollapse(liElem))
                        this.collapse(liElem);
                    else {
                        liElem = this.getPrevVisibleEntry(liElem);
                        if (!liElem) return false;
                        this.setSelect(liElem, true);
                        this.sendSelectEvent();
                    }
                    return false;
                } else if (key === "ArrowRight" || key === "Right") {
                    var liElem = this.getSelect();
                    if (!liElem) return false;
                    if (this.canExpand(liElem))
                        this.expand(liElem);
                    else {
                        liElem = this.getNextVisibleEntry(liElem);
                        if (!liElem) return false;
                        this.setSelect(liElem, true);
                        this.sendSelectEvent();
                    }
                    return false;
                } else if (key === "Enter") {
                    var liElem = this.getSelect();
                    if (!liElem) return false;
                    this.sendClickEvent();
                    return false;
                }
                return true;
            });
        }

        private sendClickEvent(): void {
            setTimeout((): void => {
                var event = document.createEvent("Event");
                event.initEvent("tree_click", true, true);
                this.Control.dispatchEvent(event);
            }, 1);
        }

        private sendSelectEvent(): void {
            setTimeout((): void => {
                var event = document.createEvent("Event");
                event.initEvent("tree_select", true, true);
                this.Control.dispatchEvent(event);
            }, 1);
        }

        private sendDropEvent(): void {
            setTimeout((): void => {
                var event = document.createEvent("Event");
                event.initEvent("tree_drop", true, true);
                this.Control.dispatchEvent(event);
            }, 1);
        }

        /* Drag & Drop */

        public dragStart(ev: DragEvent): void {
            var li = $YetaWF.elementClosest(ev.target as HTMLElement, "li") as HTMLLIElement;
            this.DDSource = li;
            this.DDSourceAnchor = ev.target as HTMLAnchorElement;
            ev.dataTransfer!.setData("tree", "tree");
            ev.dataTransfer!.setDragImage(this.DDSourceAnchor, 0, 0);
        }
        public dragOver(ev: DragEvent): void {
            var liTarget = $YetaWF.elementClosestCond(ev.target as HTMLElement, "li") as HTMLLIElement;
            if (liTarget) {
                var ddTargetAnchor = ev.target as HTMLAnchorElement;
                if (ddTargetAnchor.tagName.toLowerCase() === "a") {
                    if (ddTargetAnchor !== this.DDSourceAnchor && !this.DDSource!.contains(ddTargetAnchor)) {
                        var targetRect = ddTargetAnchor.getBoundingClientRect();
                        if (this.getFirstDirectChild(liTarget) != null) {
                            this.setDragDropTarget(liTarget, ddTargetAnchor, TargetPositionEnum.before);
                        } else {
                            var third = targetRect.height / 3;
                            if (ev.offsetY < third)
                                this.setDragDropTarget(liTarget, ddTargetAnchor, TargetPositionEnum.before);
                            else if (ev.offsetY > 2 * third)
                                this.setDragDropTarget(liTarget, ddTargetAnchor, TargetPositionEnum.after);
                            else
                                this.setDragDropTarget(liTarget, ddTargetAnchor, TargetPositionEnum.on);
                        }
                        ev.dataTransfer!.dropEffect = "move";
                        ev.preventDefault();
                        return;
                    }
                }
            }
            this.setDragDropTarget(null);
        }
        private setDragDropTarget(liTarget: HTMLLIElement | null, targetAnchor?: HTMLAnchorElement, position?: TargetPositionEnum): void {

            if (this.DDLastTargetAnchor) {
                var divPos = $YetaWF.getElement1BySelector(".t_ddpos", [this.DDLastTargetAnchor]);
                divPos.remove();
            }
            this.DDLastTarget = null;
            this.DDLastTargetAnchor = null;

            if (liTarget) {
                this.DDLastTarget = liTarget;
                this.DDLastTargetAnchor = targetAnchor!;
                this.DDTargetPosition = position || TargetPositionEnum.on;
                var pos = "";
                switch (position) {
                    default:
                    case TargetPositionEnum.on: pos = "on"; break;
                    case TargetPositionEnum.before: pos = "before"; break;
                    case TargetPositionEnum.after: pos = "after"; break;
                }
                this.DDLastTargetAnchor.insertAdjacentHTML("afterbegin", `<div class="t_ddpos t_${pos}"></div>`);
            }
        }
        public drop(ev: DragEvent): void {
            if (this.DDSource && this.DDLastTarget && this.DDLastTargetAnchor) {

                // find the source item's parent ul in case it's the only item so we can update expand/collapse and file/folder icons
                var parentUl = $YetaWF.elementClosest(this.DDSource, "ul");

                switch (this.DDTargetPosition) {
                    default:
                    case TargetPositionEnum.on:
                        // we're droppiong on an item, that means it has no child items yet
                        // add ul
                        var ul = document.createElement("ul");
                        // source items to up
                        ul.appendChild(this.DDSource);
                        // all ul & child items to target
                        this.DDLastTarget.insertAdjacentElement("beforeend", ul);

                        // update target's file/folder icon (now a folder)
                        var iElem = $YetaWF.getElement1BySelector("i.t_icfile", [this.DDLastTarget]);
                        $YetaWF.elementRemoveClass(iElem, "t_icfile");
                        $YetaWF.elementAddClass(iElem, "t_icfolder");

                        // update target's expand/collapse icon (now expanded)
                        iElem = $YetaWF.getElement1BySelector("i.t_icempty", [this.DDLastTarget]);
                        $YetaWF.elementAddClass(iElem, "t_icdown");
                        break;
                    case TargetPositionEnum.before:
                        this.DDLastTarget.insertAdjacentElement("beforebegin", this.DDSource);
                        break;
                    case TargetPositionEnum.after:
                        this.DDLastTarget.insertAdjacentElement("afterend", this.DDSource);
                        break;
                }
                // Update the source's parent item in case we moved the only child items in which case we need to update expand/collapse and file/folder icons
                while (parentUl) {
                    if (parentUl.childElementCount === 0) {
                        if (!$YetaWF.elementHasClass(parentUl, "tg_root")) {
                            var liElem = parentUl.parentElement as HTMLLIElement;
                            // remove the empty ul element
                            parentUl.remove();
                            // update target's file/folder icon (now a file)
                            var iElem = $YetaWF.getElement1BySelector("i.t_icfolder", [liElem]);
                            $YetaWF.elementRemoveClass(iElem, "t_icfolder");
                            $YetaWF.elementAddClass(iElem, "t_icfile");

                            // update target's expand/collapse icon (now expanded)
                            iElem = $YetaWF.getElement1BySelector("i.t_icdown", [liElem]);
                            $YetaWF.elementRemoveClass(iElem, "t_icdown");
                            $YetaWF.elementAddClass(iElem, "t_icempty");

                            parentUl = liElem.parentElement as HTMLUListElement;
                        }
                    } else
                        break;
                }
                this.sendDropEvent();
            }
            this.setDragDropTarget(null);
            this.DDSource = null;
            this.DDSourceAnchor = null;
        }
        public dragEnd(ev: DragEvent): void {
            this.setDragDropTarget(null);
            this.DDSource = null;
            this.DDSourceAnchor = null;
        }

        public static onDragStart(ev: DragEvent): void {
            TreeComponent.DDTree = YetaWF.ComponentBaseDataImpl.getControlFromTag<TreeComponent>(ev.target as HTMLElement, TreeComponent.SELECTOR);
            TreeComponent.DDTree.dragStart(ev);
        }
        public static onDragOver(ev: DragEvent): void {
            if (TreeComponent.DDTree) {
                TreeComponent.DDTree.dragOver(ev);
            }
        }
        public static onDrop(ev: DragEvent): void {
            if (TreeComponent.DDTree) {
                TreeComponent.DDTree.drop(ev);
            }
            TreeComponent.DDTree = null;
        }
        public static onDragEnd(ev: DragEvent): void {
            if (TreeComponent.DDTree)
                TreeComponent.DDTree.dragEnd(ev);
            TreeComponent.DDTree = null;
        }

        // API

        public canCollapse(liElem: HTMLLIElement): boolean {
            var ul = $YetaWF.getElement1BySelectorCond("ul", [liElem]); // get the subitems
            if (!ul || ul.style.display !== "") return false;
            return true;
        }
        public collapse(liElem: HTMLLIElement): void {
            var ul = $YetaWF.getElement1BySelector("ul", [liElem]); // get the subitems
            ul.style.display = "none";
            var iElem = $YetaWF.getElement1BySelector("i.t_icdown", [liElem]);
            $YetaWF.elementRemoveClass(iElem, "t_icdown");
            $YetaWF.elementAddClass(iElem, "t_icright");
        }
        public canExpand(liElem: HTMLLIElement): boolean {
            var ul = $YetaWF.getElement1BySelectorCond("ul", [liElem]); // get the subitems
            if (!ul || ul.style.display === "") return false;
            return true;
        }
        public expand(liElem: HTMLLIElement): void {
            var ul = $YetaWF.getElement1BySelector("ul", [liElem]); // get the subitems
            ul.style.display = "";
            var iElem = $YetaWF.getElement1BySelector("i.t_icright", [liElem]);
            $YetaWF.elementRemoveClass(iElem, "t_icright");
            $YetaWF.elementAddClass(iElem, "t_icdown");
        }
        public expandAll(): void {
            var li = this.getFirstVisibleItem();
            while (li) {
                if (this.canExpand(li))
                    this.expand(li);
                li = this.getNextVisibleEntry(li);
            }
        }
        public collapseAll(): void {
            var li = this.getFirstVisibleItem();
            while (li) {
                if (this.canCollapse(li))
                    this.collapse(li);
                li = this.getNextEntry(li);
            }
        }
        public getElementData(liElem: HTMLLIElement): any {
            if (!this.Setup.StaticData) throw "no static data";
            var rec = Number($YetaWF.getAttribute(liElem, "data-record"));
            if (rec < 0 || rec >= this.Setup.StaticData.length)
                throw `Unexpected record # ${rec} in ${liElem.outerHTML}`;
            return this.Setup.StaticData[rec];
        }
        public getSelect(): HTMLLIElement | null {
            var entry = $YetaWF.getElement1BySelectorCond(".t_entry.t_select", [this.Control]) as HTMLElement;
            if (!entry) return null;
            var liElem = $YetaWF.elementClosest(entry, "li") as HTMLLIElement | null;
            return liElem;
        }
        public setSelect(liElem: HTMLLIElement, focus?:boolean): void {
            this.clearSelect();
            var entry = $YetaWF.getElement1BySelector(".t_entry", [liElem]);
            $YetaWF.elementAddClass(entry, "t_select");
            if (focus === true)
                entry.focus();
        }
        public getSelectData(): any | null {
            if (!this.Setup.StaticData) throw "no static data";
            var liElem = this.getSelect();
            if (!liElem) return null;
            var rec = Number($YetaWF.getAttribute(liElem, "data-record"));
            if (rec < 0 || rec >= this.Setup.StaticData.length) return null;
            return this.Setup.StaticData[rec];
        }
        public getSelectText(): string {
            var entry = $YetaWF.getElement1BySelector(".t_entry.t_select", [this.Control]) as HTMLElement;
            return entry.innerText;
        }
        public setSelectText(text: string): void {
            var entry = $YetaWF.getElement1BySelector(".t_entry.t_select", [this.Control]) as HTMLElement;
            entry.innerText = text;
        }
        public clearSelect(): void {
            var entries = $YetaWF.getElementsBySelector(".t_entry.t_select", [this.Control]);
            for (let entry of entries)
                $YetaWF.elementRemoveClass(entry, "t_select");
        }
        public removeEntry(liElem: HTMLLIElement): void {
            if (!this.Setup.StaticData) throw "no static data";
            var rec = Number($YetaWF.getAttribute(liElem, "data-record"));
            if (rec < 0 || rec >= this.Setup.StaticData.length) return;

            // remove the record(s) from static data (including child items)
            this.DeletedRecords.push(rec);
            this.Setup.StaticData[rec] = null;
            var childLis = $YetaWF.getElementsBySelector("li", [liElem]) as HTMLLIElement[];
            for (let childLi of childLis) {
                rec = Number($YetaWF.getAttribute(childLi, "data-record"));
                if (rec < 0 || rec >= this.Setup.StaticData.length)
                    throw `Unexpected record # ${rec} in ${childLi.outerHTML}`;
                this.DeletedRecords.push(rec);
            }

            // remove li element (and possibly parent(s))
            var ul = $YetaWF.elementClosest(liElem, "ul") as HTMLUListElement | null;
            liElem.remove();
            for (; ;) {
                if (!ul) break;
                if (ul.children.length > 0)
                    break;
                if ($YetaWF.elementHasClass(ul, "tg_root"))
                    break;
                var parentUl = $YetaWF.elementClosest(ul.parentElement!, "ul") as HTMLUListElement | null;
                ul.remove();
                ul = parentUl;
            }
        }
        public getNextSibling(liElem: HTMLLIElement): HTMLLIElement | null {
            var li = liElem.nextElementSibling as HTMLLIElement | null;
            return li;
        }
        public getPrevSibling(liElem: HTMLLIElement): HTMLLIElement | null {
            var li = liElem.previousElementSibling as HTMLLIElement | null;
            return li;
        }
        public getFirstDirectChild(liElem: HTMLLIElement): HTMLLIElement | null {
            var ul = $YetaWF.getElement1BySelectorCond("ul", [liElem]) as HTMLUListElement | null;
            if (ul)
                return ul.children[0] as HTMLLIElement;
            return null;
        }
        public getLastDirectChild(liElem: HTMLLIElement): HTMLLIElement | null {
            var ul = $YetaWF.getElement1BySelectorCond("ul", [liElem]) as HTMLUListElement | null;
            if (ul)
                return ul.children[ul.children.length - 1] as HTMLLIElement;
            return null;
        }
        public getNextEntry(liElem: HTMLLIElement): HTMLLIElement | null {
            var ul = $YetaWF.getElement1BySelectorCond("ul", [liElem]) as HTMLUListElement | null;
            // get item in subentries
            if (ul)
                return ul.children[0] as HTMLLIElement;
            // no subentries, try next sibling (work your way up the tree)
            for (; ;) {
                var li = liElem.nextElementSibling as HTMLLIElement | null;
                if (li)
                    return li;
                // no next sibling, go to ul and parent il
                ul = liElem.parentElement as HTMLUListElement;
                if ($YetaWF.elementHasClass(ul, "tg_root"))
                    return null;
                li = $YetaWF.elementClosest(ul, "li") as HTMLLIElement | null;
                if (!li)
                    return null;
                liElem = li;
            }
        }
        public getNextVisibleEntry(liElem: HTMLLIElement): HTMLLIElement | null {
            var ul = $YetaWF.getElement1BySelectorCond("ul", [liElem]) as HTMLUListElement | null;
            // get item in subentries
            if (ul && ul.style.display === "")
                return ul.children[0] as HTMLLIElement;
            // no subentries, try next sibling (work your way up the tree)
            for (; ;) {
                var li = liElem.nextElementSibling as HTMLLIElement | null;
                if (li)
                    return li;
                // no next sibling, go to ul and parent il
                ul = liElem.parentElement as HTMLUListElement;
                if ($YetaWF.elementHasClass(ul, "tg_root"))
                    return null;
                li = $YetaWF.elementClosest(ul, "li") as HTMLLIElement | null;
                if (!li)
                    return null;
                liElem = li;
            }
        }
        public getPrevEntry(liElem: HTMLLIElement): HTMLLIElement | null {
            var li: HTMLLIElement | null = liElem.previousElementSibling as HTMLLIElement | null;
            if (li) {
                var ul = $YetaWF.getElement1BySelectorCond("ul", [li]) as HTMLUListElement | null;
                if (ul)
                    return ul.children[ul.children.length - 1] as HTMLLIElement | null;
                return li;
            }
            ul = liElem.parentElement as HTMLUListElement;
            if ($YetaWF.elementHasClass(ul, "tg_root"))
                return null;
            li = $YetaWF.elementClosest(ul, "li") as HTMLLIElement | null;
            if (!li)
                return null;
            return li;
        }
        public getPrevVisibleEntry(liElem: HTMLLIElement): HTMLLIElement | null {
            var li: HTMLLIElement | null = liElem.previousElementSibling as HTMLLIElement | null;
            if (li) {
                for (; ;) {
                    var ul = $YetaWF.getElement1BySelectorCond("ul", [li]) as HTMLUListElement | null;
                    if (!ul || ul.style.display !== "")
                        return li;
                    li = ul.children[ul.children.length - 1] as HTMLLIElement;
                }
            }
            ul = liElem.parentElement as HTMLUListElement;
            if ($YetaWF.elementHasClass(ul, "tg_root"))
                return null;
            li = $YetaWF.elementClosest(ul, "li") as HTMLLIElement | null;
            if (!li)
                return null;
            return li;
        }
        public getFirstVisibleItem(): HTMLLIElement | null {
            var liElem = $YetaWF.getElement1BySelectorCond("ul li", [this.Control]) as HTMLLIElement | null;
            return liElem;
        }
        public getLastVisibleItem(): HTMLLIElement | null {

            var ul: HTMLUListElement | null = $YetaWF.getElement1BySelector("ul.tg_root", [this.Control]) as HTMLUListElement;
            var liElem: HTMLLIElement | null = null;
            for (; ;) {
                if (!ul || ul.style.display !== "")
                    break;
                // get last item in subentries
                liElem = ul.children[ul.children.length - 1] as HTMLLIElement;

                ul = $YetaWF.getElement1BySelectorCond("ul", [liElem]) as HTMLUListElement | null;
            }
            return liElem;
        }
        public addEntry(liElem: HTMLLIElement, text:string, data: any): HTMLLIElement {
            var text = $YetaWF.htmlEscape(text);
            var entry = this.getNewEntry(text, data);
            liElem.insertAdjacentHTML("afterend", entry);
            return this.getNextSibling(liElem)!;
        }
        public insertEntry(liElem: HTMLLIElement, text: string, data: any): HTMLLIElement {
            var text = $YetaWF.htmlEscape(text);
            var entry = this.getNewEntry(text, data);
            liElem.insertAdjacentHTML("beforebegin", entry);
            return this.getPrevSibling(liElem)!;
        }
        private getNewEntry(text: string, data: any): string {
            var rec = this.getAvailableRecord(data);
            var dd = "";
            if (this.Setup.DragDrop)
                dd = " draggable='true' ondrop='YetaWF_ComponentsHTML.TreeComponent.onDrop(event)' ondragover='YetaWF_ComponentsHTML.TreeComponent.onDragOver(event)' ondragstart='YetaWF_ComponentsHTML.TreeComponent.onDragStart(event)'";
            var entry = `<li data-record="${rec}"><i class="t_icempty"></i> <i class="t_icfile"></i><a class="t_entry" href="#"${dd}>${text}</a></li>`;
            return entry;
        }
        private getAvailableRecord(data: any): number {
            if (!this.Setup.StaticData) throw "no static data";
            var rec: number;
            if (this.DeletedRecords.length > 0) {
                rec = this.DeletedRecords[0];
                this.DeletedRecords.splice(0, 1);
                this.Setup.StaticData[rec] = data;
            } else {
                rec = this.Setup.StaticData.length;
                this.Setup.StaticData.push(data);
            }
            return rec;
        }

        /** Scroll the selected item into the viewable area */
        public scrollIntoView(container: HTMLElement): void {
            var liElem = this.getSelect();
            if (!liElem) return;
            var rectLi = liElem.getBoundingClientRect();
            var rectContainer = container.getBoundingClientRect();
            container.scrollTop = rectLi.top - rectContainer.height / 2;
        }
    }
}
