/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

var YetaWF_Url = {};

/* LEGACY COMPATIBILITY ONLY */

// Enable a url object
// $control refers to the <div class="yt_url t_edit">
YetaWF_Url.Enable = function ($control, enabled) {
    if ($control.length > 1) throw "too many controls";
    if ($control.length == 0) return;
    YetaWF_ComponentsHTML.UrlEditComponent.getControlFromTag($control[0]).enable(enabled);
};
// Load a value into a url object
// $control refers to the <div class="yt_url t_edit">
YetaWF_Url.Update = function ($control, urlString) {
    if ($control.length > 1) throw "too many controls";
    if ($control.length == 0) return;
    YetaWF_ComponentsHTML.UrlEditComponent.getControlFromTag($control[0]).value = urlString;
};
YetaWF_Url.Clear = function ($control) {
    if ($control.length > 1) throw "too many controls";
    if ($control.length == 0) return;
    YetaWF_ComponentsHTML.UrlEditComponent.getControlFromTag($control[0]).clear();
};
YetaWF_Url.Retrieve = function ($control) {
    if ($control.length > 1) throw "too many controls";
    if ($control.length == 0) return;
    return YetaWF_ComponentsHTML.UrlEditComponent.getControlFromTag($control[0]).value;
};
YetaWF_Url.HasChanged = function ($control, data) {
    if ($control.length > 1) throw "too many controls";
    if ($control.length == 0) return;
    var val = YetaWF_ComponentsHTML.UrlEditComponent.getControlFromTag($control[0]).value;
    return !$YetaWF.stringCompare(data, val);
};
