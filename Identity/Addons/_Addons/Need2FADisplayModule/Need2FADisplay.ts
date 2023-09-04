/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

namespace YetaWF_Identity {

    export class Need2FADisplayModule extends YetaWF.ModuleBaseNoDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_Identity_Need2FADisplay";

        constructor(id: string) {
            super(id, Need2FADisplayModule.SELECTOR, null);

            // Move to top of page
            document.body.insertBefore(this.Module, document.body.firstChild);

            Need2FADisplayModule.propagateSize();
        }

        public static propagateSize(): void {
            const modules = $YetaWF.getElementsBySelector(Need2FADisplayModule.SELECTOR);
            for (const module of modules) {
                const container = $YetaWF.getElement1BySelector(".t_container") as HTMLDivElement;
                const rect = container.getBoundingClientRect();
                module.style.height = `${rect.height}px`;
            }
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        if (ev.detail.container === document.body)
            Need2FADisplayModule.propagateSize();
        return true;
    });
}

