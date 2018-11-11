/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* LEGACY COMPATIBILITY ONLY */

'use strict';

var YetaWF_MultiString = {};

// Load a multistring UI object with data
// ms refers to the div class="yt_multistring t_edit"
// data is an object with language data for each language
// if no data is available for a language, the default is used
// this is mainly so we can always validate the input field without having to worry about the selected language
YetaWF_MultiString.Update = function($control, data) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.MultiStringEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR);
    if (ctrl == null) return;
    ctrl.update(data);
}

// Get data from multistring
// ms refers to the div class="yt_multistring t_edit"
// data is an object with language data for each language
YetaWF_MultiString.Retrieve = function ($control, data) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.MultiStringEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR);
    if (ctrl == null) return false;
    return ctrl.retrieveData(data);
}

// Test whether the data has changed
// ms refers to the div class="yt_multistring t_edit"
// data is an object with language data for each language
YetaWF_MultiString.HasChanged = function ($control, data) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.MultiStringEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR);
    if (ctrl == null) return false;
    return ctrl.hasChanged(data);
}

// Enable a multistring object
YetaWF_MultiString.Enable = function ($control, enabled) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.MultiStringEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR);
    if (ctrl == null) return;
    ctrl.enable(enabled);
}

// Clear a multistring object
YetaWF_MultiString.Clear = function ($control) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.MultiStringEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR);
    if (ctrl == null) return;
    ctrl.clear();
}

// get the default value from a multistring
YetaWF_MultiString.getDefaultValue = function ($control) {
    if ($control.length > 1) throw "too many controls";
    var ctrl = YetaWF_ComponentsHTML.MultiStringEditComponent.getControlFromTagCond($control[0], YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR);
    if (ctrl == null) return "";
    return ctrl.defaultValue;
}

