/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

var YetaWF_Url = {};

/* LEGACY COMPATIBILITY ONLY */

// Enable a url object
// $control refers to the <div class="yt_url t_edit">
YetaWF_Url.Enable = function ($control, enabled) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.UrlEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.UrlEditComponent.SELECTOR);
    if (ctrl == null) return;
    ctrl.enable(enabled);
};
// Load a value into a url object
// $control refers to the <div class="yt_url t_edit">
YetaWF_Url.Update = function ($control, urlString) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.UrlEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.UrlEditComponent.SELECTOR);
    if (ctrl == null) return;
    ctrl.value = urlString;
};
YetaWF_Url.Clear = function ($control) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.UrlEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.UrlEditComponent.SELECTOR);
    if (ctrl == null) return;
    ctrl.clear();
};
YetaWF_Url.Retrieve = function ($control) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.UrlEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.UrlEditComponent.SELECTOR);
    if (ctrl == null) return "";
    return ctrl.value;
};
YetaWF_Url.HasChanged = function ($control, data) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.UrlEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.UrlEditComponent.SELECTOR);
    if (ctrl == null) return false;
    var val = ctrl.value;
    return !$YetaWF.stringCompare(data, val);
};
