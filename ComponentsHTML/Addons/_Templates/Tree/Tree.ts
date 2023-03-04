/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    //export interface IPackageLocs {
    //    TreeTotals: string;
    //}

    interface TreeSetup {
        DragDrop: boolean;
        ContextMenu: boolean;

        HoverCss: string;
        HighlightCss: string;
        DisabledCss: string;
        RowHighlightCss: string;
        RowDragDropHighlightCss: string;
        SelectedCss: string;
        AjaxUrl: string;
    }
    interface TreePartialResult {
        Records: number;
        HTML: string;
    }
    interface TreePartialViewData extends YetaWF.PartialViewData {
        Entry: any;// one record
    }

    export interface TreeEntry {
        DynamicSubEntries: boolean;
        UrlNew?: string;
        UrlContent?: string;
    }

    enum TargetPositionEnum {
        on = 0,
        before = 1,
        after = 2,
    }
    export class TreeComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_tree";
        public static readonly SELECTOR: string = ".yt_tree";

        public static readonly EVENT: string = "tree_click";// obsolete
        public static readonly EVENTCLICK: string = "tree_click";
        public static readonly EVENTDBLCLICK: string = "tree_dblclick";
        public static readonly EVENTSELECT: string = "tree_select";
        public static readonly EVENTDROP: string = "tree_drop";
        public static readonly EVENTCONTEXTMENU: string = "tree_contextmenu";

        private Setup: TreeSetup;

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
                return this.sendClickEvent(liElem);
            });
            $YetaWF.registerEventHandler(this.Control, "dblclick", "a.t_entry", (ev: MouseEvent): boolean => {
                var liElem = $YetaWF.elementClosest(ev.__YetaWFElem, "li") as HTMLLIElement; // get row we're on
                this.setSelect(liElem);
                return this.sendDblClickEvent(liElem);
            });
            $YetaWF.registerEventHandler(this.Control, "dblclick", "a.t_entry", (ev: MouseEvent): boolean => {
                var liElem = $YetaWF.elementClosest(ev.__YetaWFElem, "li") as HTMLLIElement; // get row we're on
                this.setSelect(liElem);
                if (this.canExpand(liElem))
                    this.expand(liElem);
                else if (this.canCollapse(liElem))
                    this.collapse(liElem);
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "click", "i.t_icdown", (ev: MouseEvent): boolean => {
                var li = $YetaWF.elementClosest(ev.__YetaWFElem, "li") as HTMLLIElement; // get row we're on
                this.collapse(li);
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "click", "i.t_icright", (ev: MouseEvent): boolean => {
                var li = $YetaWF.elementClosest(ev.__YetaWFElem, "li") as HTMLLIElement; // get row we're on
                this.expand(li);
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "keydown", null, (ev: KeyboardEvent): boolean => {
                var key = ev.key;
                if (key === "ArrowDown" || key === "Down") {
                    var liElem = this.getSelect();
                    if (!liElem) return false;
                    liElem = this.getNextVisibleEntry(liElem);
                    if (!liElem) return false;
                    this.setSelect(liElem);
                    this.sendSelectEvent();
                    return false;
                } else if (key === "ArrowUp" || key === "Up") {
                    var liElem = this.getSelect();
                    if (!liElem) return false;
                    liElem = this.getPrevVisibleEntry(liElem);
                    if (!liElem) return false;
                    this.setSelect(liElem);
                    this.sendSelectEvent();
                    return false;
                } else if (key === "Home") {
                    var liElem = this.getFirstVisibleItem();
                    if (!liElem) return false;
                    this.setSelect(liElem);
                    this.sendSelectEvent();
                    return false;
                } else if (key === "End") {
                    var liElem = this.getLastVisibleItem();
                    if (!liElem) return false;
                    this.setSelect(liElem);
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
                        this.setSelect(liElem);
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
                        this.setSelect(liElem);
                        this.sendSelectEvent();
                    }
                    return false;
                } else if (key === "Enter") {
                    var liElem = this.getSelect();
                    if (!liElem) return false;
                    return this.sendClickEvent(liElem);
                }
                return true;
            });
            if (this.Setup.ContextMenu) {
                $YetaWF.registerEventHandler(this.Control, "contextmenu", "a.t_entry", (ev: MouseEvent): boolean => {
                    var liElem = $YetaWF.elementClosest(ev.__YetaWFElem, "li") as HTMLLIElement; // get row we're on
                    this.setSelect(liElem);
                    ev.preventDefault();
                    return this.sendContextMenuEvent();
                });
            }
        }

        private sendClickEvent(liElem: HTMLLIElement): boolean {
            let data = this.getElementDataCond(liElem);
            if (!data || (!data.UrlNew && !data.UrlContent)) {
                return $YetaWF.sendCustomEvent(this.Control, TreeComponent.EVENTCLICK);
            }
            return true;
        }
        private sendDblClickEvent(liElem: HTMLLIElement): boolean {
            let data = this.getElementDataCond(liElem);
            if (!data || (!data.UrlNew && !data.UrlContent)) {
                return $YetaWF.sendCustomEvent(this.Control, TreeComponent.EVENTDBLCLICK);
            }
            return true;
        }
        private sendSelectEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, TreeComponent.EVENTSELECT);
        }
        private sendDropEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, TreeComponent.EVENTDROP);
        }
        private sendContextMenuEvent(): boolean {
            return $YetaWF.sendCustomEvent(this.Control, TreeComponent.EVENTCONTEXTMENU);
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
                var parentUl = $YetaWF.elementClosest(this.DDSource, "ul.t_sub");

                switch (this.DDTargetPosition) {
                    default:
                    case TargetPositionEnum.on:
                        // we're droppiong on an item, that means it has no child items yet
                        // add ul
                        var ul = document.createElement("ul");
                        $YetaWF.elementAddClass(ul, "t_sub");
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
                            $YetaWF.processClearDiv(parentUl);
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

        // expand/collapse

        private expandItem(liElem: HTMLLIElement): void {

            if (!this.Setup.AjaxUrl)
                throw `Tree control doesn't have an AJAX URL - ${this.Control.outerHTML}`;

            if (!$YetaWF.isLoading) {

                // fetch data from servers
                const formJson = $YetaWF.Forms.getJSONInfo(this.Control);
                let data: TreePartialViewData = {
                    __UniqueIdCounters: YVolatile.Basics.UniqueIdCounters,
                    __ModuleGuid: formJson.ModuleGuid,
                    __RequestVerificationToken: formJson.RequestVerificationToken,
                    Entry: this.getElementDataCond(liElem),
                }

                var uri = $YetaWF.parseUrl(this.Setup.AjaxUrl);

                $YetaWF.postJSON(uri, formJson, null, data, (success: boolean, partial: TreePartialResult): void =>{
                    if (success) {
                        let iElem = $YetaWF.getElement1BySelector("i.t_icright", [liElem]);
                        $YetaWF.elementRemoveClasses(iElem, ["t_icright", "t_icdown", "t_icempty"]);
                        if (partial.Records > 0) {
                            // add new items
                            $YetaWF.appendMixedHTML(liElem, partial.HTML);
                            // mark expanded
                            $YetaWF.elementAddClass(iElem, "t_icdown");
                        } else {
                            // mark not expandable
                            $YetaWF.elementAddClass(iElem, "t_icempty");
                        }
                    }
                });
            }
        }

        // API

        public canCollapse(liElem: HTMLLIElement): boolean {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]); // get the subitems
            if (!ul || ul.style.display !== "") return false;
            return true;
        }
        public collapse(liElem: HTMLLIElement): void {
            var ul = $YetaWF.getElement1BySelector("ul.t_sub", [liElem]); // get the subitems
            if (ul) {
                let data = this.getElementDataCond(liElem);
                if (data && data.DynamicSubEntries) {
                    $YetaWF.setLoading(true);
                    $YetaWF.processClearDiv(ul);
                    ul.remove();
                    $YetaWF.setLoading(false);
                } else {
                    ul.style.display = "none";
                }
            }
            var iElem = $YetaWF.getElement1BySelector("i.t_icdown", [liElem]);
            $YetaWF.elementRemoveClass(iElem, "t_icdown");
            $YetaWF.elementAddClass(iElem, "t_icright");
        }
        public canExpand(liElem: HTMLLIElement): boolean {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]); // get the subitems
            if (ul) {
                if (ul.style.display === "") return false;
                return true;
            } else {
                let data = this.getElementDataCond(liElem);
                return data != null && data.DynamicSubEntries;
            }
        }
        public expand(liElem: HTMLLIElement): void {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]); // get the subitems
            if (ul == null) {
                let data = this.getElementData(liElem);
                if (data.DynamicSubEntries) {
                    this.expandItem(liElem);
                }
            } else {
                ul.style.display = "";
                let iElem = liElem.firstElementChild as HTMLElement;
                if (iElem) {
                    $YetaWF.elementRemoveClass(iElem, "t_icright");
                    $YetaWF.elementAddClass(iElem, "t_icdown");
                }
            }
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
        public makeVisible(liElem: HTMLLIElement): void {
            let li: HTMLLIElement | null = liElem;
            for ( ; li ; ) {
                li = this.getParent(li);
                if (!li)
                    return;
                this.expand(li);
            }
        }
        public getParent(liElem: HTMLLIElement): HTMLLIElement | null {
            let ul = liElem.parentElement as HTMLUListElement;
            if ($YetaWF.elementHasClass(ul, "tg_root"))
                return null;
            return $YetaWF.elementClosest(ul, "li") as HTMLLIElement | null;
        }
        public getElementDataCond<T = TreeEntry>(liElem: HTMLLIElement): T | null {
            let recData = $YetaWF.getAttributeCond(liElem, "data-record");
            if (!recData)
                return null;
            return JSON.parse(recData);
        }
        public getElementData<T = TreeEntry>(liElem: HTMLLIElement): T {
            let data = this.getElementDataCond<T>(liElem);
            if (!data)
                throw `No record data for ${liElem.outerHTML}`;
            return data;
        }
        public setElementData<T = TreeEntry>(liElem: HTMLLIElement, data: T): void {
            $YetaWF.setAttribute(liElem, "data-record", JSON.stringify(data));
        }
        public getEntryFromTagCond(tag: HTMLElement): HTMLLIElement | null {
            return $YetaWF.elementClosest(tag, "li") as HTMLLIElement | null;
        }

        public getSelect(): HTMLLIElement | null {
            let entry = $YetaWF.getElement1BySelectorCond(`.t_entry.${this.Setup.SelectedCss}`, [this.Control]) as HTMLElement;
            if (!entry) return null;
            let liElem = $YetaWF.elementClosest(entry, "li") as HTMLLIElement | null;
            return liElem;
        }
        public setSelect(liElem: HTMLLIElement): void {
            this.clearSelect();
            $YetaWF.elementAddClass(liElem, "t_select");
            let entry = $YetaWF.getElement1BySelector(".t_entry", [liElem]);
            $YetaWF.elementAddClass(entry, this.Setup.SelectedCss);
            entry.focus();
        }
        public getSelectData<T = TreeEntry>(): T | null {
            var liElem = this.getSelect();
            if (!liElem) return null;
            return this.getElementDataCond(liElem);
        }
        public setSelectData<T = TreeEntry>(data: T): void {
            var liElem = this.getSelect();
            if (!liElem)
                return;
            this.setElementData(liElem, data);
        }
        public getSelectText(): string {
            var entry = $YetaWF.getElement1BySelector(`.t_entry.${this.Setup.SelectedCss}`, [this.Control]) as HTMLElement;
            return entry.innerText;
        }
        public setSelectText(text: string): void {
            var entry = $YetaWF.getElement1BySelector(`.t_entry.${this.Setup.SelectedCss}`, [this.Control]) as HTMLElement;
            entry.innerText = text;
        }
        public clearSelect(): void {
            let entries = $YetaWF.getElementsBySelector("li.t_select", [this.Control]);
            for (let entry of entries)
                $YetaWF.elementRemoveClass(entry, "t_select");
            entries = $YetaWF.getElementsBySelector(`.t_entry.${this.Setup.SelectedCss}`, [this.Control]);
            for (let entry of entries)
                $YetaWF.elementRemoveClass(entry, this.Setup.SelectedCss);
        }
        public removeEntry(liElem: HTMLLIElement): void {
            // remove li element (and possibly parent(s))
            var ul = $YetaWF.elementClosest(liElem, "ul.t_sub") as HTMLUListElement | null;
            $YetaWF.processClearDiv(liElem);
            liElem.remove();
            for (; ;) {
                if (!ul) break;
                if (ul.children.length > 0)
                    break;
                if ($YetaWF.elementHasClass(ul, "tg_root"))
                    break;
                var parentUl = $YetaWF.elementClosest(ul.parentElement!, "ul.t_sub") as HTMLUListElement | null;
                $YetaWF.processClearDiv(ul);
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
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]) as HTMLUListElement | null;
            if (ul)
                return ul.children[0] as HTMLLIElement;
            return null;
        }
        public getLastDirectChild(liElem: HTMLLIElement): HTMLLIElement | null {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]) as HTMLUListElement | null;
            if (ul)
                return ul.children[ul.children.length - 1] as HTMLLIElement;
            return null;
        }
        public getNextEntry(liElem: HTMLLIElement): HTMLLIElement | null {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]) as HTMLUListElement | null;
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
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]) as HTMLUListElement | null;
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
                var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [li]) as HTMLUListElement | null;
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
                    var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [li]) as HTMLUListElement | null;
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
            var liElem = $YetaWF.getElement1BySelectorCond("ul.t_sub li", [this.Control]) as HTMLLIElement | null;
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

                ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]) as HTMLUListElement | null;
            }
            return liElem;
        }
        public addEntry<T = TreeEntry>(liElem: HTMLLIElement, text: string, data?: T): HTMLLIElement {
            var text = $YetaWF.htmlEscape(text);
            var entry = this.getNewEntry(text);
            liElem.insertAdjacentHTML("afterend", entry);
            let newElem = this.getNextSibling(liElem)!;
            if (data)
                this.setElementData(newElem, data);
            return newElem;
        }
        public insertEntry<T = TreeEntry>(liElem: HTMLLIElement, text: string, data?: T): HTMLLIElement {
            var text = $YetaWF.htmlEscape(text);
            var entry = this.getNewEntry(text);
            liElem.insertAdjacentHTML("beforebegin", entry);
            let newElem = this.getPrevSibling(liElem)!;
            if (data)
                this.setElementData(newElem, data);
            return newElem;
        }
        private getNewEntry(text: string): string {
            let dd = "";
            if (this.Setup.DragDrop)
                dd = " draggable='true' ondrop='YetaWF_ComponentsHTML.TreeComponent.onDrop(event)' ondragend='YetaWF_ComponentsHTML.TreeComponent.onDragEnd(event)' ondragover='YetaWF_ComponentsHTML.TreeComponent.onDragOver(event)' ondragstart='YetaWF_ComponentsHTML.TreeComponent.onDragStart(event)'";
            let entry = `<li><i class="t_icempty">&nbsp;</i><i class="t_icfile">&nbsp;</i><a class="t_entry" data-nohref='true' href="#"${dd}>${text}</a></li>`;
            return entry;
        }

        /** Scroll the selected item into the viewable area */
        public scrollIntoView(container: HTMLElement): void {
            var liElem = this.getSelect();
            if (!liElem) return;
            var rectLi = liElem.getBoundingClientRect();
            var rectContainer = container.getBoundingClientRect();
            let t = container.scrollTop + (rectLi.top - rectContainer.height / 2);
            if (t < 0) t = 0;
            container.scrollTop = t;
        }
    }
}
