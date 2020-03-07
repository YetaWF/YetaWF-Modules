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
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
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
                            if (_this.ActiveTabHidden)
                                _this.ActiveTabHidden.value = (ui.newTab.length > 0) ? (ui.newTab.attr("data-tab") || "-1") : "-1";
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
                        }
                    }
                }).data("kendoTabStrip");
            }
            else
                throw "Invalid tab style " + _this.Setup.TabStyle;
            return _this;
        }
        TabsComponent.TEMPLATE = "yt_tabs";
        TabsComponent.SELECTOR = ".yt_tabs";
        return TabsComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.TabsComponent = TabsComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
