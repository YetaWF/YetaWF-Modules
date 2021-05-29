/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
        jquery: boolean; // defines whether jquery has been loaded
    }
    export interface IPackageConfigs {

    }
}

namespace YetaWF_ComponentsHTML {

    export interface PropertyListVisibleEntry {
        callback(tag: HTMLElement): void;
    }
    export interface CancelableFadeInOut {
        Active: boolean;
        Canceled: boolean;
    }

    export class ComponentsHTML {

        // Loader
        // Loader
        // Loader

        public MUSTHAVE_JQUERY(): void {
            if (!YVolatile.YetaWF_ComponentsHTML.jquery)
                throw "jquery is required but has not been loaded";
        }

        public REQUIRES_JQUERY(run: () => void): void {

            if (!YVolatile.YetaWF_ComponentsHTML.jquery) {

                YVolatile.YetaWF_ComponentsHTML.jquery = true;

                $YetaWF.ContentHandling.loadAddons([
                    { AreaName: "YetaWF_ComponentsHTML", ShortName: "jquery", Argument1: null }
                ], (): void => {
                    run();
                });
            } else {
                run();
            }
        }

        // PropertyListVisible
        // PropertyListVisible
        // PropertyListVisible

        private PropertyListVisibleHandlers: PropertyListVisibleEntry[] = [];

        /**
         * Register a callback to be called when a propertylist becomes visible.
         */
        public registerPropertyListVisible(callback: (tag: HTMLElement) => void): void {
            this.PropertyListVisibleHandlers.push({ callback: callback });
        }
        /**
         * Called to call all registered callbacks when a propertylist becomes visible.
         */
        public processPropertyListVisible(tag: HTMLElement): void {
            for (const entry of this.PropertyListVisibleHandlers) {
                entry.callback(tag);
            }
        }

        // Fade in/out
        // Fade in/out
        // Fade in/out

        public cancelFadeInOut(cancelable: CancelableFadeInOut): void {
            cancelable.Canceled = true;
            cancelable.Active = false;
        }
        public isActiveFadeInOut(cancelable: CancelableFadeInOut): boolean {
            return cancelable.Active;
        }
        private clearFadeInOut(cancelable?: CancelableFadeInOut): void {
            if (cancelable) {
                cancelable.Canceled = false;
                cancelable.Active = false;
            }
        }

        public fadeIn(elem: HTMLElement, ms: number, cancelable?: CancelableFadeInOut): void {

            elem.style.opacity = "0";
            if (cancelable) {
                cancelable.Canceled = false;
                cancelable.Active = true;
            }

            if (ms) {
                var opacity = 0;
                elem.style.display = "block";
                this.processPropertyListVisible(elem);
                const timer = setInterval(() : void => {
                    if (cancelable && cancelable.Canceled) {
                        this.clearFadeInOut(cancelable);
                        return;
                    }
                    opacity += 20 / ms;
                    if (opacity >= 1) {
                        clearInterval(timer);
                        opacity = 1;
                        this.clearFadeInOut(cancelable);
                    }
                    elem.style.opacity = opacity.toString();
                }, 20);
            } else {
                elem.style.opacity = "1";
                this.clearFadeInOut(cancelable);
            }
        }

        public fadeOut(elem: HTMLElement, ms: number, done?: () => void, cancelable?: CancelableFadeInOut) : void {

            elem.style.opacity = "1";
            if (cancelable) {
                cancelable.Canceled = false;
                cancelable.Active = true;
            }

            if (ms) {
                var opacity = 1;
                const timer = setInterval(() : void => {
                    if (cancelable && cancelable.Canceled) {
                        this.clearFadeInOut(cancelable);
                        return;
                    }
                    opacity -= 20 / ms;
                    if (opacity <= 0) {
                        clearInterval(timer);
                        opacity = 0;
                        elem.style.display = "none";
                        this.clearFadeInOut(cancelable);
                        this.processPropertyListVisible(elem);
                        if (done)
                            done();
                    }
                    elem.style.opacity = opacity.toString();
                }, 20);
            } else {
                elem.style.opacity = "0";
                this.clearFadeInOut(cancelable);
                if (done)
                    done();
            }
        }
    }
}

var ComponentsHTMLHelper = new YetaWF_ComponentsHTML.ComponentsHTML();