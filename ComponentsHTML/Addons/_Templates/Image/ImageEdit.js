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
    var ImageEditComponent = /** @class */ (function (_super) {
        __extends(ImageEditComponent, _super);
        function ImageEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, ImageEditComponent.TEMPLATE, ImageEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: function (control) {
                    if (control.HiddenInput.value === ImageEditComponent.CLEAREDFILE)
                        return null;
                    return control.HiddenInput.value;
                },
                Enable: null,
            }) || this;
            _this.Setup = setup;
            _this.UploadControl = YetaWF.ComponentBaseDataImpl.getControlById(_this.Setup.UploadId, YetaWF_ComponentsHTML.FileUpload1Component.SELECTOR);
            _this.PreviewImg = $YetaWF.getElement1BySelector(".t_preview", [_this.Control]);
            _this.HiddenInput = $YetaWF.getElement1BySelector("input[type='hidden']", [_this.Control]);
            _this.HaveImageDiv = $YetaWF.getElement1BySelector(".t_haveimage", [_this.Control]);
            if (_this.HiddenInput.value === ImageEditComponent.CLEAREDFILE)
                _this.HiddenInput.value = "";
            // set upload control settings
            _this.UploadControl.SetSuccessfullUpload(function (data) {
                _this.HiddenInput.value = data.FileName;
                _this.setPreview(data.FileName);
                _this.HaveImageDiv.style.display = data.FileName.length > 0 ? "block" : "none";
            });
            _this.UploadControl.SetGetFileName(function () { return _this.HiddenInput.value; });
            // handle the clear button
            $YetaWF.registerEventHandler(_this.Control, "click", "input.t_clear", function (ev) {
                _this.UploadControl.RemoveFile(_this.HiddenInput.value);
                _this.clearFileName();
                return false;
            });
            if (_this.HiddenInput.value.length === 0)
                _this.clearFileName();
            return _this;
        }
        ImageEditComponent.prototype.clearFileName = function () {
            this.HiddenInput.value = ImageEditComponent.CLEAREDFILE;
            this.setPreview("");
            this.HaveImageDiv.style.display = "none";
        };
        ImageEditComponent.prototype.setPreview = function (name) {
            var currUri = $YetaWF.parseUrl(this.PreviewImg.src);
            currUri.removeSearch("Name");
            currUri.addSearch("Name", name);
            this.PreviewImg.src = currUri.toUrl();
        };
        ImageEditComponent.TEMPLATE = "yt_image";
        ImageEditComponent.SELECTOR = ".yt_image.t_edit";
        ImageEditComponent.CLEAREDFILE = "(CLEARED)";
        return ImageEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.ImageEditComponent = ImageEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=ImageEdit.js.map
