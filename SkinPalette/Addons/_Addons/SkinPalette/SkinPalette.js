"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */
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
var YetaWF_SkinPalette;
(function (YetaWF_SkinPalette) {
    var BasicThemeEnum;
    (function (BasicThemeEnum) {
        BasicThemeEnum[BasicThemeEnum["Light"] = 0] = "Light";
        BasicThemeEnum[BasicThemeEnum["Dark"] = 1] = "Dark";
    })(BasicThemeEnum || (BasicThemeEnum = {}));
    var SkinPaletteModule = /** @class */ (function (_super) {
        __extends(SkinPaletteModule, _super);
        function SkinPaletteModule(id, setup) {
            var _this = _super.call(this, id, SkinPaletteModule.SELECTOR, null) || this;
            _this.Setup = setup;
            _this.Form = $YetaWF.Forms.getInnerForm(_this.Module);
            _this.ExpandCollapse = $YetaWF.getElement1BySelector(".t_palette", [_this.Module]);
            _this.Tabs = YetaWF_ComponentsHTML.TabsComponent.getControlFromSelector(".yt_tabs", YetaWF_ComponentsHTML.TabsComponent.SELECTOR, [_this.Module]);
            _this.Contents = $YetaWF.getElement1BySelector(".t_contents", [_this.Module]);
            _this.Config = $YetaWF.getElement1BySelector("textarea[name='CSSVariables']", [_this.Module]);
            _this.ConfigSave = $YetaWF.getElement1BySelector("a[data-name='Save']", [_this.Module]);
            _this.AutoGenerate = $YetaWF.getElement1BySelector("a[data-name='AutoGen']", [_this.Module]);
            _this.Theme = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("[name='GenTheme']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [_this.Form]);
            _this.populateFromVariables();
            _this.updateGenValues();
            $YetaWF.registerEventHandler(_this.ExpandCollapse, "click", null, function (ev) {
                if (_this.Contents.style.display === "") {
                    _this.Contents.style.display = "none";
                }
                else {
                    _this.Contents.style.display = "";
                }
                return false;
            });
            $YetaWF.registerEventHandler(_this.ConfigSave, "click", null, function (ev) {
                if (!$YetaWF.Forms.validate(_this.Form)) {
                    $YetaWF.Forms.showErrors(_this.Form);
                    return false;
                }
                _this.updateSkin();
                return false;
            });
            $YetaWF.registerEventHandler(_this.AutoGenerate, "click", null, function (ev) {
                if (!$YetaWF.Forms.validate(_this.Form)) {
                    $YetaWF.Forms.showErrors(_this.Form);
                    return false;
                }
                _this.generateSkin();
                _this.populateFromModel();
                return false;
            });
            _this.Tabs.Control.addEventListener(YetaWF_ComponentsHTML.TabsComponent.EVENTSWITCHED, function (evt) {
                _this.updateConfig();
            });
            _this.Theme.Control.addEventListener(YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, function (evt) {
                _this.updateGenValues();
            });
            return _this;
        }
        SkinPaletteModule.prototype.populateFromVariables = function () {
            YetaWF_ComponentsHTML_Validation.clearValidation(this.Module);
            var root = document.documentElement;
            var styles = getComputedStyle(root);
            var _loop_1 = function (entry) {
                var val = styles.getPropertyValue(entry.CSSVarName);
                val = val.trim();
                switch (entry.UIHint) {
                    case "Color":
                        var clrControl_1 = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector("input[type=\"text\"][name=\"" + entry.ModelName + "\"]", YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this_1.Module]);
                        clrControl_1.value = val;
                        clrControl_1.Control.addEventListener(YetaWF_ComponentsHTML.ColorEditComponent.EVENTCHANGE, function (evt) {
                            root.style.setProperty(entry.CSSVarName, clrControl_1.value);
                        });
                        break;
                    case "Decimal":
                        var decControl_1 = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector("input[type=\"text\"][name=\"" + entry.ModelName + "\"]", YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this_1.Module]);
                        decControl_1.value = Number(val);
                        decControl_1.Control.addEventListener(YetaWF_ComponentsHTML.DecimalEditComponent.EVENTCHANGE, function (evt) {
                            root.style.setProperty(entry.CSSVarName, decControl_1.valueText);
                        });
                        break;
                    default:
                        var elem_1 = $YetaWF.getElement1BySelector("input[type=\"text\"][name=\"" + entry.ModelName + "\"]", [this_1.Module]);
                        elem_1.value = val;
                        $YetaWF.registerEventHandler(elem_1, "change", null, function (ev) {
                            root.style.setProperty(entry.CSSVarName, elem_1.value);
                            return true;
                        });
                        break;
                }
            };
            var this_1 = this;
            for (var _i = 0, _a = this.Setup.Properties; _i < _a.length; _i++) {
                var entry = _a[_i];
                _loop_1(entry);
            }
        };
        SkinPaletteModule.prototype.populateFromModel = function () {
            var root = document.documentElement;
            for (var _i = 0, _a = this.Setup.Properties; _i < _a.length; _i++) {
                var entry = _a[_i];
                switch (entry.UIHint) {
                    case "Color":
                        var clrControl = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector("input[type=\"text\"][name=\"" + entry.ModelName + "\"]", YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this.Module]);
                        root.style.setProperty(entry.CSSVarName, clrControl.value);
                        break;
                    case "Decimal":
                        var decControl = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector("input[type=\"text\"][name=\"" + entry.ModelName + "\"]", YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this.Module]);
                        root.style.setProperty(entry.CSSVarName, decControl.valueText);
                        break;
                    default:
                        var elem = $YetaWF.getElement1BySelector("input[type=\"text\"][name=\"" + entry.ModelName + "\"]", [this.Module]);
                        root.style.setProperty(entry.CSSVarName, elem.value);
                        break;
                }
            }
        };
        SkinPaletteModule.prototype.updateConfig = function () {
            var text = ":root {\n";
            var root = document.documentElement;
            var styles = getComputedStyle(root);
            for (var _i = 0, _a = this.Setup.Properties; _i < _a.length; _i++) {
                var entry = _a[_i];
                var val = styles.getPropertyValue(entry.CSSVarName);
                val = val.trim();
                text += "   " + entry.CSSVarName + ": " + val + ";\n";
            }
            text = text + "}\n";
            this.Config.value = text;
        };
        SkinPaletteModule.prototype.updateSkin = function () {
            // add a dummy div with new --vars and retrieve the new styles
            var div = $YetaWF.createElement("div", { id: "YetaWF_SkinPalette_Styles" },
                $YetaWF.createElement("style", null),
                $YetaWF.createElement("div", { class: 't_active' }));
            var styleElem = $YetaWF.getElement1BySelector("style", [div]);
            var text = this.Config.value;
            text = text.replace(":root", "#YetaWF_SkinPalette_Styles .t_active"); // we're replacing :root with the specific id
            styleElem.innerHTML = text;
            var styledElem = $YetaWF.getElement1BySelector(".t_active", [div]);
            document.body.appendChild(div);
            // extract all style values from style section and update root
            var root = document.documentElement;
            var styles = getComputedStyle(styledElem);
            for (var _i = 0, _a = this.Setup.Properties; _i < _a.length; _i++) {
                var entry = _a[_i];
                var val = styles.getPropertyValue(entry.CSSVarName);
                root.style.setProperty(entry.CSSVarName, val);
            }
            // remove style element
            div.remove();
            // update all fields with changes
            this.populateFromVariables();
            $YetaWF.Forms.submit(this.Form, true);
        };
        SkinPaletteModule.prototype.updateGenValues = function () {
            var mode = Number(this.Theme.value);
            var values;
            if (mode === BasicThemeEnum.Light) {
                values = {
                    GenTheme: BasicThemeEnum.Light,
                    GenBg: "#FFFFFF",
                    GenClr: "#7e7e7e",
                    GenBgActive: "#2fa4e7",
                    GenClrActive: "#FFFFFF",
                    GenBgShaded: "#f6f6f6",
                    GenClrShaded: "#454545",
                    GenBgShadedActive: "#1a99e2",
                    GenClrShadedActive: "#FFFFFF",
                    GenFont: "normal normal normal 1rem 'Open Sans', sans-serif",
                    GenBorderWidth: 1,
                    GenBorderClr: "#c5c5c5",
                    GenBorderRadius: 3,
                };
            }
            else {
                values = {
                    GenTheme: BasicThemeEnum.Dark,
                    GenBg: "#5c5c5c",
                    GenClr: "#FFFFFF",
                    GenBgActive: "#FFFFFF",
                    GenClrActive: "#2fa4e7",
                    GenBgShaded: "#454545",
                    GenClrShaded: "#f6f6f6",
                    GenBgShadedActive: "#a6a6a6",
                    GenClrShadedActive: "#ffffff",
                    GenFont: "normal normal normal 1rem 'Open Sans', sans-serif",
                    GenBorderWidth: 1,
                    GenBorderClr: "#777",
                    GenBorderRadius: 3,
                };
            }
            this.getColor("GenBg").value = values.GenBg;
            this.getColor("GenClr").value = values.GenClr;
            this.getColor("GenBgActive").value = values.GenBgActive;
            this.getColor("GenClrActive").value = values.GenClrActive;
            this.getColor("GenBgShaded").value = values.GenBgShaded;
            this.getColor("GenClrShaded").value = values.GenClrShaded;
            this.getColor("GenBgShadedActive").value = values.GenBgShadedActive;
            this.getColor("GenClrShadedActive").value = values.GenClrShadedActive;
            this.getInput("GenFont").value = values.GenFont;
            this.getIntValue("GenBorderWidth").value = values.GenBorderWidth;
            this.getColor("GenBorderClr").value = values.GenBorderClr;
            this.getIntValue("GenBorderRadius").value = values.GenBorderRadius;
        };
        SkinPaletteModule.prototype.generateSkin = function () {
            var values = this.getValues();
            this.getColor("BodyBg").value = values.GenBg;
            this.getColor("BodyClr").value = values.GenClr;
            this.getInput("BodyFont").value = values.GenFont;
            this.getColor("AnchorClr").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 20) : this.Lighten(values.GenClr, 20);
            this.getInput("AnchorDec").value = "none";
            this.getColor("AnchorClrHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getInput("AnchorDecHover").value = "underline";
            this.getColor("AnchorClrFocus").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getInput("AnchorDecFocus").value = "underline";
            this.getColor("OverlayBg").value = values.GenTheme === BasicThemeEnum.Light ? "#aaaaaa" : "#444444";
            this.getDecimal("OverlayOpacity").value = values.GenTheme === BasicThemeEnum.Light ? 0.5 : 0.8;
            this.getInput("MainMenu0Padding").value = "0";
            this.getColor("MainMenu0Clr").value = values.GenClr;
            this.getInput("MainMenu0Font").value = values.GenFont;
            this.getInput("MainMenu0Border").value = "none";
            this.getInput("MainMenu0BorderRadius").value = values.GenBorderRadius + "px";
            this.getColor("MainMenu0Bg").value = values.GenBg;
            this.getColor("MainMenu0ABg").value = values.GenBg;
            this.getColor("MainMenu0AClr").value = values.GenClr;
            this.getColor("MainMenu0ABgHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("MainMenu0AClrHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getColor("MainMenu0ABgPath").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("MainMenu0AClrPath").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getInput("MainMenu0APadding").value = "0.7em 1.5em";
            this.getInput("MainMenu0ASmallPadding").value = "0.7em 1.5em";
            this.getInput("MainMenu0DDWidth").value = "16px";
            this.getInput("MainMenu0DDHeight").value = "16px";
            this.getColor("MainMenu1Bg").value = values.GenBg;
            this.getColor("MainMenu1Clr").value = values.GenClr;
            this.getInput("MainMenu1Font").value = values.GenFont;
            this.getInput("MainMenu1Border").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("MainMenu1BorderRadius").value = values.GenBorderRadius + "px";
            this.getInput("MainMenu1SmallMargin").value = "0.5em";
            this.getInput("MainMenu1Padding").value = "0.3em 0";
            this.getInput("MainMenu1SmallPadding").value = "0.5em 0 0.5em 0";
            this.getColor("MainMenu1ABg").value = values.GenBg;
            this.getColor("MainMenu1AClr").value = values.GenClr;
            this.getColor("MainMenu1ABgHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("MainMenu1AClrHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getColor("MainMenu1ABgPath").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("MainMenu1AClrPath").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getInput("MainMenu1APadding").value = "0.1em 1.5em 0.1em 1.5em";
            this.getInput("MainMenu1ASmallPadding").value = "0.4em 0.5em 0.4em 0.75em";
            this.getInput("MainMenu1DDWidth").value = "16px";
            this.getInput("MainMenu1DDHeight").value = "16px";
            this.getInput("MainMenu1MMWidth").value = "800px";
            this.getColor("MainMenu1MMBg").value = values.GenBg;
            this.getColor("MainMenu1MMClr").value = values.GenClr;
            this.getInput("MainMenu1MMFont").value = values.GenFont;
            this.getColor("MainMenu2Bg").value = values.GenBg;
            this.getColor("MainMenu2Clr").value = values.GenClr;
            this.getInput("MainMenu2Font").value = values.GenFont;
            this.getInput("MainMenu2Border").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("MainMenu2BorderRadius").value = values.GenBorderRadius + "px";
            this.getInput("MainMenu2SmallMargin").value = "0.5em";
            this.getInput("MainMenu2Padding").value = "0.3em 0";
            this.getInput("MainMenu2SmallPadding").value = "0.5em 0";
            this.getColor("MainMenu2ABg").value = values.GenBg;
            this.getColor("MainMenu2AClr").value = values.GenClr;
            this.getColor("MainMenu2ABgHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("MainMenu2AClrHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getColor("MainMenu2ABgPath").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("MainMenu2AClrPath").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getInput("MainMenu2APadding").value = "0.1em 1em 0.1em 1em";
            this.getInput("MainMenu2ASmallPadding").value = "0.4em 0.5em 0.4em 0.75em";
            this.getInput("MainMenu2DDWidth").value = "16px";
            this.getInput("MainMenu2DDHeight").value = "16px";
            this.getInput("MainMenuSVGSmallBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("MainMenuSVGSmallBorderRadius").value = values.GenBorderRadius + "px";
            this.getColor("PopupMenu1Bg").value = values.GenBg;
            this.getColor("PopupMenu1Clr").value = values.GenClr;
            this.getInput("PopupMenu1Font").value = values.GenFont;
            this.getInput("PopupMenu1Border").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("PopupMenu1BorderRadius").value = values.GenBorderRadius + "px";
            this.getInput("PopupMenu1Padding").value = "0";
            this.getColor("PopupMenu1ABg").value = values.GenBg;
            this.getColor("PopupMenu1AClr").value = values.GenClr;
            this.getColor("PopupMenu1ABgHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("PopupMenu1AClrHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getColor("PopupMenu1ABgPath").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("PopupMenu1AClrPath").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getInput("PopupMenu1APadding").value = "0.4em 0.5em 0.4em 0.5em";
            this.getInput("PopupMenu1DDWidth").value = "16px";
            this.getInput("PopupMenu1DDHeight").value = "16px";
            this.getColor("PopupMenu2Bg").value = values.GenBg;
            this.getColor("PopupMenu2Clr").value = values.GenClr;
            this.getInput("PopupMenu2Font").value = values.GenFont;
            this.getInput("PopupMenu2Border").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("PopupMenu2BorderRadius").value = values.GenBorderRadius + "px";
            this.getInput("PopupMenu2Padding").value = "0";
            this.getColor("PopupMenu2ABg").value = values.GenBg;
            this.getColor("PopupMenu2AClr").value = values.GenClr;
            this.getColor("PopupMenu2ABgHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("PopupMenu2AClrHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getColor("PopupMenu2ABgPath").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("PopupMenu2AClrPath").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getInput("PopupMenu2APadding").value = "0.4em 0.5em 0.4em 0.5em";
            this.getInput("PopupMenu2DDWidth").value = "16px";
            this.getInput("PopupMenu2DDHeight").value = "16px";
            this.getColor("TTBg").value = values.GenBg;
            this.getColor("TTClr").value = values.GenClr;
            this.getInput("TTFont").value = values.GenFont;
            this.getInput("TTBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("TTBorderRadius").value = values.GenBorderRadius + "px";
            this.getInput("TTShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getColor("DialogBg").value = values.GenBg;
            this.getColor("DialogClr").value = values.GenClr;
            this.getColor("DialogTitleBg").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBgShaded, 40) : this.Lighten(values.GenBgShaded, 40);
            this.getColor("DialogTitleClr").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClrShaded, 40) : this.Lighten(values.GenClrShaded, 40);
            this.getInput("DialogTitleFont").value = values.GenClr;
            this.getInput("DialogLine").value = values.GenBorderClr;
            this.getInput("DialogBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("DialogBorderRadius").value = values.GenBorderRadius + "px";
            this.getInput("TstTitleFont").value = values.GenFont;
            this.getInput("TstMsgFont").value = values.GenFont;
            this.getInput("TstBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("TstBorderRadius").value = values.GenBorderRadius + "px";
            this.getColor("TstInfoBg").value = "#9bef9f";
            this.getColor("TstInfoClr").value = "#555";
            this.getInput("TstInfoShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getInput("TstInfoLine").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getColor("TstWarnBg").value = "orange";
            this.getColor("TstWarnClr").value = "rgb(49, 46, 46)";
            this.getInput("TstWarnShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getInput("TstWarnLine").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getColor("TstErrorBg").value = "red";
            this.getColor("TstErrorClr").value = "white";
            this.getInput("TstErrorShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getInput("TstErrorLine").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getColor("BarBg").value = values.GenBgShaded;
            this.getColor("BarClr").value = values.GenClrShaded;
            this.getColor("BarBgHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBgShaded, 40) : this.Lighten(values.GenBgShaded, 40);
            this.getColor("BarClrHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClrShaded, 40) : this.Lighten(values.GenClrShaded, 40);
            this.getColor("BarBgActive").value = values.GenBgShadedActive;
            this.getColor("BarClrActive").value = values.GenClrShadedActive;
            this.getInput("BarBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("BarH1FontSize").value = "1.25rem";
            this.getInput("BarH1PaddingBottom").value = ".7rem";
            this.getInput("PropBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("PropBorderRadius").value = values.GenBorderRadius + "px";
            this.getColor("PropCatBg").value = values.GenBgShaded;
            this.getColor("PropCatClr").value = values.GenClrShaded;
            this.getInput("PropCatBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("PropCatBorderRadius").value = values.GenBorderRadius + "px";
            this.getColor("InputBg").value = values.GenBg;
            this.getColor("InputClr").value = values.GenClr;
            this.getInput("InputBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("InputBorderHover").value = values.GenBorderWidth + "px solid " + (values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBorderClr, 40) : this.Lighten(values.GenBorderClr, 40));
            this.getInput("InputBorderFocus").value = values.GenBorderWidth + "px solid " + (values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBorderClr, 80) : this.Lighten(values.GenBorderClr, 80));
            this.getInput("InputBorderRadius").value = values.GenBorderRadius + "px";
            this.getColor("InputError").value = "red";
            this.getColor("ButtonBg").value = values.GenTheme === BasicThemeEnum.Light ?
                "linear-gradient(" + this.Lighten(values.GenClr, 10) + ", " + values.GenClr + " 60%, " + this.Lighten(values.GenClr, 20) + ")" :
                "linear-gradient(" + this.Darken(values.GenClr, 10) + ", " + values.GenClr + " 60%, " + this.Darken(values.GenClr, 20) + ")";
            this.getColor("ButtonClr").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 40) : this.Lighten(values.GenBg, 40);
            this.getColor("ButtonBgHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 40) : this.Lighten(values.GenClr, 40);
            this.getColor("ButtonClrHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 10) : this.Lighten(values.GenBg, 10);
            this.getColor("ButtonBgFocus").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 20) : this.Lighten(values.GenClr, 20);
            this.getColor("ButtonClrFocus").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBg, 10) : this.Lighten(values.GenBg, 10);
            this.getInput("ButtonBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("ButtonBorderRadius").value = values.GenBorderRadius + "px";
            this.getInput("ButtonShadow").value = "0px 0px 3px 0px rgba(0, 0, 0, 0.25)";
            this.getInput("ButtonShadowFocus").value = "0px 0px 3px 0px rgba(0, 0, 0, 0.25)";
            this.getColor("PbarBg").value = values.GenBgShaded;
            this.getColor("PbarValueBg").value = values.GenClrShaded;
            this.getInput("PbarBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("PbarBorderRadius").value = values.GenBorderRadius + "px";
            this.getColor("TabsBg").value = values.GenBg;
            this.getColor("TabsClr").value = values.GenClr;
            this.getColor("TabsStripBg").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBgShaded, 40) : this.Lighten(values.GenBgShaded, 40);
            this.getInput("TabsStripBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("TabsStripBorderRadius").value = values.GenBorderRadius + "px";
            this.getColor("TabsTabBg").value = values.GenBgShaded;
            this.getColor("TabsTabClr").value = values.GenClrShaded;
            this.getColor("TabsTabBgHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBgShaded, 40) : this.Lighten(values.GenBgShaded, 40);
            this.getColor("TabsTabClrHover").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClrShaded, 40) : this.Lighten(values.GenClrShaded, 40);
            this.getColor("TabsTabBgActive").value = values.GenBgShadedActive;
            this.getColor("TabsTabClrActive").value = values.GenClrShadedActive;
            this.getColor("TabsTabBgFocus").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBgShadedActive, 40) : this.Lighten(values.GenBgShadedActive, 40);
            this.getColor("TabsTabClrFocus").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClrShadedActive, 40) : this.Lighten(values.GenClrShadedActive, 40);
            this.getInput("TabsTabBorder").value = values.GenBorderWidth + "px solid " + values.GenBorderClr;
            this.getInput("TabsTabBorderRadius").value = values.GenBorderRadius + "px";
            this.getColor("StepBg").value = values.GenBgShaded;
            this.getColor("StepClr").value = values.GenClrShaded;
            this.getColor("StepBgActive").value = values.GenBgShadedActive;
            this.getColor("StepClrActive").value = values.GenClrShadedActive;
        };
        SkinPaletteModule.prototype.getValues = function () {
            var ddTheme = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("[name='GenTheme']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Form]);
            return {
                GenTheme: Number(ddTheme.value),
                GenBg: this.getInput("GenBg").value,
                GenClr: this.getInput("GenClr").value,
                GenBgActive: this.getInput("GenBgActive").value,
                GenClrActive: this.getInput("GenClrActive").value,
                GenBgShaded: this.getInput("GenBgShaded").value,
                GenClrShaded: this.getInput("GenClrShaded").value,
                GenBgShadedActive: this.getInput("GenBgShadedActive").value,
                GenClrShadedActive: this.getInput("GenClrShadedActive").value,
                GenFont: this.getInput("GenFont").value,
                GenBorderClr: this.getInput("GenBorderClr").value,
                GenBorderWidth: Number(this.getInput("GenBorderWidth").value),
                GenBorderRadius: Number(this.getInput("GenBorderRadius").value),
            };
        };
        SkinPaletteModule.prototype.getInput = function (name) {
            return $YetaWF.getElement1BySelector("input[name='" + name + "']", [this.Form]);
        };
        SkinPaletteModule.prototype.getColor = function (name) {
            var control = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector("[name='" + name + "']", YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this.Form]);
            return control;
        };
        SkinPaletteModule.prototype.getIntValue = function (name) {
            var control = YetaWF_ComponentsHTML.IntValueEditComponent.getControlFromSelector("[name='" + name + "']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Form]);
            return control;
        };
        SkinPaletteModule.prototype.getDecimal = function (name) {
            var control = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector("[name='" + name + "']", YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this.Form]);
            return control;
        };
        SkinPaletteModule.prototype.Lighten = function (color, amount) {
            return this.LightenDarken(color, amount);
        };
        SkinPaletteModule.prototype.Darken = function (color, amount) {
            return this.LightenDarken(color, -amount);
        };
        SkinPaletteModule.prototype.LightenDarken = function (color, amount) {
            color = color.replace(/^#/, "");
            if (color.length === 3)
                color = color[0] + color[0] + color[1] + color[1] + color[2] + color[2];
            var arr;
            arr = color.match(/.{2}/g);
            if (!arr)
                return color;
            var r = parseInt(arr[0], 16) + amount;
            var g = parseInt(arr[1], 16) + amount;
            var b = parseInt(arr[2], 16) + amount;
            var sr = Math.max(Math.min(255, r), 0).toString(16);
            var sg = Math.max(Math.min(255, g), 0).toString(16);
            var sb = Math.max(Math.min(255, b), 0).toString(16);
            var rr = (sr.length < 2 ? "0" : "") + sr;
            var gg = (sg.length < 2 ? "0" : "") + sg;
            var bb = (sb.length < 2 ? "0" : "") + sb;
            return "#" + rr + gg + bb;
        };
        SkinPaletteModule.SELECTOR = ".YetaWF_SkinPalette_SkinPalette";
        return SkinPaletteModule;
    }(YetaWF.ModuleBaseDataImpl));
    YetaWF_SkinPalette.SkinPaletteModule = SkinPaletteModule;
})(YetaWF_SkinPalette || (YetaWF_SkinPalette = {}));

//# sourceMappingURL=SkinPalette.js.map
