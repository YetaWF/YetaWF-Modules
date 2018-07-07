/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* This is only used by components in ComponentsHTML (this package) */

namespace YetaWF_ComponentsHTML {

    export class FormsSupportClass {

        // Validation

        private dontUpdateWarningIcons: boolean = false;

        /**
         * Validate one element.
         */
        public validateElement(elem: HTMLElement): void {
            if (typeof YetaWF_Forms !== "undefined" && YetaWF_Forms !== undefined) (YetaWF_Forms as any).validateElement($(elem)); //$$$
        }

        /**
         * Returns whether the form has errors.
         */
        public hasErrors($form: JQuery<HTMLElement>): boolean {
            return $('.validation-summary-errors li', $form).length > 0;
        };

        /**
         * Returns the form's error summary (div).
         */
        public formErrorSummary($form: JQuery<HTMLElement>): JQuery<HTMLElement> {
            var $summary = $('.validation-summary-errors', $form);
            if ($summary.length != 1) throw "Error summary not found";/*DEBUG*/
            return $summary;
        };

        public showErrors($form: JQuery<HTMLElement>): void {
            var $summary: JQuery<HTMLElement> = this.formErrorSummary($form);
            var $list = $('ul li', $summary);

            // only show unique messages (no duplicates)
            var list: string[] = [];
            $list.each(function () {
                list.push($(this).text());
            });
            var uniqueMsgs : string[] = [];
            $.each(list, function (i, el) {
                if ($.inArray(el, uniqueMsgs) === -1) uniqueMsgs.push(el);
            });

            // build output
            var s : string = "";
            $.each(uniqueMsgs, function (i, el) {
                s += el + '(+nl)';
            });
            this.dontUpdateWarningIcons = true;
            YetaWF_Basics.Y_Error(YLocs.Forms.FormErrors + s);
            this.dontUpdateWarningIcons = false;

            if (this.dontUpdateWarningIcons) { }//$$$remove
        };
    }
}

var FormsSupport: YetaWF_ComponentsHTML.FormsSupportClass = new YetaWF_ComponentsHTML.FormsSupportClass();