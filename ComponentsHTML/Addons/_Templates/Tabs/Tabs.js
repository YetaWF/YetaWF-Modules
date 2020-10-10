"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var TabStyleEnum;
    (function (TabStyleEnum) {
        TabStyleEnum[TabStyleEnum["JQuery"] = 0] = "JQuery";
        TabStyleEnum[TabStyleEnum["Kendo"] = 1] = "Kendo";
    })(TabStyleEnum = YetaWF_ComponentsHTML.TabStyleEnum || (YetaWF_ComponentsHTML.TabStyleEnum = {}));
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
            $YetaWF.registerEventHandler(_this.Control, "click", "#" + _this.ControlId + " > ul.t_tabstrip > li", function (ev) {
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
            $YetaWF.registerEventHandler(_this.Control, "focusin", null, function (ev) {
                var curentTab = _this.currentTab;
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery) {
                    for (var _i = 0, _a = _this.tabs; _i < _a.length; _i++) {
                        var tab = _a[_i];
                        $YetaWF.elementRemoveClass(tab, "ui-state-focus");
                    }
                    $YetaWF.elementAddClass(curentTab, "ui-state-focus");
                }
                else {
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "focusout", null, function (ev) {
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery) {
                    for (var _i = 0, _a = _this.tabs; _i < _a.length; _i++) {
                        var tab = _a[_i];
                        $YetaWF.elementRemoveClass(tab, "ui-state-focus");
                    }
                }
                else {
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "mousemove", "#" + _this.ControlId + " > ul.t_tabstrip > li", function (ev) {
                var curentTab = ev.__YetaWFElem;
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery) {
                    for (var _i = 0, _a = _this.tabs; _i < _a.length; _i++) {
                        var tab = _a[_i];
                        $YetaWF.elementRemoveClass(tab, "ui-state-hover");
                    }
                    $YetaWF.elementAddClass(curentTab, "ui-state-hover");
                }
                else {
                    for (var _b = 0, _c = _this.tabs; _b < _c.length; _b++) {
                        var tab = _c[_b];
                        $YetaWF.elementRemoveClass(tab, "k-state-hover");
                    }
                    $YetaWF.elementAddClass(curentTab, "k-state-hover");
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "mouseout", "#" + _this.ControlId + " > ul.t_tabstrip > li", function (ev) {
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery) {
                    for (var _i = 0, _a = _this.tabs; _i < _a.length; _i++) {
                        var tab = _a[_i];
                        $YetaWF.elementRemoveClass(tab, "ui-state-hover");
                    }
                }
                else {
                    for (var _b = 0, _c = _this.tabs; _b < _c.length; _b++) {
                        var tab = _c[_b];
                        $YetaWF.elementRemoveClass(tab, "k-state-hover");
                    }
                }
                return true;
            });
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
            if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery) {
                for (var _i = 0, tabs_1 = tabs; _i < tabs_1.length; _i++) {
                    var tab = tabs_1[_i];
                    $YetaWF.elementRemoveClasses(tab, ["ui-tabs-active", "ui-state-active"]);
                    $YetaWF.setAttribute(tab, "tabindex", "-1");
                    $YetaWF.setAttribute(tab, "aria-selected", "false");
                    $YetaWF.setAttribute(tab, "aria-expanded", "false");
                }
                $YetaWF.elementAddClasses(activeTab, ["ui-tabs-active", "ui-state-active"]);
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
            }
            else if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.Kendo) {
                for (var _b = 0, tabs_2 = tabs; _b < tabs_2.length; _b++) {
                    var tab = tabs_2[_b];
                    $YetaWF.elementRemoveClasses(tab, ["k-state-active", "k-tab-on-top"]);
                    $YetaWF.setAttribute(tab, "aria-selected", "false");
                    tab.removeAttribute("id");
                }
                $YetaWF.elementAddClasses(activeTab, ["k-state-active", "k-tab-on-top"]);
                $YetaWF.setAttribute(activeTab, "aria-selected", "true");
                var ariaId = this.Control.getAttribute("aria-activedescendant");
                activeTab.id = ariaId;
                for (var _c = 0, panels_2 = panels; _c < panels_2.length; _c++) {
                    var panel = panels_2[_c];
                    $YetaWF.elementRemoveClasses(panel, ["k-state-active"]);
                    $YetaWF.setAttribute(panel, "aria-hidden", "true");
                    $YetaWF.setAttribute(panel, "aria-expanded", "false");
                    panel.style.display = "none";
                }
                $YetaWF.elementAddClass(activePanel, "k-state-active");
                $YetaWF.setAttribute(activePanel, "aria-hidden", "false");
                $YetaWF.setAttribute(activePanel, "aria-expanded", "true");
                activePanel.style.display = "block";
            }
            else
                throw "Unknown tab style " + YVolatile.Forms.TabStyle; /*DEBUG*/
            this.ActiveTabHidden.value = index.toString();
            $YetaWF.processActivateDivs([activePanel]);
            $YetaWF.processPanelSwitched(activePanel);
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
        TabsComponent.TEMPLATE = "yt_tabs";
        TabsComponent.SELECTOR = ".yt_tabs";
        TabsComponent.EVENTSWITCHED = "tabs_switched";
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
                $YetaWF.processActivateDivs([panel]);
                $YetaWF.processPanelSwitched(panel);
            }
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Tabs.js.map
