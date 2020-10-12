"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
                this.LoadingDiv = $YetaWF.createElement("div", { id: "yLoading", style: "display:none" },
                    $YetaWF.createElement("img", null));
                $YetaWF.getElement1BySelector("img", [this.LoadingDiv]).src = YConfigs.YetaWF_ComponentsHTML.LoaderGif;
                document.body.appendChild(this.LoadingDiv);
            }
        };
        LoadingClass.prototype.positionLoading = function () {
            if (!this.LoadingDiv)
                return;
            var left = this.CursorX + LoadingClass.OFFSETLEFT + window.pageXOffset;
            var top = this.CursorY + LoadingClass.OFFSETTOP + window.pageYOffset;
            this.LoadingDiv.style.top = top + "px";
            this.LoadingDiv.style.left = left + "px";
        };
        LoadingClass.OFFSETTOP = 13;
        LoadingClass.OFFSETLEFT = 0;
        return LoadingClass;
    }());
    YetaWF_ComponentsHTML.LoadingClass = LoadingClass;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var LoadingSupport = new YetaWF_ComponentsHTML.LoadingClass();

//# sourceMappingURL=Loading.js.map
