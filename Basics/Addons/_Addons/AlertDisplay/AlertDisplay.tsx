/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

// If this javascript snippet is included, that means we're displaying the alert.
// The alert is displayed until dismissed or if the page doesn't reference this module (dynamic content).

namespace YetaWF_Basics {

    class AlertDisplayModule {

        static readonly MODULEGUID: string = "24b7dc07-e96a-409d-911f-47bffd38d0fc";

        static dismissed: boolean = false;
        static on: boolean = true;

        constructor() {
            $YetaWF.registerEventHandlerBody("click", ".YetaWF_Basics_AlertDisplay .t_close a", (ev: MouseEvent): boolean =>{
                AlertDisplayModule.dismissed = true;

                let alert = $YetaWF.getElement1BySelector(".YetaWF_Basics_AlertDisplay");
                let close = $YetaWF.getElement1BySelector(".t_close", [alert]);
                let ajaxurl = $YetaWF.getAttribute(close, "data-ajaxurl");

                alert.style.display = "none";

                // Save alert status server side
                let request: XMLHttpRequest = new XMLHttpRequest();
                request.open("POST", ajaxurl, true);
                request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.send("");
                // we don't care about the result of this request
                return false;
            });
        }

        public initAlert(): void {
            let alert = $YetaWF.getElement1BySelector(".YetaWF_Basics_AlertDisplay");
            if (!AlertDisplayModule.dismissed && AlertDisplayModule.on)
                alert.style.display = "";
            else
                alert.style.display = "none";
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        AlertDisplay.initAlert();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, (ev: CustomEvent<YetaWF.DetailsAddonChanged>): boolean => {
        let addonGuid = ev.detail.addonGuid;
        let on = ev.detail.on;
        if (addonGuid === AlertDisplayModule.MODULEGUID) {
            AlertDisplayModule.on = on;
        }
        return true;
    });

    let AlertDisplay: AlertDisplayModule = new AlertDisplayModule();
}
