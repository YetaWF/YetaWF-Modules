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
            this.UploadControl.SetSuccessfullUpload((data: FileUploadResponse): void => {
                this.HiddenInput.value = data.FileName;
                this.setPreview(data.FileName);
                this.HaveImageDiv.style.display = data.FileName.length > 0 ? "block" : "none";
            });
            this.UploadControl.SetGetFileName((): string => { return this.HiddenInput.value; });

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



