"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */
var YetaWF_ImageRepository;
(function (YetaWF_ImageRepository) {
    var FlashRepository = /** @class */ (function () {
        function FlashRepository(divId) {
            var _this = this;
            this.Control = $YetaWF.getElementById(divId);
            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]);
            this.List = $YetaWF.getElement1BySelector("select[name='List']", [this.Control]);
            this.ButtonDiv = $YetaWF.getElement1BySelector(".t_haveflash", [this.Control]);
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
                $YetaWF.setLoading(true);
                var uri = $YetaWF.parseUrl(_this.RemoveButton.href);
                uri.removeSearch("Name");
                uri.addSearch("Name", _this.Hidden.value);
                var request = new XMLHttpRequest();
                request.open("POST", uri.toUrl(), true);
                request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                request.onreadystatechange = function (ev) {
                    if (request.readyState === 4 /*DONE*/) {
                        $YetaWF.setLoading(false);
                        $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, undefined, undefined, function (result) {
                            $YetaWF.setLoading(false);
                            if (result.startsWith(YConfigs.Basics.AjaxJavascriptReturn)) {
                                var script = result.substring(YConfigs.Basics.AjaxJavascriptReturn.length);
                                // tslint:disable-next-line:no-eval
                                eval(script);
                                return;
                            }
                            else if (result.startsWith(YConfigs.Basics.AjaxJavascriptErrorReturn)) {
                                var script = result.substring(YConfigs.Basics.AjaxJavascriptErrorReturn.length);
                                // tslint:disable-next-line:no-eval
                                eval(script);
                                return;
                            }
                            var resp = JSON.parse(result);
                            // tslint:disable-next-line:no-eval
                            eval(resp.Result);
                            _this.List.innerHTML = resp.List;
                            _this.clearFileName();
                        });
                    }
                };
                request.send();
                return false;
            });
        }
        FlashRepository.prototype.setPreview = function (name) {
            this.ButtonDiv.style.display = (name && name.length > 0) ? "" : "none";
            var param = $YetaWF.getElement1BySelectorCond(".t_preview param[name='movie']", [this.Control]);
            var embed = $YetaWF.getElement1BySelectorCond(".t_preview embed", [this.Control]);
            var obj = $YetaWF.getElement1BySelectorCond(".t_preview object", [this.Control]);
            if (obj) {
                // change object data= (if present)
                if (obj && obj.data) {
                    var currUri = $YetaWF.parseUrl(obj.data);
                    currUri.removeSearch("Name");
                    currUri.addSearch("Name", name);
                    obj.data = currUri.toUrl();
                }
                // change param movie (if present)
                if (param && param.value) {
                    var currUri = $YetaWF.parseUrl(param.value);
                    currUri.removeSearch("Name");
                    currUri.addSearch("Name", name);
                    param.value = currUri.toUrl();
                }
                // change embed (if present)
                if (embed && embed.src) {
                    currUri = $YetaWF.parseUrl(embed.src);
                    currUri.removeSearch("Name");
                    currUri.addSearch("Name", name);
                    embed.src = currUri.toUrl();
                }
                var s = obj.outerHTML;
                obj.outerHTML = s; // replace entire object to make flash recognize the image change
            }
        };
        FlashRepository.prototype.clearFileName = function () {
            this.Hidden.value = "";
            this.List.value = "";
            this.setPreview(null);
        };
        return FlashRepository;
    }());
    YetaWF_ImageRepository.FlashRepository = FlashRepository;
})(YetaWF_ImageRepository || (YetaWF_ImageRepository = {}));

//# sourceMappingURL=FlashSelection.js.map
