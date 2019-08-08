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
    var TextAreaSourceOnlyEditComponent = /** @class */ (function (_super) {
        __extends(TextAreaSourceOnlyEditComponent, _super);
        function TextAreaSourceOnlyEditComponent(controlId /*, setup: TextAreaSourceOnlyEditSetup*/) {
            return _super.call(this, controlId, TextAreaSourceOnlyEditComponent.TEMPLATE, TextAreaSourceOnlyEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.TextArea,
                ChangeEvent: null,
                GetValue: function (control) {
                    return control.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    $YetaWF.elementEnableToggle(control, enable);
                    if (clearOnDisable)
                        control.value = "";
                },
            }) || this;
            //this.Setup = setup;
        }
        TextAreaSourceOnlyEditComponent.initAll = function (tag) {
            if (TextAreaSourceOnlyEditComponent.clip != null)
                return;
            //var elems = $YetaWF.getElementsBySelector(".yt_textareasourceonly_copy", [tag]);
            //if (elems.length === 0) return;
            TextAreaSourceOnlyEditComponent.clip = new ClipboardJS(".yt_textareasourceonly_copy", {
                target: function (trigger) {
                    return trigger.previousElementSibling;
                },
            });
            TextAreaSourceOnlyEditComponent.clip.on("success", function (e) {
                $YetaWF.confirm(YLocs.YetaWF_ComponentsHTML.CopyToClip);
            });
        };
        TextAreaSourceOnlyEditComponent.TEMPLATE = "yt_textareasourceonly";
        TextAreaSourceOnlyEditComponent.SELECTOR = ".yt_textareasourceonly.t_edit";
        TextAreaSourceOnlyEditComponent.clip = null;
        return TextAreaSourceOnlyEditComponent;
    }(YetaWF.ComponentBaseNoDataImpl));
    YetaWF_ComponentsHTML.TextAreaSourceOnlyEditComponent = TextAreaSourceOnlyEditComponent;
    $YetaWF.addWhenReady(TextAreaSourceOnlyEditComponent.initAll);
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=TextAreaSourceOnlyEdit.js.map
