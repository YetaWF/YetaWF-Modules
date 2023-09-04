"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */
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
var YetaWF_Identity;
(function (YetaWF_Identity) {
    var Need2FADisplayModule = /** @class */ (function (_super) {
        __extends(Need2FADisplayModule, _super);
        function Need2FADisplayModule(id) {
            var _this = _super.call(this, id, Need2FADisplayModule.SELECTOR, null) || this;
            // Move to top of page
            document.body.insertBefore(_this.Module, document.body.firstChild);
            Need2FADisplayModule.propagateSize();
            return _this;
        }
        Need2FADisplayModule.propagateSize = function () {
            var modules = $YetaWF.getElementsBySelector(Need2FADisplayModule.SELECTOR);
            for (var _i = 0, modules_1 = modules; _i < modules_1.length; _i++) {
                var module = modules_1[_i];
                var container = $YetaWF.getElement1BySelector(".t_container");
                var rect = container.getBoundingClientRect();
                module.style.height = "".concat(rect.height, "px");
            }
        };
        Need2FADisplayModule.SELECTOR = ".YetaWF_Identity_Need2FADisplay";
        return Need2FADisplayModule;
    }(YetaWF.ModuleBaseNoDataImpl));
    YetaWF_Identity.Need2FADisplayModule = Need2FADisplayModule;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        if (ev.detail.container === document.body)
            Need2FADisplayModule.propagateSize();
        return true;
    });
})(YetaWF_Identity || (YetaWF_Identity = {}));

//# sourceMappingURL=Need2FADisplay.js.map
