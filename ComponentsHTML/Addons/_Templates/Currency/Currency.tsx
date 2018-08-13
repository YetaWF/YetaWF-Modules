/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageVolatiles {
        CurrencyFormat: string;
    }

    export class CurrencyComponent {
        /**
         * Initializes all currency fields in the specified tag.
         * @param tag - an element containing Currency template controls.
         */
        initSection(tag: HTMLElement): void {
            var list: HTMLElement[] = $YetaWF.getElementsBySelector("input.yt_currency.t_edit", [tag]);
            for (let el of list) {
                var d = el.getAttribute("data-min");
                var sd: number = d ? Number(d) : 0;
                d = el.getAttribute("data-max");
                var ed: number = d ? Number(d) : 99999999.99;
                $(el).kendoNumericTextBox({
                    format: YVolatile.YetaWF_ComponentsHTML.CurrencyFormat,
                    min: sd, max: ed,
                    culture: YVolatile.Basics.Language
                });
            }
        }
    }
    // initializes new currency elements on demand
    $YetaWF.addWhenReady((section: HTMLElement): void => {
        new CurrencyComponent().initSection(section);
    });
    // A <div> is being emptied. Destroy all kendoNumericTextBox the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        var list: HTMLElement[] = $YetaWF.getElementsBySelector("input.yt_currency.t_edit", [tag]);
        for (let el of list) {
            var numTextBox: kendo.ui.NumericTextBox = $(el).data("kendoNumericTextBox");
            if (numTextBox) numTextBox.destroy();
        }
    });
}

