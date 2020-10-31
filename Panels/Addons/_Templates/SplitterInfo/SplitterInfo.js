"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */
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
    var SplitterInfoComponent = /** @class */ (function (_super) {
        __extends(SplitterInfoComponent, _super);
        function SplitterInfoComponent(controlId, setup) {
            var _this = _super.call(this, controlId, SplitterInfoComponent.TEMPLATE, SplitterInfoComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.SMALLSCREEN = 1200;
            _this.Setup = setup;
            _this.Left = $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_left", [_this.Control]);
            _this.Collapse = $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_coll", [_this.Control]);
            _this.Expand = $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_exp", [_this.Control]);
            _this.Resize = $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_resize", [_this.Control]);
            _this.resized();
            // expand/collapse
            $YetaWF.registerEventHandler(_this.Collapse, "click", null, function (ev) {
                _this.toggleExpandCollapse();
                return false;
            });
            $YetaWF.registerEventHandler(_this.Expand, "click", null, function (ev) {
                _this.toggleExpandCollapse();
                return false;
            });
            $YetaWF.registerEventHandler(_this.Resize, "mousedown", null, function (ev) {
                document.body.style.cursor = "col-resize";
                SplitterInfoComponent.ResizeSplitter = _this;
                window.addEventListener("mousemove", SplitterInfoComponent.resizeWidth, false);
                window.addEventListener("mouseup", SplitterInfoComponent.resizeWidthDone, false);
                return false;
            });
            return _this;
        }
        SplitterInfoComponent.prototype.resized = function () {
            if (this.Setup.Height === 0) {
                if ($YetaWF.isPrinting) {
                    this.Control.style.height = "";
                }
                else {
                    // Resize in height so we fill the remaining page height
                    // While this is possible in css also, it can't be done without knowing the structure of the page, which we can't assume in this component
                    // so we just do it at load time (and when the window is resized).
                    var winHeight = window.innerHeight;
                    var winWidth = window.innerWidth;
                    if (winWidth >= this.SMALLSCREEN) {
                        var ctrlRect = this.Control.getBoundingClientRect();
                        var ctrlHeight = winHeight - ctrlRect.top;
                        if (ctrlHeight < 0)
                            ctrlHeight = 0;
                        this.Control.style.height = ctrlHeight + "px";
                        this.Left.style.flexBasis = this.Setup.Width + "%";
                    }
                    else {
                        this.Control.style.height = "100%";
                        if ($YetaWF.elementHasClass(this.Control, "t_expanded"))
                            this.Left.style.flexBasis = "100%";
                    }
                }
            }
        };
        SplitterInfoComponent.prototype.toggleExpandCollapse = function () {
            if ($YetaWF.elementHasClass(this.Control, "t_expanded")) {
                $YetaWF.elementRemoveClass(this.Control, "t_expanded");
            }
            else {
                $YetaWF.elementAddClass(this.Control, "t_expanded");
            }
        };
        SplitterInfoComponent.prototype.collapseSmallScreen = function () {
            var winWidth = window.innerWidth;
            if (winWidth <= this.SMALLSCREEN)
                $YetaWF.elementRemoveClass(this.Control, "t_expanded");
        };
        SplitterInfoComponent.resizeWidth = function (ev) {
            var ctrl = SplitterInfoComponent.ResizeSplitter;
            if (!ctrl)
                return false;
            var rect = ctrl.Left.getBoundingClientRect();
            var newActualWidth = ev.clientX - rect.left;
            if (newActualWidth < ctrl.Setup.MinWidth)
                newActualWidth = ctrl.Setup.MinWidth;
            ctrl.Left.style.flexBasis = newActualWidth + "px";
            return false;
        };
        SplitterInfoComponent.resizeWidthDone = function (ev) {
            var ctrl = SplitterInfoComponent.ResizeSplitter;
            if (!ctrl)
                return false;
            document.body.style.cursor = "default";
            window.removeEventListener("mousemove", SplitterInfoComponent.resizeWidth, false);
            window.removeEventListener("mouseup", SplitterInfoComponent.resizeWidthDone, false);
            return false;
        };
        SplitterInfoComponent.TEMPLATE = "yt_panels_splitterinfo";
        SplitterInfoComponent.SELECTOR = ".yt_panels_splitterinfo.t_display";
        SplitterInfoComponent.TEMPLATENAME = "YetaWF_Panels_SplitterInfo";
        SplitterInfoComponent.ResizeSplitter = null;
        return SplitterInfoComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_Panels.SplitterInfoComponent = SplitterInfoComponent;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        var ctrlDivs = $YetaWF.getElementsBySelector(SplitterInfoComponent.SELECTOR);
        for (var _i = 0, ctrlDivs_1 = ctrlDivs; _i < ctrlDivs_1.length; _i++) {
            var ctrlDiv = ctrlDivs_1[_i];
            if ($YetaWF.elementHas(ev.detail.container, ctrlDiv)) {
                var ctrl = SplitterInfoComponent.getControlFromTag(ctrlDiv, SplitterInfoComponent.SELECTOR);
                ctrl.resized();
            }
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, function (ev) {
        var ctrlDivs = $YetaWF.getElementsBySelector(SplitterInfoComponent.SELECTOR);
        for (var _i = 0, ctrlDivs_2 = ctrlDivs; _i < ctrlDivs_2.length; _i++) {
            var ctrlDiv = ctrlDivs_2[_i];
            if ($YetaWF.elementHas(ev.detail.container, ctrlDiv)) {
                var ctrl = SplitterInfoComponent.getControlFromTag(ctrlDiv, SplitterInfoComponent.SELECTOR);
                ctrl.resized();
            }
        }
        return true;
    });
})(YetaWF_Panels || (YetaWF_Panels = {}));

//# sourceMappingURL=SplitterInfo.js.map
