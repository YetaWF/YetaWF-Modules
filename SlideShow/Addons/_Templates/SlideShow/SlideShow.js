/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SlideShow#License */

var YetaWF_SlideShow = {};

YetaWF_SlideShow.init = function (divId) {
    var TEMPLATENAME = 'YetaWF_SlideShow_SlideShowInfo';
    var $control = $('#' + divId);
    if ($control.length != 1) throw "div not found";/*DEBUG*/

    // disable the first << button
    $('input.t_up', $control).filter(":first").attr("disabled", "disabled");
    // disable the last >> button
    $('input.t_down', $control).filter(":last").attr("disabled", "disabled");
    // disable if there is only only Remove button
    var $dels = $('input.t_delete', $control);
    if ($dels.length <= 1)
        $dels.attr("disabled", "disabled");

    function getSlideIndex(obj) {
        var $obj = $(obj);
        var $slide = $obj.closest('.t_slide');
        if ($slide.length != 1) throw "Can't find slide";/*DEBUG*/
        var index = $('input[name$=".Index"]', $slide).val();
        if (index == undefined) throw "Can't find slide index (hidden input)";/*DEBUG*/
        return index;
    }

    // << button click
    $('input.t_up', $control).on('click', function () {
        var slideIndex = getSlideIndex(this);
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_SlideShow.Action_MoveLeft, slideIndex);
    });
    // >> button click
    $('input.t_down', $control).on('click', function () {
        var slideIndex = getSlideIndex(this);
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_SlideShow.Action_MoveRight, slideIndex);
    });
    // delete button click
    $dels.on('click', function () {
        var btn = this;
        Y_AlertYesNo(YLocs.YetaWF_SlideShow.RemoveConfirm, YLocs.YetaWF_SlideShow.RemoveTitle, function () {
            var slideIndex = getSlideIndex(btn);
            YetaWF_Forms.submitTemplate(btn, true, TEMPLATENAME, YConfigs.YetaWF_SlideShow.Action_Remove, slideIndex);
        });
    });
    // Insert button click
    $('input.t_ins', $control).on('click', function () {
        var slideIndex = getSlideIndex(this);
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_SlideShow.Action_Insert, slideIndex);
    });
    // Add button click
    $('input.t_add', $control).on('click', function () {
        var slideIndex = getSlideIndex(this);
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_SlideShow.Action_Add, slideIndex);
    });
};

