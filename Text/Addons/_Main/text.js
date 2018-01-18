/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

$(document).on('click', '.YetaWF_Text .FAQ_Q', function (e) {
    var $this = $(this);
    $this.next('.FAQ_A').toggle();
});
