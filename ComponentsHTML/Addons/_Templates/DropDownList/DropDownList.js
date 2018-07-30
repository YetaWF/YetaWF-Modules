/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

var YetaWF_TemplateDropDownList = {};
var _YetaWF_TemplateDropDownList = {};

YetaWF_TemplateDropDownList.initOne = function ($this) {
    var w = $this.width();
    if (w > 0 && $this.attr("data-needinit") !== undefined) {
        $this.removeAttr("data-needinit");
        $this.kendoDropDownList({});
        var avgw = $this.attr("data-charavgw");
        if (!avgw) throw "dropdowlist without avg char width";/*DEBUG*/
        $this.closest('.k-widget.yt_dropdownlist,.k-widget.yt_dropdownlist_base,.k-widget.yt_enum').width(w + 3 * avgw);
    }
}
// Enable a dropdownlist object
// $control refers to the <div class="yt_dropdownlist...">
YetaWF_TemplateDropDownList.Enable = function ($control, enabled) {
    if ($control.attr("data-needinit") !== undefined) {
        if (enabled)
            $control.removeAttr("disabled");
        else
            $control.attr("disabled","disabled");
    } else {
        var dropdownlist = $control.data("kendoDropDownList");
        dropdownlist.enable(enabled);
    }
}
// Update a dropdownlist object
// $control refers to the <div class="yt_dropdownlist...">
YetaWF_TemplateDropDownList.Update = function ($control, value) {
    if ($control.attr("data-needinit") !== undefined)
        $control.val(value);
    else {
        var dropdownlist = $control.data("kendoDropDownList");
        dropdownlist.value(value);
        if (dropdownlist.select() < 0)
            dropdownlist.select(0);
    }
}
// Clear a dropdownlist object (select the first item)
// $control refers to the <div class="yt_dropdownlist...">
YetaWF_TemplateDropDownList.Clear = function ($control) {
    if ($control.attr("data-needinit") !== undefined)
        $control.prop('selectedIndex', 0);
    else {
        var dropdownlist = $control.data("kendoDropDownList");
        dropdownlist.select(0);
    }
}

// retrieve the tooltip for the nth item (index) in the dropdown list $this
YetaWF_TemplateDropDownList.getTitle = function ($this, index) {
    var tts = $this.data("tooltips");
    if (tts === undefined || tts == null) return null;
    if (index < 0 || index >= tts.length) return null;
    return tts[index];
}
YetaWF_TemplateDropDownList.getTitleFromId = function (id, index) {
    return YetaWF_TemplateDropDownList.getTitle($('#{0}'.format(id)), index);
}

// Send data to server using ajaxurl and update the dropdownlist with the returned data object (text,value & tooltips)
YetaWF_TemplateDropDownList.AjaxUpdate = function ($control, data, ajaxurl, onSuccess, onFailure) {
    'use strict';
    $.ajax({
        url: ajaxurl,
        type: 'post',
        data: data,
        success: function (result, textStatus, jqXHR) {
            $YetaWF.setLoading(false);
            if (result.startsWith(YConfigs.Basics.AjaxJavascriptReturn)) {
                var script = result.substring(YConfigs.Basics.AjaxJavascriptReturn.length);
                var data = JSON.parse(script);
                $control.val(null);
                $control.kendoDropDownList({
                    dataTextField: "t",
                    dataValueField: "v",
                    dataSource: data.data,
                });
                $control.data("tooltips", data.tooltips);
                if (onSuccess !== undefined) {
                    onSuccess(data);
                } else {
                    $control.select(0);
                    $control.trigger('change');
                }
            } else if (result.startsWith(YConfigs.Basics.AjaxJavascriptErrorReturn)) {
                var script = result.substring(YConfigs.Basics.AjaxJavascriptErrorReturn.length);
                eval(script);
                if (onFailure !== undefined)
                    onFailure(data);
            } else {
                if (onFailure !== undefined)
                    onFailure(data);
                throw "Unexpected data returned";
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $YetaWF.setLoading(false);
            $YetaWF.alert(YLocs.Forms.AjaxError.format(jqXHR.status, jqXHR.statusText), YLocs.Forms.AjaxErrorTitle);
            if (onFailure !== undefined)
                onFailure(data);
        }
    });
}

// We need to delay initialization until divs become visible so we can calculate the dropdown width
$YetaWF.registerActivateDivs(function (divs) {
    for (var i in divs) {
        var $ctls = $('select.yt_dropdownlist_base[data-needinit]', $(divs[i]));
        $ctls.each(function (index) {
            YetaWF_TemplateDropDownList.initOne($(this));
        });
    }
});

// handle submit/apply
$(document).on('change', ".ysubmitonchange .k-dropdown select.yt_dropdownlist_base", function (ev) {
    $YetaWF.Forms.submitOnChange(ev.target);
});
$(document).on('change', ".yapplyonchange .k-dropdown select.yt_dropdownlist_base", function (ev) {
    $YetaWF.Forms.applyOnChange(ev.target);
});
$(document).on('change', ".yreloadonchange .k-dropdown select.yt_dropdownlist_base", function (ev) {
    $YetaWF.Forms.reloadOnChange(ev.target);
});

// A <div> is being emptied. Destroy all date/time pickers the <div> may contain.
$YetaWF.addClearDiv(function (tag) {
    var list = tag.querySelectorAll(".yt_dropdownlist_base select");
    var len = list.length;
    for (var i = 0; i < len; ++i) {
        var el = list[i];
        var dropdownlist = $(el).data("kendoDropDownList");
        if (dropdownlist) dropdownlist.destroy();
    }
});

