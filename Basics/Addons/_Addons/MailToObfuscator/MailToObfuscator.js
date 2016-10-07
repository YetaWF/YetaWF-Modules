/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

// http://stackoverflow.com/questions/483212/effective-method-to-hide-email-from-spam-bots
function YGenMailTo(name, domain, subject, linkText) {
    'use strict';
    var $a = $('<a>')
    $a.addClass('ymailto');
    if (linkText === undefined)
        linkText = "{0}@{1}".format(name, domain);
    $a.text(linkText);
    if (subject !== undefined) {
        subject = subject.replace(' ', '+');
        subject = $('<div/>').html(subject + '').text()
    } else
        subject = ""
    $a.attr('href', 'mailto:{0}@{1}?subject={2}'.format(name, domain, subject))
    document.write($a[0].outerHTML)
}