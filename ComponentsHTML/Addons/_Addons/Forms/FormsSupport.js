"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
/* This is only used by components in ComponentsHTML (this package) */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var FormsSupportClass = /** @class */ (function () {
        function FormsSupportClass() {
            // Validation
            this.dontUpdateWarningIcons = false;
        }
        /**
         * Validate one element.
         */
        FormsSupportClass.prototype.validateElement = function (elem) {
            if (typeof YetaWF_Forms !== "undefined" && YetaWF_Forms !== undefined)
                YetaWF_Forms.validateElement($(elem)); //$$$
        };
        /**
         * Returns whether the form has errors.
         */
        FormsSupportClass.prototype.hasErrors = function ($form) {
            return $('.validation-summary-errors li', $form).length > 0;
        };
        ;
        /**
         * Returns the form's error summary (div).
         */
        FormsSupportClass.prototype.formErrorSummary = function ($form) {
            var $summary = $('.validation-summary-errors', $form);
            if ($summary.length != 1)
                throw "Error summary not found"; /*DEBUG*/
            return $summary;
        };
        ;
        FormsSupportClass.prototype.showErrors = function ($form) {
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
            if (this.dontUpdateWarningIcons) { } //$$$remove
        };
        ;
        return FormsSupportClass;
    }());
    YetaWF_ComponentsHTML.FormsSupportClass = FormsSupportClass;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var FormsSupport = new YetaWF_ComponentsHTML.FormsSupportClass();

//# sourceMappingURL=FormsSupport.js.map
