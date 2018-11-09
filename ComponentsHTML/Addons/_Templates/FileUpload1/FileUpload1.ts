/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        StatusUploadNoResp: string;
        StatusUploadFailed: string;
        FileTypeError: string;
        FileSizeError: string;
        FallbackMode: string;
    }

    interface FileUpload1Setup {
        SaveUrl: string;
        RemoveUrl: string;
        SerializeForm: boolean;
    }

    export class FileUpload1Component extends YetaWF.ComponentBase<HTMLButtonElement> {

        public static readonly SELECTOR: string = ".yt_fileupload1";

        private Setup: FileUpload1Setup;
        private $Control: JQuery<HTMLElement>;
        private inputFileName: HTMLInputElement;
        private divProgressbar: HTMLDivElement;
        private $divProgressbar: JQuery<HTMLElement> | null = null;

        constructor(controlId: string, setup: FileUpload1Setup) {
            super(controlId);
            this.Setup = setup;

            $YetaWF.addObjectDataById(controlId, this);

            this.inputFileName = $YetaWF.getElement1BySelector("input.t_filename", [this.Control]) as HTMLInputElement;
            this.divProgressbar = $YetaWF.getElement1BySelectorCond(".t_progressbar", [this.Control]) as HTMLDivElement;
            if (this.divProgressbar) {
                this.$divProgressbar = $(this.divProgressbar);
                this.$divProgressbar.progressbar({
                    max: 100,
                    value: 0,
                });
                this.$divProgressbar.hide();
            }

            this.$Control = $(this.Control);

            // trigger upload button
            $YetaWF.registerEventHandler(this.Control, "click", ".t_upload", (ev: MouseEvent): boolean => {
                $(this.inputFileName).trigger("click");
                return false;
            });

            // Uploader control
            ($(this.$Control) as any).dmUploader({
                url: this.Setup.SaveUrl,
                //dataType: 'json',  //don't use otherwise response is not recognized in case of errors
                //allowedTypes: '*',
                //extFilter: 'jpg,png,gif',
                fileName: "__filename",
                onInit: (): void => { },
                onBeforeUpload: (id: string): void => {
                    $YetaWF.setLoading(true);
                },
                onExtraData: (id: string, data: any): void => {
                    if (this.$Control.data().getFileName !== undefined) {
                        var filename = this.$Control.data().getFileName();
                        data.append("__lastInternalName", filename);// the previous real filename of the file to remove
                    }
                    if (this.Setup.SerializeForm) {
                        var form = $YetaWF.Forms.getForm(this.Control);
                        var formData = $YetaWF.Forms.serializeFormArray(form);
                        for (let f of formData) {
                            data.append(f.name, f.value);
                        }
                    }
                },
                onNewFile: (id: string, file: string): void => {
                    console.log(`onNewFile #${id} ${file}`);
                },
                onComplete: (): void => {
                    if (this.$divProgressbar)
                        this.$divProgressbar.hide();
                },
                onUploadProgress: (id: string, percent: string): void => {
                    if (this.$divProgressbar) {
                        this.$divProgressbar.show();
                        this.$divProgressbar.progressbar("value", percent);
                    }
                },
                onUploadError: (id: string, message: string): void => {
                    $YetaWF.setLoading(false);
                    if (message === "")
                        $YetaWF.error(YLocs.YetaWF_ComponentsHTML.StatusUploadNoResp);
                    else
                        $YetaWF.error(YLocs.YetaWF_ComponentsHTML.StatusUploadFailed.format(message));
                },
                onFileTypeError: (file: string): void => {
                    $YetaWF.error(YLocs.YetaWF_ComponentsHTML.FileTypeError);
                },
                onFileSizeError: (file: string): void => {
                    $YetaWF.error(YLocs.YetaWF_ComponentsHTML.FileSizeError);
                },
                onFallbackMode: (message: string): void => {
                    $YetaWF.error(YLocs.YetaWF_ComponentsHTML.FallbackMode);
                },
                onUploadSuccess: (id: string, data: any): void => {
                    //{
                    //    "result":      "$YetaWF.confirm(\"Image \\\"logo_233x133.jpg\\\" successfully uploaded\");",
                    //    "filename": "tempc8eb1eb6-31ef-4e5d-9100-9fab50761a81.jpg",
                    //    "realFilename": "logo_233x133.jpg",
                    //    "attributes": "233 x 123 (w x h)"
                    //}
                    $YetaWF.setLoading(false);
                    if (data.startsWith(YConfigs.Basics.AjaxJavascriptReturn)) {
                        var script = data.substring(YConfigs.Basics.AjaxJavascriptReturn.length);
                        // tslint:disable-next-line:no-eval
                        eval(script);
                        return;
                    }
                    if (data.startsWith(YConfigs.Basics.AjaxJavascriptErrorReturn)) {
                        var script = data.substring(YConfigs.Basics.AjaxJavascriptErrorReturn.length);
                        // tslint:disable-next-line:no-eval
                        eval(script);
                        return;
                    }
                    // result has quotes around it
                    var js = JSON.parse(data);
                    // tslint:disable-next-line:no-eval
                    eval(js.result);
                    if (this.$Control.data().successfullUpload !== undefined)
                        this.$Control.data().successfullUpload(js);
                    //if ($control.data().setAttributes != undefined)
                    //    filename = $control.data().setAttributes('');
                },
            });
        }

        // API
        public RemoveFile = (name: string): void => {
            $.ajax({
                url: this.Setup.RemoveUrl,
                type: 'post',
                data: '__internalName=' + encodeURIComponent(name) + '&__filename=' + encodeURIComponent(name),
                success: (result: any, textStatus: string, jqXHR: JQueryXHR): void => { },
                error: (jqXHR: JQueryXHR, textStatus: string, errorThrown: any): void => {
                    $YetaWF.alert(YLocs.Forms.AjaxError.format(jqXHR.status, jqXHR.statusText), YLocs.Forms.AjaxErrorTitle);
                }
            });
        }

        public destroy(): void {
            $YetaWF.removeObjectDataById(this.Control.id);
        }

        public static getControlFromTag(elem: HTMLElement): FileUpload1Component { return super.getControlBaseFromTag<FileUpload1Component>(elem, FileUpload1Component.SELECTOR); }
        public static getControlFromSelector(selector: string, tags: HTMLElement[]): FileUpload1Component { return super.getControlBaseFromSelector<FileUpload1Component>(selector, FileUpload1Component.SELECTOR, tags); }
        public static getControlById(id: string): FileUpload1Component { return super.getControlBaseById<FileUpload1Component>(id, FileUpload1Component.SELECTOR); }
    }

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBase.clearDiv<FileUpload1Component>(tag, FileUpload1Component.SELECTOR, (control: FileUpload1Component): void => {
            control.destroy();
        });
    });
}





