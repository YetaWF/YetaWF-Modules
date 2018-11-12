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
    //interface Setup {
    //}
    var MultiStringEditComponent = /** @class */ (function (_super) {
        __extends(MultiStringEditComponent, _super);
        function MultiStringEditComponent(controlId /*, setup: Setup*/) {
            var _this = _super.call(this, controlId) || this;
            //this.Setup = setup;
            _this.SelectLang = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("select", [_this.Control]);
            _this.InputText = $YetaWF.getElement1BySelector("input.yt_multistring_text", [_this.Control]);
            // selection change (put language specific text into text box)
            _this.SelectLang.Control.addEventListener("dropdownlist_change", function (evt) {
                var sel = _this.SelectLang.selectedIndex;
                var hid = $YetaWF.getElement1BySelector("input[name$='[" + sel + "].value']", [_this.Control]);
                var newText = hid.value;
                if (newText.length === 0 && sel > 0) {
                    var hid0 = $YetaWF.getElement1BySelector("input[name$='[0].value']", [_this.Control]);
                    newText = hid0.value;
                    hid.value = newText;
                }
                _this.InputText.value = newText;
            });
            // textbox change (save text in language specific hidden fields)
            $YetaWF.registerEventHandler(_this.InputText, "input", null, function (ev) {
                var sel = _this.SelectLang.selectedIndex;
                var newText = _this.InputText.value;
                var hid = $YetaWF.getElement1BySelector("input[name$='[" + sel + "].value']", [_this.Control]);
                hid.value = newText;
                if (sel === 0)
                    _this.SelectLang.enable(newText.length > 0);
                return false;
            });
            $YetaWF.registerEventHandler(_this.InputText, "blur", null, function (ev) {
                var sel = _this.SelectLang.selectedIndex;
                if (sel === 0) {
                    var hid0 = $YetaWF.getElement1BySelector("input[name$='[0].value']", [_this.Control]);
                    var text = hid0.value;
                    if (text.length === 0) {
                        // the default text was cleared, clear all languages
                        var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
                        for (var index = 0; index < count; ++index) {
                            var hid = $YetaWF.getElement1BySelector("input[name$='[" + index + "].value']", [_this.Control]);
                            hid.value = "";
                        }
                    }
                }
                return false;
            });
            return _this;
        }
        // API
        MultiStringEditComponent.prototype.enable = function (enabled) {
            $YetaWF.elementEnableToggle(this.InputText, enabled);
            this.SelectLang.enable(enabled && YConfigs.YetaWF_ComponentsHTML.Localization);
        };
        MultiStringEditComponent.prototype.clear = function () {
            this.SelectLang.clear();
            var hids = $YetaWF.getElementsBySelector("input[name$='.value']", [this.Control]);
            for (var _i = 0, hids_1 = hids; _i < hids_1.length; _i++) {
                var hid = hids_1[_i];
                hid.value = "";
            }
            this.InputText.value = "";
        };
        Object.defineProperty(MultiStringEditComponent.prototype, "defaultValue", {
            get: function () {
                var hid0 = $YetaWF.getElement1BySelector("input[name$='[0].value']", [this.Control]);
                return hid0.value;
            },
            enumerable: true,
            configurable: true
        });
        MultiStringEditComponent.prototype.hasChanged = function (data) {
            var text = this.InputText.value;
            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count; ++index) {
                var hid = $YetaWF.getElement1BySelector("input[name$='[" + index + "].value']", [this.Control]);
                var langText = hid.value;
                if (langText.trim() === "")
                    langText = text;
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                if (data[lang] != null && !$YetaWF.stringCompare(data[lang], langText))
                    return true;
            }
            return false;
        };
        MultiStringEditComponent.prototype.retrieveData = function (data) {
            var newText = this.InputText.value;
            var sel = this.SelectLang.selectedIndex;
            var hid = $YetaWF.getElement1BySelector("input[name$='[" + sel + "].value']", [this.Control]);
            hid.value = newText;
            // now check whether it actually changed
            // if nothing is specified for a language, save what is entered in the text box
            var changed = false;
            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count; ++index) {
                hid = $YetaWF.getElement1BySelector("input[name$='[" + index + "].value']", [this.Control]);
                var langText = hid.value.trim();
                if (langText === "")
                    langText = newText;
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                if (!$YetaWF.stringCompare(data[lang], langText)) {
                    changed = true;
                    data[lang] = langText;
                }
            }
            return changed;
        };
        MultiStringEditComponent.prototype.update = function (data) {
            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count; ++index) {
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                var s = "";
                if (data.hasOwnProperty(lang))
                    s = data[lang];
                else if (data.hasOwnProperty(YLocs.YetaWF_ComponentsHTML.Languages[0]))
                    s = data[lang] = data[YLocs.YetaWF_ComponentsHTML.Languages[0]]; // use default for languages w/o data
                else
                    throw "No language data";
                var hid = $YetaWF.getElement1BySelector("input[name$='[" + index + "].value']", [this.Control]);
                hid.value = s;
                if (index === 0)
                    this.InputText.value = s;
            }
            this.SelectLang.clear();
        };
        MultiStringEditComponent.SELECTOR = ".yt_multistring.t_edit";
        return MultiStringEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.MultiStringEditComponent = MultiStringEditComponent;
    if (YLocs.YetaWF_ComponentsHTML.Languages === undefined)
        throw "YLocs.YetaWF_ComponentsHTML.Languages missing"; /*DEBUG*/
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        MultiStringEditComponent.clearDiv(tag, MultiStringEditComponent.SELECTOR);
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=MultiStringEdit.js.map
