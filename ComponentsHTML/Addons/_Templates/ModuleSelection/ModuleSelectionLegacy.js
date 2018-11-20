/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* LEGACY COMPATIBILITY ONLY */

var YetaWF_ModuleSelection = {};

// Load a moduleselection UI object with data
// $control refers to the div class="yt_moduleselection t_edit"
// this will select the correct package in the dropdownlist and select the module (the package is detected using ajax)
YetaWF_ModuleSelection.UpdateComplete = function ($control, modGuid) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.ModuleSelectionEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.ModuleSelectionEditComponent.SELECTOR);
    if (ctrl == null) return;
    ctrl.updateComplete(modGuid);
};

// Get data from moduleselection
// $control refers to the div class="yt_moduleselection t_edit"
YetaWF_ModuleSelection.Retrieve = function ($control) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.ModuleSelectionEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.ModuleSelectionEditComponent.SELECTOR);
    if (ctrl == null) return "";
    return ctrl.value;
}
// Test whether the data has changed
// $control refers to the div class="yt_moduleselection t_edit"
YetaWF_ModuleSelection.HasChanged = function ($control, data) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.ModuleSelectionEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.ModuleSelectionEditComponent.SELECTOR);
    if (ctrl == null) return false;
    return ctrl.hasChanged(data);
}
// Enable a moduleselection object
// $control refers to the <div class="yt_moduleselection t_edit">
YetaWF_ModuleSelection.Enable = function ($control, enabled) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.ModuleSelectionEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.ModuleSelectionEditComponent.SELECTOR);
    if (ctrl == null) return;
    ctrl.enable(enabled);
}
// Clear a moduleselection object
// $control refers to the <div class="yt_moduleselection t_edit">
YetaWF_ModuleSelection.Clear = function ($control) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.ModuleSelectionEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.ModuleSelectionEditComponent.SELECTOR);
    if (ctrl == null) return;
    ctrl.clear();
}

