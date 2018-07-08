/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* This is only used by components in ComponentsHTML (this package) */

/* TODO: Legacy, to be converted to FormsSupport */

// Validation

// re-validate all fields within the div, typically used after paging in a grid
// to let jquery.validate update all fields
YetaWF_Forms.updateValidation = function ($div) {
    $.validator.unobtrusive.parse($div);
    $('input,select,textarea', $div).has("[data-val=true]").trigger('focusout');
};
// Validate one element
YetaWF_Forms.validateElement = function ($ctrl) {
    var $form = YetaWF_Forms.getFormCond($ctrl);
    if ($form === null) return;
    $form.validate().element($ctrl);
}

_YetaWF_Forms.hasErrors = function ($form) { //$$$remove
    return $('.validation-summary-errors li', $form).length > 0;
};
_YetaWF_Forms.formErrorSummary = function ($form) {//$$$remove
    var $summary = $('.validation-summary-errors', $form);
    if ($summary.length != 1) throw "Error summary not found";/*DEBUG*/
    return $summary;
};

// when we display a popup for the error summary, the focus loss causes validation to occur. We suppress updating icons if dontUpdateWarningIcons == true
_YetaWF_Forms.dontUpdateWarningIcons = false;

$(document).ready(function () {

    // running this overrides some jQuery Validate stuff so we can hook into its validations.
    // triggerElementValidationsOnFormValidation is optional and will fire off all of your
    // element validations WHEN the form validation runs ... it requires jquery.validate.unobtrusive
    $('form').addTriggersToJqueryValidate().triggerElementValidationsOnFormValidation();

    // You can bind to events that the forms/elements trigger on validation
    //$('form').bind('formValidation', function (event, element, result) {
    //    console.log(['validation ran for form:', element, 'and the result was:', result]);
    //});

    //// Or you can use the helper functions that we created for binding to these events
    //$('form').formValidation(function (element, result) {
    //    console.log(['validation ran for form:', element, 'and the result was:', result]);
    //});

    //$('input.something').elementValidation(function (element, result) {
    //    console.log(['validation ran for element:', element, 'and the result was:', result]);
    //});

    //$('input#address').elementValidationSuccess(function (element) {
    //    console.log(['validations just ran for this element and it was valid!', element]);
    //});
    $('body').on('elementValidationError', function (element) {
        if (_YetaWF_Forms.dontUpdateWarningIcons) return;
        var $input = $(element.target);
        var $form = YetaWF_Forms.getForm($input);
        var name = $input.attr("name");
        // remove the error icon
        var $err = $('img.{0}[name="{1}"]'.format(YConfigs.Forms.CssWarningIcon, name), $form);
        $err.remove();
        // find the validation message
        var $val = $('span.field-validation-error[data-valmsg-for="{0}"]'.format(name), $form);// get the validation message (which follows the input field but is hidden via CSS)
        // some templates incorrectly add  @Html.ValidationMessageFor(m => Model) to the rendered template - THIS IS WRONG
        // rather than going back and testing each template, we'll just use the first validation error for the field we find.
        if ($val.length < 1) throw "Validation message not found";/*DEBUG*/
        // insert a new error icon
        $val.eq(0).before('<img src="{0}" name="{1}" class="{2}" {3}="{4}"/>'.format(YetaWF_Basics.htmlAttrEscape(YConfigs.Forms.CssWarningIconUrl), name, YConfigs.Forms.CssWarningIcon, YConfigs.Basics.CssTooltip, YetaWF_Basics.htmlAttrEscape($val.text())));
    });
    $('body').on('elementValidationSuccess', function (element) {
        if (_YetaWF_Forms.dontUpdateWarningIcons) return;
        var $input = $(element.target);
        var $form = YetaWF_Forms.getForm($input);
        var name = $input.attr("name");
        // remove the error icon
        var $err = $('img.{0}[name="{1}"]'.format(YConfigs.Forms.CssWarningIcon, name), $form);
        $err.remove();
    });
});

// Forms initialization

_YetaWF_Forms.init = function ($tag) {

    $.validator.unobtrusive.parse($('form', $tag));
    $('form', $tag).addTriggersToJqueryValidate().triggerElementValidationsOnFormValidation();

    var $forms = $('form', $tag).filter('.yValidateImmediately');
    if ($forms.length > 0) {
        $forms.each(function () {
            $(this).validate();
            $(this).valid(); // force all fields to show valid/not valid
        });
    }
}
YetaWF_Basics.whenReady.push({
    callback: _YetaWF_Forms.init
});

// Forms validation

_YetaWF_Forms.showErrors = function ($form) {
    var $summary = _YetaWF_Forms.formErrorSummary($form);
    var $list = $('ul li', $summary);

    // only show unique messages (no duplicates)
    var list = [];
    $list.each(function () {
        list.push($(this).text());
    });
    var uniqueMsgs = [];
    $.each(list, function (i, el) {
        if ($.inArray(el, uniqueMsgs) === -1) uniqueMsgs.push(el);
    });

    // build output
    var s = "";
    $.each(uniqueMsgs, function (i, el) {
        s += el + '(+nl)';
    });
    _YetaWF_Forms.dontUpdateWarningIcons = true;
    YetaWF_Basics.Y_Error(YLocs.Forms.FormErrors + s);
    _YetaWF_Forms.dontUpdateWarningIcons = false;
};

// Forms submission

YetaWF_Forms.serializeFormArray = function ($form) {
    // disable all fields that we don't want to submit (marked with YConfigs.Forms.CssFormNoSubmit)
    var $disabledFields = $('.' + YConfigs.Forms.CssFormNoSubmit, $form).not(':disabled');
    $disabledFields.attr('disabled', 'disabled');
    // disable all input fields in containers (usually grids) - we don't want to submit them - they're collected separately
    var $disabledGridFields = $('.{0} input,.{0} select'.format(YConfigs.Forms.CssFormNoSubmitContents), $form).not(':disabled');
    $disabledGridFields.attr('disabled', 'disabled');
    // serialize the form
    var formData = $form.serializeArray();
    // and enable all the input fields we just disabled
    $disabledFields.removeAttr('disabled');
    $disabledGridFields.removeAttr('disabled');
    return formData;
}

YetaWF_Forms.serializeForm = function ($form) {
    // disable all fields that we don't want to submit (marked with YConfigs.Forms.CssFormNoSubmit)
    var $disabledFields = $('.' + YConfigs.Forms.CssFormNoSubmit, $form).not(':disabled');
    $disabledFields.attr('disabled', 'disabled');
    // disable all input fields in containers (usually grids) - we don't want to submit them - they're collected separately
    var $disabledGridFields = $('.{0} input,.{0} select'.format(YConfigs.Forms.CssFormNoSubmitContents), $form).not(':disabled');
    $disabledGridFields.attr('disabled', 'disabled');
    // serialize the form
    var formData = $form.serialize();
    // and enable all the input fields we just disabled
    $disabledFields.removeAttr('disabled');
    $disabledGridFields.removeAttr('disabled');
    return formData;
}

// Cancel the form when a Cancel button is clicked

$(document).on('click', 'form .' + YConfigs.Forms.CssFormCancel, function (e) {

    if (YetaWF_Basics.isInPopup()) {
        // we're in a popup, just close it
        YetaWF_Basics.closePopup();
    } else {
        // go to the last entry in the origin list, pop that entry and pass it in the url
        var originList = YVolatile.Basics.OriginList;
        if (originList.length > 0) {
            var origin = originList.pop();
            var uri = new URI(origin.Url);
            uri.removeSearch(YGlobals.Link_ToEditMode);
            if (origin.EditMode != YVolatile.Basics.EditModeActive)
                uri.addSearch(YGlobals.Link_ToEditMode, !YVolatile.Basics.EditModeActive);
            uri.removeSearch(YGlobals.Link_OriginList);
            if (originList.length > 0)
                uri.addSearch(YGlobals.Link_OriginList, JSON.stringify(originList));
            if (!YetaWF_Basics.ContentHandling.setContent(uri, true))
                window.location.assign(uri);
        } else {
            // we don't know where to return so just close the browser
            window.close();
        }
    }
});

// Submit the form when an apply button is clicked

$(document).on('click', 'form input[type="button"][{0}]'.format(YConfigs.Forms.CssDataApplyButton), function (e) {
    e.preventDefault();
    var $form = YetaWF_Forms.getForm(this);
    YetaWF_Forms.submit($form, true, YGlobals.Link_SubmitIsApply + "=y");
});

// Submit the form when a submit button is clicked

$(document).on('submit', 'form.' + YConfigs.Forms.CssFormAjax, function (e) {
    var $form = $(this);
    e.preventDefault();
    YetaWF_Forms.submit($form, true);
});

