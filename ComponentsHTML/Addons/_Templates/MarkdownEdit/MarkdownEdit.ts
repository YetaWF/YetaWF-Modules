/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/// <reference types="showdown" />

namespace YetaWF_ComponentsHTML {

    export class MarkdownEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_markdown";
        public static readonly SELECTOR: string = ".yt_markdown.t_edit";
        public static readonly EVENT: string = "markdown_change";

        private TextArea: HTMLTextAreaElement;
        private Preview: HTMLElement;
        private InputHTML: HTMLInputElement;

        constructor(controlId: string /*, setup: MarkdownEditSetup*/) {
            super(controlId, MarkdownEditComponent.TEMPLATE, MarkdownEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.TextArea,
                ChangeEvent: MarkdownEditComponent.EVENT,
                GetValue: (control: MarkdownEditComponent): string | null => {
                    return control.TextArea.value;
                },
                Enable: (control: MarkdownEditComponent, enable: boolean, clearOnDisable: boolean): void => { },
            });

            this.TextArea = $YetaWF.getElement1BySelector("textarea", [this.Control]) as HTMLTextAreaElement;
            this.Preview = $YetaWF.getElement1BySelector(".t_previewpane", [this.Control]);
            this.InputHTML = $YetaWF.getElement1BySelector(".t_html", [this.Control]) as HTMLInputElement;

            // Update rendered html before form submit
            $YetaWF.Forms.addPreSubmitHandler(true, {
                form: $YetaWF.Forms.getForm(this.Control),
                callback: (entry: YetaWF.SubmitHandlerEntry): void => {
                    this.toHTML();
                },
                userdata: this
            });

            $YetaWF.registerEventHandler(this.TextArea, "blur", null, (ev: Event): boolean => {
                FormsSupport.validateElement(this.TextArea);
                var event = document.createEvent("Event");
                event.initEvent(MarkdownEditComponent.EVENT, true, true);
                this.Control.dispatchEvent(event);
                return true;
            });
        }

        private toHTML(): void {
            let converter = new showdown.Converter({ "headerLevelStart": 3, "simplifiedAutoLink": true, "excludeTrailingPunctuationFromURLs": true, "literalMidWordUnderscores": true});
            let html = converter.makeHtml(this.TextArea.value);
            this.Preview.innerHTML = html;
            this.InputHTML.value = html;
        }
        public makeVisible(): void {
            if ($YetaWF.isVisible(this.Preview)) {
                this.toHTML();
            }
        }
    }

    // inner tab control switched
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTACTIVATEDIV, null, (ev: CustomEvent<YetaWF.DetailsActivateDiv>): boolean => {
        for (let tag of ev.detail.tags) {
            let md = MarkdownEditComponent.getControlFromTagCond<MarkdownEditComponent>(tag, MarkdownEditComponent.SELECTOR);
            if (md)
                md.makeVisible();
        }
        return true;
    });
}
