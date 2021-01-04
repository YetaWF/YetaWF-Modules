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
    var SkinPaletteModule = /** @class */ (function (_super) {
        __extends(SkinPaletteModule, _super);
        function SkinPaletteModule(id, setup) {
            var _this = _super.call(this, id, SkinPaletteModule.SELECTOR, null) || this;
            _this.Setup = setup;
            _this.ExpandCollapse = $YetaWF.getElement1BySelector(".t_palette", [_this.Module]);
            _this.Tabs = YetaWF_ComponentsHTML.TabsComponent.getControlFromSelector(".yt_tabs", YetaWF_ComponentsHTML.TabsComponent.SELECTOR, [_this.Module]);
            _this.Contents = $YetaWF.getElement1BySelector(".t_contents", [_this.Module]);
            _this.Config = $YetaWF.getElement1BySelector("textarea[name='CSSVariables']", [_this.Module]);
            _this.ConfigApply = $YetaWF.getElement1BySelector("a[data-name='Apply']", [_this.Module]);
            _this.populate();
            $YetaWF.registerEventHandler(_this.ExpandCollapse, "click", null, function (ev) {
                if (_this.Contents.style.display === "") {
                    _this.Contents.style.display = "none";
                }
                else {
                    _this.Contents.style.display = "";
                }
                return false;
            });
            $YetaWF.registerEventHandler(_this.ConfigApply, "click", null, function (ev) {
                _this.updateSkin();
                return false;
            });
            _this.Tabs.Control.addEventListener(YetaWF_ComponentsHTML.TabsComponent.EVENTSWITCHED, function (evt) {
                _this.updateConfig();
            });
            return _this;
        }
        SkinPaletteModule.prototype.populate = function () {
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
            this.populate();
        };
        SkinPaletteModule.SELECTOR = ".YetaWF_SkinPalette_SkinPalette";
        return SkinPaletteModule;
    }(YetaWF.ModuleBaseDataImpl));
    YetaWF_SkinPalette.SkinPaletteModule = SkinPaletteModule;
})(YetaWF_SkinPalette || (YetaWF_SkinPalette = {}));

//# sourceMappingURL=SkinPalette.js.map
