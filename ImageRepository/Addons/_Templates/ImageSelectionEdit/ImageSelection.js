"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */
var YetaWF_ImageRepository;
(function (YetaWF_ImageRepository) {
    var ImageRepository = /** @class */ (function () {
        function ImageRepository(divId) {
            var _this = this;
            this.Control = $YetaWF.getElementById(divId);
            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]);
            this.List = $YetaWF.getElement1BySelector("select[name='List']", [this.Control]);
            this.Image = $YetaWF.getElement1BySelector(".t_preview img", [this.Control]);
            this.ButtonDiv = $YetaWF.getElement1BySelector(".t_haveimage", [this.Control]);
            this.ClearButton = $YetaWF.getElement1BySelector("a[data-name='Clear']", [this.Control]);
            this.RemoveButton = $YetaWF.getElement1BySelector("a[data-name='Remove']", [this.Control]);
            // show initial selection (if any)
            this.List.value = this.Hidden.value;
            this.setPreview(this.List.value);
            this.UploadControl = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond(".yt_fileupload1", YetaWF_ComponentsHTML.FileUpload1Component.SELECTOR, [this.Control]);
            if (this.UploadControl) {
                this.UploadControl.SetSuccessfullUpload(function (response) {
                    _this.Hidden.value = response.FileName;
                    _this.List.innerHTML = response.List;
                    _this.List.value = response.FileName;
                    _this.setPreview(response.FileName);
                });
            }
            // user changed the selected image
            $YetaWF.registerEventHandler(this.List, "change", null, function (ev) {
                _this.Hidden.value = _this.List.value;
                _this.setPreview(_this.List.value);
                return false;
            });
            $YetaWF.registerEventHandler(this.ClearButton, "click", null, function (ev) {
                _this.clearFileName();
                return false;
            });
            $YetaWF.registerEventHandler(this.RemoveButton, "click", null, function (ev) {
                // get url to remove the file
                if ($YetaWF.isLoading)
                    return false;
                var uri = $YetaWF.parseUrl(_this.RemoveButton.href);
                uri.removeSearch("Name");
                uri.addSearch("Name", _this.Hidden.value);
                $YetaWF.post(uri.toUrl(), null, function (success, resp) {
                    if (success) {
                        // eslint-disable-next-line no-eval
                        eval(resp.Result);
                        _this.List.innerHTML = resp.List;
                        _this.clearFileName();
                    }
                });
                return false;
            });
        }
        ImageRepository.prototype.setPreview = function (name) {
            this.ButtonDiv.style.display = (name && name.length > 0) ? "" : "none";
            var currUri = $YetaWF.parseUrl(this.Image.src);
            currUri.removeSearch("Name");
            currUri.addSearch("Name", name);
            this.Image.src = currUri.toUrl();
        };
        ImageRepository.prototype.clearFileName = function () {
            this.Hidden.value = "";
            this.List.value = "";
            this.setPreview(null);
        };
        return ImageRepository;
    }());
    YetaWF_ImageRepository.ImageRepository = ImageRepository;
})(YetaWF_ImageRepository || (YetaWF_ImageRepository = {}));

//# sourceMappingURL=ImageSelection.js.map
