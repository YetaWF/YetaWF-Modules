/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

namespace YetaWF_ImageRepository {

    export class FlashRepository {

        private Control: HTMLDivElement;
        private Hidden: HTMLInputElement;
        private List: HTMLSelectElement;
        private ButtonDiv: HTMLDivElement;
        private ClearButton: HTMLAnchorElement;
        private RemoveButton: HTMLAnchorElement;
        private UploadControl: YetaWF_ComponentsHTML.FileUpload1Component | null;

        public constructor(divId: string) {

            this.Control = $YetaWF.getElementById(divId) as HTMLDivElement;
            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;
            this.List = $YetaWF.getElement1BySelector("select[name='List']", [this.Control]) as HTMLSelectElement;
            this.ButtonDiv = $YetaWF.getElement1BySelector(".t_haveflash", [this.Control]) as HTMLDivElement;
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
                $YetaWF.setLoading(true);

                var uri = $YetaWF.parseUrl(this.RemoveButton.href);
                uri.removeSearch("Name");
                uri.addSearch("Name", this.Hidden.value);

                var request: XMLHttpRequest = new XMLHttpRequest();
                request.open("POST", uri.toUrl(), true);
                request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                request.onreadystatechange = (ev: Event): any => {
                    if (request.readyState === 4 /*DONE*/) {
                        $YetaWF.setLoading(false);
                        $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, undefined, undefined, (result: string) => {

                            $YetaWF.setLoading(false);

                            if (result.startsWith(YConfigs.Basics.AjaxJavascriptReturn)) {
                                var script = result.substring(YConfigs.Basics.AjaxJavascriptReturn.length);
                                // tslint:disable-next-line:no-eval
                                eval(script);
                                return;
                            } else if (result.startsWith(YConfigs.Basics.AjaxJavascriptErrorReturn)) {
                                var script = result.substring(YConfigs.Basics.AjaxJavascriptErrorReturn.length);
                                // tslint:disable-next-line:no-eval
                                eval(script);
                                return;
                            }
                            var resp: YetaWF_ComponentsHTML.FileUploadRemoveResponse = JSON.parse(result);
                            // tslint:disable-next-line:no-eval
                            eval(resp.Result);

                            this.List.innerHTML = resp.List;
                            this.clearFileName();
                        });
                    }
                };
                request.send();
                return false;
            });
        }

        private setPreview(name: string| null): void {
            this.ButtonDiv.style.display = (name && name.length > 0) ? "" : "none";

            var param = $YetaWF.getElement1BySelectorCond(".t_preview param[name='movie']", [this.Control]) as HTMLParamElement;
            var embed = $YetaWF.getElement1BySelectorCond(".t_preview embed", [this.Control]) as HTMLEmbedElement;
            var obj = $YetaWF.getElement1BySelectorCond(".t_preview object", [this.Control]) as HTMLObjectElement;
            if (obj) {
                // change object data= (if present)
                if (obj && obj.data) {
                    var currUri = $YetaWF.parseUrl(obj.data);
                    currUri.removeSearch("Name");
                    currUri.addSearch("Name", name);
                    obj.data = currUri.toUrl();
                }
                // change param movie (if present)
                if (param && param.value) {
                    var currUri = $YetaWF.parseUrl(param.value);
                    currUri.removeSearch("Name");
                    currUri.addSearch("Name", name);
                    param.value = currUri.toUrl();
                }
                // change embed (if present)
                if (embed && embed.src) {
                    currUri = $YetaWF.parseUrl(embed.src);
                    currUri.removeSearch("Name");
                    currUri.addSearch("Name", name);
                    embed.src = currUri.toUrl();
                }
                var s = obj.outerHTML;
                obj.outerHTML = s;// replace entire object to make flash recognize the image change
            }
        }
        private clearFileName(): void {
            this.Hidden.value = "";
            this.List.value = "";
            this.setPreview(null);
        }
    }
}