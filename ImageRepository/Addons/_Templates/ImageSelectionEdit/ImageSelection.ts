/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

namespace YetaWF_ImageRepository {

    export class ImageRepository {

        private Control: HTMLDivElement;
        private Hidden: HTMLInputElement;
        private List: HTMLSelectElement;
        private Image: HTMLImageElement;
        private ButtonDiv: HTMLDivElement;
        private ClearButton: HTMLAnchorElement;
        private RemoveButton: HTMLAnchorElement;
        private UploadControl: YetaWF_ComponentsHTML.FileUpload1Component | null;

        public constructor(divId: string) {

            this.Control = $YetaWF.getElementById(divId) as HTMLDivElement;
            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;
            this.List = $YetaWF.getElement1BySelector("select[name='List']", [this.Control]) as HTMLSelectElement;
            this.Image = $YetaWF.getElement1BySelector(".t_preview img", [this.Control]) as HTMLImageElement;
            this.ButtonDiv = $YetaWF.getElement1BySelector(".t_haveimage", [this.Control]) as HTMLDivElement;
            this.ClearButton = $YetaWF.getElement1BySelector("a[data-name='Clear']", [this.Control]) as HTMLAnchorElement;
            this.RemoveButton = $YetaWF.getElement1BySelector("a[data-name='Remove']", [this.Control]) as HTMLAnchorElement;

            // show initial selection (if any)
            this.List.value = this.Hidden.value;
            this.setPreview(this.List.value);

            this.UploadControl = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond<YetaWF_ComponentsHTML.FileUpload1Component>(".yt_fileupload1", YetaWF_ComponentsHTML.FileUpload1Component.SELECTOR, [this.Control]);
            if (this.UploadControl) {
                this.UploadControl.SetSuccessfullUpload((response: YetaWF_ComponentsHTML.FileUploadResponse): void => {
                    this.Hidden.value = response.FileName;
                    this.List.innerHTML = response.List;
                    this.List.value = response.FileName;
                    this.setPreview(response.FileName);
                });
            }

            // user changed the selected image
            $YetaWF.registerEventHandler(this.List, "change", null, (ev: Event): boolean => {
                this.Hidden.value = this.List.value;
                this.setPreview(this.List.value);
                return false;
            });

            $YetaWF.registerEventHandler(this.ClearButton, "click", null, (ev: MouseEvent): boolean => {
                this.clearFileName();
                return false;
            });
            $YetaWF.registerEventHandler(this.RemoveButton, "click", null, (ev: MouseEvent): boolean => {

                // get url to remove the file

                if ($YetaWF.isLoading) return false;

                var uri = $YetaWF.parseUrl(this.RemoveButton.href);
                uri.removeSearch("Name");
                uri.addSearch("Name", this.Hidden.value);

                const formJson = $YetaWF.Forms.getJSONInfo(this.Control);
                $YetaWF.postJSON(uri, formJson, null, null, (success: boolean, resp: YetaWF_ComponentsHTML.FileUploadRemoveResponse): void => {
                    if (success) {
                        // eslint-disable-next-line no-eval
                        eval(resp.Result);

                        this.List.innerHTML = resp.List;
                        this.clearFileName();
                    }
                });
                return false;
            });
        }

        private setPreview(name: string| null): void {
            this.ButtonDiv.style.display = (name && name.length > 0) ? "" : "none";
            var currUri = $YetaWF.parseUrl(this.Image.src);
            currUri.removeSearch("Name");
            currUri.addSearch("Name", name);
            this.Image.src = currUri.toUrl();
        }
        private clearFileName(): void {
            this.Hidden.value = "";
            this.List.value = "";
            this.setPreview(null);
        }
    }
}