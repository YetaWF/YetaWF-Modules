"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
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
            var _this = _super.call(this, controlId, MultiStringEditComponent.TEMPLATE, MultiStringEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: MultiStringEditComponent.EVENT,
                GetValue: function (control) {
                    return _this.Hidden.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }) || this;
            //this.Setup = setup;
            _this.Hidden = $YetaWF.getElement1BySelector("input.t_multistring_hidden", [_this.Control]);
            _this.SelectLang = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [_this.Control]);
            _this.InputText = $YetaWF.getElement1BySelector("input.t_multistring_text", [_this.Control]);
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
                var newText = _this.InputText.value;
                var sel = _this.SelectLang.selectedIndex;
                var hid = $YetaWF.getElement1BySelector("input[name$='[" + sel + "].value']", [_this.Control]);
                hid.value = newText;
                if (sel === 0)
                    _this.Hidden.value = newText;
                _this.updateSelectLang();
                FormsSupport.validateElement(_this.Hidden);
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
                        _this.Hidden.value = "";
                    }
                    _this.updateSelectLang();
                }
                _this.sendChangedEvent();
                return true;
            });
            return _this;
        }
        MultiStringEditComponent.prototype.updateSelectLang = function () {
            if (this.SelectLang.selectedIndex === 0)
                this.SelectLang.enable(YConfigs.YetaWF_ComponentsHTML.Localization && this.InputText.value.length > 0);
        };
        MultiStringEditComponent.prototype.sendChangedEvent = function () {
            FormsSupport.validateElement(this.Hidden);
            var event = document.createEvent("Event");
            event.initEvent(MultiStringEditComponent.EVENT, true, true);
            this.Control.dispatchEvent(event);
        };
        // API
        MultiStringEditComponent.prototype.enable = function (enabled) {
            $YetaWF.elementEnableToggle(this.InputText, enabled);
            this.updateSelectLang();
        };
        MultiStringEditComponent.prototype.clear = function () {
            var hids = $YetaWF.getElementsBySelector("input[name$='.value']", [this.Control]);
            for (var _i = 0, hids_1 = hids; _i < hids_1.length; _i++) {
                var hid = hids_1[_i];
                hid.value = "";
            }
            this.Hidden.value = "";
            this.InputText.value = "";
            this.SelectLang.clear();
            this.updateSelectLang();
        };
        Object.defineProperty(MultiStringEditComponent.prototype, "defaultValue", {
            get: function () {
                return this.InputText.value;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(MultiStringEditComponent.prototype, "value", {
            get: function () {
                var data = {};
                var newText = this.InputText.value;
                var sel = this.SelectLang.selectedIndex;
                var hid = $YetaWF.getElement1BySelector("input[name$='[" + sel + "].value']", [this.Control]);
                hid.value = newText;
                var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
                for (var index = 0; index < count; ++index) {
                    hid = $YetaWF.getElement1BySelector("input[name$='[" + index + "].value']", [this.Control]);
                    var langText = hid.value;
                    if (langText === "")
                        langText = newText;
                    var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                    data[lang] = langText;
                }
                return data;
            },
            set: function (data) {
                var textDefault = this.findLanguageText(data, YLocs.YetaWF_ComponentsHTML.Languages[0]);
                var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
                for (var index = 0; index < count; ++index) {
                    var s = "";
                    var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                    var text = this.findLanguageText(data, lang);
                    if (text)
                        s = text;
                    else if (textDefault)
                        s = textDefault; // use default for languages w/o data
                    var hid = $YetaWF.getElement1BySelector("input[name$='[" + index + "].value']", [this.Control]);
                    hid.value = s;
                    if (index === 0) {
                        this.Hidden.value = s;
                        this.InputText.value = s;
                    }
                }
                this.SelectLang.clear();
                this.updateSelectLang();
            },
            enumerable: false,
            configurable: true
        });
        MultiStringEditComponent.prototype.hasChanged = function (data) {
            var textDefault = this.findLanguageText(data, YLocs.YetaWF_ComponentsHTML.Languages[0]);
            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count; ++index) {
                var hid = $YetaWF.getElement1BySelector("input[name$='[" + index + "].value']", [this.Control]);
                var langText = hid.value;
                if (langText === "")
                    langText = textDefault || "";
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                var ms = this.findLanguageText(data, lang);
                if (!ms)
                    ms = textDefault;
                if (!$YetaWF.stringCompare(ms, langText))
                    return true;
            }
            return false;
        };
        MultiStringEditComponent.prototype.findLanguageText = function (data, lang) {
            if (!data.hasOwnProperty(lang))
                return null;
            return data[lang];
        };
        MultiStringEditComponent.TEMPLATE = "yt_multistring";
        MultiStringEditComponent.SELECTOR = ".yt_multistring.t_edit";
        MultiStringEditComponent.EVENT = "multistring_change";
        return MultiStringEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.MultiStringEditComponent = MultiStringEditComponent;
    if (YLocs.YetaWF_ComponentsHTML.Languages === undefined)
        throw "YLocs.YetaWF_ComponentsHTML.Languages missing"; /*DEBUG*/
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=MultiStringEdit.js.map
