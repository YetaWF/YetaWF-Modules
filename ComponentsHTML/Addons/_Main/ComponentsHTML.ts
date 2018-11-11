/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF {
    export interface IVolatile {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageVolatiles;
    }
    export interface IConfigs {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageConfigs;
    }
}
namespace YetaWF_ComponentsHTML {
    export interface IPackageVolatiles {

    }
    export interface IPackageConfigs {

    }
}


declare var YetaWF_TemplateDropDownList: any; // TODO: Temporary, use until we fix dropdown

namespace YetaWF_ComponentsHTML {

    export interface PropertyListVisibleEntry {
        callback(tag: HTMLElement): void;
    }

    export class ComponentsHTML {

        // PropertyListVisible
        // PropertyListVisible
        // PropertyListVisible

        private PropertyListVisibleHandlers: PropertyListVisibleEntry[] = [];

        /**
         * Register a callback to be called when a propertylist become visible.
         */
        public registerPropertyListVisible(callback: (tag: HTMLElement) => void): void {
            this.PropertyListVisibleHandlers.push({ callback: callback });
        }
        /**
         * Called to call all registered callbacks when a propertylist become visible.
         */
        public processPropertyListVisible(tag: HTMLElement): void {
            for (const entry of this.PropertyListVisibleHandlers) {
                entry.callback(tag);
            }
        }

        // Fade in/out
        // Fade in/out
        // Fade in/out

        public fadeIn(elem: HTMLElement, ms: number) : void {

            elem.style.opacity = "0";

            if (ms) {
                var opacity = 0;
                elem.style.display = "block";
                this.processPropertyListVisible(elem);
                const timer = setInterval(() => {
                    opacity += 50 / ms;
                    if (opacity >= 1) {
                        clearInterval(timer);
                        opacity = 1;
                    }
                    elem.style.opacity = opacity.toString();
                }, 50);
            } else {
                elem.style.opacity = "1";
            }
        }

        public fadeOut(elem: HTMLElement, ms: number) : void {

            elem.style.opacity = "1";

            if (ms) {
                var opacity = 1;
                const timer = setInterval(() => {
                    opacity -= 50 / ms;
                    if (opacity <= 0) {
                        clearInterval(timer);
                        opacity = 0;
                        elem.style.display = "none";
                        this.processPropertyListVisible(elem);
                    }
                    elem.style.opacity = opacity.toString();
                }, 50);
            } else {
                elem.style.opacity = "0";
            }
        }
    }
}

var ComponentsHTML = new YetaWF_ComponentsHTML.ComponentsHTML();