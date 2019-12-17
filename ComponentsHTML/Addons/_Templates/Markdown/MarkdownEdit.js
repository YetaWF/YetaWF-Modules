"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
    var MarkdownEditComponent = /** @class */ (function (_super) {
        __extends(MarkdownEditComponent, _super);
        function MarkdownEditComponent(controlId /*, setup: MarkdownEditSetup*/) {
            var _this = _super.call(this, controlId, MarkdownEditComponent.TEMPLATE, MarkdownEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: function (control) {
                    return control.TextArea.value;
                },
                Enable: function (control, enable, clearOnDisable) { },
            }) || this;
            _this.TextArea = $YetaWF.getElement1BySelector("textarea", [_this.Control]);
            _this.Preview = $YetaWF.getElement1BySelector(".t_previewpane", [_this.Control]);
            _this.InputHTML = $YetaWF.getElement1BySelector(".t_html", [_this.Control]);
            // inner tab control switched
            $YetaWF.registerActivateDiv(function (div) {
                var md = $YetaWF.elementClosestCond(div, MarkdownEditComponent.SELECTOR);
                if (md === _this.Control) {
                    if ($YetaWF.isVisible(_this.Preview)) {
                        _this.toHTML();
                    }
                }
            });
            // Update rendered html before form submit
            $YetaWF.Forms.addPreSubmitHandler(true, {
                form: $YetaWF.Forms.getForm(_this.Control),
                callback: function (entry) {
                    _this.toHTML();
                },
                userdata: _this
            });
            return _this;
        }
        MarkdownEditComponent.prototype.toHTML = function () {
            var converter = new showdown.Converter({ "headerLevelStart": 3, "simplifiedAutoLink": true, "excludeTrailingPunctuationFromURLs": true, "literalMidWordUnderscores": true });
            var html = converter.makeHtml(this.TextArea.value);
            this.Preview.innerHTML = html;
            this.InputHTML.value = html;
        };
        MarkdownEditComponent.TEMPLATE = "yt_markdown";
        MarkdownEditComponent.SELECTOR = ".yt_markdown.t_edit";
        return MarkdownEditComponent;
    }(YetaWF.ComponentBaseNoDataImpl));
    YetaWF_ComponentsHTML.MarkdownEditComponent = MarkdownEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=MarkdownEdit.js.map
