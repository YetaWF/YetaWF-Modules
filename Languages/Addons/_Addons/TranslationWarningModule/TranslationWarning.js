"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */
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
var YetaWF_Languages;
(function (YetaWF_Languages) {
    var TranslationWarningModule = /** @class */ (function (_super) {
        __extends(TranslationWarningModule, _super);
        function TranslationWarningModule(id) {
            var _this = _super.call(this, id, TranslationWarningModule.SELECTOR, null) || this;
            // Move to top of page
            document.body.insertBefore(_this.Module, document.body.firstChild);
            TranslationWarningModule.propagateSize();
            return _this;
        }
        TranslationWarningModule.propagateSize = function () {
            var modules = $YetaWF.getElementsBySelector(TranslationWarningModule.SELECTOR);
            for (var _i = 0, modules_1 = modules; _i < modules_1.length; _i++) {
                var module = modules_1[_i];
                var container = $YetaWF.getElement1BySelector(".t_container");
                var rect = container.getBoundingClientRect();
                module.style.height = "".concat(rect.height, "px");
            }
        };
        TranslationWarningModule.SELECTOR = ".YetaWF_Languages_TranslationWarning";
        return TranslationWarningModule;
    }(YetaWF.ModuleBaseNoDataImpl));
    YetaWF_Languages.TranslationWarningModule = TranslationWarningModule;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        if (ev.detail.container === document.body)
            TranslationWarningModule.propagateSize();
        return true;
    });
})(YetaWF_Languages || (YetaWF_Languages = {}));

//# sourceMappingURL=TranslationWarning.js.map
