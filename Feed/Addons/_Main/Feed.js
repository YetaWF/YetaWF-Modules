/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feed#License */

var YetaWF_Feed = {};

YetaWF_Feed.init = function (divId, interval) {
    'use strict';

    var $div = $('#' + divId);
    if ($div.length == 0) throw "no news div specified";/*DEBUG*/

    var entryTimer = undefined;

    function changeEntry() {

        // get the header entry
        var $headerentry = $('.t_headerentry', $div);
        if ($headerentry.length == 0) return;

        // get entry number to display
        var num = $('.t_header', $div).attr('data-entry');

        var $entry = $('.t_entry a', $div).eq(num);
        if ($entry.length == 0) {
            num = 0;
            $entry = $('.t_entry a', $div).eq(0);
            if ($entry.length == 0)
                return;
        }

        var formattedDate = $entry.attr('data-publishedDate');

        var newsEntry = "<a class='t_title' href='{0}' target='_blank'>{1}</a>".format($entry.attr('href'), $entry.text());
        newsEntry += "<div class='t_text'>{0}</div>".format($entry.attr('data-text'));
        var author = $entry.attr('data-author');
        newsEntry += "<div class='t_author'>{0}</div>".format(author);
        newsEntry += "<div class='t_date'>{0}</div>".format(formattedDate);

        $headerentry.html(newsEntry);
        // save next entry #
        $('.t_header', $div).attr('data-entry', ++num);
        $('a', $div).attr("target", "_blank"); // change all a tags to open a new window
    }

    changeEntry();
    if (interval > 0)
        entryTimer = setInterval(changeEntry, interval);

    // Listen for events that the page is changing
    $(document).on('YetaWF_Basics_PageChange', function (event) {
        // when the page is removed, we need to clean up
        if (entryTimer !== undefined) {
            clearInterval(entryTimer);
            entryTimer = undefined;
        }
    });
};

