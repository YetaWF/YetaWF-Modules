/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

declare var ClassicEditor: any;

namespace YetaWF_ComponentsHTML {

    //$$$export interface IPackageVolatiles {
    //    DateTimeFormat: string;
    //}

    interface ContentSetup {
    }

    export class ContentEditComponent extends YetaWF.ComponentBase<HTMLInputElement> {

        public static readonly SELECTOR: string = ".yt_content.t_edit";

        constructor(controlId: string, setup: ContentSetup) {
            super(controlId);

            $YetaWF.addObjectDataById(controlId, this);

            ClassicEditor
                .create($YetaWF.getElementById(this.ControlId), {
                    //highlight: {
                    //    options: [
                    //        {
                    //            model: 'greenMarker',
                    //            class: 'marker-green',
                    //            title: 'Green marker',
                    //            color: 'var(--ck-highlight-marker-green)',
                    //            type: 'marker'
                    //        },
                    //        {
                    //            model: 'redPen',
                    //            class: 'pen-red',
                    //            title: 'Red pen',
                    //            color: 'var(--ck-highlight-pen-red)',
                    //            type: 'pen'
                    //        }
                    //    ]
                    //},
                    //toolbar: [
                    //    'heading', '|', 'bulletedList', 'numberedList', 'highlight', 'undo', 'redo'
                    //]
                })
                .then(editor => { //$$$$
                    {
                        debugger;
                        //editor.ui.componentFactory.names();
                        console.log(editor);
                    }
                })
                .catch(error => {
                    {
                        console.error(error);
                    }
                });
        }
        //get value(): Date {
        //    return new Date(this.Hidden.value);
        //}
        //get valueText(): string {
        //    return this.Hidden.value;
        //}
        //set value(val: Date) {
        //    this.setHidden(val);
        //    this.kendoDateTimePicker.value(val);
        //}
        //public clear(): void {
        //    this.setHiddenText("");
        //    this.kendoDateTimePicker.value("");
        //}
        //public enable(enabled: boolean): void {
        //    this.kendoDateTimePicker.enable(enabled);
        //}
        public destroy(): void {
            //$$$this.kendoDateTimePicker.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        }

        public static getControlFromTag(elem: HTMLElement): ContentEditComponent { return super.getControlBaseFromTag<ContentEditComponent>(elem, ContentEditComponent.SELECTOR); }
        public static getControlFromSelector(selector: string, tags: HTMLElement[]): ContentEditComponent { return super.getControlBaseFromSelector<ContentEditComponent>(selector, ContentEditComponent.SELECTOR, tags); }
        public static getControlById(id: string): ContentEditComponent { return super.getControlBaseById<ContentEditComponent>(id, ContentEditComponent.SELECTOR); }
    }

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBase.clearDiv<ContentEditComponent>(tag, ContentEditComponent.SELECTOR, (control: ContentEditComponent): void => {
            control.destroy();
        });
    });
}

