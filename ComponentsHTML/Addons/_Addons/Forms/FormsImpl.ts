/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* Forms implementation required by YetaWF */

declare var _YetaWF_Forms: any;//$$$$remove

namespace YetaWF_ComponentsHTML {

    export class FormsImpl implements IFormsImpl {

        // Partialform Initialization

        public initPartialFormTS(elem: HTMLElement): void {
            this.initPartialForm($(elem));
        }
        public initPartialForm($partialForm: JQuery<HTMLElement>): void {

            // run registered actions (usually javascript initialization, similar to $doc.ready()
            YetaWF_Basics.processAllReady($partialForm);
            YetaWF_Basics.processAllReadyOnce($partialForm);

            // get all fields with errors (set server-side)
            var $errs = $('.field-validation-error', $partialForm);
            // add warning icons to validation errors
            $errs.each(function () {
                var $val = $(this);
                var name = $val.attr("data-valmsg-for");
                var $err = $(`img.${YConfigs.Forms.CssWarningIcon}[name="${name}"]`, $val.closest('form'));
                $err.remove();
                $val.before(`<img src="${YetaWF_Basics.htmlAttrEscape(YConfigs.Forms.CssWarningIconUrl)}" name=${name} class="${YConfigs.Forms.CssWarningIcon}" ${YConfigs.Basics.CssTooltip}="${YetaWF_Basics.htmlAttrEscape($val.text())}"/>`);
            });

            // show error popup
            var hasErrors = FormsSupport.hasErrors($partialForm);
            if (hasErrors)
                _YetaWF_Forms.showErrors($partialForm);
        };

    }
}

var YetaWF_FormsImpl: IFormsImpl = new YetaWF_ComponentsHTML.FormsImpl();

