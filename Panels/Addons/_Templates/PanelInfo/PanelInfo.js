/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

var YetaWF_Panels = {};

YetaWF_Panels.updateButtons = function ($control) {

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

YetaWF_Panels.init = function (divId) {
    var TEMPLATENAME = 'YetaWF_Panels_PanelInfo';
    var $control = $('#' + divId);
    if ($control.length != 1) throw "div not found";/*DEBUG*/

    YetaWF_Panels.updateButtons($control);

    function getPanelIndex() {
        var index = $('input[name$="_ActiveTab"]', $control).val();
        if (index == undefined) throw "Can't find panel index (hidden input _ActiveTab)";/*DEBUG*/
        return index;
    }

    // Apply button click
    $('input.t_apply', $control).on('click', function () {
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Apply, getPanelIndex());
    });
    // << button click
    $('input.t_up', $control).on('click', function () {
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_Panels.Action_MoveLeft, getPanelIndex());
    });
    // >> button click
    $('input.t_down', $control).on('click', function () {
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_Panels.Action_MoveRight, getPanelIndex());
    });
    // delete button click
    $('input.t_delete', $control).on('click', function () {
        var btn = this;
        Y_AlertYesNo(YLocs.YetaWF_Panels.RemoveConfirm, YLocs.YetaWF_Panels.RemoveTitle, function () {
            YetaWF_Forms.submitTemplate(btn, false, TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Remove, getPanelIndex());
        });
    });
    // Insert button click
    $('input.t_ins', $control).on('click', function () {
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Insert, getPanelIndex());
    });
    // Add button click
    $('input.t_add', $control).on('click', function () {
        YetaWF_Forms.submitTemplate(this, true, TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Add, getPanelIndex());
    });
};

$(document).on('YetaWF_PropertyList_PanelSwitched', function (event, panel) {
    var $panel = $(panel);
    var $control = $panel.closest('.yt_panels_panelinfo');
    $('input[name$="_ActiveTab"]', $control).val($panel.attr('data-tab'));
    YetaWF_Panels.updateButtons($control);
});

// A <div> is being emptied. Destroy all panels the <div> may contain.
YetaWF_Basics.addClearDiv(function (tag) {
    // tabs
    var list = tag.querySelectorAll(".yt_panels_panelinfo .t_panels.t_acctabs");
    var len = list.length;
    for (var i = 0; i < len; ++i) {
        var el = list[i];
        var tabs = $(el);
        if (tabs) tabs.tabs("destroy");
    }
    // jquery ui accordion
    var list = tag.querySelectorAll(".yt_panels_panelinfo .t_panels.t_accjquery");
    var len = list.length;
    for (var i = 0; i < len; ++i) {
        var el = list[i];
        var accordion = $(el);
        if (accordion) accordion.accordion("destroy");
    }
    // kendo accordion
    var list = tag.querySelectorAll(".yt_panels_panelinfo .t_panels.t_acckendo");
    var len = list.length;
    for (var i = 0; i < len; ++i) {
        var el = list[i];
        var panelBar = $(el).data("kendoPanelBar");
        if (panelBar) panelBar.destroy();
    }
});
