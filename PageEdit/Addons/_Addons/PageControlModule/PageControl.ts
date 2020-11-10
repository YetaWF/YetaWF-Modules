/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

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

    export class PageControlModule extends YetaWF.ModuleBaseDataImpl {

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

        public updateControlPanel(): void {
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

            const ps = $YetaWF.getElement1BySelectorCond("a[data-name='PageSettings']", [this.Module]) as HTMLAnchorElement | null;
            if (ps) {
                let uri = $YetaWF.parseUrl(ps.href);
                uri.removeSearch("PageGuid");
                uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                ps.href = uri.toUrl();
            }
            // Export Page
            const ep = $YetaWF.getElement1BySelectorCond("a[data-name='ExportPage']", [this.Module]) as HTMLAnchorElement | null;
            if (ep) {
                let uri = $YetaWF.parseUrl(ep.href);
                uri.removeSearch("PageGuid");
                uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                ep.href = uri.toUrl();
            }
            // Remove Page
            const rp = $YetaWF.getElement1BySelectorCond("a[data-name='RemovePage']", [this.Module]) as HTMLAnchorElement | null;
            if (rp) {
                let uri = $YetaWF.parseUrl(rp.href);
                uri.removeSearch("PageGuid");
                uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                rp.href = uri.toUrl();
            }
            // W3C validation
            const w3c = $YetaWF.getElement1BySelectorCond("a[data-name='W3CValidate']", [this.Module]) as HTMLAnchorElement | null;
            if (w3c)
                w3c.href = YConfigs.YetaWF_PageEdit.W3CUrl.format(window.location);

            const hidden = $YetaWF.getElementsBySelector("input[name='CurrentPageGuid'][type='hidden']", [this.Module]) as HTMLInputElement[];
            for (let h of hidden) {
                h.value = YVolatile.Basics.PageGuid;
            }
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: Event): boolean => {
        let mods = YetaWF.ModuleBaseDataImpl.getModules<PageControlModule>(PageControlModule.SELECTOR);
        for (let mod of mods)
            mod.updateControlPanel();
        return true;
    });

}