"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */
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
var YetaWF_Panels;
(function (YetaWF_Panels) {
    var StepInfoEditComponent = /** @class */ (function (_super) {
        __extends(StepInfoEditComponent, _super);
        function StepInfoEditComponent(controlId) {
            var _this = _super.call(this, controlId, StepInfoEditComponent.TEMPLATE, StepInfoEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.Up = $YetaWF.getElement1BySelector("input.t_up", [_this.Control]);
            _this.Down = $YetaWF.getElement1BySelector("input.t_down", [_this.Control]);
            _this.Delete = $YetaWF.getElement1BySelector("input.t_delete", [_this.Control]);
            _this.Apply = $YetaWF.getElement1BySelector("input.t_apply", [_this.Control]);
            _this.Insert = $YetaWF.getElement1BySelector("input.t_ins", [_this.Control]);
            _this.Add = $YetaWF.getElement1BySelector("input.t_add", [_this.Control]);
            _this.updateButtons();
            // Apply button click
            $YetaWF.registerEventHandler(_this.Apply, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, StepInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Apply, _this.getPanelIndex().toString());
                return false;
            });
            // << button click
            $YetaWF.registerEventHandler(_this.Up, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, StepInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_MoveLeft, _this.getPanelIndex().toString());
                return false;
            });
            // >> button click
            $YetaWF.registerEventHandler(_this.Down, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, StepInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_MoveRight, _this.getPanelIndex().toString());
                return false;
            });
            // delete button click
            $YetaWF.registerEventHandler(_this.Delete, "click", null, function (ev) {
                $YetaWF.alertYesNo(YLocs.YetaWF_Panels.RemoveStepConfirm, YLocs.YetaWF_Panels.RemoveStepTitle, function () {
                    $YetaWF.Forms.submitTemplate(_this.Control, false, StepInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Remove, _this.getPanelIndex().toString());
                });
                return false;
            });
            // Insert button click
            $YetaWF.registerEventHandler(_this.Insert, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, StepInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Insert, _this.getPanelIndex().toString());
                return false;
            });
            // Add button click
            $YetaWF.registerEventHandler(_this.Add, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, StepInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Add, _this.getPanelIndex().toString());
                return false;
            });
            return _this;
        }
        StepInfoEditComponent.prototype.getPanelIndex = function () {
            var tabActive = $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [this.Control]);
            return Number(tabActive.value);
        };
        StepInfoEditComponent.prototype.getPanelCount = function () {
            var tabs = $YetaWF.getElementsBySelector(".t_tabstrip li", [this.Control]);
            return tabs.length;
        };
        StepInfoEditComponent.prototype.updateButtons = function () {
            var panelIndex = this.getPanelIndex();
            $YetaWF.elementEnableToggle(this.Up, panelIndex !== 0);
            $YetaWF.elementEnableToggle(this.Down, panelIndex < this.getPanelCount() - 1);
            $YetaWF.elementEnableToggle(this.Delete, this.getPanelCount() > 1);
        };
        StepInfoEditComponent.TEMPLATE = "yt_panels_stepinfo";
        StepInfoEditComponent.SELECTOR = ".yt_panels_stepinfo.t_edit";
        StepInfoEditComponent.TEMPLATENAME = "YetaWF_Panels_StepInfo";
        return StepInfoEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_Panels.StepInfoEditComponent = StepInfoEditComponent;
    $YetaWF.registerPanelSwitched(function (panel) {
        var panelInfo = YetaWF.ComponentBaseDataImpl.getControlFromTagCond(panel, StepInfoEditComponent.SELECTOR);
        if (!panelInfo)
            return;
        var tabActive = $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [panelInfo.Control]);
        tabActive.value = $YetaWF.getAttribute(panel, "data-tab");
        panelInfo.updateButtons();
    });
})(YetaWF_Panels || (YetaWF_Panels = {}));

//# sourceMappingURL=StepInfoEdit.js.map
