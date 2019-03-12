/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
        jqueryUI: boolean; // defines whether jqueryui has been loaded
        jqueryUITheme: string; // the theme in use
        kendoUI: boolean; // defines whether kendoui has been loaded
        kendoUITheme: string; // the theme in use
    }
    export interface IPackageConfigs {

    }
}

namespace YetaWF_ComponentsHTML {

    export interface PropertyListVisibleEntry {
        callback(tag: HTMLElement): void;
    }

    export class ComponentsHTML {

        // Loader
        // Loader
        // Loader

        public MUSTHAVE_JQUERYUI() : void {
            if (!YVolatile.YetaWF_ComponentsHTML.jqueryUI)
                throw `jquery-ui is required but has not been loaded`;
        }

        public REQUIRES_JQUERYUI(run:() => void): void {

            if (!YVolatile.YetaWF_ComponentsHTML.jqueryUI) {

                // tslint:disable-next-line:no-debugger
                debugger;

                YVolatile.YetaWF_ComponentsHTML.jqueryUI = true;

                $YetaWF.ContentHandling.loadAddons([
                    { AreaName: "YetaWF_ComponentsHTML", ShortName: "jqueryui", Argument1: null },
                    { AreaName: "YetaWF_ComponentsHTML", ShortName: "jqueryui-themes", Argument1: YVolatile.YetaWF_ComponentsHTML.jqueryUITheme }
                ], () => {
                    console.log("Done");
                    run();
                });
            } else {
                run();
            }
        }

        public MUSTHAVE_KENDOUI(): void {
            if (!YVolatile.YetaWF_ComponentsHTML.kendoUI)
                throw `Kendo UI is required but has not been loaded`;
        }

        public REQUIRES_KENDOUI(run: () => void): void {

            if (!YVolatile.YetaWF_ComponentsHTML.kendoUI) {

                // tslint:disable-next-line:no-debugger
                debugger;

                YVolatile.YetaWF_ComponentsHTML.kendoUI = true;

                $YetaWF.ContentHandling.loadAddons([
                    { AreaName: "YetaWF_ComponentsHTML", ShortName: "telerik.com.Kendo_UI_Core", Argument1: YVolatile.YetaWF_ComponentsHTML.kendoUITheme }
                ], () => {
                    console.log("Done");
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

var ComponentsHTMLHelper = new YetaWF_ComponentsHTML.ComponentsHTML();