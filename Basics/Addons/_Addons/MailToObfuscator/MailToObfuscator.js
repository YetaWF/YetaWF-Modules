/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

var _YetaWF_Basics_MailTo = {};
_YetaWF_Basics_MailTo.on = true;

// http://stackoverflow.com/questions/483212/effective-method-to-hide-email-from-spam-bots
YetaWF_Basics.addWhenReady(function (tag) {
    if (_YetaWF_Basics_MailTo.on) {
        // find all <a> YGenMailTo tags and format an email address
        $('a.YGenMailTo', $(tag)).each(function () {
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

// Handles events turning the addon on/off (used for dynamic content)
YetaWF_Basics.registerContentChange(function (addonGuid, on) {
    if (addonGuid == '749d0ca9-75e5-40b8-82e3-466a11d3b1d2') {
        _YetaWF_Basics_MailTo.on = on;
    }
});
