/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/// <reference types="ckeditor" />

namespace YetaWF_ComponentsHTML {

    interface TextAreaEditSetup {
        InPartialView: boolean;
        CDNUrl: string;
        EmHeight: number;
        RestrictedHtml: boolean;
        FilebrowserImageBrowseUrl: string;
        FilebrowserImageBrowseLinkUrl: string;
        FilebrowserPageBrowseUrl: string;
        FilebrowserWindowFeatures: string;
    }

    export class TextAreaEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_textarea";
        public static readonly SELECTOR: string = ".yt_textarea.t_edit";

        public Setup: TextAreaEditSetup;

        constructor(controlId: string, setup: TextAreaEditSetup) {
            super(controlId, TextAreaEditComponent.TEMPLATE, TextAreaEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.TextArea,
                ChangeEvent: null,
                GetValue: (control: HTMLTextAreaElement): string | null => {
                    return control.value;
                },
                Enable: (control: HTMLTextAreaElement, enable: boolean, clearOnDisable: boolean): void => {
                    let editor = CKEDITOR.instances[control.id];
                    if (enable) {
                        control.removeAttribute("readonly");
                        $YetaWF.elementRemoveClass(control, "k-state-disabled");
                        try {
                            editor.setReadOnly(false);
                        } catch (e) {}
                    } else {
                        control.setAttribute("readonly", "readonly");
                        $YetaWF.elementAddClass(control, "k-state-disabled");
                        try {
                            editor.setReadOnly(true);
                        } catch (e) { }
                        if (clearOnDisable)
                            editor.setData("");
                    }
                },
            });
            this.Setup = setup;

            let config: CKEDITOR.config = {
                customConfig: setup.CDNUrl,
                height: `${setup.EmHeight}em`,
                allowedContent: setup.RestrictedHtml ? false : true,
                filebrowserWindowFeatures: setup.FilebrowserWindowFeatures
            };
            if (setup.FilebrowserImageBrowseUrl) {
                config.filebrowserImageBrowseUrl = setup.FilebrowserImageBrowseUrl;
                config.filebrowserImageBrowseLinkUrl = setup.FilebrowserImageBrowseUrl;
            }
            if (setup.FilebrowserPageBrowseUrl) {
                config.filebrowserBrowseUrl = setup.FilebrowserPageBrowseUrl;
            }

            let ckEd = CKEDITOR.replace(controlId, config);
            ckEd.on("blur", (): void => {
                (this.Control as HTMLTextAreaElement).value = ckEd.getData();
                FormsSupport.validateElementFully(this.Control);
            });
        }
    }

    // save data in the textarea field when the form is submitted
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Forms.EVENTPRESUBMIT, null, (ev: CustomEvent<YetaWF.DetailsPreSubmit>): boolean => {
        let ckeds = YetaWF.ComponentBaseDataImpl.getControls<TextAreaEditComponent>(TextAreaEditComponent.SELECTOR, [ev.detail.form]);
        for (let cked of ckeds) {
            let ck = CKEDITOR.instances[cked.Control.id];
            (cked.Control as HTMLTextAreaElement).value = ck.getData();
        }
        return true;
    });

    // when a tab page is switched, resize all the ckeditors in the newly visible panel (custom event)
    // when we're in a float div (property list or tabbed property list) the parent width isn't available until after the
    // page has completely loaded, so we need to set it again.
    // For other cases (outside float div) this does no harm and resizes to the current size.

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTACTIVATEDIV, null, (ev: CustomEvent<YetaWF.DetailsActivateDiv>): boolean => {
        let ckeds = YetaWF.ComponentBaseDataImpl.getControls<TextAreaEditComponent>(TextAreaEditComponent.SELECTOR, ev.detail.tags);
        for (let cked of ckeds) {
            let ck = CKEDITOR.instances[cked.Control.id];
            try {
                ck.resize("100%", cked.Setup.EmHeight, true);
            } catch (e) {}
        }
        return true;
    });

    // A <div> is being emptied. Destroy all ckeditors the <div> may contain.
    $YetaWF.registerClearDiv(false, (tag: HTMLElement): boolean => {
        let list = $YetaWF.getElementsBySelector("textarea.yt_textarea", [tag]);
        for (let el of list) {
            if (CKEDITOR.instances[el.id])
                CKEDITOR.instances[el.id].destroy();
        }
        return true;
    });
}



