/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* This is only used by components in ComponentsHTML (this package) */

namespace YetaWF_ComponentsHTML {

    export class FormsSupportClass {

        // Validation

        /**
         * Validate one element.
         * Used by components that may or may not be used within a form to support validation.
         * If the component is within a form, validation occurs, otherwise it is simply ignored.
         */
        public validateElement(elem: HTMLElement): void {
            if (typeof YetaWF_Forms !== "undefined" && YetaWF_Forms !== undefined) YetaWF_Forms.validateElement($(elem));
        }
    }
}

var FormsSupport: YetaWF_ComponentsHTML.FormsSupportClass = new YetaWF_ComponentsHTML.FormsSupportClass();

