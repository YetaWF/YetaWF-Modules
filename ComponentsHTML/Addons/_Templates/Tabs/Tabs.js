"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var TabsComponent = /** @class */ (function (_super) {
        __extends(TabsComponent, _super);
        function TabsComponent(controlId, setup) {
            var _this = _super.call(this, controlId, TabsComponent.TEMPLATE, TabsComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: TabsComponent.EVENTSWITCHED,
                GetValue: function (control) {
                    return control.activeTab.toString();
                },
                Enable: function (control, enable, clearOnDisable) { },
            }) || this;
            _this.Setup = setup;
            _this.ActiveTabHidden = $YetaWF.getElementById(_this.Setup.ActiveTabHiddenId);
            $YetaWF.registerEventHandler(_this.Control, "click", "#" + _this.ControlId + " > ul.t_tabstrip > li.t_tab", function (ev) {
                var li = ev.__YetaWFElem;
                var index = Number($YetaWF.getAttribute(li, "data-tab"));
                _this.activatePane(index);
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "keydown", "#" + _this.ControlId + " > ul.t_tabstrip > li", function (ev) {
                var index = _this.activeTab;
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
                    index = _this.tabCount - 1;
                }
                else
                    return true;
                if (index >= 0 && index < _this.tabCount) {
                    _this.activatePane(index);
                    return false;
                }
                return true;
            });
            if (_this.Setup.ContextMenu) {
                $YetaWF.registerEventHandler(_this.Control, "contextmenu", "#" + _this.ControlId + " > ul.t_tabstrip > li", function (ev) {
                    var li = ev.__YetaWFElem;
                    var index = Number($YetaWF.getAttribute(li, "data-tab"));
                    _this.activatePane(index);
                    ev.preventDefault();
                    $YetaWF.sendCustomEvent(_this.Control, TabsComponent.EVENTCONTEXTMENU);
                    return false;
                });
            }
            return _this;
        }
        // API
        /* Activate the pane that contains the specified element. The element does not need to be present. */
        TabsComponent.prototype.activatePaneByTag = function (tag) {
            if (!$YetaWF.elementHas(this.Control, tag))
                return;
            var ttabpanel = $YetaWF.elementClosestCond(tag, "div.t_tabpanel");
            if (!ttabpanel)
                return;
            var index = ttabpanel.getAttribute("data-tab");
            if (index == null)
                throw "We found a panel in a tab control without panel number (data-tab attribute).";
            this.activatePane(index);
        };
        /* Activate the pane by 0-based index. */
        TabsComponent.prototype.activatePane = function (index) {
            var tabCount = this.tabCount;
            if (tabCount === 0)
                throw "No panes found";
            if (index < 0 || index >= tabCount)
                throw "tab pane " + index + " requested - " + tabCount + " tabs present";
            var tabs = this.tabs;
            var panels = $YetaWF.getElementsBySelector("#" + this.ControlId + " > .t_tabpanel");
            if (panels.length !== tabCount)
                throw "Mismatched number of tabs (" + tabCount + ") and panels (" + panels.length + ") ";
            var activeTab = tabs[index];
            var activePanel = panels[index];
            for (var _i = 0, tabs_1 = tabs; _i < tabs_1.length; _i++) {
                var tab = tabs_1[_i];
                $YetaWF.elementRemoveClass(tab, "t_tabactive");
                $YetaWF.setAttribute(tab, "tabindex", "-1");
                $YetaWF.setAttribute(tab, "aria-selected", "false");
                $YetaWF.setAttribute(tab, "aria-expanded", "false");
            }
            $YetaWF.elementAddClass(activeTab, "t_tabactive");
            $YetaWF.setAttribute(activeTab, "tabindex", "0");
            $YetaWF.setAttribute(activeTab, "aria-selected", "true");
            $YetaWF.setAttribute(activeTab, "aria-expanded", "true");
            for (var _a = 0, panels_1 = panels; _a < panels_1.length; _a++) {
                var panel = panels_1[_a];
                $YetaWF.setAttribute(panel, "aria-hidden", "true");
                panel.style.display = "none";
            }
            $YetaWF.setAttribute(activePanel, "aria-hidden", "false");
            activePanel.style.display = "";
            this.ActiveTabHidden.value = index.toString();
            activeTab.focus();
            $YetaWF.sendActivateDivEvent([activePanel]);
            $YetaWF.sendPanelSwitchedEvent(activePanel);
            $YetaWF.sendCustomEvent(this.Control, TabsComponent.EVENTSWITCHED);
        };
        Object.defineProperty(TabsComponent.prototype, "activeTab", {
            get: function () {
                return Number(this.ActiveTabHidden.value);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TabsComponent.prototype, "tabCount", {
            get: function () {
                return this.tabs.length;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TabsComponent.prototype, "tabs", {
            get: function () {
                var tabs = $YetaWF.getElementsBySelector("#" + this.ControlId + " > ul.t_tabstrip > li");
                return tabs;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TabsComponent.prototype, "currentTab", {
            get: function () {
                var index = this.activeTab;
                var tabs = this.tabs;
                if (index < 0 || index >= tabs.length)
                    throw "tab index " + index + " invalid";
                return tabs[index];
            },
            enumerable: false,
            configurable: true
        });
        /**
         * Adds a tab/pane and returns the DIV that can be used to add contents.
         */
        TabsComponent.prototype.add = function (caption, tooltip) {
            var tabstrip = $YetaWF.getElement1BySelector("#" + this.ControlId + " > ul.t_tabstrip");
            var total = this.tabs.length;
            var tab = $YetaWF.createElement("li", { role: "tab", tabindex: "-1", class: "t_tab", "aria-selected": "false", "aria-expanded": "false" },
                $YetaWF.createElement("a", { "data-tooltip": tooltip, role: "presentation", tabindex: "-1", class: "t_tabanchor" }, caption));
            tabstrip.appendChild(tab);
            var pane = $YetaWF.createElement("div", { class: "t_proptable t_cat t_tabpanel", role: "tabpanel", style: "display:none", "aria-hidden": "true" },
                $YetaWF.createElement("div", { class: "t_contents t_loading" }, YLocs.YetaWF_ComponentsHTML.Loading));
            this.Control.appendChild(pane);
            this.resequenceTabs();
            this.activatePane(total);
            return $YetaWF.getElement1BySelector(".t_contents", [pane]);
        };
        TabsComponent.prototype.remove = function (index) {
            var tabs = this.tabs;
            if (index < 0 || index >= tabs.length)
                throw "tab index " + index + " invalid";
            $YetaWF.processClearDiv(tabs[index]);
            tabs[index].remove();
            var panes = $YetaWF.getElementsBySelector("#" + this.ControlId + " > div.t_tabpanel");
            $YetaWF.processClearDiv(panes[index]);
            panes[index].remove();
            this.resequenceTabs();
            // find a tab to activate
            tabs = this.tabs;
            if (index >= tabs.length)
                --index;
            if (index >= 0 && index < tabs.length)
                this.activatePane(index);
        };
        TabsComponent.prototype.resequenceTabs = function () {
            var count = 0;
            for (var _i = 0, _a = this.tabs; _i < _a.length; _i++) {
                var tab = _a[_i];
                var tabId = this.ControlId + "_tab" + count;
                var tabIdLb = tabId + "_lb";
                $YetaWF.setAttribute(tab, "data-tab", count.toString());
                $YetaWF.setAttribute(tab, "aria-controls", tabId);
                $YetaWF.setAttribute(tab, "aria-labelledby", tabIdLb);
                var anchor = $YetaWF.getElement1BySelector("a", [tab]);
                anchor.href = "#" + tabId;
                anchor.id = "#" + tabIdLb;
                ++count;
            }
            count = 0;
            var panes = $YetaWF.getElementsBySelector("#" + this.ControlId + " > div.t_tabpanel");
            for (var _b = 0, panes_1 = panes; _b < panes_1.length; _b++) {
                var pane = panes_1[_b];
                var tabId = this.ControlId + "_tab" + count;
                var tabIdLb = tabId + "_lb";
                $YetaWF.setAttribute(pane, "data-tab", count.toString());
                pane.id = tabId;
                $YetaWF.setAttribute(pane, "aria-labelledby", tabIdLb);
                var loadingDiv = $YetaWF.getElement1BySelectorCond(".t_contents.t_loading", [pane]);
                if (loadingDiv)
                    loadingDiv.id = tabId + "_content";
                ++count;
            }
        };
        TabsComponent.TEMPLATE = "yt_tabs";
        TabsComponent.SELECTOR = ".yt_tabs";
        TabsComponent.EVENTSWITCHED = "tabs_switched";
        TabsComponent.EVENTCONTEXTMENU = "tabs_contextmenu";
        return TabsComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.TabsComponent = TabsComponent;
    // The property list needs a bit of special love when it's made visible. Because panels have no width/height
    // while the propertylist is not visible (jquery implementation), when a propertylist is made visible using show(),
    // the default panel is not sized correctly. If you explicitly show() a propertylist that has never been visible,
    // call the following to cause the propertylist to be resized correctly:
    // ComponentsHTML.processPropertyListVisible(div);
    // div is any HTML element - all items (including child items) are checked for propertylists.
    ComponentsHTMLHelper.registerPropertyListVisible(function (tag) {
        var tabsTags = $YetaWF.getElementsBySelector(TabsComponent.SELECTOR, [tag]);
        for (var _i = 0, tabsTags_1 = tabsTags; _i < tabsTags_1.length; _i++) {
            var tabTag = tabsTags_1[_i];
            var tab = YetaWF.ComponentBaseDataImpl.getControlFromTag(tabTag, TabsComponent.SELECTOR);
            var index = tab.activeTab;
            if (index >= 0) {
                var panel = $YetaWF.getElement1BySelector("#" + tab.ControlId + "_tab" + index, [tab.Control]);
                $YetaWF.sendActivateDivEvent([panel]);
                $YetaWF.sendPanelSwitchedEvent(panel);
            }
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Tabs.js.map
