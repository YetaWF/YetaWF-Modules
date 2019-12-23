"use strict";
/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Feed#License */
var YetaWF_Feed;
(function (YetaWF_Feed) {
    var Feed = /** @class */ (function () {
        function Feed(divId, setup) {
            var _this = this;
            this.NextEntry = 0;
            this.EntryTimer = null;
            this.Div = $YetaWF.getElementById(divId);
            this.DivHeader = $YetaWF.getElement1BySelector(".t_headerentry", [this.Div]);
            this.Entries = $YetaWF.getElementsBySelector(".t_entry a", [this.Div]);
            this.changeEntry();
            if (setup.Interval)
                this.EntryTimer = setInterval(function () { _this.changeEntry(); }, setup.Interval);
            // change all a tags to open a new window
            var elems = $YetaWF.getElementsBySelector("a", [this.Div]);
            for (var _i = 0, elems_1 = elems; _i < elems_1.length; _i++) {
                var elem = elems_1[_i];
                $YetaWF.setAttribute(elem, "target", "_blank");
                $YetaWF.setAttribute(elem, "rel", "noopener noreferrer");
            }
            // Listen for events that the page is changing
            $YetaWF.registerPageChange(true, function () {
                // when the page is removed, we need to clean up
                if (_this.EntryTimer) {
                    clearInterval(_this.EntryTimer);
                    _this.EntryTimer = null;
                }
            });
        }
        Feed.prototype.changeEntry = function () {
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
        return Feed;
    }());
    YetaWF_Feed.Feed = Feed;
})(YetaWF_Feed || (YetaWF_Feed = {}));

//# sourceMappingURL=Feed.js.map
