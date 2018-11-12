"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
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
            var _this = _super.call(this, controlId) || this;
            _this.$divProgressbar = null;
            // API
            _this.RemoveFile = function (name) {
                $.ajax({
                    url: _this.Setup.RemoveUrl,
                    type: "post",
                    data: "__internalName=" + encodeURIComponent(name) + "&__filename=" + encodeURIComponent(name),
                    success: function (result, textStatus, jqXHR) { },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $YetaWF.alert(YLocs.Forms.AjaxError.format(jqXHR.status, jqXHR.statusText), YLocs.Forms.AjaxErrorTitle);
                    }
                });
            };
            _this.Setup = setup;
            $YetaWF.addObjectDataById(controlId, _this);
            _this.inputFileName = $YetaWF.getElement1BySelector("input.t_filename", [_this.Control]);
            _this.divProgressbar = $YetaWF.getElement1BySelectorCond(".t_progressbar", [_this.Control]);
            if (_this.divProgressbar) {
                _this.$divProgressbar = $(_this.divProgressbar);
                _this.$divProgressbar.progressbar({
                    max: 100,
                    value: 0,
                });
                _this.$divProgressbar.hide();
            }
            _this.$Control = $(_this.Control);
            // trigger upload button
            $YetaWF.registerEventHandler(_this.Control, "click", ".t_upload", function (ev) {
                $(_this.inputFileName).trigger("click");
                return false;
            });
            // Uploader control
            $(_this.$Control).dmUploader({
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
                    if (_this.$Control.data().getFileName !== undefined) {
                        var filename = _this.$Control.data().getFileName();
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
                    if (_this.$divProgressbar)
                        _this.$divProgressbar.hide();
                },
                onUploadProgress: function (id, percent) {
                    if (_this.$divProgressbar) {
                        _this.$divProgressbar.show();
                        _this.$divProgressbar.progressbar("value", percent);
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
                    if (data.startsWith(YConfigs.Basics.AjaxJavascriptReturn)) {
                        var script = data.substring(YConfigs.Basics.AjaxJavascriptReturn.length);
                        // tslint:disable-next-line:no-eval
                        eval(script);
                        return;
                    }
                    if (data.startsWith(YConfigs.Basics.AjaxJavascriptErrorReturn)) {
                        var script = data.substring(YConfigs.Basics.AjaxJavascriptErrorReturn.length);
                        // tslint:disable-next-line:no-eval
                        eval(script);
                        return;
                    }
                    // result has quotes around it
                    var js = JSON.parse(data);
                    // tslint:disable-next-line:no-eval
                    eval(js.result);
                    if (_this.$Control.data().successfullUpload !== undefined)
                        _this.$Control.data().successfullUpload(js);
                    //if ($control.data().setAttributes != undefined)
                    //    filename = $control.data().setAttributes('');
                },
            });
            return _this;
        }
        FileUpload1Component.prototype.destroy = function () {
            $YetaWF.removeObjectDataById(this.Control.id);
        };
        FileUpload1Component.getControlFromTag = function (elem) { return _super.getControlBaseFromTag.call(this, elem, FileUpload1Component.SELECTOR); };
        FileUpload1Component.getControlFromSelector = function (selector, tags) { return _super.getControlBaseFromSelector.call(this, selector, FileUpload1Component.SELECTOR, tags); };
        FileUpload1Component.getControlById = function (id) { return _super.getControlBaseById.call(this, id, FileUpload1Component.SELECTOR); };
        FileUpload1Component.SELECTOR = ".yt_fileupload1";
        return FileUpload1Component;
    }(YetaWF.ComponentBase));
    YetaWF_ComponentsHTML.FileUpload1Component = FileUpload1Component;
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBase.clearDiv(tag, FileUpload1Component.SELECTOR, function (control) {
            control.destroy();
        });
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=FileUpload1.js.map
