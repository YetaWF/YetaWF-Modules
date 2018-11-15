"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ModuleSelectionComponent = /** @class */ (function (_super) {
        __extends(ModuleSelectionComponent, _super);
        function ModuleSelectionComponent(controlId, setup) {
            var _this = _super.call(this, controlId) || this;
            _this.Setup = setup;
            _this.SelectPackage = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector(".t_packages select", [_this.Control]);
            _this.SelectModule = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector(".t_select select", [_this.Control]);
            _this.DivDescription = $YetaWF.getElement1BySelector(".t_description", [_this.Control]);
            _this.DivLink = $YetaWF.getElement1BySelector(".t_link", [_this.Control]);
            _this.ALink = $YetaWF.getElement1BySelector("a", [_this.DivLink]);
            _this.showDescription();
            _this.SelectPackage.Control.addEventListener("dropdownlist_change", function (evt) {
                var data = { AreaName: _this.SelectPackage.value };
                _this.SelectModule.ajaxUpdate(data, _this.Setup.AjaxUrl);
            });
            _this.SelectModule.Control.addEventListener("dropdownlist_change", function (evt) {
                _this.showDescription();
            });
            return _this;
        }
        ModuleSelectionComponent.prototype.showDescription = function () {
            var modGuid = this.SelectModule.value;
            if (this.hasValue) {
                this.ALink.href = "/!Mod/" + modGuid; // Globals.ModuleUrl
                this.ALink.style.display = "inline-block";
                this.DivDescription.textContent = this.getDescriptionText();
                this.DivDescription.style.display = "block";
            }
            else {
                this.ALink.style.display = "none";
                this.DivDescription.style.display = "none";
                this.DivDescription.textContent = "";
            }
        };
        ModuleSelectionComponent.prototype.getDescriptionText = function () {
            return this.SelectModule.getToolTip(this.SelectModule.selectedIndex);
        };
        Object.defineProperty(ModuleSelectionComponent.prototype, "hasValue", {
            // API
            get: function () {
                var modGuid = this.SelectModule.value;
                return (modGuid !== undefined && modGuid !== null && modGuid.length > 0 && modGuid !== "00000000-0000-0000-0000-000000000000");
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(ModuleSelectionComponent.prototype, "value", {
            get: function () {
                return this.SelectModule.value;
            },
            enumerable: true,
            configurable: true
        });
        ModuleSelectionComponent.prototype.enable = function (enabled) {
            this.SelectPackage.enable(enabled);
            this.SelectModule.enable(enabled);
            if (enabled && this.hasValue) {
                this.ALink.style.display = "inline-block";
                this.DivDescription.style.display = "block";
            }
            else {
                this.ALink.style.display = "none";
                this.DivDescription.style.display = "none";
            }
        };
        ModuleSelectionComponent.prototype.clear = function () {
            this.SelectPackage.value = "";
            this.SelectModule.value = "";
            this.ALink.style.display = "none";
            this.DivDescription.style.display = "none";
            this.DivDescription.textContent = "";
        };
        ModuleSelectionComponent.prototype.hasChanged = function (data) {
            if (!this.hasValue)
                return false;
            var modGuid = this.SelectModule.value;
            return modGuid !== data;
        };
        /**
         * Load object with data. Selects the correct package in the dropdownlist and selects the module (the package is detected using ajax).
         */
        ModuleSelectionComponent.prototype.updateComplete = function (modGuid) {
            var _this = this;
            if (modGuid !== undefined && modGuid !== null && modGuid.length > 0 && modGuid !== "00000000-0000-0000-0000-000000000000") {
                var data = { "modGuid": modGuid };
                this.SelectModule.ajaxUpdate(data, this.Setup.AjaxUrlComplete, function (data) {
                    _this.SelectPackage.value = data.extra;
                    _this.SelectModule.value = modGuid;
                    _this.showDescription();
                    FormsSupport.validateElement(_this.SelectModule.Control);
                }, function (result) {
                    _this.clear();
                    FormsSupport.validateElement(_this.SelectModule.Control);
                });
            }
            else {
                this.clear();
            }
        };
        ModuleSelectionComponent.SELECTOR = ".yt_moduleselection.t_edit";
        return ModuleSelectionComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.ModuleSelectionComponent = ModuleSelectionComponent;
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        ModuleSelectionComponent.clearDiv(tag, ModuleSelectionComponent.SELECTOR);
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
