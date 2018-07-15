/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class DecimalComponent {
        /**
         * Initializes all decimal fields in the specified tag.
         * @param tag - an element containing Decimal template controls.
         */
        initSection(tag: HTMLElement):void {
            var list: HTMLElement[] = $YetaWF.getElementsBySelector("input.yt_decimal.t_edit", [tag]);
            for (let el of list) {
                var d = el.getAttribute("data-min");
                var sd: number = d ? Number(d) : 0.0;
                d = el.getAttribute("data-max");
                var ed: number = d ? Number(d) : 99999999.99;
                $(el).kendoNumericTextBox({
                    format: "0.00",
                    min: sd, max: ed,
                    culture: YVolatile.Basics.Language
                });
            }
        }
    }
    // initializes new decimal elements on demand
    $YetaWF.addWhenReady(function (section: HTMLElement): void {
        new DecimalComponent().initSection(section);
    });
    // A <div> is being emptied. Destroy all kendoNumericTextBox the <div> may contain.
    $YetaWF.addClearDiv(function (tag: HTMLElement): void {
        var list: HTMLElement[] = $YetaWF.getElementsBySelector("input.yt_decimal.t_edit", [tag]);
        for (let el of list) {
            var numTextBox: kendo.ui.NumericTextBox = $(el).data("kendoNumericTextBox");
            if (numTextBox) numTextBox.destroy();
        }
    });
}

