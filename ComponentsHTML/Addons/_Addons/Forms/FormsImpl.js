"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
/* Forms implementation required by YetaWF */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var FormsImpl = /** @class */ (function () {
        function FormsImpl() {
            // Partialform Initialization
            // Validation
            // when we display a popup for the error summary, the focus loss causes validation to occur. We suppress updating icons if dontUpdateWarningIcons == true
            this.dontUpdateWarningIcons = false;
        }
        /**
         * Initializes a partialform.
         */
        FormsImpl.prototype.initPartialForm = function ($partialForm) {
            // get all fields with errors (set server-side)
            var $errs = $('.field-validation-error', $partialForm);
            // add warning icons to validation errors
            $errs.each(function () {
                var $val = $(this);
                var name = $val.attr("data-valmsg-for");
                var $err = $("img." + YConfigs.Forms.CssWarningIcon + "[name=\"" + name + "\"]", $val.closest('form'));
                $err.remove();
                $val.before("<img src=\"" + YetaWF_Basics.htmlAttrEscape(YConfigs.Forms.CssWarningIconUrl) + "\" name=" + name + " class=\"" + YConfigs.Forms.CssWarningIcon + "\" " + YConfigs.Basics.CssTooltip + "=\"" + YetaWF_Basics.htmlAttrEscape($val.text()) + "\"/>");
            });
        };
        ;
        /**
         * Re-validate all fields within the div, typically used after paging in a grid to let jquery.validate update all fields
         */
        FormsImpl.prototype.updateValidation = function ($div) {
            $.validator.unobtrusive.parse($div);
            $('input,select,textarea', $div).has("[data-val=true]").trigger('focusout');
        };
        /**
         * Validates one elements.
         */
        FormsImpl.prototype.validateElement = function ($ctrl) {
            var $form = YetaWF_Forms.getFormCond($ctrl);
            if ($form === null)
                return;
            $form.validate().element($ctrl);
        };
        /**
         * Returns whether the form has errors.
         */
        FormsImpl.prototype.hasErrors = function ($form) {
            return $('.validation-summary-errors li', $form).length > 0;
        };
        ;
        /**
         * Shows all form errors in a popup.
         */
        FormsImpl.prototype.showErrors = function ($form) {
            var $summary = this.formErrorSummary($form);
            var $list = $('ul li', $summary);
            // only show unique messages (no duplicates)
            var list = [];
            $list.each(function () {
                list.push($(this).text());
            });
            var uniqueMsgs = [];
            $.each(list, function (i, el) {
                if ($.inArray(el, uniqueMsgs) === -1)
                    uniqueMsgs.push(el);
            });
            // build output
            var s = "";
            $.each(uniqueMsgs, function (i, el) {
                s += el + '(+nl)';
            });
            this.dontUpdateWarningIcons = true;
            YetaWF_Basics.Y_Error(YLocs.Forms.FormErrors + s);
            this.dontUpdateWarningIcons = false;
        };
        ;
        /**
         * Returns the form's error summary (div).
         */
        FormsImpl.prototype.formErrorSummary = function ($form) {
            var $summary = $('.validation-summary-errors', $form);
            if ($summary.length != 1)
                throw "Error summary not found"; /*DEBUG*/
            return $summary;
        };
        ;
        // Forms initialization
        /**
         * Initialize the form when page/content is ready.
         * No external use.
         */
        FormsImpl.prototype.initForm = function ($tag) {
            $.validator.unobtrusive.parse($('form', $tag));
            $('form', $tag).addTriggersToJqueryValidate().triggerElementValidationsOnFormValidation();
            var $forms = $('form', $tag).filter('.yValidateImmediately');
            if ($forms.length > 0) {
                $forms.each(function () {
                    var f = $(this);
                    f.validate();
                    f.valid(); // force all fields to show valid/not valid
                });
            }
        };
        /**
         * Initialize overall form validation.
         * No external use.
         */
        FormsImpl.prototype.initValidation = function () {
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
                    var fi = YetaWF_FormsImpl;
                    if (fi.dontUpdateWarningIcons)
                        return;
                    var $input = $(element.target);
                    var $form = YetaWF_Forms.getForm($input);
                    var name = $input.attr("name");
                    // remove the error icon
                    var $err = $("img." + YConfigs.Forms.CssWarningIcon + "[name=\"" + name + "\"]", $form);
                    $err.remove();
                    // find the validation message
                    var $val = $("span.field-validation-error[data-valmsg-for=\"" + name + "\"]", $form); // get the validation message (which follows the input field but is hidden via CSS)
                    // some templates incorrectly add  @Html.ValidationMessageFor(m => Model) to the rendered template - THIS IS WRONG
                    // rather than going back and testing each template, we'll just use the first validation error for the field we find.
                    if ($val.length < 1)
                        throw "Validation message not found"; /*DEBUG*/
                    // insert a new error icon
                    $val.eq(0).before("<img src=\"" + YetaWF_Basics.htmlAttrEscape(YConfigs.Forms.CssWarningIconUrl) + "\" name=\"" + name + "\" class=\"" + YConfigs.Forms.CssWarningIcon + "\" " + YConfigs.Basics.CssTooltip + "=\"" + YetaWF_Basics.htmlAttrEscape($val.text()) + "\"/>");
                });
                $('body').on('elementValidationSuccess', function (element) {
                    var fi = YetaWF_FormsImpl;
                    if (fi.dontUpdateWarningIcons)
                        return;
                    var $input = $(element.target);
                    var $form = YetaWF_Forms.getForm($input);
                    var name = $input.attr("name");
                    // remove the error icon
                    var $err = $("img." + YConfigs.Forms.CssWarningIcon + "[name=\"" + name + "\"]", $form);
                    $err.remove();
                });
            });
        };
        return FormsImpl;
    }());
    YetaWF_ComponentsHTML.FormsImpl = FormsImpl;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var YetaWF_FormsImpl = new YetaWF_ComponentsHTML.FormsImpl();
/* Page load */
YetaWF_Basics.whenReady.push({
    callback: YetaWF_FormsImpl.initForm
});
/* Initialize validation system */
YetaWF_FormsImpl.initValidation();
