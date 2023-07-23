"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */
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
                $YetaWF.setLoading(true);
                $YetaWF.pleaseWait("Saving theme...");
                setTimeout(function () {
                    if (!$YetaWF.Forms.validate(_this.Form)) {
                        $YetaWF.setLoading(false);
                        $YetaWF.Forms.showErrors(_this.Form);
                        return;
                    }
                    _this.updateSkin();
                    $YetaWF.setLoading(false);
                }, 1);
                return false;
            });
            $YetaWF.registerEventHandler(_this.AutoGenerate, "click", null, function (ev) {
                $YetaWF.setLoading(true);
                if (!$YetaWF.Forms.validate(_this.Form)) {
                    $YetaWF.setLoading(false);
                    $YetaWF.Forms.showErrors(_this.Form);
                    return false;
                }
                _this.generateSkin();
                _this.populateFromModel();
                $YetaWF.setLoading(false);
                $YetaWF.message("CSS variables successfully generated and updated");
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
                        var clrControl_1 = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector("[name=\"".concat(entry.ModelName, "\"]"), YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this_1.Module]);
                        clrControl_1.value = val;
                        clrControl_1.Control.addEventListener(YetaWF_ComponentsHTML.ColorEditComponent.EVENTCHANGE, function (evt) {
                            root.style.setProperty(entry.CSSVarName, clrControl_1.value);
                        });
                        break;
                    case "Decimal":
                        var decControl_1 = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector("[name=\"".concat(entry.ModelName, "\"]"), YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this_1.Module]);
                        decControl_1.value = Number(val);
                        decControl_1.Control.addEventListener(YetaWF_ComponentsHTML.DecimalEditComponent.EVENT, function (evt) {
                            root.style.setProperty(entry.CSSVarName, decControl_1.valueText);
                        });
                        break;
                    default:
                        var elem_1 = $YetaWF.getElement1BySelector("[name=\"".concat(entry.ModelName, "\"]"), [this_1.Module]);
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
                        var clrControl = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector("[name=\"".concat(entry.ModelName, "\"]"), YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this.Module]);
                        root.style.setProperty(entry.CSSVarName, clrControl.value);
                        break;
                    case "Decimal":
                        var decControl = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector("[name=\"".concat(entry.ModelName, "\"]"), YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this.Module]);
                        root.style.setProperty(entry.CSSVarName, decControl.valueText);
                        break;
                    default:
                        var elem = $YetaWF.getElement1BySelector("[name=\"".concat(entry.ModelName, "\"]"), [this.Module]);
                        root.style.setProperty(entry.CSSVarName, elem.value);
                        break;
                }
            }
        };
        SkinPaletteModule.prototype.updateConfig = function () {
            var text = "@media screen {\n\n:root {\n";
            var root = document.documentElement;
            var styles = getComputedStyle(root);
            for (var _i = 0, _a = this.Setup.Properties; _i < _a.length; _i++) {
                var entry = _a[_i];
                var val = styles.getPropertyValue(entry.CSSVarName);
                val = val.trim();
                text += "   ".concat(entry.CSSVarName, ": ").concat(val, ";\n");
            }
            text = text + "}\n\n}\n";
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
                    GenClrActive: "#9e9e9e",
                    GenBgShaded: "#f6f6f6",
                    GenClrShaded: "#454545",
                    GenBgShadedActive: "#1a99e2",
                    GenClrShadedActive: "#FFFFFF",
                    GenFont: "normal normal normal 1rem 'Open Sans', sans-serif",
                    GenSmallFont: "normal normal normal .7rem 'Open Sans', sans-serif",
                    GenBorderWidth: 1,
                    GenBorderClr: "#c5c5c5",
                    GenBorderRadius: 3,
                    GenOpacity: 0.5,
                };
            }
            else {
                values = {
                    GenTheme: BasicThemeEnum.Dark,
                    GenBg: "#5c5c5c",
                    GenClr: "#FFFFFF",
                    GenBgActive: "#787878",
                    GenClrActive: "#ffffff",
                    GenBgShaded: "#454545",
                    GenClrShaded: "#f6f6f6",
                    GenBgShadedActive: "#a6a6a6",
                    GenClrShadedActive: "#ffffff",
                    GenFont: "normal normal normal 1rem 'Open Sans', sans-serif",
                    GenSmallFont: "normal normal normal .7rem 'Open Sans', sans-serif",
                    GenBorderWidth: 1,
                    GenBorderClr: "#777",
                    GenBorderRadius: 3,
                    GenOpacity: 0.5,
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
            this.getInput("GenSmallFont").value = values.GenSmallFont;
            this.getIntValue("GenBorderWidth").value = values.GenBorderWidth;
            this.getColor("GenBorderClr").value = values.GenBorderClr;
            this.getIntValue("GenBorderRadius").value = values.GenBorderRadius;
            this.getDecimal("BodyDisabledOpacity").value = values.GenOpacity;
        };
        SkinPaletteModule.prototype.generateSkin = function () {
            var values = this.getValues();
            this.getColor("BodyBg").value = values.GenBg;
            this.getColor("BodyClr").value = values.GenClr;
            this.getInput("BodyFont").value = values.GenFont;
            this.getDecimal("BodyDisabledOpacity").value = values.GenOpacity;
            this.getColor("OverlayBg").value = values.GenTheme === BasicThemeEnum.Light ? "#aaaaaa" : "#444444";
            this.getDecimal("OverlayOpacity").value = values.GenOpacity;
            this.getInput("ModStandardTitleFont").value = values.GenFont;
            this.getColor("ModPanelBg").value = values.GenBgShaded;
            this.getColor("ModPanelClr").value = values.GenClrShaded;
            this.getInput("ModPanelBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("ModPanelBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("ModPanelTitleFont").value = values.GenFont;
            this.getInput("ModPanelLinkFont").value = values.GenSmallFont;
            this.getInput("MainMenuHorz0Padding").value = "0";
            this.getColor("MainMenuHorz0Clr").value = values.GenClr;
            this.getInput("MainMenuHorz0Font").value = values.GenFont;
            this.getInput("MainMenuHorz0Border").value = "none";
            this.getInput("MainMenuHorz0BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("MainMenuHorz0Bg").value = values.GenBg;
            this.getColor("MainMenuHorz0ABg").value = values.GenBg;
            this.getColor("MainMenuHorz0AClr").value = values.GenClr;
            this.getColor("MainMenuHorz0ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz0AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuHorz0ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz0AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuHorz0APadding").value = "0.7em 1.5em";
            this.getInput("MainMenuHorz0DDWidth").value = "16px";
            this.getInput("MainMenuHorz0DDHeight").value = "16px";
            this.getColor("MainMenuHorz1Bg").value = values.GenBg;
            this.getColor("MainMenuHorz1Clr").value = values.GenClr;
            this.getInput("MainMenuHorz1Font").value = values.GenFont;
            this.getInput("MainMenuHorz1Border").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("MainMenuHorz1BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("MainMenuHorz1Padding").value = "0.3em 0";
            this.getColor("MainMenuHorz1ABg").value = values.GenBg;
            this.getColor("MainMenuHorz1AClr").value = values.GenClr;
            this.getColor("MainMenuHorz1ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz1AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuHorz1ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz1AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuHorz1APadding").value = "0.1em 1.5em 0.1em 1.5em";
            this.getInput("MainMenuHorz1DDWidth").value = "16px";
            this.getInput("MainMenuHorz1DDHeight").value = "16px";
            this.getInput("MainMenuHorz1MMWidth").value = "800px";
            this.getColor("MainMenuHorz1MMBg").value = values.GenBg;
            this.getColor("MainMenuHorz1MMClr").value = values.GenClr;
            this.getInput("MainMenuHorz1MMFont").value = values.GenFont;
            this.getColor("MainMenuHorz2Bg").value = values.GenBg;
            this.getColor("MainMenuHorz2Clr").value = values.GenClr;
            this.getInput("MainMenuHorz2Font").value = values.GenFont;
            this.getInput("MainMenuHorz2Border").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("MainMenuHorz2BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("MainMenuHorz2Padding").value = "0.3em 0";
            this.getColor("MainMenuHorz2ABg").value = values.GenBg;
            this.getColor("MainMenuHorz2AClr").value = values.GenClr;
            this.getColor("MainMenuHorz2ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz2AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuHorz2ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuHorz2AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuHorz2APadding").value = "0.1em 1em 0.1em 1em";
            this.getInput("MainMenuHorz2DDWidth").value = "16px";
            this.getInput("MainMenuHorz2DDHeight").value = "16px";
            this.getInput("MainMenuVert0Padding").value = "0";
            this.getColor("MainMenuVert0Clr").value = values.GenClr;
            this.getInput("MainMenuVert0Font").value = values.GenFont;
            this.getInput("MainMenuVert0Border").value = "none";
            this.getInput("MainMenuVert0BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("MainMenuVert0Bg").value = values.GenBg;
            this.getColor("MainMenuVert0ABg").value = values.GenBg;
            this.getColor("MainMenuVert0AClr").value = values.GenClr;
            this.getColor("MainMenuVert0ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert0AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuVert0ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert0AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuVert0APadding").value = "0.7em 1.5em";
            this.getInput("MainMenuVert0DDWidth").value = "16px";
            this.getInput("MainMenuVert0DDHeight").value = "16px";
            this.getColor("MainMenuVert1Bg").value = values.GenBg;
            this.getColor("MainMenuVert1Clr").value = values.GenClr;
            this.getInput("MainMenuVert1Font").value = values.GenFont;
            this.getInput("MainMenuVert1Border").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("MainMenuVert1BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("MainMenuVert1Padding").value = "0.3em 0";
            this.getColor("MainMenuVert1ABg").value = values.GenBg;
            this.getColor("MainMenuVert1AClr").value = values.GenClr;
            this.getColor("MainMenuVert1ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert1AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuVert1ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert1AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuVert1APadding").value = "0.1em 1.5em 0.1em 1.5em";
            this.getInput("MainMenuVert1DDWidth").value = "16px";
            this.getInput("MainMenuVert1DDHeight").value = "16px";
            this.getInput("MainMenuVert1MMWidth").value = "800px";
            this.getColor("MainMenuVert1MMBg").value = values.GenBg;
            this.getColor("MainMenuVert1MMClr").value = values.GenClr;
            this.getInput("MainMenuVert1MMFont").value = values.GenFont;
            this.getColor("MainMenuVert2Bg").value = values.GenBg;
            this.getColor("MainMenuVert2Clr").value = values.GenClr;
            this.getInput("MainMenuVert2Font").value = values.GenFont;
            this.getInput("MainMenuVert2Border").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("MainMenuVert2BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("MainMenuVert2Padding").value = "0.3em 0";
            this.getColor("MainMenuVert2ABg").value = values.GenBg;
            this.getColor("MainMenuVert2AClr").value = values.GenClr;
            this.getColor("MainMenuVert2ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert2AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuVert2ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuVert2AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuVert2APadding").value = "0.1em 1em 0.1em 1em";
            this.getInput("MainMenuVert2DDWidth").value = "16px";
            this.getInput("MainMenuVert2DDHeight").value = "16px";
            this.getInput("MainMenuSm0Padding").value = "0";
            this.getColor("MainMenuSm0Clr").value = values.GenClr;
            this.getInput("MainMenuSm0Font").value = values.GenFont;
            this.getInput("MainMenuSm0Border").value = "none";
            this.getInput("MainMenuSm0BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("MainMenuSm0Bg").value = values.GenBg;
            this.getColor("MainMenuSm0ABg").value = values.GenBg;
            this.getColor("MainMenuSm0AClr").value = values.GenClr;
            this.getColor("MainMenuSm0ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm0AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuSm0ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm0AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuSm0APadding").value = "0.7em 1.5em";
            this.getInput("MainMenuSm0DDWidth").value = "16px";
            this.getInput("MainMenuSm0DDHeight").value = "16px";
            this.getColor("MainMenuSm1Bg").value = values.GenBg;
            this.getColor("MainMenuSm1Clr").value = values.GenClr;
            this.getInput("MainMenuSm1Font").value = values.GenFont;
            this.getInput("MainMenuSm1Border").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("MainMenuSm1BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("MainMenuSm1Padding").value = "0.5em 0 0.5em 0";
            this.getColor("MainMenuSm1ABg").value = values.GenBg;
            this.getColor("MainMenuSm1AClr").value = values.GenClr;
            this.getColor("MainMenuSm1ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm1AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuSm1ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm1AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuSm1APadding").value = "0.4em 0.5em 0.4em 0.75em";
            this.getInput("MainMenuSm1DDWidth").value = "16px";
            this.getInput("MainMenuSm1DDHeight").value = "16px";
            this.getInput("MainMenuSm1MMWidth").value = "800px";
            this.getColor("MainMenuSm1MMBg").value = values.GenBg;
            this.getColor("MainMenuSm1MMClr").value = values.GenClr;
            this.getInput("MainMenuSm1MMFont").value = values.GenFont;
            this.getColor("MainMenuSm2Bg").value = values.GenBg;
            this.getColor("MainMenuSm2Clr").value = values.GenClr;
            this.getInput("MainMenuSm2Font").value = values.GenFont;
            this.getInput("MainMenuSm2Border").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("MainMenuSm2BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("MainMenuSm2Padding").value = "0.5em 0";
            this.getColor("MainMenuSm2ABg").value = values.GenBg;
            this.getColor("MainMenuSm2AClr").value = values.GenClr;
            this.getColor("MainMenuSm2ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm2AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("MainMenuSm2ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("MainMenuSm2AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("MainMenuSm2APadding").value = "0.4em 0.5em 0.4em 0.75em";
            this.getInput("MainMenuSm2DDWidth").value = "16px";
            this.getInput("MainMenuSm2DDHeight").value = "16px";
            this.getInput("MenuSVGSmallBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("MenuSVGSmallBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("PopupMenu1Bg").value = values.GenBg;
            this.getColor("PopupMenu1Clr").value = values.GenClr;
            this.getInput("PopupMenu1Font").value = values.GenSmallFont;
            this.getInput("PopupMenu1Border").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("PopupMenu1BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("PopupMenu1Padding").value = "0";
            this.getColor("PopupMenu1ABg").value = values.GenBg;
            this.getColor("PopupMenu1AClr").value = values.GenClr;
            this.getColor("PopupMenu1ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("PopupMenu1AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("PopupMenu1ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("PopupMenu1AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("PopupMenu1APadding").value = "0.4em 0.5em 0.4em 0.5em";
            this.getInput("PopupMenu1DDWidth").value = "16px";
            this.getInput("PopupMenu1DDHeight").value = "16px";
            this.getColor("PopupMenu2Bg").value = values.GenBg;
            this.getColor("PopupMenu2Clr").value = values.GenClr;
            this.getInput("PopupMenu2Font").value = values.GenSmallFont;
            this.getInput("PopupMenu2Border").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("PopupMenu2BorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("PopupMenu2Padding").value = "0";
            this.getColor("PopupMenu2ABg").value = values.GenBg;
            this.getColor("PopupMenu2AClr").value = values.GenClr;
            this.getColor("PopupMenu2ABgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("PopupMenu2AClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("PopupMenu2ABgPath").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("PopupMenu2AClrPath").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("PopupMenu2APadding").value = "0.4em 0.5em 0.4em 0.5em";
            this.getInput("PopupMenu2DDWidth").value = "16px";
            this.getInput("PopupMenu2DDHeight").value = "16px";
            this.getColor("TTBg").value = values.GenBg;
            this.getColor("TTClr").value = values.GenClr;
            this.getInput("TTFont").value = values.GenFont;
            this.getInput("TTBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("TTBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("TTShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getColor("DialogBg").value = values.GenBg;
            this.getColor("DialogClr").value = values.GenClr;
            this.getColor("DialogTitleBg").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBgShaded, 40) : this.Lighten(values.GenBgShaded, 40);
            this.getColor("DialogTitleClr").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClrShaded, 40) : this.Lighten(values.GenClrShaded, 40);
            this.getInput("DialogTitleFont").value = values.GenClr;
            this.getInput("DialogLine").value = values.GenBorderClr;
            this.getInput("DialogBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("DialogBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("TstTitleFont").value = "normal normal normal 1rem/2rem 'Open Sans', sans-serif";
            this.getInput("TstMsgFont").value = values.GenFont;
            this.getInput("TstBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("TstBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("TstInfoBg").value = "#9bef9f";
            this.getColor("TstInfoClr").value = "#555";
            this.getInput("TstInfoShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getInput("TstInfoLine").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getColor("TstWarnBg").value = "orange";
            this.getColor("TstWarnClr").value = "rgb(49, 46, 46)";
            this.getInput("TstWarnShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getInput("TstWarnLine").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getColor("TstErrorBg").value = "red";
            this.getColor("TstErrorClr").value = "white";
            this.getInput("TstErrorShadow").value = "0px 0px 5px 0px rgba(0, 0, 0, 0.5)";
            this.getInput("TstErrorLine").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getColor("BarBg").value = values.GenBgShaded;
            this.getColor("BarClr").value = values.GenClrShaded;
            this.getColor("BarBgHover").value = this.ToHover(values.GenTheme, values.GenBgShaded);
            this.getColor("BarClrHover").value = this.ToHover(values.GenTheme, values.GenClrShaded);
            this.getColor("BarBgActive").value = values.GenBgShadedActive;
            this.getColor("BarClrActive").value = values.GenClrShadedActive;
            this.getInput("BarBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("BarBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("BarH1FontSize").value = "1.25rem";
            this.getInput("PropBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("PropBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("PropCatBg").value = values.GenBgShaded;
            this.getColor("PropCatClr").value = values.GenClrShaded;
            this.getInput("PropCatBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("PropCatBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("InputBg").value = values.GenBg;
            this.getColor("InputClr").value = values.GenClr;
            this.getInput("InputBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("InputBorderHover").value = "".concat(values.GenBorderWidth, "px solid ").concat(this.ToHover(values.GenTheme, values.GenBorderClr));
            this.getInput("InputBorderFocus").value = "".concat(values.GenBorderWidth, "px solid ").concat(this.ToFocus(values.GenTheme, values.GenBorderClr));
            this.getInput("InputBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("InputError").value = "red";
            this.getColor("InputPlaceholderClr").value = values.GenTheme === BasicThemeEnum.Light ? "black" : "white";
            this.getDecimal("InputPlaceholderOpacity").value = values.GenOpacity;
            this.getColor("DDBg").value = values.GenBg;
            this.getColor("DDClr").value = values.GenClr;
            this.getColor("DDBgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("DDClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("DDBgActive").value = values.GenBgActive;
            this.getColor("DDClrActive").value = values.GenClrActive;
            this.getColor("DDBgFocus").value = this.ToFocus(values.GenTheme, values.GenBg);
            this.getColor("DDClrFocus").value = this.ToFocus(values.GenTheme, values.GenClr);
            this.getInput("DDBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("DDBorderHover").value = "".concat(values.GenBorderWidth, "px solid ").concat(this.ToHover(values.GenTheme, values.GenBorderClr));
            this.getInput("DDBorderFocus").value = "".concat(values.GenBorderWidth, "px solid ").concat(this.ToFocus(values.GenTheme, values.GenBorderClr));
            this.getInput("DDBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("DDListBg").value = values.GenBg;
            this.getColor("DDListClr").value = values.GenClr;
            this.getColor("DDListBgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("DDListClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("DDListBgFocus").value = this.ToFocus(values.GenTheme, values.GenBg);
            this.getColor("DDListClrFocus").value = this.ToFocus(values.GenTheme, values.GenClr);
            this.getInput("DDListBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("DDListBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("DDListShadow").value = "0 2px 2px 0 rgba(0,0,0,.3)";
            this.getInput("DDListShadowFocus").value = "inset 0px 0px 3px 0px rgba(0, 0, 0, 0.25)";
            this.getColor("AnchorClr").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenClr, 20) : this.Lighten(values.GenClr, 20);
            this.getInput("AnchorDec").value = "none";
            this.getColor("AnchorClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getInput("AnchorDecHover").value = "underline";
            this.getColor("AnchorClrFocus").value = values.GenClrActive;
            this.getInput("AnchorDecFocus").value = "underline";
            this.getColor("ButtonBg").value = values.GenBg;
            this.getColor("ButtonClr").value = values.GenClr;
            this.getColor("ButtonBgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("ButtonClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("ButtonBgFocus").value = this.ToFocus(values.GenTheme, values.GenBg);
            this.getColor("ButtonClrFocus").value = this.ToFocus(values.GenTheme, values.GenClr);
            this.getInput("ButtonBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("ButtonBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("ButtonShadow").value = "0px 0px 3px 0px rgba(0, 0, 0, 0.25)";
            this.getInput("ButtonShadowFocus").value = "inset 0px 0px 3px 0px rgba(0, 0, 0, 0.25)";
            this.getColor("ButtonLiteBg").value = values.GenBgShaded;
            this.getColor("ButtonLiteClr").value = values.GenClrShaded;
            this.getColor("ButtonLiteBgHover").value = this.ToHover(values.GenTheme, values.GenBgShaded);
            this.getColor("ButtonLiteClrHover").value = this.ToHover(values.GenTheme, values.GenClrShaded);
            this.getColor("ButtonLiteBgFocus").value = this.ToFocus(values.GenTheme, values.GenBgShaded);
            this.getColor("ButtonLiteClrFocus").value = this.ToFocus(values.GenTheme, values.GenClrShaded);
            this.getInput("ButtonLiteBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("ButtonLiteBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("PbarBg").value = values.GenBgShaded;
            this.getColor("PbarValueBg").value = values.GenClrShaded;
            this.getInput("PbarBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("PbarBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("TableBg").value = values.GenBg;
            this.getColor("TableClr").value = values.GenClr;
            this.getColor("TableBgHover").value = this.ToHover(values.GenTheme, values.GenBg);
            this.getColor("TableClrHover").value = this.ToHover(values.GenTheme, values.GenClr);
            this.getColor("TableBgActive").value = values.GenBgActive;
            this.getColor("TableClrActive").value = values.GenClrActive;
            this.getColor("TableBgFocus").value = this.ToFocus(values.GenTheme, values.GenBg);
            this.getColor("TableClrFocus").value = this.ToFocus(values.GenTheme, values.GenClr);
            this.getColor("TableBgHighlight").value = values.GenTheme === BasicThemeEnum.Light ? "silver" : "white";
            this.getColor("TableClrHighlight").value = values.GenTheme === BasicThemeEnum.Light ? "white" : "silver";
            this.getColor("TableBgLowlight").value = values.GenTheme === BasicThemeEnum.Light ? "ghostwhite" : "darkslategray";
            this.getColor("TableClrLowlight").value = values.GenTheme === BasicThemeEnum.Light ? "darkslategray" : "ghostwhite";
            this.getInput("TableFont").value = values.GenSmallFont;
            this.getInput("TableBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("TableBorderLite").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenTheme === BasicThemeEnum.Light ? this.Lighten(values.GenBorderClr, 20) : this.Darken(values.GenBorderClr, 20));
            this.getInput("TableBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getInput("TableShadow").value = "0px 0px 10px 0px rgba(0,0,0,0.1)";
            this.getColor("TableHeaderBg").value = values.GenBgShaded;
            this.getColor("TableHeaderClr").value = values.GenClrShaded;
            this.getColor("TableHeaderBgHover").value = this.ToHover(values.GenTheme, values.GenBgShaded);
            this.getColor("TableHeaderClrHover").value = this.ToHover(values.GenTheme, values.GenClrShaded);
            this.getColor("TableHeaderBgActive").value = values.GenBgShadedActive;
            this.getColor("TableHeaderClrActive").value = values.GenClrShadedActive;
            this.getInput("TableHeaderFont").value = values.GenSmallFont;
            this.getColor("TabsBg").value = values.GenBg;
            this.getColor("TabsClr").value = values.GenClr;
            this.getColor("TabsStripBg").value = values.GenTheme === BasicThemeEnum.Light ? this.Darken(values.GenBgShaded, 40) : this.Lighten(values.GenBgShaded, 40);
            this.getInput("TabsStripBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("TabsStripBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("TabsTabBg").value = values.GenBgShaded;
            this.getColor("TabsTabClr").value = values.GenClrShaded;
            this.getColor("TabsTabBgHover").value = this.ToHover(values.GenTheme, values.GenBgShaded);
            this.getColor("TabsTabClrHover").value = this.ToHover(values.GenTheme, values.GenClrShaded);
            this.getColor("TabsTabBgActive").value = values.GenBgShadedActive;
            this.getColor("TabsTabClrActive").value = values.GenClrShadedActive;
            this.getColor("TabsTabBgFocus").value = this.ToFocus(values.GenTheme, values.GenBgShadedActive);
            this.getColor("TabsTabClrFocus").value = this.ToFocus(values.GenTheme, values.GenClrShadedActive);
            this.getInput("TabsTabBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("TabsTabBorderRadius").value = "".concat(values.GenBorderRadius, "px");
            this.getColor("StepBg").value = values.GenBgShaded;
            this.getColor("StepClr").value = values.GenClrShaded;
            this.getColor("StepBgActive").value = values.GenBgShadedActive;
            this.getColor("StepClrActive").value = values.GenClrShadedActive;
            this.getColor("PanelBg").value = values.GenBgShaded;
            // this.getColor("PanelClr").value = values.GenClrShaded;
            this.getInput("PanelBorder").value = "".concat(values.GenBorderWidth, "px solid ").concat(values.GenBorderClr);
            this.getInput("PanelBorderRadius").value = ".25rem";
            this.getInput("PanelShadow").value = "0px 0px 10px 0px rgba(0,0,0,0.1)";
            this.getColor("SwitchBgOn").value = "#9386ec";
            this.getColor("SwitchClrOn").value = "#FFFFFF";
            this.getColor("SwitchBgOff").value = "#9ce1f5";
            this.getColor("SwitchClrOff").value = "#1a4c4c";
            this.getColor("SwitchBgSwitch").value = "#5e65ac";
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
                GenSmallFont: this.getInput("GenSmallFont").value,
                GenBorderClr: this.getInput("GenBorderClr").value,
                GenBorderWidth: Number(this.getInput("GenBorderWidth").value),
                GenBorderRadius: Number(this.getInput("GenBorderRadius").value),
                GenOpacity: this.getDecimal("BodyDisabledOpacity").value,
            };
        };
        /** Turn a basic color into a hover color */
        SkinPaletteModule.prototype.ToHover = function (theme, color) {
            if (theme === BasicThemeEnum.Light)
                return this.Darken(color, 20);
            else
                return this.Lighten(color, 20);
        };
        /** Turn an active color into a focus color */
        SkinPaletteModule.prototype.ToFocus = function (theme, color) {
            if (theme === BasicThemeEnum.Light)
                return this.Darken(color, 40);
            else
                return this.Lighten(color, 40);
        };
        SkinPaletteModule.prototype.getInput = function (name) {
            return $YetaWF.getElement1BySelector("[name='".concat(name, "']"), [this.Form]);
        };
        SkinPaletteModule.prototype.getColor = function (name) {
            var control = YetaWF_ComponentsHTML.ColorEditComponent.getControlFromSelector("[name='".concat(name, "']"), YetaWF_ComponentsHTML.ColorEditComponent.SELECTOR, [this.Form]);
            return control;
        };
        SkinPaletteModule.prototype.getIntValue = function (name) {
            var control = YetaWF_ComponentsHTML.IntValueEditComponent.getControlFromSelector("[name='".concat(name, "']"), YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Form]);
            return control;
        };
        SkinPaletteModule.prototype.getDecimal = function (name) {
            var control = YetaWF_ComponentsHTML.DecimalEditComponent.getControlFromSelector("[name='".concat(name, "']"), YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR, [this.Form]);
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
            return "#".concat(rr).concat(gg).concat(bb);
        };
        SkinPaletteModule.SELECTOR = ".YetaWF_SkinPalette_SkinPalette";
        return SkinPaletteModule;
    }(YetaWF.ModuleBaseDataImpl));
    YetaWF_SkinPalette.SkinPaletteModule = SkinPaletteModule;
})(YetaWF_SkinPalette || (YetaWF_SkinPalette = {}));

//# sourceMappingURL=SkinPalette.js.map
