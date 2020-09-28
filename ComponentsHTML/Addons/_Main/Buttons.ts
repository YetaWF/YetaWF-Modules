/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* initialize buttons (bootstrap and/or jquery-ui) */

namespace YetaWF_ComponentsHTML {

    export class Buttons {

        public static init(tag: HTMLElement): void {
            if (YVolatile.Skin.Bootstrap && YVolatile.Skin.BootstrapButtons) {
                // bootstrap
                let buttons = $YetaWF.getElementsBySelector(`input[type=submit],input[type=button],input[type=reset],input[type=file],a[${YConfigs.Basics.CssAttrActionButton}]`, [tag]);
                for (let button of buttons) {
                    if (!$YetaWF.elementHasClass(button, "y_jqueryui") && !$YetaWF.elementHasClass(button, "btn"))
                        $YetaWF.elementAddClasses(button, ["btn", "btn-primary"]);
                }
                buttons = $YetaWF.getElementsBySelector("button", [tag]);
                for (let button of buttons) {
                    if (!$YetaWF.elementHasClass(button, "y_jqueryui") && !$YetaWF.elementHasClass(button, "btn") && !$YetaWF.elementHasClass(button, "yt_actionicons"))
                        $YetaWF.elementAddClasses(button, ["btn"]);
                }
                // explicitly marked for jquery
                buttons = $YetaWF.getElementsBySelector(`input[type=submit].y_jqueryui,input[type=button].y_jqueryui,input[type=reset].y_jqueryui,input[type=file].y_jqueryui,button.y_jqueryui,a[${YConfigs.Basics.CssAttrActionButton}].y_jqueryui`, [tag]);
                for (let button of buttons) {
                    $YetaWF.elementAddClasses(button, ["ui-button", "ui-corner-all", "ui-widget"]);
                    $YetaWF.setAttribute(button, "role", "button");
                }
            } else {
                let buttons = $YetaWF.getElementsBySelector(`input[type=submit],input[type=button],input[type=reset],input[type=file],button,a[${YConfigs.Basics.CssAttrActionButton}]`, [tag]);
                for (let button of buttons) {
                    if (!$YetaWF.elementHasClass(button, "y_bootstrap")) {
                        $YetaWF.elementAddClasses(button, ["ui-button", "ui-corner-all", "ui-widget"]);
                        $YetaWF.setAttribute(button, "role", "button");
                    }
                }
                // explicitly marked for bootstrap
                buttons = $YetaWF.getElementsBySelector(`input[type=submit].y_bootstrap,input[type=button].y_bootstrap,input[type=reset].y_bootstrap,input[type=file].y_bootstrap,button.y_bootstrap,a[${YConfigs.Basics.CssAttrActionButton}].y_bootstrap`, [tag]);
                for (let button of buttons) {
                    $YetaWF.elementAddClasses(button, ["btn", "btn-primary"]);
                }
            }
        }
    }
}

$YetaWF.addWhenReady(YetaWF_ComponentsHTML.Buttons.init);

