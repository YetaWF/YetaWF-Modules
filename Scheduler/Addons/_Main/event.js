/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

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
$(document).on("change", ".yt_yetawf_scheduler_event.t_edit select[name$='.DropDown']", function () {
    var $t = $(this);
    $div = _YetaWF_Scheduler_Event.getDiv($t);
    var args = $t.val().split(",");
    $("input[name$='.Name']", $div).val(args[0]);
    $("input[name$='.ImplementingAssembly']", $div).val(args[2]);
    $("input[name$='.ImplementingType']", $div).val(args[1]);
    $(".t_implasm", $div).text(args[2]);
    $(".t_impltype", $div).text(args[1]);
    var tip = YetaWF_TemplateDropDownList.getTitle($t, this.selectedIndex);
    $(".t_description", $div).text(tip);
});

YetaWF_Basics.whenReady.push({
    callback: function ($tag) {
        $(".yt_yetawf_scheduler_event.t_edit select[name$='.DropDown']", $tag).trigger("change");// to update all displayed info
    }
});