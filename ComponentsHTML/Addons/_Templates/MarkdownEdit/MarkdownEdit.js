"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
/// <reference types="showdown" />
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var MarkdownEditComponent = /** @class */ (function (_super) {
        __extends(MarkdownEditComponent, _super);
        function MarkdownEditComponent(controlId /*, setup: MarkdownEditSetup*/) {
            var _this = _super.call(this, controlId, MarkdownEditComponent.TEMPLATE, MarkdownEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.TextArea,
                ChangeEvent: MarkdownEditComponent.EVENT,
                GetValue: function (control) {
                    return control.TextArea.value;
                },
                Enable: function (control, enable, clearOnDisable) { },
            }) || this;
            _this.TextArea = $YetaWF.getElement1BySelector("textarea", [_this.Control]);
            _this.Preview = $YetaWF.getElement1BySelector(".t_previewpane", [_this.Control]);
            _this.InputHTML = $YetaWF.getElement1BySelector(".t_html", [_this.Control]);
            $YetaWF.registerEventHandler(_this.TextArea, "blur", null, function (ev) {
                FormsSupport.validateElement(_this.TextArea);
                var event = document.createEvent("Event");
                event.initEvent(MarkdownEditComponent.EVENT, true, true);
                _this.Control.dispatchEvent(event);
                return true;
            });
            return _this;
        }
        MarkdownEditComponent.prototype.toHTML = function () {
            var converter = new showdown.Converter({ "headerLevelStart": 3, "simplifiedAutoLink": true, "excludeTrailingPunctuationFromURLs": true, "literalMidWordUnderscores": true });
            var html = converter.makeHtml(this.TextArea.value);
            this.Preview.innerHTML = html;
            this.InputHTML.value = html;
        };
        MarkdownEditComponent.prototype.makeVisible = function () {
            if ($YetaWF.isVisible(this.Preview)) {
                this.toHTML();
            }
        };
        MarkdownEditComponent.TEMPLATE = "yt_markdown";
        MarkdownEditComponent.SELECTOR = ".yt_markdown.t_edit";
        MarkdownEditComponent.EVENT = "markdown_change";
        return MarkdownEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.MarkdownEditComponent = MarkdownEditComponent;
    // Update rendered html before form submit
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Forms.EVENTPRESUBMIT, null, function (ev) {
        var mds = YetaWF.ComponentBaseDataImpl.getControls(MarkdownEditComponent.SELECTOR, [ev.detail.form]);
        for (var _i = 0, mds_1 = mds; _i < mds_1.length; _i++) {
            var md = mds_1[_i];
            md.toHTML();
        }
        return true;
    });
    // inner tab control switched
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTACTIVATEDIV, null, function (ev) {
        for (var _i = 0, _a = ev.detail.tags; _i < _a.length; _i++) {
            var tag = _a[_i];
            var md = MarkdownEditComponent.getControlFromTagCond(tag, MarkdownEditComponent.SELECTOR);
            if (md)
                md.makeVisible();
        }
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=MarkdownEdit.js.map
