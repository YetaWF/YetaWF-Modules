/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */

// If this javascript snippet is included, that means we're displaying the alert.
// The alert is displayed until dismissed or if the page doesn't reference this module (dynamic content).

namespace YetaWF_Basics_Mods { // nonstandard namespace to avoid conflict with core YetaWF_Basics

    class AlertDisplayModule {

        static readonly MODULEGUID: string = "24b7dc07-e96a-409d-911f-47bffd38d0fc";

        static dismissed: boolean = false;
        static on: boolean = true;

        /**
         * Initializes the module instance.
         */
        init(): void {
            document.addEventListener("click", this.handleClick);

            YetaWF_Basics.addWhenReady((section: HTMLElement): void => {
                alert.initSection(section);
            });

            YetaWF_Basics.registerContentChange((addonGuid:string, on:boolean):void => {
                if (addonGuid === AlertDisplayModule.MODULEGUID) {
                    AlertDisplayModule.on = on;
                }
            });
        }

        /**
         * Initializes all alert elements in the specified tag.
         * @param tag - an element containing Alert elements.
         */
        initSection(tag: HTMLElement): void {
            var alert: HTMLElement = document.querySelector(".YetaWF_Basics_AlertDisplay") as HTMLElement;
            if (!alert) throw ".YetaWF_Basics_AlertDisplay not found";/*DEBUG*/
            if (!AlertDisplayModule.dismissed && AlertDisplayModule.on)
                alert.style.display = "";
            else
                alert.style.display = "none";
        }
        /**
         * Handles the click on the image to close the Alert.
         * @param event
         */
        private handleClick(event: MouseEvent): void {
            if (!YetaWF_Basics.elementMatches(event.srcElement, ".YetaWF_Basics_AlertDisplay .t_close img")) return;

            AlertDisplayModule.dismissed = true;

            var alert: HTMLElement = document.querySelector(".YetaWF_Basics_AlertDisplay") as HTMLElement;
            if (!alert) throw ".YetaWF_Basics_AlertDisplay not found";/*DEBUG*/

            var close: Element | null = alert.querySelector(".t_close");
            if (!close) throw "No .t_close element found";/*DEBUG*/
            var ajaxurl: string | null = close.getAttribute("data-ajaxurl");
            if (!ajaxurl) throw "No ajax url specified";/*DEBUG*/

            alert.style.display = "none";

            // Save alert status server side
            var request: XMLHttpRequest = new XMLHttpRequest();
            request.open("POST", ajaxurl, true);
            request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.send("");
            // we don't care about the result of this request
        }
    }

    var alert: AlertDisplayModule = new AlertDisplayModule();
    alert.init();
}
