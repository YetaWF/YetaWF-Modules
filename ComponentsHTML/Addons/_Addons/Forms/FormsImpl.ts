/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* Forms implementation required by YetaWF */

namespace YetaWF_ComponentsHTML {

    export class FormsImpl implements YetaWF.IFormsImpl {

        // Partialform Initialization

        /**
         * Initializes a partialform.
         */
        public initPartialForm(partialForm: HTMLElement): void {
            // find the first field in each tab control that has an input validation error and activate that tab
            // This will not work for nested tabs. Only the lowermost tab will be activated.
            var elems = $YetaWF.getElementsBySelector("div.yt_propertylist.t_tabbed", [partialForm]);
            elems.forEach((tabctrl: HTMLElement, index: number) => {
                YetaWF_FormsImpl.setErrorInTab(tabctrl);
            });
        }

        // Validation

        /**
         * Re-validate all fields within the div, typically used after paging in a grid to let jquery.validate update all fields
         */
        public updateValidation(div: HTMLElement): void {
            // apparently not used
            throw `updateValidation not implemented`;
        }
        /**
         * Clear any validation errors within the div
         */
        public clearValidation(div: HTMLElement): void {
            return YetaWF_ComponentsHTML_Validation.clearValidation(div);
        }
        /**
         * Validates one elements.
         */
        public validateElement(ctrl: HTMLElement): void {
            let form = $YetaWF.Forms.getFormCond(ctrl);
            if (!form) return;
            YetaWF_ComponentsHTML_Validation.validateField(form, ctrl, true);
        }

        /**
         * Returns whether a div has form errors.
         */
        public hasErrors(div: HTMLElement): boolean {
            let errImgs = $YetaWF.getElementsBySelector("span[data-v-for] img", [div]) as HTMLImageElement[];
            return errImgs.length > 0;
        }

        /**
         * Shows all div form errors in a popup.
         */
        public showErrors(div: HTMLElement): void {
            let errImgs = $YetaWF.getElementsBySelector("span[data-v-for] img", [div]) as HTMLImageElement[];
            var s: string = "";
            for (let errImg of errImgs) {
                s += $YetaWF.getAttribute(errImg, "data-tooltip") + "(+nl)";
            }
            $YetaWF.error(YLocs.Forms.FormErrors + s);
        }

        /**
         * Serializes the form and returns a name/value pairs array
         */
        public serializeFormArray(form: HTMLFormElement): YetaWF.NameValuePair[] {
            let array: YetaWF.NameValuePair[] = [];

            let elems = $YetaWF.getElementsBySelector("input,select,textarea", [form]);
            for (let elem of elems) {
                let name = $YetaWF.getAttributeCond(elem, "name");
                if (!name ||
                    $YetaWF.getAttributeCond(elem, "disabled") || // don't submit disabled fields
                    $YetaWF.getAttributeCond(elem, "readonly") || // don't submit readonly fields
                    $YetaWF.elementHasClass(elem, YConfigs.Forms.CssFormNoSubmit) || // don't submit nosubmit fields
                    $YetaWF.elementClosestCond(elem, `.${YConfigs.Forms.CssFormNoSubmitContents}`)) // don't submit input fields in containers (usually grids)
                    continue;

                array.push({
                    name: name,
                    value: YetaWF_ComponentsHTML_Validation.getFieldValue(elem)
                });
            }
            return array;
        }


        /**
         * Validate all fields in the current form.
         */
        public validate(form: HTMLFormElement): boolean {
            return YetaWF_ComponentsHTML_Validation.validateForm(form, true);
        }
        /**
         * Returns whether all fields in the current form are valid.
         */
        public isValid(form: HTMLFormElement): boolean {
            return YetaWF_ComponentsHTML_Validation.isFormValid(form);
        }
        /**
         * If there is a validation in the specified tab control, the tab is activated.
         */
        public setErrorInTab(tabctrl: HTMLElement): void {
            // get the first field in error (if any)
            var errField = $YetaWF.getElement1BySelectorCond(".v-valerror", [tabctrl]);
            if (errField) {
                // find out which tab panel we're on
                var ttabpanel = $YetaWF.elementClosest(errField, "div.t_tabpanel");
                var panel = ttabpanel.getAttribute("data-tab") as number | null;
                if (!panel) throw "We found a panel in a tab control without panel number (data-tab attribute).";/*DEBUG*/
                // get the tab entry

                // TODO: MOVE TO TAB CONTROL
                let $tabctrl = $(tabctrl);
                var $te = $("ul.t_tabstrip > li", $tabctrl).eq(panel);
                if ($te.length === 0) throw "We couldn't find the tab entry for panel " + panel;/*DEBUG*/
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery)
                    $tabctrl.tabs("option", "active", panel);
                else if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.Kendo)
                    $tabctrl.data("kendoTabStrip").activateTab($te);
                else throw "Unknown tab style";/*DEBUG*/
            }
        }

        // Forms initialization

        /**
         * Initialize the form when page/content is ready.
         * No external use.
         */
        public initForm(tag: HTMLElement): void {
            let forms = $YetaWF.getElementsBySelector("form", [tag]) as HTMLFormElement[];
            for (let form of forms) {
                if ($YetaWF.elementHasClass(form, "yValidateImmediately")) {
                    YetaWF_ComponentsHTML_Validation.validateForm(form, true);
                }
            }
        }
    }
}

// tslint:disable-next-line:variable-name
var YetaWF_FormsImpl: YetaWF.IFormsImpl = new YetaWF_ComponentsHTML.FormsImpl();

/* Page load */
$YetaWF.addWhenReady((YetaWF_FormsImpl as YetaWF_ComponentsHTML.FormsImpl).initForm);

