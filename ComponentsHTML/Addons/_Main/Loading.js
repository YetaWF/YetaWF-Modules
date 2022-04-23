"use strict";
/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    // Loading indicator
    var LoadingClass = /** @class */ (function () {
        function LoadingClass() {
            var _this = this;
            this.LoadingDiv = null;
            this.On = false;
            this.CursorX = 0;
            this.CursorY = 0;
            this.Width = 0;
            this.Height = 0;
            $YetaWF.registerEventHandlerBody("mousemove", null, function (ev) {
                _this.CursorX = ev.clientX;
                _this.CursorY = ev.clientY;
                if (_this.On)
                    _this.positionLoading();
                return true;
            });
        }
        LoadingClass.prototype.show = function () {
            if (this.On)
                return;
            this.createLoading();
            this.positionLoading();
            if (this.LoadingDiv)
                this.LoadingDiv.style.display = "";
            this.On = true;
        };
        LoadingClass.prototype.hide = function () {
            if (this.LoadingDiv)
                this.LoadingDiv.style.display = "none";
            this.On = false;
        };
        LoadingClass.prototype.createLoading = function () {
            if (!this.LoadingDiv) {
                this.LoadingDiv = $YetaWF.createElement("div", { id: "yLoading" },
                    $YetaWF.createElement("img", null));
                $YetaWF.getElement1BySelector("img", [this.LoadingDiv]).src = YConfigs.YetaWF_ComponentsHTML.LoaderGif;
                document.body.appendChild(this.LoadingDiv);
                var rect = this.LoadingDiv.getBoundingClientRect();
                this.LoadingDiv.style.display = "none";
                this.Width = rect.width;
                this.Height = rect.height;
            }
        };
        LoadingClass.prototype.positionLoading = function () {
            if (!this.LoadingDiv)
                return;
            var htmlDiv = document.querySelector("html");
            var x = this.CursorX + LoadingClass.OFFSETLEFT;
            var y = this.CursorY + LoadingClass.OFFSETTOP;
            if (x + this.Width > htmlDiv.clientWidth)
                x = htmlDiv.clientWidth - this.Width;
            if (x < 0)
                x = 0;
            if (y + this.Height > htmlDiv.clientHeight)
                y = htmlDiv.clientHeight - this.Height;
            if (y < 0)
                y = 0;
            var left = x + window.pageXOffset;
            var top = y + window.pageYOffset;
            this.LoadingDiv.style.top = "".concat(top, "px");
            this.LoadingDiv.style.left = "".concat(left, "px");
        };
        LoadingClass.OFFSETTOP = 13;
        LoadingClass.OFFSETLEFT = 0;
        return LoadingClass;
    }());
    YetaWF_ComponentsHTML.LoadingClass = LoadingClass;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var LoadingSupport = new YetaWF_ComponentsHTML.LoadingClass();

//# sourceMappingURL=Loading.js.map
