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
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_BootstrapCarousel;
(function (YetaWF_BootstrapCarousel) {
    var CarouselComponent = /** @class */ (function (_super) {
        __extends(CarouselComponent, _super);
        function CarouselComponent(controlId, setup) {
            var _this = _super.call(this, controlId, CarouselComponent.TEMPLATE, CarouselComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.currentImage = 0;
            _this.Setup = setup;
            _this.List = $YetaWF.getElement1BySelector(".t_inner", [_this.Control]);
            _this.Images = $YetaWF.getElementsBySelector(".t_item", [_this.List]);
            $YetaWF.registerEventHandler(_this.Control, "click", ".t_prev", function (ev) {
                _this.scroll(false);
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "click", ".t_next", function (ev) {
                _this.scroll(true);
                return false;
            });
            if (_this.Setup.Keyboard) {
                $YetaWF.registerEventHandler(_this.Control, "keydown", null, function (ev) {
                    var key = ev.key;
                    if (key === "ArrowRight" || key === "Right") {
                        _this.scroll(true);
                        return false;
                    }
                    else if (key === "ArrowLeft" || key === "Left") {
                        _this.scroll(false);
                        return false;
                    }
                    else if (key === "Home") {
                        _this.setImage(0);
                        return false;
                    }
                    else if (key === "End") {
                        _this.setImage(999999);
                        return false;
                    }
                    return true;
                });
            }
            $YetaWF.registerEventHandler(_this.Control, "mousedown", ".t_indicators li", function (ev) {
                var li = ev.__YetaWFElem;
                var inds = $YetaWF.getElementsBySelector(".t_indicators li");
                var index = inds.indexOf(li);
                _this.setImage(index);
                return true;
            });
            return _this;
        }
        CarouselComponent.prototype.updateIndicators = function () {
            var inds = $YetaWF.getElementsBySelector(".t_indicators li");
            for (var _i = 0, inds_1 = inds; _i < inds_1.length; _i++) {
                var ind = inds_1[_i];
                $YetaWF.elementRemoveClass(ind, "t_active");
            }
            $YetaWF.elementAddClass(inds[this.currentImage], "t_active");
        };
        CarouselComponent.prototype.startScroll = function (offset) {
            var _this = this;
            var incr = (offset - this.List.scrollLeft) / CarouselComponent.STEPS;
            if (incr === 0) {
                this.List.scrollLeft = offset;
                return;
            }
            var interval = setInterval(function () {
                var newOffs = _this.List.scrollLeft + incr;
                if (incr > 0) {
                    if (newOffs < offset) {
                        _this.List.scrollLeft = newOffs;
                        return;
                    }
                }
                else {
                    if (newOffs > offset) {
                        _this.List.scrollLeft = newOffs;
                        return;
                    }
                }
                _this.List.scrollLeft = offset;
                clearInterval(interval);
            }, CarouselComponent.SCROLLTIME / CarouselComponent.STEPS + 1);
        };
        // API
        CarouselComponent.prototype.scroll = function (next) {
            if (next)
                this.setImage(this.currentImage + 1);
            else
                this.setImage(this.currentImage - 1);
        };
        CarouselComponent.prototype.setImage = function (index) {
            var nextIndex = index;
            if (nextIndex >= this.Setup.ImageCount)
                nextIndex = 0;
            else if (nextIndex < 0)
                nextIndex = this.Setup.ImageCount - 1;
            var offset = this.Images[nextIndex].offsetLeft;
            this.startScroll(offset);
            this.currentImage = nextIndex;
            this.updateIndicators();
        };
        CarouselComponent.TEMPLATE = "yt_carousel";
        CarouselComponent.SELECTOR = ".yt_carousel.t_display";
        CarouselComponent.SCROLLTIME = 300;
        CarouselComponent.STEPS = 20;
        return CarouselComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_BootstrapCarousel.CarouselComponent = CarouselComponent;
})(YetaWF_BootstrapCarousel || (YetaWF_BootstrapCarousel = {}));

//# sourceMappingURL=BootstrapCarousel.js.map
