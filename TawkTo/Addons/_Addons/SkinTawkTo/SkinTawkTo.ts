/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

// If this javascript snippet is included, that means we're displaying the chat.

var Tawk_API: any;

namespace YetaWF_TawkTo {

    export interface IPackageConfigs {
        IncludedPagesCss: string;
        ExcludedPagesCss: string;
    }

    export class SkinTawkToModule extends YetaWF.ModuleBaseNoDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_TawkTo_FileDocumentShow";
        public static readonly MODULEGUID: string = "c063e089-aff3-44e4-ac44-063911853579";

        public static on: boolean = true;

        constructor(id: string) {
            super(id, SkinTawkToModule.SELECTOR, null);

            $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
                this.showInvite(SkinTawkToModule.on);
                if (SkinTawkToModule.on) {
                    // Functionality not available in Tawk.to to record a new page
                    //if (typeof ActivEngage !== "undefined" && ActivEngage !== undefined) {
                    //    if (typeof ActivEngage.recordPageView !== "undefined" && ActivEngage.recordPageView !== undefined)
                    //        ActivEngage.recordPageView({ "href": url });
                    //}
                }
                return true;
            });
        }
        /**
         * Show/hide chat invite
         * @param True to show, false to hide.
         */
        private showInvite(show: boolean): void {

            if (!Tawk_API || !Tawk_API.showWidget) return; // not yet initialized

            let body: HTMLElement = document.querySelector("body") as HTMLElement;
            if (!body) return;

            let invite: boolean = show;
            if (invite) {
                let inclCss = YConfigs.YetaWF_TawkTo.IncludedPagesCss;
                let exclCss = YConfigs.YetaWF_TawkTo.ExcludedPagesCss;
                if (inclCss && inclCss.length > 0) {
                    // only included css pages show the chat invite
                    invite = false;
                    if (inclCss) {
                        let csses: string[] = inclCss.split(" ");
                        for (let css of csses) {
                            if ($YetaWF.elementHasClass(body, css)) {
                                invite = true;
                                break;
                            }
                        }
                    }
                }
                // now check if page is explicitly excluded
                if (exclCss && invite) {
                    let csses = exclCss.split(" ");
                    for (let css of csses) {
                        if ($YetaWF.elementHasClass(body, css)) {
                            invite = false;
                            break;
                        }
                    }
                }
            }
            if (!invite && show) {
                if (Tawk_API.isVisitorEngaged())
                    invite = true;
            }
            if (invite) {
                Tawk_API.showWidget();
            } else {
                Tawk_API.hideWidget();
            }
        }
    }
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, (ev: CustomEvent<YetaWF.DetailsAddonChanged>): boolean => {
        let addonGuid = ev.detail.addonGuid;
        let on = ev.detail.on;
        if (addonGuid === SkinTawkToModule.MODULEGUID) {
            SkinTawkToModule.on = on;
        }
        return true;
    });

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTBEFOREPRINT, null, (ev: Event): boolean => {
        if (Tawk_API) {
            Tawk_API.hideWidget();
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTAFTERPRINT, null, (ev: Event): boolean => {
        if (Tawk_API) {
            Tawk_API.showWidget();
        }
        return true;
    });
}
