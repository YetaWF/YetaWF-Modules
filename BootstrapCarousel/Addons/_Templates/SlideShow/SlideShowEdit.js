"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */
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
var YetaWF_BootstrapCarousel;
(function (YetaWF_BootstrapCarousel) {
    var SlideShowEdit = /** @class */ (function (_super) {
        __extends(SlideShowEdit, _super);
        function SlideShowEdit(controlId) {
            var _this = _super.call(this, controlId) || this;
            _this.buttonUp = $YetaWF.getElement1BySelector("input.t_up", [_this.Control]);
            _this.buttonDown = $YetaWF.getElement1BySelector("input.t_down", [_this.Control]);
            _this.buttonDelete = $YetaWF.getElement1BySelector("input.t_delete", [_this.Control]);
            // Apply button click
            $YetaWF.registerEventHandler(_this.Control, "click", "input.t_apply", function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Apply, _this.getPanelIndex().toString());
                return false;
            });
            // << button click
            $YetaWF.registerEventHandler(_this.buttonUp, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_MoveLeft, _this.getPanelIndex().toString());
                return false;
            });
            // >> button click
            $YetaWF.registerEventHandler(_this.buttonDown, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_MoveRight, _this.getPanelIndex().toString());
                return false;
            });
            // delete button click
            $YetaWF.registerEventHandler(_this.buttonDelete, "click", null, function (ev) {
                $YetaWF.alertYesNo(YLocs.YetaWF_BootstrapCarousel.RemoveConfirm, YLocs.YetaWF_BootstrapCarousel.RemoveTitle, function () {
                    $YetaWF.Forms.submitTemplate(_this.Control, false, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Remove, _this.getPanelIndex().toString());
                });
                return false;
            });
            // Insert button click
            $YetaWF.registerEventHandler(_this.Control, "click", "input.t_ins", function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Insert, _this.getPanelIndex().toString());
                return false;
            });
            // Add button click
            $YetaWF.registerEventHandler(_this.Control, "click", "input.t_add", function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Add, _this.getPanelIndex().toString());
                return false;
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
            $YetaWF.elementEnableToggle(this.buttonUp, panelIndex !== 0);
            // disable the >> button if the last panel is active
            $YetaWF.elementEnableToggle(this.buttonDown, panelIndex < panelCount - 1);
            // disable if there is only one panel
            $YetaWF.elementEnableToggle(this.buttonDelete, panelCount > 1);
        };
        SlideShowEdit.prototype.updateActiveTab = function (panel) {
            // TODO:$$$ This needs to be moved into the tab control
            var activeTab = $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [this.Control]);
            activeTab.value = $YetaWF.getAttribute(panel, "data-tab");
            this.updateButtons();
        };
        SlideShowEdit.SELECTOR = ".yt_bootstrapcarousel_slideshow.t_edit";
        SlideShowEdit.TEMPLATENAME = "YetaWF_BootstrapCarousel_SlideShow";
        return SlideShowEdit;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_BootstrapCarousel.SlideShowEdit = SlideShowEdit;
    $YetaWF.registerPanelSwitched(function (panel) {
        var ctrl = YetaWF.ComponentBaseDataImpl.getControlFromTagCond(panel, SlideShowEdit.SELECTOR);
        if (ctrl != null)
            ctrl.updateActiveTab(panel);
    });
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBaseDataImpl.clearDiv(tag, SlideShowEdit.SELECTOR);
    });
})(YetaWF_BootstrapCarousel || (YetaWF_BootstrapCarousel = {}));

//# sourceMappingURL=SlideShowEdit.js.map
