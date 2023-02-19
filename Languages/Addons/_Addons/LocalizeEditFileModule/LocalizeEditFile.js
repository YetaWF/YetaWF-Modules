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
var YetaWF_Languages;
(function (YetaWF_Languages) {
    var LocalizeEditFileModule = /** @class */ (function (_super) {
        __extends(LocalizeEditFileModule, _super);
        function LocalizeEditFileModule(id) {
            var _this = _super.call(this, id, LocalizeEditFileModule.SELECTOR, null) || this;
            _this.ResetButton = $YetaWF.getElement1BySelector("input[name='Reset']", [_this.Module]);
            var form = $YetaWF.Forms.getInnerForm(_this.Module);
            $YetaWF.registerEventHandler(_this.ResetButton, "click", null, function (ev) {
                $YetaWF.alertYesNo(YLocs.YetaWF_Languages.ConfirmResetText, undefined, function () {
                    $YetaWF.Forms.submit(form, true, { RestoreDefaults: true });
                });
                return false;
            });
            return _this;
        }
        LocalizeEditFileModule.SELECTOR = ".YetaWF_Languages_LocalizeEditFile";
        return LocalizeEditFileModule;
    }(YetaWF.ModuleBaseNoDataImpl));
    YetaWF_Languages.LocalizeEditFileModule = LocalizeEditFileModule;
})(YetaWF_Languages || (YetaWF_Languages = {}));

//# sourceMappingURL=LocalizeEditFile.js.map
