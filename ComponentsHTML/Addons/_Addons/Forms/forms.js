/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
'use strict';

/* Global interface dealing with forms, for all components in all packages */

//$$$ implements required externally callable functions.
//$$$ Should be namespace YetaWF_ComponentsHTML class Forms

var _YetaWF_Forms = {}; // legacy

// When a form is about to be submitted, all the functions in YPreSubmitHandler are called one by one
// This is used to add control-specific data to the data submitted by the form
// Usage:
// YetaWF_Forms.addPreSubmitHandler(@Manager.InPartialForm ? 1 : 0, {
//   form: $form,               // form <div> to be processed
//   callback: function() {}    // function to be called - the callback returns extra data appended to the submit url
//   userdata: callback-data,   // any data suitable to callback
// });
_YetaWF_Forms.YPreSubmitHandlerAll = []; // done every time before submit (never cleared) - used on main forms
_YetaWF_Forms.YPreSubmitHandler1 = []; // done once before submit, then cleared - used in partial forms

YetaWF_Forms.addPreSubmitHandler = function(inPartialForm, func)
{
    if (inPartialForm) {
        _YetaWF_Forms.YPreSubmitHandler1.push(func);
    } else {
        _YetaWF_Forms.YPreSubmitHandlerAll.push(func);
    }
}

_YetaWF_Forms.callPreSubmitHandler = function($form, onSubmitExtraData) {
    for (var index in _YetaWF_Forms.YPreSubmitHandlerAll) {
        var entry = _YetaWF_Forms.YPreSubmitHandlerAll[index]
        if (entry.form[0] == $form[0]) {
            // form specific
            var extra = entry.callback(entry);
            if (extra != undefined) {
                if (onSubmitExtraData.length > 0)
                    onSubmitExtraData = onSubmitExtraData + "&";
                onSubmitExtraData += extra;
            }
        }
    }
    for (var index in _YetaWF_Forms.YPreSubmitHandler1) {
        var entry = _YetaWF_Forms.YPreSubmitHandler1[index];
        if (entry.form[0] == $form[0]) {
            var extra = entry.callback(entry);
            if (extra != undefined) {
                if (onSubmitExtraData.length > 0)
                    onSubmitExtraData = onSubmitExtraData + "&";
                onSubmitExtraData += extra;
            }
        }
    }
    return onSubmitExtraData;
}

// When a form has been successfully submitted, all the functions in YPostSubmitHandler are called one by one
// Usage:
// YetaWF_Forms.addPostSubmitHandler(@Manager.InPartialForm ? 1 : 0, {
//   form: $form,               // form <div> to be processed - may be null
//   callback: function() {}    // function to be called
//   userdata: callback-data,   // any data suitable to callback
// });
_YetaWF_Forms.YPostSubmitHandlerAll = []; // done every time after submit (never cleared) - used on main forms
_YetaWF_Forms.YPostSubmitHandler1 = []; // done once after submit, then cleared - used in partial forms

YetaWF_Forms.addPostSubmitHandler = function (inPartialForm, func) {
    if (inPartialForm) {
        _YetaWF_Forms.YPostSubmitHandler1.push(func);
    } else {
        _YetaWF_Forms.YPostSubmitHandlerAll.push(func);
    }
}

_YetaWF_Forms.callPostSubmitHandler = function ($form, onSubmitExtraData) {
    for (var index in _YetaWF_Forms.YPostSubmitHandlerAll) {
        var entry = _YetaWF_Forms.YPostSubmitHandlerAll[index];
        if (entry.form == null) {
            // global
            entry.callback(entry);
        } else if (entry.form[0] == $form[0]) {
            // form specific
            entry.callback(entry);
        }
    }
    for (var index in _YetaWF_Forms.YPostSubmitHandler1) {
        var entry = _YetaWF_Forms.YPostSubmitHandler1[index];
        if (entry.form[0] == $form[0])
            entry.callback(entry);
    }
    return onSubmitExtraData;
}

// Form submission

YetaWF_Forms.DATACLASS = 'yetawf_forms_data'; // add divs with this class to form for any data that needs to be submitted (will be removed before calling (pre)submit handlers

YetaWF_Forms.submit = function ($form, useValidation, extraData, successFunc, failFunc) {

    $('div.' + YetaWF_Forms.DATACLASS).remove();

    var form = $form.get(0);

    var onSubmitExtraData = extraData == undefined ? "" : extraData;
    onSubmitExtraData = _YetaWF_Forms.callPreSubmitHandler($form, onSubmitExtraData);

    if (useValidation)
        $form.validate();

    YetaWF_Basics.setLoading(true);

    if (!useValidation || $form.valid()) {

        // serialize the form
        var formData = YetaWF_Forms.serializeForm($form);
        // add extra data
        if (onSubmitExtraData)
            formData = onSubmitExtraData + "&" + formData;
        // add the origin list in case we need to navigate back
        var originList = YVolatile.Basics.OriginList;
        if ($form.attr(YConfigs.Basics.CssSaveReturnUrl) != undefined) {// form says we need to save the return address on submit
            var currUri = new URI(window.location.href);
            currUri.removeSearch(YGlobals.Link_OriginList);// remove originlist from current URL
            currUri.removeSearch(YGlobals.Link_InPopup);// remove popup info from current URL
            originList = YVolatile.Basics.OriginList.slice(0);// copy saved originlist
            var newOrigin = { Url: currUri.toString(), EditMode: YVolatile.Basics.EditModeActive != 0, InPopup: YetaWF_Basics.isInPopup() };
            originList.push(newOrigin);
            if (originList.length > 5)// only keep the last 5 urls
                originList = originList.slice(originList.length - 5);
        }
        // include the character dimension info
        {
            var charSize = YetaWF_Basics.getCharSizeFromTag(form);
            formData = formData + "&" + YGlobals.Link_CharInfo + "=" + charSize.width.toString() + ',' + charSize.height.toString();
        }

        formData = formData + "&" + YGlobals.Link_OriginList + "=" + encodeURIComponent(JSON.stringify(originList));
        // add the status of the Pagecontrol
        if (YVolatile.Basics.PageControlVisible)
            formData = formData + "&" + YGlobals.Link_PageControl + "=y";
        // add if we're in a popup
        if (YetaWF_Basics.isInPopup())
            formData = formData + "&" + YGlobals.Link_InPopup + "=y";

        $.ajax({
            url: form.action,
            type: form.method,
            data: formData,
            success: function (result, textStatus, jqXHR) {
                YetaWF_Basics.setLoading(false);
                YetaWF_Basics.processAjaxReturn(result, textStatus, jqXHR, $form, undefined, function () {
                    _YetaWF_Forms.YPreSubmitHandler1 = [];
                    var $partForm = $('.' + YConfigs.Forms.CssFormPartial, $form);
                    if ($partForm.length > 0) {
                        // clean up everything that's about to be removed
                        YetaWF_Basics.processClearDiv($partForm[0]);
                        // preserve the original css classes on the partial form (PartialFormCss)
                        var cls = $partForm[0].className;
                        $partForm.replaceWith(result);
                        $partForm = $('.' + YConfigs.Forms.CssFormPartial, $form);
                        $partForm[0].className = cls;
                    }
                });
                _YetaWF_Forms.callPostSubmitHandler($form);
                _YetaWF_Forms.YPostSubmitHandler1 = [];
                if (successFunc) // executed on successful ajax submit
                    successFunc(_YetaWF_Forms.hasErrors($form));
            },
            error: function (jqXHR, textStatus, errorThrown) {
                YetaWF_Basics.setLoading(false);
                YetaWF_Basics.Y_Alert(YLocs.Forms.AjaxError.format(jqXHR.status, jqXHR.statusText), YLocs.Forms.AjaxErrorTitle);
                if (failFunc)
                    failFunc();
            },
        });
    } else {
        YetaWF_Basics.setLoading(false);
        // find the first field in a tab control that has an input validation error and activate that tab
        // This will not work for nested tabs. Only the lowermost tab will be activated.
        $("div.yt_propertylisttabbed", $form).each(function (index) {
            var $tabctrl = $(this);
            // get the first field in error (if any)
            var $errField = $('.input-validation-error', $tabctrl).eq(0);
            if ($errField.length > 0) {
                // find out which tab panel we're on
                var $ttabpanel = $errField.closest('div.t_tabpanel');
                if ($ttabpanel.length == 0) throw "We found a validation error in a tab control, but we couldn't find the tab panel.";/*DEBUG*/
                var panel = $ttabpanel.attr('data-tab');
                if (!panel) throw "We found a panel in a tab control without panel number (data-tab attribute).";/*DEBUG*/
                // get the tab entry
                var $te = $('ul.t_tabstrip > li', $tabctrl).eq(panel);
                if ($te.length == 0) throw "We couldn't find the tab entry for panel " + panel;/*DEBUG*/
                if (YVolatile.Forms.TabStyle == 0)//jquery ui
                    $tabctrl.tabs("option", "active", panel);
                else if (YVolatile.Forms.TabStyle == 1)//Kendo UI
                    $tabctrl.data("kendoTabStrip").activateTab($te);
                else throw "Unknown tab style";/*DEBUG*/
            }
        });
        var hasErrors = _YetaWF_Forms.hasErrors($form);
        if (hasErrors)
            _YetaWF_Forms.showErrors($form);
        // call callback (if any)
        if (successFunc)
            successFunc(_YetaWF_Forms.hasErrors($form));
    }
    $('div.' + YetaWF_Forms.DATACLASS).remove();
    return false;
};

YetaWF_Forms.submitTemplate = function (obj, useValidation, templateName, templateAction, templateExtraData) {
    var qs = "{0}={1}&{2}=y".format(YConfigs.Basics.TemplateName, templateName, YGlobals.Link_SubmitIsApply);
    if (templateAction != undefined)
        qs += "&{0}={1}".format(YConfigs.Basics.TemplateAction, encodeURIComponent(templateAction));
    if (templateExtraData != undefined)
        qs += "&{0}={1}".format(YConfigs.Basics.TemplateExtraData, encodeURIComponent(templateExtraData));
    YetaWF_Forms.submit(YetaWF_Forms.getForm($(obj)), useValidation, qs);
};

// Forms retrieval

YetaWF_Forms.getForm = function (obj) {
    var $form = $(obj).closest('form');
    if ($form.length == 0) throw "Can't locate enclosing form";/*DEBUG*/
    return $form;
};
YetaWF_Forms.getFormCond = function (obj) {
    var $form = $(obj).closest('form');
    if ($form.length == 0) return null;
    return $form;
};

// get RequestVerificationToken, UniqueIdPrefix and ModuleGuid in query string format (usually for ajax requests)
YetaWF_Forms.getFormInfo = function (obj) {
    var $form = YetaWF_Forms.getForm(obj);
    var info = {};
    var s = $('input[name="' + YConfigs.Forms.RequestVerificationToken + '"]', $form).val();
    if (s == undefined || s.length == 0) throw "Can't locate " + YConfigs.Forms.RequestVerificationToken;/*DEBUG*/
    info.RequestVerificationToken = s;
    s = $('input[name="' + YConfigs.Forms.UniqueIdPrefix + '"]', $form).val();
    if (s == undefined || s.length == 0) throw "Can't locate " + YConfigs.Forms.UniqueIdPrefix;/*DEBUG*/
    info.UniqueIdPrefix = s;
    s = $('input[name="' + YConfigs.Basics.ModuleGuid + '"]', $form).val();
    if (s == undefined || s.length == 0) throw "Can't locate " + YConfigs.Basics.ModuleGuid;/*DEBUG*/
    info.ModuleGuid = s;

    var charSize = YetaWF_Basics.getCharSizeFromTag($form);

    info.QS = "&" + YConfigs.Forms.RequestVerificationToken + "=" + encodeURIComponent(info.RequestVerificationToken) +
              "&" + YConfigs.Forms.UniqueIdPrefix + "=" + encodeURIComponent(info.UniqueIdPrefix) +
              "&" + YConfigs.Basics.ModuleGuid + "=" + encodeURIComponent(info.ModuleGuid) +
              "&" + YGlobals.Link_CharInfo + "=" + charSize.width.toString() + ',' + charSize.height.toString();
    return info;
};

