/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

declare var CKEDITOR: any;

namespace YetaWF_ComponentsHTML {

    interface TextAreaEditSetup {
        InPartialView: boolean;
        CDNUrl: string;
        EmHeight: Number;
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

            let config: any = {
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

            // save data in the textarea field when the form is submitted
            $YetaWF.Forms.addPreSubmitHandler(setup.InPartialView, {
                form: $YetaWF.Forms.getForm($YetaWF.getElementById(controlId)),
                callback: (entry: YetaWF.SubmitHandlerEntry): void => {
                    let ckEd = entry.userdata;
                    (this.Control as HTMLTextAreaElement).value = ckEd.getData();
                },
                userdata: ckEd,
            });
        }
    }

    // Override the built-in Save button to use our Form submit
    // Need to wait for the ckeditor instance to finish initialization
    // because CKEDITOR.instances.editor.commands is an empty object
    // if you try to use it immediately after CKEDITOR.replace('editor');

    if (typeof CKEDITOR !== "undefined") { // CKEDITOR is only defined when an editable textarea is used

        CKEDITOR.on("instanceReady", (ev: any): void => {

            // Replace the old save's exec function with the new one
            if (ev.editor.commands.save) {
                // Create a new command with the desired exec function
                let overridecmd = new CKEDITOR.command(ev.editor, {
                    exec: (editor: any): void => {
                        let $form = $(editor.element.$).closest("form." + YConfigs.Forms.CssFormAjax);
                        if ($form.length !== 1) throw "Couldn't find form";/*DEBUG*/
                        $YetaWF.Forms.submit($form[0], false);
                    }
                });

                ev.editor.commands.save.exec = overridecmd.exec;
            }
        });
    }

    // when a tab page is switched, resize all the ckeditors in the newly visible panel (custom event)
    // when we're in a float div (property list or tabbed property list) the parent width isn't available until after the
    // page has completely loaded, so we need to set it again.
    // For other cases (outside float div) this does no harm and resizes to the current size.

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTACTIVATEDIV, null, (ev: CustomEvent<YetaWF.DetailsActivateDiv>): boolean => {
        let ckeds = $YetaWF.getElementsBySelector(TextAreaEditComponent.SELECTOR, ev.detail.tags);
        for (let cked of ckeds) {
            let ctrl = TextAreaEditComponent.getControlFromTag<TextAreaEditComponent>(cked, TextAreaEditComponent.SELECTOR);
            let ck = CKEDITOR.instances[cked.id];
            try {
                ck.resize("100%", ctrl.Setup.EmHeight, true);
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



