/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        Only1FileSupported: string;
    }

    interface FileUpload1Setup {
        SaveUrl: string;
        RemoveUrl: string;
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

                if (!ev.dataTransfer || !ev.dataTransfer.files || ev.dataTransfer!.files!.length !== 1) {
                    $YetaWF.error(YLocs.YetaWF_ComponentsHTML.Only1FileSupported);
                    return false;
                }

                const files: FileList = ev.dataTransfer.files;
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

            let uri = $YetaWF.parseUrl(this.Setup.SaveUrl);

            fd.append("__filename", this.InputFileName.files![0]);

            if (this.GetFileNameCallback) {
                const filename = this.GetFileNameCallback();
                uri.addSearch("__lastInternalName", filename);// the previous real filename of the file to remove
            }

            const info = $YetaWF.Forms.getJSONInfo(this.Control);
            uri.replaceSearch(YConfigs.Basics.ModuleGuid, info.ModuleGuid);

            document.cookie = `${YConfigs.Basics.AntiforgeryCookieName}=${YVolatile.Basics.AntiforgeryCookieToken}`;

            const request: XMLHttpRequest = new XMLHttpRequest();
            request.open("POST", uri.toUrl(), true);
            request.setRequestHeader(YConfigs.Basics.AntiforgeryRequestName, YVolatile.Basics.AntiforgeryRequestToken);

            request.upload.onprogress = (ev: ProgressEvent<EventTarget>): any => {
                let percent = 0;
                const position = ev.loaded;
                const total = ev.total;
                if (ev.lengthComputable) {
                    percent = Math.ceil(position / total * 100);
                    if (this.ProgressBar)
                        this.ProgressBar.value = Number(percent);
                }
            };

            $YetaWF.handleReadyStateChange(request, (success: boolean, response: FileUploadResponse): void => {
                if (this.ProgressBar) {
                    this.ProgressBar.hide();
                    this.ProgressBar.reset();
                }
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
            let uri = $YetaWF.parseUrl(this.Setup.RemoveUrl);
            const info = $YetaWF.Forms.getJSONInfo(this.Control);
            $YetaWF.postJSON(uri, info, null, null, (success: boolean, data: FileUploadRemoveResponse): void => {
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





