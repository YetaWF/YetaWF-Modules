/* eslint-disable @typescript-eslint/indent */
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Lightbox#License */
/*!
 * jQuery Lightbox ported to TypeScript - Original by Lokesh Dhakar - http://lokeshdhakar.com/projects/lightbox2/
 */

namespace YetaWF_Lightbox {

    interface LinkEntry {
        Url: string;
        Title: string;
        Group: string;
    }

    export class LightboxModule {

        public static On: boolean = true;

        private LightboxOverlay: HTMLElement|null = null;
        private LightboxDiv: HTMLElement|null = null;
        private ImageLinks : LinkEntry[] | null = null;
        private ImageIndex: number = 0;

        constructor() {

            $YetaWF.registerEventHandlerBody("click", "a[rel^=lightbox], area[rel^=lightbox], a[data-lightbox], area[data-lightbox]", (ev: MouseEvent) : boolean => {
                if (!LightboxModule.On) return true;
                let link = ev.__YetaWFElem;
                return !YetaWF_Lightbox_Module.open(link);
            });
            $YetaWF.registerEventHandlerBody("keydown", null, (ev: KeyboardEvent) : boolean => {
                if (!LightboxModule.On) return true;
                let key = ev.key;
                if (key === "Escape") {
                    this.close();
                    return false;
                } else if (key === "ArrowRight") {
                    this.nextImage();
                    return false;
                } else if (key === "ArrowLeft") {
                    this.prevImage();
                    return false;
                }
                return true;
            });

            // Handles events turning the addon on/off (used for dynamic content)
            $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, (ev: CustomEvent<YetaWF.DetailsAddonChanged>): boolean => {
                var addonGuid = ev.detail.addonGuid;
                var on = ev.detail.on;
                if (addonGuid === "39244dbc-0536-4c85-88d1-b84b504510ac") {
                    LightboxModule.On = on;
                }
                return true;
            });

            $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
                if (!LightboxModule.On) return true;
                YetaWF_Lightbox_Module.setMaxSize();
                return true;
            });
        }

        private open(link: HTMLElement): boolean {
            $YetaWF.setLoading(true);
            $YetaWF.closeOverlays();
            if (this.getImages(link)) {
                this.addDivs();
                this.showImage();
                $YetaWF.setLoading(false);
                return true;
            } else {
                $YetaWF.setLoading(false);
                return false;
            }
        }
        private close(): void {
            if (this.LightboxDiv) {
                this.LightboxDiv.remove();
                this.LightboxDiv = null;
            }
            if (this.LightboxOverlay) {
                this.LightboxOverlay.remove();
                this.LightboxOverlay = null;
            }
        }

        private getImages(link: HTMLElement): boolean {
            let requestedGroup = $YetaWF.getAttributeCond(link, "data-lightbox") || "";
            let elems = $YetaWF.getElementsBySelector(`[data-lightbox='${requestedGroup}']`);
            this.ImageLinks = [];
            let index = 0;
            for (let elem of elems) {
                let title = $YetaWF.getAttributeCond(elem, "data-title");
                let group = $YetaWF.getAttributeCond(elem, "data-lightbox");
                if (group === requestedGroup) {
                    if (elem.tagName === "A")
                        this.ImageLinks.push( { Url: (elem as HTMLAnchorElement).href, Title: title || "", Group: group || "" } );
                    else if (elem.tagName === "AREA")
                        this.ImageLinks.push( { Url: (elem as HTMLAreaElement).href, Title: title || "", Group: group || "" } );
                    else
                        throw `Unexpected tag ${elem.tagName}`;
                    if (elem === link)
                        this.ImageIndex = index;
                    ++index;
                }
            }
            return this.ImageLinks.length > 0;
        }

        private showImage(): void {
            if (!this.LightboxDiv || !this.ImageLinks) return;
            let entry = this.ImageLinks[this.ImageIndex];
            let image = $YetaWF.getElement1BySelector(".t_image", [this.LightboxDiv]) as HTMLImageElement;
            image.src = entry.Url;
            let caption = $YetaWF.getElement1BySelector(".t_caption", [this.LightboxDiv]) as HTMLSpanElement;
            caption.innerText = entry.Title;
            let imgNumber = $YetaWF.getElement1BySelector(".t_number", [this.LightboxDiv]) as HTMLSpanElement;
            imgNumber.innerText = YLocs.YetaWF_Lightbox.ImageNumber.format(this.ImageIndex+1, this.ImageLinks.length);
            this.setMaxSize();
        }

        private nextImage(): void {
            this.ImageIndex++;
            if (this.ImageIndex >= this.ImageLinks!.length)
                this.ImageIndex = 0;
            this.showImage();
        }
        private prevImage(): void {
            this.ImageIndex--;
            if (this.ImageIndex < 0)
                this.ImageIndex = this.ImageLinks!.length-1;
            this.showImage();
        }

        private setMaxSize(): void {
            if (!this.LightboxDiv || !this.ImageLinks) return;
            let containerImg = $YetaWF.getElement1BySelector(".t_container img", [this.LightboxDiv]);
            let maxWidth = window.innerWidth - this.paddingWidth;
            let maxHeight = window.innerHeight - this.paddingHeight;
            containerImg.style.maxWidth = `${maxWidth}px`;
            containerImg.style.maxHeight = `${maxHeight}px`;
        }

        private get paddingWidth(): number {
            return $YetaWF.isMobile() ? 2*10 : 2*50;
        }
        private get paddingHeight(): number {
            return $YetaWF.isMobile() ? 20 + 50 : 2*50 + 50; // extra for caption
        }

        private addDivs(): void {
            if (!this.LightboxOverlay) {
                this.LightboxOverlay = <div id="lightboxOverlay" class="lightboxOverlay"></div> as HTMLElement;
                document.body.appendChild(this.LightboxOverlay);

                $YetaWF.registerEventHandler(this.LightboxOverlay, "click", null, (ev: MouseEvent): boolean =>{
                    this.close();
                    return false;
                });
            }
            if (!this.LightboxDiv) {
                this.LightboxDiv =
                        <div id="lightbox" class="lightbox">
                            <div class="t_outerContainer">
                                <div class="t_container">
                                    <img class="t_image" src="" />
                                    <div class="t_nav">
                                        <a class="t_prev" href=""></a>
                                        <a class="t_next" href=""></a>
                                    </div>
                                    <div class="t_loader"><a class="t_cancel"></a></div>
                                </div>
                                <div class="t_data">
                                    <div class="t_details"><span class="t_caption"></span><span class="t_number"></span></div>
                                    <div class="t_closeContainer"><a class="t_close"></a></div>
                                </div>
                            </div>
                        </div> as HTMLElement;
                document.body.appendChild(this.LightboxDiv);

                if (this.ImageLinks!.length <= 1) {
                    let prev = $YetaWF.getElement1BySelector(".t_prev", [this.LightboxDiv]);
                    prev.style.display = "none";
                    let next = $YetaWF.getElement1BySelector(".t_next", [this.LightboxDiv]);
                    next.style.display = "none";
                } else {
                    $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".t_prev", [this.LightboxDiv]), "click", null, (ev: MouseEvent): boolean => {
                        this.prevImage();
                        return false;
                    });
                    $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".t_next", [this.LightboxDiv]), "click", null, (ev: MouseEvent): boolean => {
                        this.nextImage();
                        return false;
                    });
                }

                $YetaWF.registerEventHandler(this.LightboxDiv, "click", null, (ev: MouseEvent): boolean =>{
                    this.close();
                    return false;
                });
                $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".t_close", [this.LightboxDiv]), "click", null, (ev: MouseEvent): boolean => {
                    this.close();
                    return false;
                });
            }
        }
    }
}

var YetaWF_Lightbox_Module = new YetaWF_Lightbox.LightboxModule();

