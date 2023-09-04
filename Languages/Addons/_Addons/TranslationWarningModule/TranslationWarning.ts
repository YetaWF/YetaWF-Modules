/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

namespace YetaWF_Languages {

    export class TranslationWarningModule extends YetaWF.ModuleBaseNoDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_Languages_TranslationWarning";

        constructor(id: string) {
            super(id, TranslationWarningModule.SELECTOR, null);

            // Move to top of page
            document.body.insertBefore(this.Module, document.body.firstChild);

            TranslationWarningModule.propagateSize();
        }

        public static propagateSize(): void {
            const modules = $YetaWF.getElementsBySelector(TranslationWarningModule.SELECTOR);
            for (const module of modules) {
                const container = $YetaWF.getElement1BySelector(".t_container") as HTMLDivElement;
                const rect = container.getBoundingClientRect();
                module.style.height = `${rect.height}px`;
            }
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        if (ev.detail.container === document.body)
            TranslationWarningModule.propagateSize();
        return true;
    });
}

