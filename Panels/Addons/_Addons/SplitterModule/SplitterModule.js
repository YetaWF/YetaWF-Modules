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
    var SplitterModule = /** @class */ (function (_super) {
        __extends(SplitterModule, _super);
        function SplitterModule(id) {
            var _this = _super.call(this, id, SplitterModule.SELECTOR, null) || this;
            $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".yt_panels_splitterinfo_left .t_area", [_this.Module]), "scroll", null, function (ev) {
                $YetaWF.sendContainerScrollEvent(ev.__YetaWFElem);
                return true;
            });
            return _this;
        }
        SplitterModule.SELECTOR = ".YetaWF_Panels_Splitter";
        return SplitterModule;
    }(YetaWF.ModuleBaseDataImpl));
    YetaWF_Panels.SplitterModule = SplitterModule;
})(YetaWF_Panels || (YetaWF_Panels = {}));

//# sourceMappingURL=SplitterModule.js.map
