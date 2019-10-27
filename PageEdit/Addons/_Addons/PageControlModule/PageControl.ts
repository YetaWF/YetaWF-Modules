/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

/* Page Control */

namespace YetaWF {
    export interface IConfigs {
        YetaWF_PageEdit: YetaWF_PageEdit.IPackageConfigs;
    }
}
namespace YetaWF_PageEdit {
    export interface IPackageConfigs {
        PageControlMod: string;
        W3CUrl: string;
    }
}

namespace YetaWF_PageEdit {

    export class PageControlModule extends YetaWF.ModuleBaseNoDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_PageEdit_PageControl";

        private readonly PageControlMod: HTMLDivElement;
        private readonly FadeTime: number = 250;

        constructor(id: string) {
            super(id, PageControlModule.SELECTOR, null);

            this.PageControlMod = $YetaWF.getElementById(YConfigs.YetaWF_PageEdit.PageControlMod) as HTMLDivElement;

            const pagebutton = $YetaWF.getElementById("tid_pagecontrolbutton");
            $YetaWF.registerEventHandler(pagebutton, "click", null, (ev: MouseEvent): boolean => {
                this.toggleControlPanel();
                return false;
            });

            // on page load, show control panel if wanted
            if (YVolatile.Basics.PageControlVisible) {
                this.PageControlMod.style.display = "block";
                ComponentsHTMLHelper.processPropertyListVisible(this.PageControlMod);
            }

            // handle Page Settings, Remove Current Page, W3C Validation - this is needed in case we're in a unified page set
            // in which case the original pageguid and url in the module actions have changed
            // when a new page becomes active, update the module actions reflecting the new page/url
            // also update all hidden fields with the new current page guid
            $YetaWF.addWhenReady((tag: HTMLElement): void => {

                if ($YetaWF.isInPopup()) {
                    if (YVolatile.Basics.PageControlVisible) {
                        YVolatile.Basics.PageControlVisible = false;
                        this.toggleControlPanel();
                    }
                    return;
                }

                const pagebutton = $YetaWF.getElementByIdCond("tid_pagecontrolbutton");
                if (pagebutton) {
                    if (YVolatile.Basics.TemporaryPage) {
                        if (YVolatile.Basics.PageControlVisible) {
                            YVolatile.Basics.PageControlVisible = false;
                            this.toggleControlPanel();
                        }
                        pagebutton.style.display = "none";
                    } else {
                        pagebutton.style.display = "block";
                    }
                }

                const ps = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='PageSettings']") as HTMLAnchorElement | null;
                if (ps) {
                    let uri = $YetaWF.parseUrl(ps.href);
                    uri.removeSearch("PageGuid");
                    uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                    ps.href = uri.toUrl();
                }
                // Export Page
                const ep = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='ExportPage']") as HTMLAnchorElement | null;
                if (ep) {
                    let uri = $YetaWF.parseUrl(ep.href);
                    uri.removeSearch("PageGuid");
                    uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                    ep.href = uri.toUrl();
                }
                // Remove Page
                const rp = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='RemovePage']") as HTMLAnchorElement | null;
                if (rp) {
                    let uri = $YetaWF.parseUrl(rp.href);
                    uri.removeSearch("PageGuid");
                    uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                    rp.href = uri.toUrl();
                }
                // W3C validation
                const w3c = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='W3CValidate']") as HTMLAnchorElement | null;
                if (w3c)
                    w3c.href = YConfigs.YetaWF_PageEdit.W3CUrl.format(window.location);

                const hidden = $YetaWF.getElementsBySelector(".YetaWF_PageEdit_PageControl input[name='CurrentPageGuid'][type='hidden']") as HTMLInputElement[];
                for (let h of hidden) {
                    h.value = YVolatile.Basics.PageGuid;
                }
            });
        }
        private toggleControlPanel() :void {
            if (!this.PageControlMod) return;
            if ($YetaWF.isVisible(this.PageControlMod)) {
                YVolatile.Basics.PageControlVisible = false;
                ComponentsHTMLHelper.fadeOut(this.PageControlMod, this.FadeTime);
            } else {
                YVolatile.Basics.PageControlVisible = true;
                ComponentsHTMLHelper.fadeIn(this.PageControlMod, this.FadeTime);
                ComponentsHTMLHelper.processPropertyListVisible(this.Module);
            }
        }
    }
}