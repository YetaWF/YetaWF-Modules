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
// Kendo UI menu use
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
                    return control.activePane.toString();
                },
                Enable: function (control, enable, clearOnDisable) { },
            }, false, function (tag, control) {
                control.internalDestroy();
            }) || this;
            _this.ActiveTabHidden = null;
            _this.Setup = setup;
            if (_this.Setup.ActiveTabHiddenId)
                _this.ActiveTabHidden = $YetaWF.getElementById(_this.Setup.ActiveTabHiddenId);
            if (_this.Setup.TabStyle === TabStyleEnum.JQuery) {
                ComponentsHTMLHelper.MUSTHAVE_JQUERYUI();
                $(_this.Control).tabs({
                    active: _this.Setup.ActiveTabIndex,
                    activate: function (ev, ui) {
                        if (ui.newPanel !== undefined) {
                            $YetaWF.processActivateDivs([ui.newPanel[0]]);
                            $YetaWF.processPanelSwitched(ui.newPanel[0]);
                            var index = Number((ui.newTab.length > 0) ? (ui.newTab.attr("data-tab") || "-1") : "-1");
                            if (_this.ActiveTabHidden)
                                _this.ActiveTabHidden.value = index.toString();
                            _this.sendEvent();
                        }
                    }
                });
            }
            else if (_this.Setup.TabStyle === TabStyleEnum.Kendo) {
                ComponentsHTMLHelper.MUSTHAVE_KENDOUI();
                // mark the active tab with .k-state-active before initializing the tabstrip
                var tabs = $YetaWF.getElementsBySelector("#" + _this.ControlId + ">ul>li");
                for (var _i = 0, tabs_1 = tabs; _i < tabs_1.length; _i++) {
                    var tab = tabs_1[_i];
                    $YetaWF.elementRemoveClass(tab, "k-state-active");
                }
                $YetaWF.elementAddClass(tabs[_this.Setup.ActiveTabIndex], "k-state-active");
                // init tab control
                $(_this.Control).kendoTabStrip({
                    animation: false,
                    activate: function (ev) {
                        if (ev.contentElement !== undefined) {
                            $YetaWF.processActivateDivs([ev.contentElement]);
                            $YetaWF.processPanelSwitched(ev.contentElement);
                            if (_this.ActiveTabHidden)
                                _this.ActiveTabHidden.value = $(ev.item).attr("data-tab");
                            _this.sendEvent();
                        }
                    }
                }).data("kendoTabStrip");
            }
            else
                throw "Invalid tab style " + _this.Setup.TabStyle;
            return _this;
        }
        TabsComponent.prototype.sendEvent = function () {
            $YetaWF.sendCustomEvent(this.Control, TabsComponent.EVENTSWITCHED);
        };
        TabsComponent.prototype.internalDestroy = function () {
            if (this.Setup.TabStyle === TabStyleEnum.JQuery) {
                $(this.Control).tabs("destroy");
            }
            else if (this.Setup.TabStyle === TabStyleEnum.Kendo) {
                var tab = $(this.Control).data("kendoTabStrip");
                if (!tab)
                    throw "No kendo object found";
                tab.destroy();
            }
            else
                throw "Invalid tab style " + this.Setup.TabStyle;
        };
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
            var panels = $YetaWF.getElementsBySelector("ul.t_tabstrip > li", [this.Control]);
            if (panels.length === 0)
                throw "No panes found";
            if (index < 0 || index >= panels.length)
                throw "tab pane " + index + " requested - " + panels.length + " tabs present";
            if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery)
                $(this.Control).tabs("option", "active", index);
            else if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.Kendo) {
                $(this.Control).data("kendoTabStrip").activateTab($(panels[index]));
            }
            else
                throw "Unknown tab style " + YVolatile.Forms.TabStyle; /*DEBUG*/
        };
        Object.defineProperty(TabsComponent.prototype, "activePane", {
            get: function () {
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery)
                    return Number($(this.Control).tabs("option", "active"));
                else if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.Kendo) {
                    var ts = $(this.Control).data("kendoTabStrip");
                    return Number(ts.select().attr("data-tab"));
                }
                else
                    throw "Unknown tab style " + YVolatile.Forms.TabStyle; /*DEBUG*/
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
            var index = tab.activePane;
            if (index >= 0) {
                var panel = $YetaWF.getElement1BySelector("#" + tab.ControlId + "_tab" + index, [tab.Control]);
                $YetaWF.processActivateDivs([panel]);
                $YetaWF.processPanelSwitched(panel);
            }
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Tabs.js.map
