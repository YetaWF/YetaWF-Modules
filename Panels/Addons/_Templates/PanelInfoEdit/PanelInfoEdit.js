"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
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
    var PanelInfoEditComponent = /** @class */ (function (_super) {
        __extends(PanelInfoEditComponent, _super);
        function PanelInfoEditComponent(controlId) {
            var _this = _super.call(this, controlId, PanelInfoEditComponent.TEMPLATE, PanelInfoEditComponent.SELECTOR, {
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
                $YetaWF.Forms.submitTemplate(_this.Control, true, PanelInfoEditComponent.TEMPLATENAME, YetaWF.PanelAction.Apply, _this.getPanelIndex().toString());
                return false;
            });
            // << button click
            $YetaWF.registerEventHandler(_this.Up, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, PanelInfoEditComponent.TEMPLATENAME, YetaWF.PanelAction.MoveLeft, _this.getPanelIndex().toString());
                return false;
            });
            // >> button click
            $YetaWF.registerEventHandler(_this.Down, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, PanelInfoEditComponent.TEMPLATENAME, YetaWF.PanelAction.MoveRight, _this.getPanelIndex().toString());
                return false;
            });
            // delete button click
            $YetaWF.registerEventHandler(_this.Delete, "click", null, function (ev) {
                $YetaWF.alertYesNo(YLocs.YetaWF_Panels.RemovePanelConfirm, YLocs.YetaWF_Panels.RemovePanelTitle, function () {
                    $YetaWF.Forms.submitTemplate(_this.Control, false, PanelInfoEditComponent.TEMPLATENAME, YetaWF.PanelAction.Remove, _this.getPanelIndex().toString());
                });
                return false;
            });
            // Insert button click
            $YetaWF.registerEventHandler(_this.Insert, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, PanelInfoEditComponent.TEMPLATENAME, YetaWF.PanelAction.Insert, _this.getPanelIndex().toString());
                return false;
            });
            // Add button click
            $YetaWF.registerEventHandler(_this.Add, "click", null, function (ev) {
                $YetaWF.Forms.submitTemplate(_this.Control, true, PanelInfoEditComponent.TEMPLATENAME, YetaWF.PanelAction.Add, _this.getPanelIndex().toString());
                return false;
            });
            return _this;
        }
        PanelInfoEditComponent.prototype.getPanelIndex = function () {
            var tabActive = $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [this.Control]);
            return Number(tabActive.value);
        };
        PanelInfoEditComponent.prototype.getPanelCount = function () {
            var tabs = $YetaWF.getElementsBySelector(".t_tabstrip li", [this.Control]);
            return tabs.length;
        };
        PanelInfoEditComponent.prototype.updateButtons = function () {
            var panelIndex = this.getPanelIndex();
            $YetaWF.elementEnableToggle(this.Up, panelIndex !== 0);
            $YetaWF.elementEnableToggle(this.Down, panelIndex < this.getPanelCount() - 1);
            $YetaWF.elementEnableToggle(this.Delete, this.getPanelCount() > 1);
        };
        PanelInfoEditComponent.TEMPLATE = "yt_panels_panelinfo";
        PanelInfoEditComponent.SELECTOR = ".yt_panels_panelinfo.t_edit";
        PanelInfoEditComponent.TEMPLATENAME = "YetaWF_Panels_PanelInfo";
        return PanelInfoEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_Panels.PanelInfoEditComponent = PanelInfoEditComponent;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTPANELSWITCHED, null, function (ev) {
        var panel = ev.detail.panel;
        var panelInfo = YetaWF.ComponentBaseDataImpl.getControlFromTagCond(panel, PanelInfoEditComponent.SELECTOR);
        if (panelInfo) {
            var tabActive = $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [panelInfo.Control]);
            tabActive.value = $YetaWF.getAttribute(panel, "data-tab");
            panelInfo.updateButtons();
        }
        return true;
    });
})(YetaWF_Panels || (YetaWF_Panels = {}));

//# sourceMappingURL=PanelInfoEdit.js.map
