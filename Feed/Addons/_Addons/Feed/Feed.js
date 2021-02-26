"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feed#License */
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
var YetaWF_Feed;
(function (YetaWF_Feed) {
    var FeedModule = /** @class */ (function (_super) {
        __extends(FeedModule, _super);
        function FeedModule(id, setup) {
            var _this = _super.call(this, id, FeedModule.SELECTOR, null, function (tag, module) {
                // when the page is removed, we need to clean up
                if (module.EntryTimer) {
                    clearInterval(module.EntryTimer);
                    module.EntryTimer = null;
                }
            }) || this;
            _this.NextEntry = 0;
            _this.EntryTimer = null;
            _this.DivHeader = $YetaWF.getElement1BySelector(".t_headerentry", [_this.Module]);
            _this.Entries = $YetaWF.getElementsBySelector(".t_entry a", [_this.Module]);
            _this.changeEntry();
            if (setup.Interval) {
                _this.EntryTimer = setInterval(function () {
                    _this.changeEntry();
                }, setup.Interval);
            }
            // change all a tags to open a new window
            var elems = $YetaWF.getElementsBySelector("a", [_this.Module]);
            for (var _i = 0, elems_1 = elems; _i < elems_1.length; _i++) {
                var elem = elems_1[_i];
                $YetaWF.setAttribute(elem, "target", "_blank");
                $YetaWF.setAttribute(elem, "rel", "noopener noreferrer");
            }
            return _this;
        }
        FeedModule.prototype.changeEntry = function () {
            var entry = this.Entries[this.NextEntry];
            var newsEntry = "<a class='t_title' href='" + entry.href + "' target='_blank' rel='noopener noreferrer'>" + entry.innerText + "</a>";
            var text = $YetaWF.getAttributeCond(entry, "data-text") || "";
            newsEntry += "<div class='t_text'>" + text + "</div>";
            var author = $YetaWF.getAttributeCond(entry, "data-author") || "";
            newsEntry += "<div class='t_author'>" + author + "</div>";
            var formattedDate = $YetaWF.getAttributeCond(entry, "data-publishedDate") || "";
            newsEntry += "<div class='t_date'>" + formattedDate + "</div>";
            this.DivHeader.innerHTML = newsEntry;
            ++this.NextEntry;
            if (this.NextEntry >= this.Entries.length)
                this.NextEntry = 0;
        };
        FeedModule.SELECTOR = ".YetaWF_Feed_Feed";
        return FeedModule;
    }(YetaWF.ModuleBaseDataImpl));
    YetaWF_Feed.FeedModule = FeedModule;
})(YetaWF_Feed || (YetaWF_Feed = {}));

//# sourceMappingURL=Feed.js.map
