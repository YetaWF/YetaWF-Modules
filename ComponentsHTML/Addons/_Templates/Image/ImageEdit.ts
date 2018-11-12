/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface ImageEditSetup {
        UploadId: string;
    }

    export class ImageEditComponent extends YetaWF.ComponentBase<HTMLDivElement> {

        private static readonly CLEAREDFILE: string = "(CLEARED)";

        private Setup: ImageEditSetup;
        private UploadControl: FileUpload1Component;
        private PreviewImg: HTMLImageElement;
        private HiddenInput: HTMLInputElement;
        private HaveImageDiv: HTMLDivElement;

        constructor(controlId: string, setup: ImageEditSetup) {
            super(controlId);
            this.Setup = setup;

            this.UploadControl = FileUpload1Component.getControlById(this.Setup.UploadId);
            this.PreviewImg = $YetaWF.getElement1BySelector(".t_preview", [this.Control]) as HTMLImageElement;
            this.HiddenInput = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;
            this.HaveImageDiv = $YetaWF.getElement1BySelector(".t_haveimage", [this.Control]) as HTMLDivElement;

            if (this.HiddenInput.value === ImageEditComponent.CLEAREDFILE)
                this.HiddenInput.value = "";

            // set upload control settings
            $(this.UploadControl).data({
                getFileName: (): string => { return this.HiddenInput.value; },
                successfullUpload: (js: any): void => {
                    this.HiddenInput.value = js.filename;
                    this.setPreview(js.filename);
                    this.HaveImageDiv.style.display = js.filename.length > 0 ? "block" : "none";
                }
            });

            // handle the clear button
            $YetaWF.registerEventHandler(this.Control, "click", "input.t_clear", (ev: MouseEvent):boolean => {
                this.UploadControl.RemoveFile(this.HiddenInput.value);
                this.clearFileName();
                return false;
            });

            if (this.HiddenInput.value.length === 0)
                this.clearFileName();
        }
        private clearFileName(): void {
            this.HiddenInput.value = ImageEditComponent.CLEAREDFILE;
            this.setPreview("");
            this.HaveImageDiv.style.display = "none";
        }
        private setPreview(name: string): void {
            var currUri = $YetaWF.parseUrl(this.PreviewImg.src);
            currUri.removeSearch("Name");
            currUri.addSearch("Name", name);
            this.PreviewImg.src = currUri.toUrl();
        }
    }
}




