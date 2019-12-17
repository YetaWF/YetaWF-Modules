"use strict";
/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
// Kendo UI menu use
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    //export interface IPackageLocs {
    //    TreeTotals: string;
    //}
    var TargetPositionEnum;
    (function (TargetPositionEnum) {
        TargetPositionEnum[TargetPositionEnum["on"] = 0] = "on";
        TargetPositionEnum[TargetPositionEnum["before"] = 1] = "before";
        TargetPositionEnum[TargetPositionEnum["after"] = 2] = "after";
    })(TargetPositionEnum || (TargetPositionEnum = {}));
    var TreeComponent = /** @class */ (function (_super) {
        __extends(TreeComponent, _super);
        function TreeComponent(controlId, setup) {
            var _this = _super.call(this, controlId, TreeComponent.TEMPLATE, TreeComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.DDSource = null;
            _this.DDSourceAnchor = null;
            _this.DDLastTarget = null;
            _this.DDLastTargetAnchor = null;
            _this.DDTargetPosition = TargetPositionEnum.on;
            _this.AddCounter = 0;
            _this.Setup = setup;
            $YetaWF.registerEventHandler(_this.Control, "click", "a.t_entry", function (ev) {
                var liElem = $YetaWF.elementClosest(ev.__YetaWFElem, "li"); // get row we're on
                _this.setSelect(liElem);
                _this.sendClickEvent(liElem);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "dblclick", "a.t_entry", function (ev) {
                var liElem = $YetaWF.elementClosest(ev.__YetaWFElem, "li"); // get row we're on
                _this.setSelect(liElem);
                if (_this.canExpand(liElem))
                    _this.expand(liElem);
                else if (_this.canCollapse(liElem))
                    _this.collapse(liElem);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "click", "i.t_icdown", function (ev) {
                var li = $YetaWF.elementClosest(ev.__YetaWFElem, "li"); // get row we're on
                setTimeout(function () {
                    _this.collapse(li);
                }, 1);
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "click", "i.t_icright", function (ev) {
                var li = $YetaWF.elementClosest(ev.__YetaWFElem, "li"); // get row we're on
                setTimeout(function () {
                    _this.expand(li);
                }, 1);
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "keydown", null, function (ev) {
                var key = ev.key;
                if (key === "ArrowDown" || key === "Down") {
                    var liElem = _this.getSelect();
                    if (!liElem)
                        return false;
                    liElem = _this.getNextVisibleEntry(liElem);
                    if (!liElem)
                        return false;
                    _this.setSelect(liElem, true);
                    _this.sendSelectEvent();
                    return false;
                }
                else if (key === "ArrowUp" || key === "Up") {
                    var liElem = _this.getSelect();
                    if (!liElem)
                        return false;
                    liElem = _this.getPrevVisibleEntry(liElem);
                    if (!liElem)
                        return false;
                    _this.setSelect(liElem, true);
                    _this.sendSelectEvent();
                    return false;
                }
                else if (key === "Home") {
                    var liElem = _this.getFirstVisibleItem();
                    if (!liElem)
                        return false;
                    _this.setSelect(liElem, true);
                    _this.sendSelectEvent();
                    return false;
                }
                else if (key === "End") {
                    var liElem = _this.getLastVisibleItem();
                    if (!liElem)
                        return false;
                    _this.setSelect(liElem, true);
                    _this.sendSelectEvent();
                    return false;
                }
                else if (key === "ArrowLeft" || key === "Left") {
                    var liElem = _this.getSelect();
                    if (!liElem)
                        return false;
                    if (_this.canCollapse(liElem))
                        _this.collapse(liElem);
                    else {
                        liElem = _this.getPrevVisibleEntry(liElem);
                        if (!liElem)
                            return false;
                        _this.setSelect(liElem, true);
                        _this.sendSelectEvent();
                    }
                    return false;
                }
                else if (key === "ArrowRight" || key === "Right") {
                    var liElem = _this.getSelect();
                    if (!liElem)
                        return false;
                    if (_this.canExpand(liElem))
                        _this.expand(liElem);
                    else {
                        liElem = _this.getNextVisibleEntry(liElem);
                        if (!liElem)
                            return false;
                        _this.setSelect(liElem, true);
                        _this.sendSelectEvent();
                    }
                    return false;
                }
                else if (key === "Enter") {
                    var liElem = _this.getSelect();
                    if (!liElem)
                        return false;
                    _this.sendClickEvent(liElem);
                    return false;
                }
                return true;
            });
            return _this;
        }
        TreeComponent.prototype.sendClickEvent = function (liElem) {
            var _this = this;
            var data = this.getElementDataCond(liElem);
            if (!data || (!data.UrlNew && !data.UrlContent)) {
                setTimeout(function () {
                    var event = document.createEvent("Event");
                    event.initEvent("tree_click", true, true);
                    _this.Control.dispatchEvent(event);
                }, 1);
            }
        };
        TreeComponent.prototype.sendSelectEvent = function () {
            var _this = this;
            setTimeout(function () {
                var event = document.createEvent("Event");
                event.initEvent("tree_select", true, true);
                _this.Control.dispatchEvent(event);
            }, 1);
        };
        TreeComponent.prototype.sendDropEvent = function () {
            var _this = this;
            setTimeout(function () {
                var event = document.createEvent("Event");
                event.initEvent("tree_drop", true, true);
                _this.Control.dispatchEvent(event);
            }, 1);
        };
        /* Drag & Drop */
        TreeComponent.prototype.dragStart = function (ev) {
            var li = $YetaWF.elementClosest(ev.target, "li");
            this.DDSource = li;
            this.DDSourceAnchor = ev.target;
            ev.dataTransfer.setData("tree", "tree");
            ev.dataTransfer.setDragImage(this.DDSourceAnchor, 0, 0);
        };
        TreeComponent.prototype.dragOver = function (ev) {
            var liTarget = $YetaWF.elementClosestCond(ev.target, "li");
            if (liTarget) {
                var ddTargetAnchor = ev.target;
                if (ddTargetAnchor.tagName.toLowerCase() === "a") {
                    if (ddTargetAnchor !== this.DDSourceAnchor && !this.DDSource.contains(ddTargetAnchor)) {
                        var targetRect = ddTargetAnchor.getBoundingClientRect();
                        if (this.getFirstDirectChild(liTarget) != null) {
                            this.setDragDropTarget(liTarget, ddTargetAnchor, TargetPositionEnum.before);
                        }
                        else {
                            var third = targetRect.height / 3;
                            if (ev.offsetY < third)
                                this.setDragDropTarget(liTarget, ddTargetAnchor, TargetPositionEnum.before);
                            else if (ev.offsetY > 2 * third)
                                this.setDragDropTarget(liTarget, ddTargetAnchor, TargetPositionEnum.after);
                            else
                                this.setDragDropTarget(liTarget, ddTargetAnchor, TargetPositionEnum.on);
                        }
                        ev.dataTransfer.dropEffect = "move";
                        ev.preventDefault();
                        return;
                    }
                }
            }
            this.setDragDropTarget(null);
        };
        TreeComponent.prototype.setDragDropTarget = function (liTarget, targetAnchor, position) {
            if (this.DDLastTargetAnchor) {
                var divPos = $YetaWF.getElement1BySelector(".t_ddpos", [this.DDLastTargetAnchor]);
                divPos.remove();
            }
            this.DDLastTarget = null;
            this.DDLastTargetAnchor = null;
            if (liTarget) {
                this.DDLastTarget = liTarget;
                this.DDLastTargetAnchor = targetAnchor;
                this.DDTargetPosition = position || TargetPositionEnum.on;
                var pos = "";
                switch (position) {
                    default:
                    case TargetPositionEnum.on:
                        pos = "on";
                        break;
                    case TargetPositionEnum.before:
                        pos = "before";
                        break;
                    case TargetPositionEnum.after:
                        pos = "after";
                        break;
                }
                this.DDLastTargetAnchor.insertAdjacentHTML("afterbegin", "<div class=\"t_ddpos t_" + pos + "\"></div>");
            }
        };
        TreeComponent.prototype.drop = function (ev) {
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
                            var liElem = parentUl.parentElement;
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
                            parentUl = liElem.parentElement;
                        }
                    }
                    else
                        break;
                }
                this.sendDropEvent();
            }
            this.setDragDropTarget(null);
            this.DDSource = null;
            this.DDSourceAnchor = null;
        };
        TreeComponent.prototype.dragEnd = function (ev) {
            this.setDragDropTarget(null);
            this.DDSource = null;
            this.DDSourceAnchor = null;
        };
        TreeComponent.onDragStart = function (ev) {
            TreeComponent.DDTree = YetaWF.ComponentBaseDataImpl.getControlFromTag(ev.target, TreeComponent.SELECTOR);
            TreeComponent.DDTree.dragStart(ev);
        };
        TreeComponent.onDragOver = function (ev) {
            if (TreeComponent.DDTree) {
                TreeComponent.DDTree.dragOver(ev);
            }
        };
        TreeComponent.onDrop = function (ev) {
            if (TreeComponent.DDTree) {
                TreeComponent.DDTree.drop(ev);
            }
            TreeComponent.DDTree = null;
        };
        TreeComponent.onDragEnd = function (ev) {
            if (TreeComponent.DDTree)
                TreeComponent.DDTree.dragEnd(ev);
            TreeComponent.DDTree = null;
        };
        // expand/collapse
        TreeComponent.prototype.expandItem = function (liElem) {
            if (!this.Setup.AjaxUrl)
                throw "Tree control doesn't have an AJAX URL - " + this.Control.outerHTML;
            if (!$YetaWF.isLoading) {
                $YetaWF.setLoading(true);
                // fetch data from servers
                var uri = $YetaWF.parseUrl(this.Setup.AjaxUrl);
                var recData = $YetaWF.getAttribute(liElem, "data-record");
                if (recData)
                    uri.addSearch("Data", recData);
                uri.addFormInfo(this.Control);
                var uniqueIdCounters = { UniqueIdPrefix: this.ControlId + "ls", UniqueIdPrefixCounter: ++this.AddCounter, UniqueIdCounter: 0 };
                uri.addSearch(YConfigs.Forms.UniqueIdCounters, JSON.stringify(uniqueIdCounters));
                var request = new XMLHttpRequest();
                request.open("POST", this.Setup.AjaxUrl);
                request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                request.onreadystatechange = function (ev) {
                    if (request.readyState === 4 /*DONE*/) {
                        $YetaWF.setLoading(false);
                        $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, undefined, undefined, function (result) {
                            var partial = JSON.parse(request.responseText);
                            var iElem = $YetaWF.getElement1BySelector("i.t_icright", [liElem]);
                            $YetaWF.elementRemoveClasses(iElem, ["t_icright", "t_icdown", "t_icempty"]);
                            if (partial.Records > 0) {
                                // add new items
                                $YetaWF.appendMixedHTML(liElem, partial.HTML);
                                // mark expanded
                                $YetaWF.elementAddClass(iElem, "t_icdown");
                            }
                            else {
                                // mark not expandable
                                $YetaWF.elementAddClass(iElem, "t_icempty");
                            }
                        });
                    }
                };
                var data = uri.toFormData();
                request.send(data);
            }
        };
        // API
        TreeComponent.prototype.canCollapse = function (liElem) {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]); // get the subitems
            if (!ul || ul.style.display !== "")
                return false;
            return true;
        };
        TreeComponent.prototype.collapse = function (liElem) {
            var ul = $YetaWF.getElement1BySelector("ul.t_sub", [liElem]); // get the subitems
            if (ul) {
                var data = this.getElementDataCond(liElem);
                if (data && data.DynamicSubEntries) {
                    $YetaWF.processClearDiv(ul);
                    ul.remove();
                }
                else {
                    ul.style.display = "none";
                }
            }
            var iElem = $YetaWF.getElement1BySelector("i.t_icdown", [liElem]);
            $YetaWF.elementRemoveClass(iElem, "t_icdown");
            $YetaWF.elementAddClass(iElem, "t_icright");
        };
        TreeComponent.prototype.canExpand = function (liElem) {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]); // get the subitems
            if (ul) {
                if (ul.style.display === "")
                    return false;
                return true;
            }
            else {
                var data = this.getElementDataCond(liElem);
                return data != null && data.DynamicSubEntries;
            }
        };
        TreeComponent.prototype.expand = function (liElem) {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]); // get the subitems
            if (ul == null) {
                var data = this.getElementData(liElem);
                if (data.DynamicSubEntries) {
                    this.expandItem(liElem);
                }
            }
            else {
                ul.style.display = "";
                var iElem = liElem.firstElementChild;
                if (iElem) {
                    $YetaWF.elementRemoveClass(iElem, "t_icright");
                    $YetaWF.elementAddClass(iElem, "t_icdown");
                }
            }
        };
        TreeComponent.prototype.expandAll = function () {
            var li = this.getFirstVisibleItem();
            while (li) {
                if (this.canExpand(li))
                    this.expand(li);
                li = this.getNextVisibleEntry(li);
            }
        };
        TreeComponent.prototype.collapseAll = function () {
            var li = this.getFirstVisibleItem();
            while (li) {
                if (this.canCollapse(li))
                    this.collapse(li);
                li = this.getNextEntry(li);
            }
        };
        TreeComponent.prototype.makeVisible = function (liElem) {
            var li = liElem;
            for (; li;) {
                li = this.getParent(li);
                if (!li)
                    return;
                this.expand(li);
            }
        };
        TreeComponent.prototype.getParent = function (liElem) {
            var ul = liElem.parentElement;
            if ($YetaWF.elementHasClass(ul, "tg_root"))
                return null;
            return $YetaWF.elementClosest(ul, "li");
        };
        TreeComponent.prototype.getElementDataCond = function (liElem) {
            var recData = $YetaWF.getAttributeCond(liElem, "data-record");
            if (!recData)
                return null;
            return JSON.parse(recData);
        };
        TreeComponent.prototype.getElementData = function (liElem) {
            var data = this.getElementDataCond(liElem);
            if (!data)
                throw "No record data for " + liElem.outerHTML;
            return data;
        };
        TreeComponent.prototype.setElementData = function (liElem, data) {
            $YetaWF.setAttribute(liElem, "data-record", JSON.stringify(data));
        };
        TreeComponent.prototype.getSelect = function () {
            var entry = $YetaWF.getElement1BySelectorCond(".t_entry." + this.Setup.SelectedCss, [this.Control]);
            if (!entry)
                return null;
            var liElem = $YetaWF.elementClosest(entry, "li");
            return liElem;
        };
        TreeComponent.prototype.setSelect = function (liElem, focus) {
            this.clearSelect();
            $YetaWF.elementAddClass(liElem, "t_select");
            var entry = $YetaWF.getElement1BySelector(".t_entry", [liElem]);
            $YetaWF.elementAddClass(entry, this.Setup.SelectedCss);
            if (focus === true)
                entry.focus();
        };
        TreeComponent.prototype.getSelectData = function () {
            var liElem = this.getSelect();
            if (!liElem)
                return null;
            return this.getElementDataCond(liElem);
        };
        TreeComponent.prototype.setSelectData = function (data) {
            var liElem = this.getSelect();
            if (!liElem)
                return;
            this.setElementData(liElem, data);
        };
        TreeComponent.prototype.getSelectText = function () {
            var entry = $YetaWF.getElement1BySelector(".t_entry." + this.Setup.SelectedCss, [this.Control]);
            return entry.innerText;
        };
        TreeComponent.prototype.setSelectText = function (text) {
            var entry = $YetaWF.getElement1BySelector(".t_entry." + this.Setup.SelectedCss, [this.Control]);
            entry.innerText = text;
        };
        TreeComponent.prototype.clearSelect = function () {
            var entries = $YetaWF.getElementsBySelector("li.t_select", [this.Control]);
            for (var _i = 0, entries_1 = entries; _i < entries_1.length; _i++) {
                var entry = entries_1[_i];
                $YetaWF.elementRemoveClass(entry, "t_select");
            }
            entries = $YetaWF.getElementsBySelector(".t_entry." + this.Setup.SelectedCss, [this.Control]);
            for (var _a = 0, entries_2 = entries; _a < entries_2.length; _a++) {
                var entry = entries_2[_a];
                $YetaWF.elementRemoveClass(entry, this.Setup.SelectedCss);
            }
        };
        TreeComponent.prototype.removeEntry = function (liElem) {
            // remove li element (and possibly parent(s))
            var ul = $YetaWF.elementClosest(liElem, "ul.t_sub");
            $YetaWF.processClearDiv(liElem);
            liElem.remove();
            for (;;) {
                if (!ul)
                    break;
                if (ul.children.length > 0)
                    break;
                if ($YetaWF.elementHasClass(ul, "tg_root"))
                    break;
                var parentUl = $YetaWF.elementClosest(ul.parentElement, "ul.t_sub");
                $YetaWF.processClearDiv(ul);
                ul.remove();
                ul = parentUl;
            }
        };
        TreeComponent.prototype.getNextSibling = function (liElem) {
            var li = liElem.nextElementSibling;
            return li;
        };
        TreeComponent.prototype.getPrevSibling = function (liElem) {
            var li = liElem.previousElementSibling;
            return li;
        };
        TreeComponent.prototype.getFirstDirectChild = function (liElem) {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]);
            if (ul)
                return ul.children[0];
            return null;
        };
        TreeComponent.prototype.getLastDirectChild = function (liElem) {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]);
            if (ul)
                return ul.children[ul.children.length - 1];
            return null;
        };
        TreeComponent.prototype.getNextEntry = function (liElem) {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]);
            // get item in subentries
            if (ul)
                return ul.children[0];
            // no subentries, try next sibling (work your way up the tree)
            for (;;) {
                var li = liElem.nextElementSibling;
                if (li)
                    return li;
                // no next sibling, go to ul and parent il
                ul = liElem.parentElement;
                if ($YetaWF.elementHasClass(ul, "tg_root"))
                    return null;
                li = $YetaWF.elementClosest(ul, "li");
                if (!li)
                    return null;
                liElem = li;
            }
        };
        TreeComponent.prototype.getNextVisibleEntry = function (liElem) {
            var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]);
            // get item in subentries
            if (ul && ul.style.display === "")
                return ul.children[0];
            // no subentries, try next sibling (work your way up the tree)
            for (;;) {
                var li = liElem.nextElementSibling;
                if (li)
                    return li;
                // no next sibling, go to ul and parent il
                ul = liElem.parentElement;
                if ($YetaWF.elementHasClass(ul, "tg_root"))
                    return null;
                li = $YetaWF.elementClosest(ul, "li");
                if (!li)
                    return null;
                liElem = li;
            }
        };
        TreeComponent.prototype.getPrevEntry = function (liElem) {
            var li = liElem.previousElementSibling;
            if (li) {
                var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [li]);
                if (ul)
                    return ul.children[ul.children.length - 1];
                return li;
            }
            ul = liElem.parentElement;
            if ($YetaWF.elementHasClass(ul, "tg_root"))
                return null;
            li = $YetaWF.elementClosest(ul, "li");
            if (!li)
                return null;
            return li;
        };
        TreeComponent.prototype.getPrevVisibleEntry = function (liElem) {
            var li = liElem.previousElementSibling;
            if (li) {
                for (;;) {
                    var ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [li]);
                    if (!ul || ul.style.display !== "")
                        return li;
                    li = ul.children[ul.children.length - 1];
                }
            }
            ul = liElem.parentElement;
            if ($YetaWF.elementHasClass(ul, "tg_root"))
                return null;
            li = $YetaWF.elementClosest(ul, "li");
            if (!li)
                return null;
            return li;
        };
        TreeComponent.prototype.getFirstVisibleItem = function () {
            var liElem = $YetaWF.getElement1BySelectorCond("ul.t_sub li", [this.Control]);
            return liElem;
        };
        TreeComponent.prototype.getLastVisibleItem = function () {
            var ul = $YetaWF.getElement1BySelector("ul.tg_root", [this.Control]);
            var liElem = null;
            for (;;) {
                if (!ul || ul.style.display !== "")
                    break;
                // get last item in subentries
                liElem = ul.children[ul.children.length - 1];
                ul = $YetaWF.getElement1BySelectorCond("ul.t_sub", [liElem]);
            }
            return liElem;
        };
        TreeComponent.prototype.addEntry = function (liElem, text, data) {
            var text = $YetaWF.htmlEscape(text);
            var entry = this.getNewEntry(text);
            liElem.insertAdjacentHTML("afterend", entry);
            var newElem = this.getNextSibling(liElem);
            if (data)
                this.setElementData(newElem, data);
            return newElem;
        };
        TreeComponent.prototype.insertEntry = function (liElem, text, data) {
            var text = $YetaWF.htmlEscape(text);
            var entry = this.getNewEntry(text);
            liElem.insertAdjacentHTML("beforebegin", entry);
            var newElem = this.getPrevSibling(liElem);
            if (data)
                this.setElementData(newElem, data);
            return newElem;
        };
        TreeComponent.prototype.getNewEntry = function (text) {
            var dd = "";
            if (this.Setup.DragDrop)
                dd = " draggable='true' ondrop='YetaWF_ComponentsHTML.TreeComponent.onDrop(event)' ondragover='YetaWF_ComponentsHTML.TreeComponent.onDragOver(event)' ondragstart='YetaWF_ComponentsHTML.TreeComponent.onDragStart(event)'";
            var entry = "<li><i class=\"t_icempty\"></i> <i class=\"t_icfile\"></i><a class=\"t_entry\" href=\"#\"" + dd + ">" + text + "</a></li>";
            return entry;
        };
        /** Scroll the selected item into the viewable area */
        TreeComponent.prototype.scrollIntoView = function (container) {
            var liElem = this.getSelect();
            if (!liElem)
                return;
            var rectLi = liElem.getBoundingClientRect();
            var rectContainer = container.getBoundingClientRect();
            var t = container.scrollTop + (rectLi.top - rectContainer.height / 2);
            if (t < 0)
                t = 0;
            container.scrollTop = t;
        };
        TreeComponent.TEMPLATE = "yt_tree";
        TreeComponent.SELECTOR = ".yt_tree";
        TreeComponent.DDTree = null;
        return TreeComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.TreeComponent = TreeComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Tree.js.map
