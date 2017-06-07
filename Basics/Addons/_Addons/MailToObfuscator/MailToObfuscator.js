/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

// http://stackoverflow.com/questions/483212/effective-method-to-hide-email-from-spam-bots
YetaWF_Basics.whenReady.push({
    callback: function ($tag) {
        // find all <a> YGenMailTo tags and format an email address
        $('a.YGenMailTo', $tag).each(function () {
            var $this = $(this);
            var addr = $this.attr('data-name') + "@" + $this.attr('data-domain');
            var s = 'mailto:' + addr;
            var subj = $this.attr('data-subject');
            if (subj !== undefined) {
                subj = subj.replace(' ', '+');
                subj = $('<div/>').html(subj + '').text();
                s += '?subject=' + subj;
            }
            var text = $this.attr('data-text');
            if (text === undefined)
                $this.text(addr);
            else
                $this.text(text);
            $this.attr('href', s);
        });
    }
});

