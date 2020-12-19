"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEar#License */
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
var YetaWF_PageEar;
(function (YetaWF_PageEar) {
    var PageEarModule = /** @class */ (function (_super) {
        __extends(PageEarModule, _super);
        function PageEarModule(id, setup) {
            var _this = _super.call(this, id, PageEarModule.SELECTOR, null, function (tag, module) {
                // when the page is removed, we need to clean up
                $("#peelback").remove();
            }) || this;
            _this.Setup = setup;
            var $$ = $;
            $$("body").peelback({
                adImage: _this.Setup.AdImage,
                peelImage: _this.Setup.PeelImage,
                clickURL: _this.Setup.ClickURL,
                smallSize: _this.Setup.SmallSize,
                bigSize: _this.Setup.LargeSize,
                autoAnimate: _this.Setup.AutoAnimate,
                //gaTrack: true, //RFFU
                //gaLabel: '#1 Stegosaurus',
                debug: false
            });
            return _this;
        }
        PageEarModule.SELECTOR = ".YetaWF_PageEar_PageEar";
        return PageEarModule;
    }(YetaWF.ModuleBaseDataImpl));
    YetaWF_PageEar.PageEarModule = PageEarModule;
})(YetaWF_PageEar || (YetaWF_PageEar = {}));

//# sourceMappingURL=PageEar.js.map
