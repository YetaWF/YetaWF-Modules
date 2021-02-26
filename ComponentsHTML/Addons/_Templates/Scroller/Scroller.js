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
    //interface Setup {
    //}
    var ScrollerComponent = /** @class */ (function (_super) {
        __extends(ScrollerComponent, _super);
        function ScrollerComponent(controlId /*, setup: Setup*/) {
            var _this = _super.call(this, controlId, ScrollerComponent.TEMPLATE, ScrollerComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.Panel = 0;
            //this.Setup = setup;
            _this.ElemLeft = $YetaWF.getElement1BySelector(".t_left", [_this.Control]);
            _this.ElemRight = $YetaWF.getElement1BySelector(".t_right", [_this.Control]);
            _this.Panels = $YetaWF.getElementsBySelector(".t_item", [_this.Control]).length;
            _this.DivItems = $YetaWF.getElement1BySelector(".t_items", [_this.Control]);
            _this.DivItems.style.left = "0px";
            _this.updateButtons();
            $YetaWF.registerEventHandler(_this.ElemLeft, "click", null, function (ev) {
                _this.scroll(-1);
                return false;
            });
            $YetaWF.registerEventHandler(_this.ElemRight, "click", null, function (ev) {
                _this.scroll(1);
                return false;
            });
            return _this;
        }
        // API
        ScrollerComponent.prototype.updateButtons = function () {
            this.ElemLeft.style.backgroundPosition = this.Panel === 0 ? "0px 0px" : "0px -48px";
            $YetaWF.elementEnableToggle(this.ElemLeft, this.Panel !== 0);
            var controlRect = this.Control.getBoundingClientRect();
            var width = controlRect.width;
            var itemRect = $YetaWF.getElement1BySelector(".t_item", [this.Control]).getBoundingClientRect();
            var itemwidth = itemRect.width;
            var skip = Math.floor(width / itemwidth);
            this.ElemRight.style.backgroundPosition = this.Panel + skip < this.Panels ? "-48px -48px" : "-48px 0px";
            $YetaWF.elementEnableToggle(this.ElemRight, this.Panel + skip < this.Panels);
        };
        ScrollerComponent.prototype.scroll = function (direction) {
            var controlRect = this.Control.getBoundingClientRect();
            var width = controlRect.width;
            var itemRect = $YetaWF.getElement1BySelector(".t_item", [this.Control]).getBoundingClientRect();
            var itemwidth = itemRect.width;
            var skip = Math.floor(width / itemwidth);
            if (skip < 1)
                skip = 1;
            this.Panel = this.Panel + skip * direction;
            //if (this.Panel >= this.Panels - skip) this.Panel %= this.Panels;
            //if (this.Panel < 0) this.Panel = this.Panels + this.Panel;
            if (this.Panel >= this.Panels)
                this.Panel = this.Panels - skip;
            if (this.Panel < 0)
                this.Panel = 0;
            this.updateButtons();
            var offs = this.Panel * itemwidth;
            this.DivItems.style.transition = "all 250ms";
            this.DivItems.style.left = -offs + "px";
            //$('.t_items', this.Control).animate({
            //    left: -offs,
            //}, 250, function () { });
        };
        ScrollerComponent.TEMPLATE = "yt_scroller";
        ScrollerComponent.SELECTOR = ".yt_scroller.t_display";
        return ScrollerComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.ScrollerComponent = ScrollerComponent;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        var scrolls = YetaWF.ComponentBaseDataImpl.getControls(ScrollerComponent.SELECTOR, [ev.detail.container]);
        for (var _i = 0, scrolls_1 = scrolls; _i < scrolls_1.length; _i++) {
            var scroll_1 = scrolls_1[_i];
            scroll_1.updateButtons();
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, function (ev) {
        var scrolls = YetaWF.ComponentBaseDataImpl.getControls(ScrollerComponent.SELECTOR, [ev.detail.container]);
        for (var _i = 0, scrolls_2 = scrolls; _i < scrolls_2.length; _i++) {
            var scroll_2 = scrolls_2[_i];
            scroll_2.updateButtons();
        }
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Scroller.js.map
