"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_Panels;
(function (YetaWF_Panels) {
    var PageBarInfoComponent = /** @class */ (function (_super) {
        __extends(PageBarInfoComponent, _super);
        function PageBarInfoComponent(controlId, setup) {
            var _this = _super.call(this, controlId, PageBarInfoComponent.TEMPLATE, PageBarInfoComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.Setup = setup;
            _this.resize();
            // Link click, activate entry
            $YetaWF.registerEventHandler(_this.Control, "click", ".yt_panels_pagebarinfo_list a.t_entry", function (ev) {
                _this.activateEntry(ev.__YetaWFElem);
                return true; // allow link click to be processed
            });
            // keyboard
            $YetaWF.registerEventHandler(_this.Control, "keydown", ".yt_panels_pagebarinfo_list", function (ev) {
                var index = _this.activeEntry;
                if (index < 0)
                    index = 0;
                var key = ev.key;
                if (key === "ArrowDown" || key === "Down" || key === "ArrowRight" || key === "Right") {
                    ++index;
                }
                else if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                    --index;
                }
                else if (key === "Home") {
                    index = 0;
                }
                else if (key === "End") {
                    index = _this.count - 1;
                }
                else
                    return true;
                if (index >= 0 && index < _this.count) {
                    _this.activeEntry = index;
                    return false;
                }
                return true;
            });
            // expand/collapse
            $YetaWF.registerEventHandler(_this.Control, "click", ".t_expcoll", function (ev) {
                _this.toggleExpandCollapse();
                return false;
            });
            // scrolling page bar
            $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".yt_panels_pagebarinfo_list", [_this.Control]), "scroll", null, function (ev) {
                _this.repositionExpColl();
                return true;
            });
            // scrolling in panel area
            $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".t_area", [_this.Control]), "scroll", null, function (ev) {
                $YetaWF.sendContainerScrollEvent(_this.Control);
                return true;
            });
            return _this;
        }
        PageBarInfoComponent.prototype.activateEntry = function (tag) {
            var entry = $YetaWF.elementClosestCond(tag, ".yt_panels_pagebarinfo_list .t_entry");
            if (!entry)
                return;
            var entries = $YetaWF.getElementsBySelector(".yt_panels_pagebarinfo_list .t_entry", [this.Control]);
            for (var _i = 0, entries_1 = entries; _i < entries_1.length; _i++) {
                var e = entries_1[_i];
                $YetaWF.elementRemoveClassList(e, this.Setup.ActiveCss);
            }
            $YetaWF.elementAddClassList(entry, this.Setup.ActiveCss);
        };
        PageBarInfoComponent.prototype.sendExpandedCollapsed = function (expanded) {
            var uri = $YetaWF.parseUrl(this.Setup.ExpandCollapseUrl);
            var query = {
                Expanded: expanded,
            };
            $YetaWF.postJSONIgnore(uri, query, null);
        };
        PageBarInfoComponent.prototype.repositionExpColl = function () {
            var expColl = $YetaWF.getElement1BySelectorCond(".t_expcoll", [this.Control]);
            if (!expColl)
                return;
            var expRect = expColl.getBoundingClientRect();
            if (!expRect.width || !expRect.height)
                return;
            var list = $YetaWF.getElement1BySelector(".yt_panels_pagebarinfo_list", [this.Control]);
            var listRect = list.getBoundingClientRect();
            var top = list.offsetTop + (listRect.height - expRect.height) / 2;
            expColl.style.top = "".concat(top + list.scrollTop, "px");
        };
        Object.defineProperty(PageBarInfoComponent.prototype, "count", {
            get: function () {
                return this.entries.length;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(PageBarInfoComponent.prototype, "entries", {
            get: function () {
                return $YetaWF.getElementsBySelector(".yt_panels_pagebarinfo_list .t_entry", [this.Control]);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(PageBarInfoComponent.prototype, "activeEntry", {
            get: function () {
                var entries = this.entries;
                var active = $YetaWF.getElement1BySelectorCond(".yt_panels_pagebarinfo_list .t_entry.t_active", [this.Control]);
                if (!active)
                    return -1;
                var index = entries.indexOf(active);
                return index;
            },
            set: function (index) {
                var entries = this.entries;
                if (index < 0 || index >= entries.length)
                    throw "Panel index ".concat(index, " is invalid");
                var entry = entries[index];
                // and make panel active
                entry.focus();
                entry.click();
            },
            enumerable: false,
            configurable: true
        });
        PageBarInfoComponent.prototype.toggleExpandCollapse = function () {
            if ($YetaWF.elementHasClass(this.Control, "t_expanded")) {
                $YetaWF.elementRemoveClass(this.Control, "t_expanded");
                this.sendExpandedCollapsed(false);
            }
            else {
                $YetaWF.elementAddClass(this.Control, "t_expanded");
                this.sendExpandedCollapsed(true);
            }
        };
        PageBarInfoComponent.prototype.resize = function () {
            if (!this.Setup.Resize)
                return;
            // Resize the page bar in height so we fill the remaining page height
            // While this is possible in css also, it can't be done without knowing the structure of the page, which we can't assume in this page bar
            // so we just do it at load time (and when the window is resized).
            var winHeight = window.innerHeight;
            var docRect = document.body.getBoundingClientRect();
            var h = docRect.height - winHeight;
            var ctrlRect = this.Control.getBoundingClientRect();
            this.Control.style.height = "".concat(ctrlRect.height - h, "px");
            this.repositionExpColl();
        };
        PageBarInfoComponent.TEMPLATE = "yt_panels_pagebarinfo";
        PageBarInfoComponent.SELECTOR = ".yt_panels_pagebarinfo.t_display";
        return PageBarInfoComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_Panels.PageBarInfoComponent = PageBarInfoComponent;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        var ctrlDivs = $YetaWF.getElementsBySelector(PageBarInfoComponent.SELECTOR);
        for (var _i = 0, ctrlDivs_1 = ctrlDivs; _i < ctrlDivs_1.length; _i++) {
            var ctrlDiv = ctrlDivs_1[_i];
            if ($YetaWF.elementHas(ev.detail.container, ctrlDiv)) {
                var ctrl = PageBarInfoComponent.getControlFromTag(ctrlDiv, PageBarInfoComponent.SELECTOR);
                ctrl.resize();
            }
        }
        return true;
    });
})(YetaWF_Panels || (YetaWF_Panels = {}));

//# sourceMappingURL=PageBarInfo.js.map
