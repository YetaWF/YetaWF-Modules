"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var FormsImpl = /** @class */ (function () {
        function FormsImpl() {
        }
        // Partialform Initialization
        FormsImpl.prototype.initPartialFormTS = function (elem) {
            this.initPartialForm($(elem));
        };
        FormsImpl.prototype.initPartialForm = function ($partialForm) {
            // run registered actions (usually javascript initialization, similar to $doc.ready()
            YetaWF_Basics.processAllReady($partialForm);
            YetaWF_Basics.processAllReadyOnce($partialForm);
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
            // show error popup
            var hasErrors = FormsSupport.hasErrors($partialForm);
            if (hasErrors)
                _YetaWF_Forms.showErrors($partialForm);
        };
        ;
        return FormsImpl;
    }());
    YetaWF_ComponentsHTML.FormsImpl = FormsImpl;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var YetaWF_FormsImpl = new YetaWF_ComponentsHTML.FormsImpl();

//# sourceMappingURL=FormsImpl.js.map
