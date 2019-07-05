/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* Forms implementation required by YetaWF */

namespace YetaWF_ComponentsHTML {

    export class FormsImpl implements YetaWF.IFormsImpl {

        // Partialform Initialization

        /**
         * Initializes a partialform.
         */
        public initPartialForm(partialForm: HTMLElement): void {
            // get all fields with errors (set server-side)
            var $partialForm = $(partialForm);
            var $errs = $(".field-validation-error", $partialForm);
            // add warning icons to validation errors
            $errs.each((index: number, element: HTMLElement): void => {
                var $val = $(element);
                var name = $val.attr("data-valmsg-for");
                var $err = $(`img.${YConfigs.Forms.CssWarningIcon}[name="${name}"]`, $val.closest("form"));
                $err.remove();
                $val.before(`<img src="${$YetaWF.htmlAttrEscape(YConfigs.Forms.CssWarningIconUrl)}" name=${name} class="${YConfigs.Forms.CssWarningIcon}" ${YConfigs.Basics.CssTooltip}="${$YetaWF.htmlAttrEscape($val.text())}"/>`);
            });
        }

        // Validation

        // when we display a popup for the error summary, the focus loss causes validation to occur. We suppress updating icons if dontUpdateWarningIcons == true
        public dontUpdateWarningIcons: boolean = false;

        /**
         * Re-validate all fields within the div, typically used after paging in a grid to let jquery.validate update all fields
         */
        public updateValidation(div: HTMLElement): void {
            var $div = $(div);
            ($ as any).validator.unobtrusive.parse($div);
            $("input,select,textarea", $div).has("[data-val=true]").trigger("focusout");
        }
        /**
         * Clear any validation errors within the div
         */
        public clearValidation(div: HTMLElement): void {
            var $err = $(`img.${YConfigs.Forms.CssWarningIcon}`, div);
            $err.remove();
        }
        /**
         * Validates one elements.
         */
        public validateElement(ctrl: HTMLElement): void {
            var form = $YetaWF.Forms.getFormCond(ctrl);
            if (form === null) return;
            ($(form) as any).validate().element($(ctrl));
        }

        /**
         * Returns whether a div has form errors.
         */
        public hasErrors(elem: HTMLElement): boolean {
            var $elem = $(elem);
            return $(".validation-summary-errors li", $elem).length > 0;
        }

        /**
         * Shows all div form errors in a popup.
         */
        public showErrors(elem: HTMLElement): void {
            var $elem = $(elem);
            var $summary: JQuery<HTMLElement> = this.formErrorSummary($elem);
            var $list = $("ul li", $summary);

            // only show unique messages (no duplicates)
            var list: string[] = [];
            $list.each((index:number, element: HTMLElement) :void => {
                list.push($(element).text());
            });
            var uniqueMsgs: string[] = [];
            $.each(list, (i: number, el: string) : void => {
                if ($.inArray(el, uniqueMsgs) === -1) uniqueMsgs.push(el);
            });

            // build output
            var s: string = "";
            $.each(uniqueMsgs, (i: number, el: string) : void => {
                s += el + "(+nl)";
            });
            this.dontUpdateWarningIcons = true;
            $YetaWF.error(YLocs.Forms.FormErrors + s);
            this.dontUpdateWarningIcons = false;
        }
        /**
         * Returns the form's error summary (div).
         */
        private formErrorSummary($form: JQuery<HTMLElement>): JQuery<HTMLElement> {
            var $summary = $(".validation-summary-errors", $form);
            if ($summary.length !== 1) throw "Error summary not found";/*DEBUG*/
            return $summary;
        }

        /**
         * Serializes the form and returns a name/value pairs array
         */
        public serializeFormArray(form: HTMLFormElement): YetaWF.NameValuePair[] {
            // disable all fields that we don't want to submit (marked with YConfigs.Forms.CssFormNoSubmit)
            var $disabledFields = $(`.${YConfigs.Forms.CssFormNoSubmit},.yform-nosubmit-temp`, $(form)).not(":disabled");
            $disabledFields.attr("disabled", "disabled");
            // disable all input fields in containers (usually grids) - we don't want to submit them - they're collected separately
            var $disabledGridFields = $(`.${YConfigs.Forms.CssFormNoSubmitContents} input,.${YConfigs.Forms.CssFormNoSubmitContents} select,.${YConfigs.Forms.CssFormNoSubmitContents} textarea`, $(form)).not(":disabled");
            $disabledGridFields.attr("disabled", "disabled");
            // serialize the form
            var formData = $(form).serializeArray();
            // and enable all the input fields we just disabled
            $disabledFields.removeAttr("disabled");
            $disabledGridFields.removeAttr("disabled");
            return formData;
        }
        /**
         * Validate all fields in the current form.
         */
        public validate(form: HTMLFormElement): void {
            // mark all input fields in containers (usually grids) - we don't want to validate
            var $markedFields = $(`.${YConfigs.Forms.CssFormNoSubmitContents} input,.${YConfigs.Forms.CssFormNoSubmitContents} select,.${YConfigs.Forms.CssFormNoSubmitContents} textarea`, $(form)).not(":disabled");
            $markedFields.addClass("yform-novalidate");
            // validate
            let $f = ($(form) as any);
            let fValidator = $f.validate() as JQueryValidation.Validator;
            fValidator.resetForm();
            // and enable all the input fields we just disabled
            $markedFields.removeClass("yform-novalidate");
        }
        /**
         * Returns whether all fields in the current form are valid.
         */
        public isValid(form: HTMLFormElement): boolean {
            // mark all input fields in containers (usually grids) - we don't want to validate
            var $markedFields = $(`.${YConfigs.Forms.CssFormNoSubmitContents} input,.${YConfigs.Forms.CssFormNoSubmitContents} select,.${YConfigs.Forms.CssFormNoSubmitContents} textarea`, $(form)).not(":disabled");
            $markedFields.addClass("yform-novalidate");
            // validate
            let $f = ($(form) as any);
            let valid = $f.valid();
            // and enable all the input fields we just disabled
            $markedFields.removeClass("yform-novalidate");
            return valid;
        }
        /**
         * If there is a validation in the specified tab control, the tab is activated.
         */
        public setErrorInTab(tabctrl: HTMLElement): void {
            var $tabctrl = $(tabctrl);
            // get the first field in error (if any)
            var errField = $YetaWF.getElement1BySelectorCond(".input-validation-error", [tabctrl]);
            if (errField) {
                // find out which tab panel we're on
                var ttabpanel = $YetaWF.elementClosest(errField, "div.t_tabpanel");
                var panel = ttabpanel.getAttribute("data-tab") as number | null;
                if (!panel) throw "We found a panel in a tab control without panel number (data-tab attribute).";/*DEBUG*/
                // get the tab entry
                var $te = $("ul.t_tabstrip > li", $tabctrl).eq(panel);
                if ($te.length === 0) throw "We couldn't find the tab entry for panel " + panel;/*DEBUG*/
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery)
                    $tabctrl.tabs("option", "active", panel);
                else if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.Kendo)
                    $tabctrl.data("kendoTabStrip").activateTab($te);
                else throw "Unknown tab style";/*DEBUG*/
            }
        }

        // Forms initialization

        /**
         * Initialize the form when page/content is ready.
         * No external use.
         */
        public initForm(tag: HTMLElement): void {

            var $tag = $(tag);
            ($ as any).validator.unobtrusive.parse($("form", $tag));
            ($("form", $tag) as any).addTriggersToJqueryValidate().triggerElementValidationsOnFormValidation();

            var $forms = $("form", $tag).filter(".yValidateImmediately");
            if ($forms.length > 0) {
                $forms.each((index: number, elem: HTMLElement) => {
                    var f: any = $(elem);
                    f.validate();
                    f.valid(); // force all fields to show valid/not valid
                });
            }
        }

        /**
         * Initialize overall form validation.
         * No external use.
         */
        public initValidation(): void {

            $(document).ready(() => {

                // running this overrides some jQuery Validate stuff so we can hook into its validations.
                // triggerElementValidationsOnFormValidation is optional and will fire off all of your
                // element validations WHEN the form validation runs ... it requires jquery.validate.unobtrusive
                ($("form") as any).addTriggersToJqueryValidate().triggerElementValidationsOnFormValidation();

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
                // tslint:disable-next-line:typedef only-arrow-functions
                $("body").on("elementValidationError", function (element) {
                    var fi: FormsImpl = YetaWF_FormsImpl as FormsImpl;
                    if (fi.dontUpdateWarningIcons) return;
                    var input = element.target;
                    var form = $YetaWF.Forms.getForm(input);
                    var $form = $(form);
                    var name = input.getAttribute("name");
                    // remove the error icon
                    var $err = $(`img.${YConfigs.Forms.CssWarningIcon}[name="${name}"]`, $form);
                    $err.remove();
                    // find the validation message
                    var $val = $(`span.field-validation-error[data-valmsg-for="${name}"]`, $form);// get the validation message (which follows the input field but is hidden via CSS)
                    // some templates incorrectly add  @Html.ValidationMessageFor(m => Model) to the rendered template - THIS IS WRONG
                    // rather than going back and testing each template, we'll just use the first validation error for the field we find.
                    if ($val.length < 1) throw "Validation message not found";/*DEBUG*/
                    // insert a new error icon
                    $val.eq(0).before(`<img src="${$YetaWF.htmlAttrEscape(YConfigs.Forms.CssWarningIconUrl)}" name="${name}" class="${YConfigs.Forms.CssWarningIcon}" ${YConfigs.Basics.CssTooltip}="${$YetaWF.htmlAttrEscape($val.text())}"/>`);
                });
                // tslint:disable-next-line:typedef only-arrow-functions
                $("body").on("elementValidationSuccess", function (element) {
                    var fi: FormsImpl = YetaWF_FormsImpl as FormsImpl;
                    if (fi.dontUpdateWarningIcons) return;
                    var input = element.target;
                    var form = $YetaWF.Forms.getForm(input);
                    var $form = $(form);
                    var name = input.getAttribute("name");
                    // remove the error icon
                    var $err = $(`img.${YConfigs.Forms.CssWarningIcon}[name="${name}"]`, $form);
                    $err.remove();
                });
            });
        }
    }
}

// tslint:disable-next-line:variable-name
var YetaWF_FormsImpl: YetaWF.IFormsImpl = new YetaWF_ComponentsHTML.FormsImpl();

/* Page load */
$YetaWF.addWhenReady((YetaWF_FormsImpl as YetaWF_ComponentsHTML.FormsImpl).initForm);

/* Initialize validation system */
(YetaWF_FormsImpl as YetaWF_ComponentsHTML.FormsImpl).initValidation();
