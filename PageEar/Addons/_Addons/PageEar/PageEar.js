"use strict";
/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEar#License */
/* eslint-disable @typescript-eslint/indent */
/*!
 * jQuery Peelback ported to TypeScript - Original by Rob Flaherty
 */
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
var YetaWF_PageEar;
(function (YetaWF_PageEar) {
    var PageEarModule = /** @class */ (function (_super) {
        __extends(PageEarModule, _super);
        function PageEarModule(id, setup) {
            var _this = _super.call(this, id, PageEarModule.SELECTOR, null, function (tag, module) {
                // when the page is removed, we need to clean up
                module.PeelDiv.remove();
            }) || this;
            _this.Interval = 0;
            _this.Increase = false;
            _this.Setup = setup;
            if (!_this.Setup.AdImage)
                throw "Ad image missing";
            if (!_this.Setup.PeelImage)
                throw "Peel effect image missing";
            if (!_this.Setup.ClickURL)
                throw "Click URL missing";
            _this.PeelDiv = $YetaWF.createElement("div", { id: "YetaWF_PageEar_Peelback" },
                $YetaWF.createElement("a", { href: _this.Setup.ClickURL, class: "yNoToolTip", target: "_blank", rel: "noopener noreferrer" },
                    $YetaWF.createElement("img", { class: "t_img", src: _this.Setup.PeelImage, alt: "", border: "0" })),
                $YetaWF.createElement("div", { class: "t_mask" }));
            _this.PeelImage = $YetaWF.getElement1BySelector(".t_img", [_this.PeelDiv]);
            _this.PeelMask = $YetaWF.getElement1BySelector(".t_mask", [_this.PeelDiv]);
            _this.PeelMask.style.backgroundImage = "url('".concat(_this.Setup.AdImage, "')");
            document.body.appendChild(_this.PeelDiv);
            _this.PeelImage.style.width = "".concat(_this.Setup.SmallSize, "px");
            _this.PeelImage.style.height = "".concat(_this.Setup.SmallSize, "px");
            _this.PeelMask.style.width = "".concat(_this.Setup.SmallSize, "px");
            _this.PeelMask.style.height = "".concat(_this.Setup.SmallSize, "px");
            _this.Increment = (_this.Setup.LargeSize - _this.Setup.SmallSize) / (PageEarModule.TIME / PageEarModule.STEPTIME);
            $YetaWF.registerEventHandler(_this.PeelDiv, "mouseover", null, function (ev) {
                _this.maximize();
                return true;
            });
            $YetaWF.registerEventHandler(_this.PeelDiv, "mouseout", null, function (ev) {
                _this.clearInterval();
                _this.minimize();
                return true;
            });
            return _this;
        }
        PageEarModule.prototype.clearInterval = function () {
            if (this.Interval)
                clearInterval(this.Interval);
            this.Interval = 0;
        };
        PageEarModule.prototype.maximize = function () {
            var _this = this;
            if (!this.Increase) {
                this.clearInterval();
                this.Increase = true;
                this.Interval = setInterval(function () { _this.DoSteps(); }, PageEarModule.STEPTIME);
            }
        };
        PageEarModule.prototype.minimize = function () {
            var _this = this;
            if (this.Increase) {
                this.clearInterval();
                this.Increase = false;
                this.Interval = setInterval(function () { _this.DoSteps(); }, PageEarModule.STEPTIME);
            }
        };
        PageEarModule.prototype.DoSteps = function () {
            var done = false;
            var width;
            if (this.Setup.AutoAnimate) {
                if (!this.Interval)
                    return;
                width = parseFloat(this.PeelImage.style.width);
                if (this.Increase) {
                    width += this.Increment;
                    if (width >= this.Setup.LargeSize) {
                        width = this.Setup.LargeSize;
                        done = true;
                    }
                }
                else {
                    width -= this.Increment;
                    if (width <= this.Setup.SmallSize) {
                        width = this.Setup.SmallSize;
                        done = true;
                    }
                }
            }
            else {
                if (this.Increase)
                    width = this.Setup.LargeSize;
                else
                    width = this.Setup.SmallSize;
                done = true;
            }
            this.PeelImage.style.width = "".concat(width, "px");
            this.PeelImage.style.height = "".concat(width, "px");
            this.PeelMask.style.width = "".concat(width, "px");
            this.PeelMask.style.height = "".concat(width, "px");
            if (done)
                this.clearInterval();
        };
        PageEarModule.SELECTOR = ".YetaWF_PageEar_PageEar";
        PageEarModule.TIME = 400; // time to grow/shrink
        PageEarModule.STEPTIME = 15; // time for 1 step
        return PageEarModule;
    }(YetaWF.ModuleBaseDataImpl));
    YetaWF_PageEar.PageEarModule = PageEarModule;
})(YetaWF_PageEar || (YetaWF_PageEar = {}));

//# sourceMappingURL=PageEar.js.map
