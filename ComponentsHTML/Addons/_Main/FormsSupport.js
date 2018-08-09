"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
/* This is only used by components in ComponentsHTML (this package) */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var FormsSupportClass = /** @class */ (function () {
        function FormsSupportClass() {
        }
        // Validation
        /**
         * Validate one element.
         * Used by components that may or may not be used within a form to support validation.
         * If the component is within a form, validation occurs, otherwise it is simply ignored.
         */
        FormsSupportClass.prototype.validateElement = function (elem) {
            if ($YetaWF.FormsAvailable())
                $YetaWF.Forms.validateElement(elem);
        };
        return FormsSupportClass;
    }());
    YetaWF_ComponentsHTML.FormsSupportClass = FormsSupportClass;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var FormsSupport = new YetaWF_ComponentsHTML.FormsSupportClass();

//# sourceMappingURL=FormsSupport.js.map
