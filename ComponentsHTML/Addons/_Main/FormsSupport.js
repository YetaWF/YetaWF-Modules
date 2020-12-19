"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
/* This is only used by components in ComponentsHTML (this package) */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var FormsSupportClass = /** @class */ (function () {
        function FormsSupportClass() {
        }
        // Validation
        /**
         * Validate one element.
         * If the contents are empty the field will be fully validated. If contents are present, the error indicator is reset.
         * Full validation takes place on blur (or using validateElementFully).
         * Used by components that may or may not be used within a form to support validation.
         * If the component is within a form, validation occurs, otherwise it is simply ignored.
         */
        FormsSupportClass.prototype.validateElement = function (elem, hasValue) {
            if ($YetaWF.FormsAvailable())
                $YetaWF.Forms.validateElement(elem, hasValue);
        };
        /**
         * Validate one element.
         * Full validation takes place.
         * Used by components that may or may not be used within a form to support validation.
         * If the component is within a form, validation occurs, otherwise it is simply ignored.
         */
        FormsSupportClass.prototype.validateElementFully = function (elem) {
            if ($YetaWF.FormsAvailable())
                $YetaWF.Forms.validateElementFully(elem);
        };
        /**
         * Clear validation error for one element.
         * Used by components that may or may not be used within a form to support validation.
         * If the component is within a form, validation occurs, otherwise it is simply ignored.
         */
        FormsSupportClass.prototype.clearValidation1 = function (elem) {
            if ($YetaWF.FormsAvailable())
                $YetaWF.Forms.clearValidation1(elem);
        };
        return FormsSupportClass;
    }());
    YetaWF_ComponentsHTML.FormsSupportClass = FormsSupportClass;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var FormsSupport = new YetaWF_ComponentsHTML.FormsSupportClass();

//# sourceMappingURL=FormsSupport.js.map
