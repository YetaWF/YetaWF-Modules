/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF {
    export interface IVolatile {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageVolatiles;
    }
}
namespace YetaWF_ComponentsHTML {
    export interface IPackageVolatiles {

    }
}

interface Event {
    __YetaWF: boolean; // we add this to a jquery Event to signal that we already translated it to native
}

namespace YetaWF_ComponentsHTML {

    export class ComponentsHTML {

        /**
         * Translate a jqury event to a native event.
         * Native events are normally not needed if all events are handled within ComponentsHTML, but there are a few exceptions,
         * like submit/apply which are handled by the framework and must generate native events.
         * The same event cannot be handled using jquery also, as it will fire 2x because of the native translation.
         * If it is necessary to handle the event using jquery also, use ev.__YetaWF to determine whether it's a native event.
         */
        public jQueryToNativeEvent($elem: JQuery<HTMLElement>, eventName: string): void {
            $elem.on(eventName, (ev: JQuery.Event) => {
                if (!ev.originalEvent || !ev.originalEvent.__YetaWF) {
                    const nev = new Event(eventName, { bubbles: true, cancelable: true });
                    nev.__YetaWF = true;// to avaid handling it again
                    $elem.each((index: number, elem: HTMLElement) => {
                        elem.dispatchEvent(nev);
                    });
                }
            });
        }
    }
}

var ComponentsHTML = new YetaWF_ComponentsHTML.ComponentsHTML();