/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* This is only used by components in ComponentsHTML (this package) */

namespace YetaWF_ComponentsHTML {

    export class FormsSupportClass {

        // Validation

        /**
         * Validate one element.
         * If the contents are empty the field will be fully validated. If contents are present, the error indicator is reset.
         * Full validation takes place on blur (or using validateElementFully).
         * Used by components that may or may not be used within a form to support validation.
         * If the component is within a form, validation occurs, otherwise it is simply ignored.
         */
        public validateElement(elem: HTMLElement, hasValue?: (value:any) => boolean): void {
            if ($YetaWF.FormsAvailable()) $YetaWF.Forms.validateElement(elem, hasValue);
        }
        /**
         * Validate one element.
         * Full validation takes place.
         * Used by components that may or may not be used within a form to support validation.
         * If the component is within a form, validation occurs, otherwise it is simply ignored.
         */
        public validateElementFully(elem: HTMLElement): void {
            if ($YetaWF.FormsAvailable()) $YetaWF.Forms.validateElementFully(elem);
        }
        /**
         * Clear validation error for one element.
         * Used by components that may or may not be used within a form to support validation.
         * If the component is within a form, validation occurs, otherwise it is simply ignored.
         */
        public clearValidation1(elem: HTMLElement): void {
            if ($YetaWF.FormsAvailable()) $YetaWF.Forms.clearValidation1(elem);
        }
    }
}

var FormsSupport: YetaWF_ComponentsHTML.FormsSupportClass = new YetaWF_ComponentsHTML.FormsSupportClass();

