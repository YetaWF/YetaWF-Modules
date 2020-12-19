"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
            _this.uploadButton = $YetaWF.getElement1BySelector(".t_upload", [_this.Control]);
            _this.inputFileName = $YetaWF.getElement1BySelector("input.t_filename", [_this.Control]);
            _this.ProgressBar = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond(YetaWF_ComponentsHTML.ProgressBarComponent.SELECTOR, YetaWF_ComponentsHTML.ProgressBarComponent.SELECTOR, [_this.Control]);
            if (_this.ProgressBar)
                _this.ProgressBar.hide();
            _this.$Control = $(_this.Control);
            // trigger upload button
            $YetaWF.registerEventHandler(_this.uploadButton, "click", null, function (ev) {
                $(_this.inputFileName).trigger("click");
                return false;
            });
            // Uploader control
            _this.$Control.dmUploader({
                url: _this.Setup.SaveUrl,
                //dataType: 'json',  //don't use otherwise response is not recognized in case of errors
                //allowedTypes: '*',
                //extFilter: 'jpg,png,gif',
                fileName: "__filename",
                onInit: function () { },
                onBeforeUpload: function (id) {
                    $YetaWF.setLoading(true);
                },
                onExtraData: function (id, data) {
                    if (_this.GetFileNameCallback) {
                        var filename = _this.GetFileNameCallback();
                        data.append("__lastInternalName", filename); // the previous real filename of the file to remove
                    }
                    if (_this.Setup.SerializeForm) {
                        var form = $YetaWF.Forms.getForm(_this.Control);
                        var formData = $YetaWF.Forms.serializeFormArray(form);
                        for (var _i = 0, formData_1 = formData; _i < formData_1.length; _i++) {
                            var f = formData_1[_i];
                            data.append(f.name, f.value);
                        }
                    }
                },
                onNewFile: function (id, file) {
                    console.log("onNewFile #" + id + " " + file);
                },
                onComplete: function () {
                    if (_this.ProgressBar)
                        _this.ProgressBar.hide();
                },
                onUploadProgress: function (id, percent) {
                    if (_this.ProgressBar) {
                        _this.ProgressBar.show();
                        _this.ProgressBar.value = Number(percent);
                    }
                },
                onUploadError: function (id, message) {
                    $YetaWF.setLoading(false);
                    if (message === "")
                        $YetaWF.error(YLocs.YetaWF_ComponentsHTML.StatusUploadNoResp);
                    else
                        $YetaWF.error(YLocs.YetaWF_ComponentsHTML.StatusUploadFailed.format(message));
                },
                onFileTypeError: function (file) {
                    $YetaWF.error(YLocs.YetaWF_ComponentsHTML.FileTypeError);
                },
                onFileSizeError: function (file) {
                    $YetaWF.error(YLocs.YetaWF_ComponentsHTML.FileSizeError);
                },
                onFallbackMode: function (message) {
                    $YetaWF.error(YLocs.YetaWF_ComponentsHTML.FallbackMode);
                },
                onUploadSuccess: function (id, data) {
                    //{
                    //    "result":      "$YetaWF.confirm(\"Image \\\"logo_233x133.jpg\\\" successfully uploaded\");",
                    //    "filename": "tempc8eb1eb6-31ef-4e5d-9100-9fab50761a81.jpg",
                    //    "realFilename": "logo_233x133.jpg",
                    //    "attributes": "233 x 123 (w x h)"
                    //}
                    $YetaWF.setLoading(false);
                    if (typeof data === "string") {
                        if (data.startsWith(YConfigs.Basics.AjaxJavascriptReturn)) {
                            var script = data.substring(YConfigs.Basics.AjaxJavascriptReturn.length);
                            // eslint-disable-next-line no-eval
                            eval(script);
                            return;
                        }
                        if (data.startsWith(YConfigs.Basics.AjaxJavascriptErrorReturn)) {
                            var script = data.substring(YConfigs.Basics.AjaxJavascriptErrorReturn.length);
                            // eslint-disable-next-line no-eval
                            eval(script);
                            return;
                        }
                        throw "Unexpected return value " + data;
                    }
                    // result has quotes around it
                    if (_this.SuccessfullUploadCallback)
                        _this.SuccessfullUploadCallback(data);
                    // eslint-disable-next-line no-eval
                    eval(data.Result);
                },
            });
            return _this;
        }
        // API
        FileUpload1Component.prototype.RemoveFile = function (name) {
            $.ajax({
                url: this.Setup.RemoveUrl,
                type: "post",
                data: "__internalName=" + encodeURIComponent(name) + "&__filename=" + encodeURIComponent(name),
                success: function (result, textStatus, jqXHR) { },
                error: function (jqXHR, textStatus, errorThrown) {
                    $YetaWF.error(YLocs.Forms.AjaxError.format(jqXHR.status, jqXHR.statusText), YLocs.Forms.AjaxErrorTitle);
                }
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
            $YetaWF.elementEnableToggle(this.uploadButton, enabled);
        };
        FileUpload1Component.TEMPLATE = "yt_fileupload1";
        FileUpload1Component.SELECTOR = ".yt_fileupload1";
        return FileUpload1Component;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.FileUpload1Component = FileUpload1Component;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=FileUpload1.js.map
