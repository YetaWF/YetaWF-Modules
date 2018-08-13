/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

var YetaWF_TemplateDropDownList = {};

/* LEGACY COMPATIBILITY ONLY */

// Enable a dropdownlist object
// $control refers to the <div class="yt_dropdownlist...">
YetaWF_TemplateDropDownList.Enable = function ($control, enabled) {
    if ($control.length > 1) throw "too many controls";
    if ($control.length == 0) return;
    YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromTag($control[0]).enable(enabled);
}
// Update a dropdownlist object
// $control refers to the <div class="yt_dropdownlist...">
YetaWF_TemplateDropDownList.Update = function (control, value) {
    var $control = $(control);
    if ($control.length > 1) throw "too many controls";
    if ($control.length == 0) return;
    YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromTag($control[0]).value = value;
}
// Clear a dropdownlist object (select the first item)
// $control refers to the <div class="yt_dropdownlist...">
YetaWF_TemplateDropDownList.Clear = function ($control) {
    if ($control.length > 1) throw "too many controls";
    if ($control.length == 0) return;
    YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromTag($control[0]).clear();
}

// retrieve the tooltip for the nth item (index) in the dropdown list $this
YetaWF_TemplateDropDownList.getTitle = function ($control, index) {
    if ($control.length > 1) throw "too many controls";
    if ($control.length == 0) return;
    return YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromTag($control[0]).getToolTip(index);
}
YetaWF_TemplateDropDownList.getTitleFromId = function (id, index) {
    return YetaWF_TemplateDropDownList.getTitle($('#{0}'.format(id)), index);
}

// Send data to server using ajaxurl and update the dropdownlist with the returned data object (text,value & tooltips)
YetaWF_TemplateDropDownList.AjaxUpdate = function ($control, data, ajaxurl, onSuccess, onFailure) {
    if ($control.length > 1) throw "too many controls";
    if ($control.length == 0) return;
    return YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromTag($control[0]).ajaxUpdate(data, ajaxurl, onSuccess, onFailure);
}
