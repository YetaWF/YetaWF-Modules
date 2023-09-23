"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */
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
var YetaWF_BootstrapCarousel;
(function (YetaWF_BootstrapCarousel) {
    var CarouselAction;
    (function (CarouselAction) {
        CarouselAction[CarouselAction["Apply"] = 0] = "Apply";
        CarouselAction[CarouselAction["MoveLeft"] = 1] = "MoveLeft";
        CarouselAction[CarouselAction["MoveRight"] = 2] = "MoveRight";
        CarouselAction[CarouselAction["Add"] = 3] = "Add";
        CarouselAction[CarouselAction["Insert"] = 4] = "Insert";
        CarouselAction[CarouselAction["Remove"] = 5] = "Remove";
    })(CarouselAction || (CarouselAction = {}));
    var SlideShowEdit = /** @class */ (function (_super) {
        __extends(SlideShowEdit, _super);
        function SlideShowEdit(controlId) {
            var _this = _super.call(this, controlId, SlideShowEdit.TEMPLATE, SlideShowEdit.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.Tabs = YetaWF_ComponentsHTML.TabsComponent.getControlFromSelector(".yt_tabs", YetaWF_ComponentsHTML.TabsComponent.SELECTOR, [_this.Control]);
            _this.ButtonUp = $YetaWF.getElement1BySelector("input.t_up", [_this.Control]);
            _this.ButtonDown = $YetaWF.getElement1BySelector("input.t_down", [_this.Control]);
            _this.ButtonDelete = $YetaWF.getElement1BySelector("input.t_delete", [_this.Control]);
            // Apply button click
            $YetaWF.registerEventHandler(_this.Control, "click", "input.t_apply", function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, CarouselAction.Apply, _this.getPanelIndex().toString());
                return false;
            });
            // << button click
            $YetaWF.registerEventHandler(_this.ButtonUp, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, CarouselAction.MoveLeft, _this.getPanelIndex().toString());
                return false;
            });
            // >> button click
            $YetaWF.registerEventHandler(_this.ButtonDown, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, CarouselAction.MoveRight, _this.getPanelIndex().toString());
                return false;
            });
            // delete button click
            $YetaWF.registerEventHandler(_this.ButtonDelete, "click", null, function (ev) {
                $YetaWF.alertYesNo(YLocs.YetaWF_BootstrapCarousel.RemoveConfirm, YLocs.YetaWF_BootstrapCarousel.RemoveTitle, function () {
                    $YetaWF.Forms.submitTemplate(_this.Control, false, SlideShowEdit.TEMPLATENAME, CarouselAction.Remove, _this.getPanelIndex().toString());
                });
                return false;
            });
            // Insert button click
            $YetaWF.registerEventHandler(_this.Control, "click", "input.t_ins", function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, CarouselAction.Insert, _this.getPanelIndex().toString());
                return false;
            });
            // Add button click
            $YetaWF.registerEventHandler(_this.Control, "click", "input.t_add", function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, CarouselAction.Add, _this.getPanelIndex().toString());
                return false;
            });
            _this.Tabs.Control.addEventListener(YetaWF_ComponentsHTML.TabsComponent.EVENTSWITCHED, function (evt) {
                _this.updateButtons();
            });
            _this.updateButtons();
            return _this;
        }
        SlideShowEdit.prototype.getActiveTab = function () {
            return $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [this.Control]);
        };
        SlideShowEdit.prototype.getPanelIndex = function () {
            var activeTab = this.getActiveTab();
            return Number(activeTab.value);
        };
        SlideShowEdit.prototype.getPanelCount = function () {
            var lis = $YetaWF.getElementsBySelector(".t_tabstrip li", [this.Control]);
            return lis.length;
        };
        SlideShowEdit.prototype.updateButtons = function () {
            var panelIndex = this.getPanelIndex();
            var panelCount = this.getPanelCount();
            // disable the << button if the active tab is the first one
            $YetaWF.elementEnableToggle(this.ButtonUp, panelIndex !== 0);
            // disable the >> button if the last panel is active
            $YetaWF.elementEnableToggle(this.ButtonDown, panelIndex < panelCount - 1);
            // disable if there is only one panel
            $YetaWF.elementEnableToggle(this.ButtonDelete, panelCount > 1);
        };
        SlideShowEdit.TEMPLATE = "yt_bootstrapcarousel_slideshow";
        SlideShowEdit.SELECTOR = ".yt_bootstrapcarousel_slideshow.t_edit";
        SlideShowEdit.TEMPLATENAME = "YetaWF_BootstrapCarousel_SlideShow";
        return SlideShowEdit;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_BootstrapCarousel.SlideShowEdit = SlideShowEdit;
})(YetaWF_BootstrapCarousel || (YetaWF_BootstrapCarousel = {}));

//# sourceMappingURL=SlideShowEdit.js.map
