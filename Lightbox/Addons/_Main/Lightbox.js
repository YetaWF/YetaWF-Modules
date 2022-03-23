"use strict";
/* eslint-disable @typescript-eslint/indent */
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Lightbox#License */
/*!
 * jQuery Lightbox ported to TypeScript - Original by Lokesh Dhakar - http://lokeshdhakar.com/projects/lightbox2/
 */
var YetaWF_Lightbox;
(function (YetaWF_Lightbox) {
    var LightboxModule = /** @class */ (function () {
        function LightboxModule() {
            var _this = this;
            this.LightboxOverlay = null;
            this.LightboxDiv = null;
            this.ImageLinks = null;
            this.ImageIndex = 0;
            $YetaWF.registerEventHandlerBody("click", "a[rel^=lightbox], area[rel^=lightbox], a[data-lightbox], area[data-lightbox]", function (ev) {
                if (!LightboxModule.On)
                    return true;
                var link = ev.__YetaWFElem;
                return !YetaWF_Lightbox_Module.open(link);
            });
            $YetaWF.registerEventHandlerBody("keydown", null, function (ev) {
                if (!LightboxModule.On)
                    return true;
                var key = ev.key;
                if (key === "Escape") {
                    _this.close();
                    return false;
                }
                else if (key === "ArrowRight") {
                    _this.nextImage();
                    return false;
                }
                else if (key === "ArrowLeft") {
                    _this.prevImage();
                    return false;
                }
                return true;
            });
            // Handles events turning the addon on/off (used for dynamic content)
            $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
                var addonGuid = ev.detail.addonGuid;
                var on = ev.detail.on;
                if (addonGuid === "39244dbc-0536-4c85-88d1-b84b504510ac") {
                    LightboxModule.On = on;
                }
                return true;
            });
            $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
                if (!LightboxModule.On)
                    return true;
                YetaWF_Lightbox_Module.setMaxSize();
                return true;
            });
        }
        LightboxModule.prototype.open = function (link) {
            $YetaWF.setLoading(true);
            $YetaWF.closeOverlays();
            if (this.getImages(link)) {
                this.addDivs();
                this.showImage();
                $YetaWF.setLoading(false);
                return true;
            }
            else {
                $YetaWF.setLoading(false);
                return false;
            }
        };
        LightboxModule.prototype.close = function () {
            if (this.LightboxDiv) {
                this.LightboxDiv.remove();
                this.LightboxDiv = null;
            }
            if (this.LightboxOverlay) {
                this.LightboxOverlay.remove();
                this.LightboxOverlay = null;
            }
        };
        LightboxModule.prototype.getImages = function (link) {
            var requestedGroup = $YetaWF.getAttributeCond(link, "data-lightbox") || "";
            var elems = $YetaWF.getElementsBySelector("[data-lightbox='".concat(requestedGroup, "']"));
            this.ImageLinks = [];
            var index = 0;
            for (var _i = 0, elems_1 = elems; _i < elems_1.length; _i++) {
                var elem = elems_1[_i];
                var title = $YetaWF.getAttributeCond(elem, "data-title");
                var group = $YetaWF.getAttributeCond(elem, "data-lightbox");
                if (group === requestedGroup) {
                    if (elem.tagName === "A")
                        this.ImageLinks.push({ Url: elem.href, Title: title || "", Group: group || "" });
                    else if (elem.tagName === "AREA")
                        this.ImageLinks.push({ Url: elem.href, Title: title || "", Group: group || "" });
                    else
                        throw "Unexpected tag ".concat(elem.tagName);
                    if (elem === link)
                        this.ImageIndex = index;
                    ++index;
                }
            }
            return this.ImageLinks.length > 0;
        };
        LightboxModule.prototype.showImage = function () {
            if (!this.LightboxDiv || !this.ImageLinks)
                return;
            var entry = this.ImageLinks[this.ImageIndex];
            var image = $YetaWF.getElement1BySelector(".t_image", [this.LightboxDiv]);
            image.src = entry.Url;
            var caption = $YetaWF.getElement1BySelector(".t_caption", [this.LightboxDiv]);
            caption.innerText = entry.Title;
            var imgNumber = $YetaWF.getElement1BySelector(".t_number", [this.LightboxDiv]);
            imgNumber.innerText = YLocs.YetaWF_Lightbox.ImageNumber.format(this.ImageIndex + 1, this.ImageLinks.length);
            this.setMaxSize();
        };
        LightboxModule.prototype.nextImage = function () {
            this.ImageIndex++;
            if (this.ImageIndex >= this.ImageLinks.length)
                this.ImageIndex = 0;
            this.showImage();
        };
        LightboxModule.prototype.prevImage = function () {
            this.ImageIndex--;
            if (this.ImageIndex < 0)
                this.ImageIndex = this.ImageLinks.length - 1;
            this.showImage();
        };
        LightboxModule.prototype.setMaxSize = function () {
            if (!this.LightboxDiv || !this.ImageLinks)
                return;
            var containerImg = $YetaWF.getElement1BySelector(".t_container img", [this.LightboxDiv]);
            var maxWidth = window.innerWidth - this.paddingWidth;
            var maxHeight = window.innerHeight - this.paddingHeight;
            containerImg.style.maxWidth = "".concat(maxWidth, "px");
            containerImg.style.maxHeight = "".concat(maxHeight, "px");
        };
        Object.defineProperty(LightboxModule.prototype, "paddingWidth", {
            get: function () {
                return $YetaWF.isMobile() ? 2 * 10 : 2 * 50;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(LightboxModule.prototype, "paddingHeight", {
            get: function () {
                return $YetaWF.isMobile() ? 20 + 50 : 2 * 50 + 50; // extra for caption
            },
            enumerable: false,
            configurable: true
        });
        LightboxModule.prototype.addDivs = function () {
            var _this = this;
            if (!this.LightboxOverlay) {
                this.LightboxOverlay = $YetaWF.createElement("div", { id: "lightboxOverlay", class: "lightboxOverlay" });
                document.body.appendChild(this.LightboxOverlay);
                $YetaWF.registerEventHandler(this.LightboxOverlay, "click", null, function (ev) {
                    _this.close();
                    return false;
                });
            }
            if (!this.LightboxDiv) {
                this.LightboxDiv =
                    $YetaWF.createElement("div", { id: "lightbox", class: "lightbox" },
                        $YetaWF.createElement("div", { class: "t_outerContainer" },
                            $YetaWF.createElement("div", { class: "t_container" },
                                $YetaWF.createElement("img", { class: "t_image", src: "" }),
                                $YetaWF.createElement("div", { class: "t_nav" },
                                    $YetaWF.createElement("a", { class: "t_prev", href: "" }),
                                    $YetaWF.createElement("a", { class: "t_next", href: "" })),
                                $YetaWF.createElement("div", { class: "t_loader" },
                                    $YetaWF.createElement("a", { class: "t_cancel" }))),
                            $YetaWF.createElement("div", { class: "t_data" },
                                $YetaWF.createElement("div", { class: "t_details" },
                                    $YetaWF.createElement("span", { class: "t_caption" }),
                                    $YetaWF.createElement("span", { class: "t_number" })),
                                $YetaWF.createElement("div", { class: "t_closeContainer" },
                                    $YetaWF.createElement("a", { class: "t_close" })))));
                document.body.appendChild(this.LightboxDiv);
                if (this.ImageLinks.length <= 1) {
                    var prev = $YetaWF.getElement1BySelector(".t_prev", [this.LightboxDiv]);
                    prev.style.display = "none";
                    var next = $YetaWF.getElement1BySelector(".t_next", [this.LightboxDiv]);
                    next.style.display = "none";
                }
                else {
                    $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".t_prev", [this.LightboxDiv]), "click", null, function (ev) {
                        _this.prevImage();
                        return false;
                    });
                    $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".t_next", [this.LightboxDiv]), "click", null, function (ev) {
                        _this.nextImage();
                        return false;
                    });
                }
                $YetaWF.registerEventHandler(this.LightboxDiv, "click", null, function (ev) {
                    _this.close();
                    return false;
                });
                $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".t_close", [this.LightboxDiv]), "click", null, function (ev) {
                    _this.close();
                    return false;
                });
            }
        };
        LightboxModule.On = true;
        return LightboxModule;
    }());
    YetaWF_Lightbox.LightboxModule = LightboxModule;
})(YetaWF_Lightbox || (YetaWF_Lightbox = {}));
var YetaWF_Lightbox_Module = new YetaWF_Lightbox.LightboxModule();

//# sourceMappingURL=Lightbox.js.map
