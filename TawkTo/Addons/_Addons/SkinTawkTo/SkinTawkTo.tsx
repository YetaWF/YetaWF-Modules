/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

// If this javascript snippet is included, that means we're displaying the chat.

var Tawk_API: any;

namespace YetaWF_TawkTo { // nonstandard namespace to avoid conflict with core YetaWF_Basics

    export interface IPackageConfigs {
        IncludedPagesCss: string;
        ExcludedPagesCss: string;
    }

    class SkinTawkToModule {

        static readonly MODULEGUID: string = "c063e089-aff3-44e4-ac44-063911853579";

        static on: boolean = true;

        /**
         * Initializes the module instance.
         */
        init(): void {

            var tawkto: SkinTawkToModule = this;

            YetaWF_Basics.registerContentChange(function (event: Event, addonGuid: string, on: boolean): void {
                if (addonGuid === SkinTawkToModule.MODULEGUID) {
                    SkinTawkToModule.on = on;
                }
            });

            YetaWF_Basics.registerNewPage(function (event: Event, url: string): void {
                tawkto.showInvite(SkinTawkToModule.on);
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
        showInvite(show: boolean): void {

            if (!Tawk_API || !Tawk_API.showWidget) return; // not yet initialized

            var body: HTMLElement = document.querySelector("body") as HTMLElement;
            if (!body) return;

            var invite: boolean = show;
            if (invite) {
                var inclCss = YConfigs.YetaWF_TawkTo.IncludedPagesCss;
                var exclCss = YConfigs.YetaWF_TawkTo.ExcludedPagesCss;
                if (inclCss && inclCss.length > 0) {
                    // only included css pages show the chat invite
                    invite = false;
                    if (inclCss) {
                        var csses: string[] = inclCss.split(" ");
                        for (var css of csses) {
                            if (YetaWF_Basics.elementHasClass(body, css)) {
                                invite = true;
                                break;
                            }
                        }
                    }
                }
                // now check if page is explicitly excluded
                if (exclCss && invite) {
                    csses = exclCss.split(" ");
                    for (css of csses) {
                        if (YetaWF_Basics.elementHasClass(body, css)) {
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

    var tawkto: SkinTawkToModule = new SkinTawkToModule();
    tawkto.init();
}
