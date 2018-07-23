/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

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

    class PageControlModule {

        public init(): void {

            $YetaWF.addWhenReadyOnce((): void => {

                const fadeTime = 250;

                const mod = $YetaWF.getElementById(YConfigs.YetaWF_PageEdit.PageControlMod);

                // Page icon
                const pagebutton = $YetaWF.getElementById("yPageControlButton");
                $YetaWF.registerEventHandler(pagebutton, "click", null, (ev: MouseEvent): boolean => {
                    if ($YetaWF.isVisible(mod)) {
                        YVolatile.Basics.PageControlVisible = false;
                        ComponentsHTML.fadeOut(mod, fadeTime);
                    } else {
                        YVolatile.Basics.PageControlVisible = true;
                        ComponentsHTML.fadeIn(mod, fadeTime);
                    }
                    return false;
                });
                // on page load, show control panel if wanted
                if (YVolatile.Basics.PageControlVisible) {
                    mod.style.display = "block";
                    ComponentsHTML.processPropertyListVisible(mod);
                }
            });

            // handle Page Settings, Remove Current Page, W3C Validation - this is needed in case we're in a unified page set
            // in which case the original pageguid and url in the module actions have changed
            // when a new page becomes active, update the module actions reflecting the new page/url
            // also update all hidden fields with the new current page guid
            $YetaWF.addWhenReady((tag: HTMLElement) : void => {
                // disregards tag as this callback is used when a page or page content has been rendered
                // Page Settings

                const pagebutton = $YetaWF.getElementById("yPageControlButton");
                if (YVolatile.Basics.TemporaryPage) {
                    if (YVolatile.Basics.PageControlVisible) {
                        const mod = $YetaWF.getElementById(YConfigs.YetaWF_PageEdit.PageControlMod);
                        YVolatile.Basics.PageControlVisible = false;
                        mod.style.display = "none";
                    }
                    pagebutton.style.display = "none";
                } else {
                    pagebutton.style.display = "block";
                }

                if (!$YetaWF.isInPopup()) {
                    const ps = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='PageSettings']") as HTMLAnchorElement | null;
                    if (ps) {
                        var uri = $YetaWF.parseUrl(ps.href);
                        uri.removeSearch("PageGuid");
                        uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                        ps.href = uri.toUrl();
                    }
                    // Export Page
                    const ep = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='ExportPage']") as HTMLAnchorElement | null;
                    if (ep) {
                        var uri = $YetaWF.parseUrl(ep.href);
                        uri.removeSearch("PageGuid");
                        uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                        ep.href = uri.toUrl();
                    }
                    // Remove Page
                    const rp = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='RemovePage']") as HTMLAnchorElement | null;
                    if (rp) {
                        var uri = $YetaWF.parseUrl(rp.href);
                        uri.removeSearch("PageGuid");
                        uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                        rp.href = uri.toUrl();
                    }
                    // W3C validation
                    const w3c = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='W3CValidate']") as HTMLAnchorElement | null;
                    if (w3c)
                        w3c.href = YConfigs.YetaWF_PageEdit.W3CUrl.format(window.location);

                    const hidden = $YetaWF.getElementsBySelector(".YetaWF_PageEdit_PageControl input[name='CurrentPageGuid'][type='hidden']") as HTMLInputElement[];
                    for (var h of hidden) {
                        h.value = YVolatile.Basics.PageGuid;
                    }
                }
            });
        }
    }

    var pageEdit: PageControlModule = new PageControlModule();
    pageEdit.init();

}