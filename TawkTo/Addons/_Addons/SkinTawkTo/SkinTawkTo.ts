/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

// If this javascript snippet is included, that means we're displaying the chat.

// tslint:disable-next-line:variable-name
var Tawk_API: any;

namespace YetaWF_TawkTo {

    export interface IPackageConfigs {
        IncludedPagesCss: string;
        ExcludedPagesCss: string;
    }

    export class SkinTawkToModule extends YetaWF.ModuleBaseNoDataImpl {

        public static readonly SELECTOR: string = ".Softelvdm_Documentation_FileDocumentShow";
        public static readonly MODULEGUID: string = "c063e089-aff3-44e4-ac44-063911853579";

        private static on: boolean = true;

        constructor(id: string) {
            super(id, SkinTawkToModule.SELECTOR, null);

            $YetaWF.registerContentChange((addonGuid: string, on: boolean): void => {
                if (addonGuid === SkinTawkToModule.MODULEGUID) {
                    SkinTawkToModule.on = on;
                }
            });

            $YetaWF.registerNewPage(false, (url: string): void => {
                this.showInvite(SkinTawkToModule.on);
                if (SkinTawkToModule.on) {
                    // Functionality not available in Tawk.to to record a new page
                    //if (typeof ActivEngage !== "undefined" && ActivEngage !== undefined) {
                    //    if (typeof ActivEngage.recordPageView !== "undefined" && ActivEngage.recordPageView !== undefined)
                    //        ActivEngage.recordPageView({ "href": url });
                    //}
                }
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
    $YetaWF.registerCustomEventHandlerDocument("print_before", null, (ev: Event): boolean => {
        if (Tawk_API) {
            Tawk_API.hideWidget();
        }
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument("print_after", null, (ev: Event): boolean => {
        if (Tawk_API) {
            Tawk_API.showWidget();
        }
        return false;
    });
}
