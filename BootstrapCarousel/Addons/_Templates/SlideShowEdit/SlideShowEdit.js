"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */
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
    var SlideShowEdit = /** @class */ (function (_super) {
        __extends(SlideShowEdit, _super);
        function SlideShowEdit(controlId) {
            var _this = _super.call(this, controlId, SlideShowEdit.TEMPLATE, SlideShowEdit.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.buttonUp = $YetaWF.getElement1BySelector("input.t_up", [_this.Control]);
            _this.buttonDown = $YetaWF.getElement1BySelector("input.t_down", [_this.Control]);
            _this.buttonDelete = $YetaWF.getElement1BySelector("input.t_delete", [_this.Control]);
            // Apply button click
            $YetaWF.registerEventHandler(_this.Control, "click", "input.t_apply", function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.Apply, _this.getPanelIndex().toString());
                return false;
            });
            // << button click
            $YetaWF.registerEventHandler(_this.buttonUp, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.MoveLeft, _this.getPanelIndex().toString());
                return false;
            });
            // >> button click
            $YetaWF.registerEventHandler(_this.buttonDown, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.MoveRight, _this.getPanelIndex().toString());
                return false;
            });
            // delete button click
            $YetaWF.registerEventHandler(_this.buttonDelete, "click", null, function (ev) {
                $YetaWF.alertYesNo(YLocs.YetaWF_BootstrapCarousel.RemoveConfirm, YLocs.YetaWF_BootstrapCarousel.RemoveTitle, function () {
                    $YetaWF.Forms.submitTemplate(_this.Control, false, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.Remove, _this.getPanelIndex().toString());
                });
                return false;
            });
            // Insert button click
            $YetaWF.registerEventHandler(_this.Control, "click", "input.t_ins", function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.Insert, _this.getPanelIndex().toString());
                return false;
            });
            // Add button click
            $YetaWF.registerEventHandler(_this.Control, "click", "input.t_add", function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.Add, _this.getPanelIndex().toString());
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
        SlideShowEdit.TEMPLATE = "yt_bootstrapcarousel_slideshow";
        SlideShowEdit.SELECTOR = ".yt_bootstrapcarousel_slideshow.t_edit";
        SlideShowEdit.TEMPLATENAME = "YetaWF_BootstrapCarousel_SlideShow";
        return SlideShowEdit;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_BootstrapCarousel.SlideShowEdit = SlideShowEdit;
})(YetaWF_BootstrapCarousel || (YetaWF_BootstrapCarousel = {}));

//# sourceMappingURL=SlideShowEdit.js.map
