/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface FileUpload1Setup {
        SaveUrl: string;
        RemoveUrl: string;
        SerializeForm: boolean;
    }

    export interface FileUploadResponse {
        Result: string;
        FileName: string;
        FileNamePlain: string;
        RealFileName: string;
        Attributes: string;
        List: string;
    }
    export interface FileUploadRemoveResponse {
        Result: string;
        List: string;
    }

    export class FileUpload1Component extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_fileupload1";
        public static readonly SELECTOR: string = ".yt_fileupload1";

        private Setup: FileUpload1Setup;
        private UploadButton: HTMLElement;
        private InputFileName: HTMLInputElement;
        private ProgressBar: ProgressBarComponent | null;

        private SuccessfullUploadCallback: ((data: FileUploadResponse) => void) | null = null;
        private GetFileNameCallback: (() => string) | null = null;

        constructor(controlId: string, setup: FileUpload1Setup) {
            super(controlId, FileUpload1Component.TEMPLATE, FileUpload1Component.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: (control: FileUpload1Component): string | null => {
                    return null;
                },
                Enable: (control: FileUpload1Component, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                }
            });

            this.Setup = setup;

            this.UploadButton = $YetaWF.getElement1BySelector(".t_upload", [this.Control]);
            this.InputFileName = $YetaWF.getElement1BySelector("input.t_filename", [this.Control]) as HTMLInputElement;
            this.ProgressBar = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond(YetaWF_ComponentsHTML.ProgressBarComponent.SELECTOR, YetaWF_ComponentsHTML.ProgressBarComponent.SELECTOR, [this.Control]);
            if (this.ProgressBar)
                this.ProgressBar.hide();

            $YetaWF.registerEventHandler(this.UploadButton, "click", null, (ev: MouseEvent): boolean => {
                this.InputFileName.click();
                return false;
            });

            $YetaWF.registerEventHandler(this.Control, "drop", null, (ev: DragEvent): boolean => {
                if (!$YetaWF.isEnabled(this.Control))
                    return false;
                const files: FileList = ev.dataTransfer?.files!;
                this.InputFileName.files = files;
                this.uploadFile();
                return false;
            });

            $YetaWF.registerEventHandler(this.InputFileName, "change", null, (ev: Event): boolean => {
                this.uploadFile();
                return false;
            });
        }

        private uploadFile(): void {

            const fd = new FormData();

            $YetaWF.setLoading(true);
            if (this.ProgressBar)
                this.ProgressBar.show();

            const request: XMLHttpRequest = new XMLHttpRequest();
            request.open("POST", this.Setup.SaveUrl, true);
            //request.setRequestHeader // doesn't work

            fd.append("__filename", this.InputFileName.files![0]);

            if (this.GetFileNameCallback) {
                const filename = this.GetFileNameCallback();
                fd.append("__lastInternalName", filename);// the previous real filename of the file to remove
            }
            if (this.Setup.SerializeForm) {
                var form = $YetaWF.Forms.getForm(this.Control);
                var formData = $YetaWF.Forms.serializeFormArray(form);
                for (let f of formData) {
                    fd.append(f.name, f.value);
                }
            }

            request.onprogress = (ev: ProgressEvent<EventTarget>): any => {
                let percent = 0;
                const position = ev.loaded;
                const total = ev.total;
                if(ev.lengthComputable) {
                    percent = Math.ceil(position / total * 100);
                    if (this.ProgressBar)
                        this.ProgressBar.value = Number(percent);
                }
            };

            $YetaWF.handleReadyStateChange(request, (success: boolean, response: FileUploadResponse): void => {
                if (this.ProgressBar)
                    this.ProgressBar.hide();

                if (success) {
                    this.InputFileName.files = null;
                    this.InputFileName.value = "";
                    if (this.SuccessfullUploadCallback)
                        this.SuccessfullUploadCallback(response);
                    if (response.Result) {
                        // eslint-disable-next-line no-eval
                        eval(response.Result);
                    }
                }
            });

            request.send(fd);
        }

        // API
        public RemoveFile(name: string): void {
            $YetaWF.post(this.Setup.RemoveUrl, null, (success: boolean, data: FileUploadRemoveResponse): void => {
                if (success && data.Result)
                    $YetaWF.message(data.Result);
            });
        }

        public SetSuccessfullUpload(callback: (data: FileUploadResponse) => void): void {
            this.SuccessfullUploadCallback = callback;
        }
        public SetGetFileName(callback: () => string): void {
            this.GetFileNameCallback = callback;
        }
        public enable(enabled: boolean): void {
            $YetaWF.elementEnableToggle(this.Control, enabled);
            $YetaWF.elementEnableToggle(this.UploadButton, enabled);
        }
    }

    // Disable document d&d events to prevent opening the file when we drop it
    document.addEventListener("dragenter", (ev:DragEvent): boolean => {
        ev.stopPropagation(); ev.preventDefault();
        return false;
    });
    document.addEventListener("dragover", (ev:DragEvent): boolean => {
        ev.stopPropagation(); ev.preventDefault();
        return false;
    });
    document.addEventListener("drop", (ev:DragEvent): boolean => {
        ev.stopPropagation(); ev.preventDefault();
        return false;
    });
}





