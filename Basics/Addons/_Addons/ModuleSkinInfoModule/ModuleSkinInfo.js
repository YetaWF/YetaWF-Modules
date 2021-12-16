"use strict";
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
var YetaWF_Basics;
(function (YetaWF_Basics) {
    var ModuleSkinInfoModule = /** @class */ (function (_super) {
        __extends(ModuleSkinInfoModule, _super);
        function ModuleSkinInfoModule(id) {
            var _this = _super.call(this, id, ModuleSkinInfoModule.SELECTOR, null) || this;
            // The text is embedded in a <span> which allows us the get the exact width (instead of the div's 100%)
            // get width/height
            var textElem = $YetaWF.getElement1BySelector(".t_row.t_characters .yt_textarea .t_chars", [_this.Module]);
            var rect = textElem.getBoundingClientRect();
            var width = rect.width;
            var height = rect.height;
            // show width/height
            var letterWidthDisp = $YetaWF.getElement1BySelector(".t_row.t_letterswidth .t_vals", [_this.Module]);
            letterWidthDisp.textContent = width.toFixed(2);
            var letterHeightDisp = $YetaWF.getElement1BySelector(".t_row.t_lettersheight .t_vals", [_this.Module]);
            letterHeightDisp.textContent = height.toFixed(2);
            // calculate width/height and show in property list
            var widthDisp = $YetaWF.getElement1BySelector(".t_row.t_width .t_vals", [_this.Module]);
            widthDisp.textContent = (width / (26 + 26 + 10)).toFixed(2);
            var heightDisp = $YetaWF.getElement1BySelector(".t_row.t_height .t_vals", [_this.Module]);
            heightDisp.textContent = (height / 2).toFixed(2);
            return _this;
        }
        ModuleSkinInfoModule.SELECTOR = ".YetaWF_Languages_ModuleSkinInfo";
        return ModuleSkinInfoModule;
    }(YetaWF.ModuleBaseNoDataImpl));
    YetaWF_Basics.ModuleSkinInfoModule = ModuleSkinInfoModule;
})(YetaWF_Basics || (YetaWF_Basics = {}));

//# sourceMappingURL=ModuleSkinInfo.js.map
