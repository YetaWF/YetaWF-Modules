"use strict";
/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var FileUpload1Component = /** @class */ (function (_super) {
        __extends(FileUpload1Component, _super);
        function FileUpload1Component(controlId, setup) {
            var _this = _super.call(this, controlId, FileUpload1Component.TEMPLATE, FileUpload1Component.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: function (control) {
                    return null;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                }
            }) || this;
            _this.SuccessfullUploadCallback = null;
            _this.GetFileNameCallback = null;
            _this.Setup = setup;
            _this.UploadButton = $YetaWF.getElement1BySelector(".t_upload", [_this.Control]);
            _this.InputFileName = $YetaWF.getElement1BySelector("input.t_filename", [_this.Control]);
            _this.ProgressBar = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond(YetaWF_ComponentsHTML.ProgressBarComponent.SELECTOR, YetaWF_ComponentsHTML.ProgressBarComponent.SELECTOR, [_this.Control]);
            if (_this.ProgressBar)
                _this.ProgressBar.hide();
            $YetaWF.registerEventHandler(_this.UploadButton, "click", null, function (ev) {
                _this.InputFileName.click();
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "drop", null, function (ev) {
                if (!$YetaWF.isEnabled(_this.Control))
                    return false;
                if (!ev.dataTransfer || !ev.dataTransfer.files || ev.dataTransfer.files.length !== 1) {
                    $YetaWF.error(YLocs.YetaWF_ComponentsHTML.Only1FileSupported);
                    return false;
                }
                var files = ev.dataTransfer.files;
                _this.InputFileName.files = files;
                _this.uploadFile();
                return false;
            });
            $YetaWF.registerEventHandler(_this.InputFileName, "change", null, function (ev) {
                _this.uploadFile();
                return false;
            });
            return _this;
        }
        FileUpload1Component.prototype.uploadFile = function () {
            var _this = this;
            var fd = new FormData();
            $YetaWF.setLoading(true);
            if (this.ProgressBar)
                this.ProgressBar.show();
            var request = new XMLHttpRequest();
            request.open("POST", this.Setup.SaveUrl, true);
            //request.setRequestHeader // doesn't work
            fd.append("__filename", this.InputFileName.files[0]);
            if (this.GetFileNameCallback) {
                var filename = this.GetFileNameCallback();
                fd.append("__lastInternalName", filename); // the previous real filename of the file to remove
            }
            if (this.Setup.SerializeForm) {
                var form = $YetaWF.Forms.getForm(this.Control);
                var formData = $YetaWF.Forms.serializeFormArray(form);
                for (var _i = 0, formData_1 = formData; _i < formData_1.length; _i++) {
                    var f = formData_1[_i];
                    fd.append(f.name, f.value);
                }
            }
            request.upload.onprogress = function (ev) {
                var percent = 0;
                var position = ev.loaded;
                var total = ev.total;
                if (ev.lengthComputable) {
                    percent = Math.ceil(position / total * 100);
                    if (_this.ProgressBar)
                        _this.ProgressBar.value = Number(percent);
                }
            };
            $YetaWF.handleReadyStateChange(request, function (success, response) {
                if (_this.ProgressBar) {
                    _this.ProgressBar.hide();
                    _this.ProgressBar.reset();
                }
                if (success) {
                    _this.InputFileName.files = null;
                    _this.InputFileName.value = "";
                    if (_this.SuccessfullUploadCallback)
                        _this.SuccessfullUploadCallback(response);
                    if (response.Result) {
                        // eslint-disable-next-line no-eval
                        eval(response.Result);
                    }
                }
            });
            request.send(fd);
        };
        // API
        FileUpload1Component.prototype.RemoveFile = function (name) {
            $YetaWF.post(this.Setup.RemoveUrl, null, function (success, data) {
                if (success && data.Result)
                    $YetaWF.message(data.Result);
            });
        };
        FileUpload1Component.prototype.SetSuccessfullUpload = function (callback) {
            this.SuccessfullUploadCallback = callback;
        };
        FileUpload1Component.prototype.SetGetFileName = function (callback) {
            this.GetFileNameCallback = callback;
        };
        FileUpload1Component.prototype.enable = function (enabled) {
            $YetaWF.elementEnableToggle(this.Control, enabled);
            $YetaWF.elementEnableToggle(this.UploadButton, enabled);
        };
        FileUpload1Component.TEMPLATE = "yt_fileupload1";
        FileUpload1Component.SELECTOR = ".yt_fileupload1";
        return FileUpload1Component;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.FileUpload1Component = FileUpload1Component;
    // Disable document d&d events to prevent opening the file when we drop it
    document.addEventListener("dragenter", function (ev) {
        ev.stopPropagation();
        ev.preventDefault();
        return false;
    });
    document.addEventListener("dragover", function (ev) {
        ev.stopPropagation();
        ev.preventDefault();
        return false;
    });
    document.addEventListener("drop", function (ev) {
        ev.stopPropagation();
        ev.preventDefault();
        return false;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=FileUpload1.js.map
