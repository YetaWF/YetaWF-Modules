/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

var YetaWF_BootstrapCarousel = {};

YetaWF_BootstrapCarousel.updateButtons = function ($control) {
    'use strict';

    function getPanelCount() {
        return $('.t_tabstrip li', $control).length;
    }

    // disable the << button if the active tab is the first one
    var panelIndex = $('input[name$="_ActiveTab"]', $control).val();
    if (panelIndex == 0)
        $('input.t_up', $control).attr("disabled", "disabled");
    else
        $('input.t_up', $control).removeAttr("disabled");

    // disable the >> button if the last panel is active
    if (panelIndex == getPanelCount() - 1)
        $('input.t_down', $control).attr("disabled", "disabled");
    else
        $('input.t_down', $control).removeAttr("disabled");

    // disable if there is only one panel
    var $del = $('input.t_delete', $control);
    if (getPanelCount() <= 1)
        $del.attr("disabled", "disabled");
    else
        $del.removeAttr("disabled");
};

YetaWF_BootstrapCarousel.init = function (divId) {
    'use strict';

    var TEMPLATENAME = 'YetaWF_BootstrapCarousel_SlideShow';
    var $control = $('#' + divId);
    if ($control.length != 1) throw "div not found";/*DEBUG*/

    YetaWF_BootstrapCarousel.updateButtons($control);

    function getPanelIndex() {
        var index = $('input[name$="_ActiveTab"]', $control).val();
        if (index == undefined) throw "Can't find panel index (hidden input _ActiveTab)";/*DEBUG*/
        return index;
    }

    // Apply button click
    $('input.t_apply', $control).on('click', function () {
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Apply, getPanelIndex());
    });
    // << button click
    $('input.t_up', $control).on('click', function () {
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_MoveLeft, getPanelIndex());
    });
    // >> button click
    $('input.t_down', $control).on('click', function () {
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_MoveRight, getPanelIndex());
    });
    // delete button click
    $('input.t_delete', $control).on('click', function () {
        var btn = this;
        Y_AlertYesNo(YLocs.YetaWF_BootstrapCarousel.RemoveConfirm, YLocs.YetaWF_BootstrapCarousel.RemoveTitle, function () {
            YetaWF_Forms.submitTemplate(btn, false, TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Remove, getPanelIndex());
        });
    });
    // Insert button click
    $('input.t_ins', $control).on('click', function () {
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Insert, getPanelIndex());
    });
    // Add button click
    $('input.t_add', $control).on('click', function () {
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Add, getPanelIndex());
    });
};

$(document).on('YetaWF_PropertyList_PanelSwitched', function (event, panel) {
    var $panel = $(panel);
    var $control = $panel.closest('.yt_bootstrapcarousel_slideshow');
    $('input[name$="_ActiveTab"]', $control).val($panel.attr('data-tab'));
    YetaWF_BootstrapCarousel.updateButtons($control);
});
