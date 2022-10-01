"use strict";
/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */
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
var YetaWF_Panels;
(function (YetaWF_Panels) {
    var SplitterModule = /** @class */ (function (_super) {
        __extends(SplitterModule, _super);
        function SplitterModule(id) {
            var _this = _super.call(this, id, SplitterModule.SELECTOR, null) || this;
            $YetaWF.registerEventHandler(_this.LeftArea, "scroll", null, function (ev) {
                $YetaWF.sendContainerScrollEvent(_this.LeftArea);
                return true;
            });
            $YetaWF.registerEventHandler(_this.RightArea, "scroll", null, function (ev) {
                $YetaWF.sendContainerScrollEvent(_this.RightArea);
                return true;
            });
            return _this;
        }
        SplitterModule.GetSplitterFromTagCond = function (tag) {
            var mod = YetaWF_Panels.SplitterModule.GetSplitterModuleFromTagCond(tag);
            if (!mod)
                return null;
            var splitter = YetaWF_Panels.SplitterInfoComponent.getControlFromSelector(YetaWF_Panels.SplitterInfoComponent.SELECTOR, YetaWF_Panels.SplitterInfoComponent.SELECTOR, [mod.Module]);
            if (!splitter)
                throw "The Splitter module cannot be found";
            return splitter;
        };
        SplitterModule.GetSplitterFromTag = function (tag) {
            var mod = YetaWF_Panels.SplitterModule.GetSplitterModuleFromTagCond(tag);
            if (!mod)
                throw "The Splitter module cannot be found";
            var splitter = YetaWF_Panels.SplitterInfoComponent.getControlFromSelector(YetaWF_Panels.SplitterInfoComponent.SELECTOR, YetaWF_Panels.SplitterInfoComponent.SELECTOR, [mod.Module]);
            if (!splitter)
                throw "The Splitter module cannot be found";
            return splitter;
        };
        SplitterModule.GetSplitterModuleFromTagCond = function (tag) {
            var modDiv = $YetaWF.elementClosestCond(tag, SplitterModule.SELECTOR);
            if (!modDiv)
                return null;
            var mod = YetaWF_Panels.SplitterModule.getModuleFromTag(modDiv);
            if (!mod)
                throw "The Splitter module cannot be found";
            return mod;
        };
        SplitterModule.GetSplitterModuleFromTag = function (tag) {
            var mod = SplitterModule.GetSplitterModuleFromTagCond(tag);
            if (!mod)
                throw "The Splitter module cannot be found";
            return mod;
        };
        Object.defineProperty(SplitterModule.prototype, "LeftArea", {
            get: function () {
                return $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_left .t_area", [this.Module]);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(SplitterModule.prototype, "RightArea", {
            get: function () {
                return $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_right .t_area", [this.Module]);
            },
            enumerable: false,
            configurable: true
        });
        SplitterModule.SELECTOR = ".YetaWF_Panels_Splitter";
        return SplitterModule;
    }(YetaWF.ModuleBaseDataImpl));
    YetaWF_Panels.SplitterModule = SplitterModule;
})(YetaWF_Panels || (YetaWF_Panels = {}));

//# sourceMappingURL=SplitterModule.js.map
