/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Scheduler#License */

/* Scheduler Event Control */

var YetaWF_Scheduler_Event = {};

// Private
var _YetaWF_Scheduler_Event = {};

// Get div enclosing the scheduler event selection controls
_YetaWF_Scheduler_Event.getDiv = function (obj) {
    var $div = $(obj).closest('.yt_yetawf_scheduler_event');
    if ($div.length != 1) throw "couldn't find parent div";/*DEBUG*/
    return $div;
}
$(document).ready(function () {
    $("body").on("change", ".yt_yetawf_scheduler_event.t_edit select[name$='.DropDown']", function () {
        var $t = $(this);
        $div = _YetaWF_Scheduler_Event.getDiv($t);
        var args = $t.val().split(",");
        $("input[name$='.Name']", $div).val(args[0]);
        $("input[name$='.ImplementingType']", $div).val(args[1]);
        $("input[name$='.ImplementingAssembly']", $div).val(args[2]);
        $(".t_implasm", $div).text(args[1]);
        $(".t_impltype", $div).text(args[2]);
        $(".t_description", $div).text(this.options[this.selectedIndex].title);
    });
    $(".yt_yetawf_scheduler_event.t_edit select[name$='.DropDown']").trigger("change");// to update all displayed info
});

