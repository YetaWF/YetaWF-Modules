/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
            YetaWF_FormsImpl.setErrorInNestedControls(partialForm);
        }

        // Validation

        /**
         * Re-validate all fields within the div, typically used after paging in a grid to let validatation update all fields
         */
        public updateValidation(div: HTMLElement): void {
            // apparently not used
            throw "updateValidation not implemented";
        }
        /**
         * Clear any validation errors within the div
         */
        public clearValidation(div: HTMLElement): void {
            return YetaWF_ComponentsHTML_Validation.clearValidation(div);
        }
        /**
         * Clear any validation errors for one element.
         */
        public clearValidation1(elem: HTMLElement): void {
            let form = $YetaWF.Forms.getForm(elem);
            return YetaWF_ComponentsHTML_Validation.clearValidation1(elem, form);
        }
        /**
         * Validate one element.
         * If the contents are empty the field will be fully validated. If contents are present, the error indicator is reset.
         * Full validation takes place on blur (or using validateElementFully).
         */
        public validateElement(ctrl: HTMLElement, hasValue?: (value: any) => boolean): void {
            let form = $YetaWF.Forms.getFormCond(ctrl);
            if (!form) return;
            YetaWF_ComponentsHTML_Validation.validateField(form, ctrl, true, hasValue);
        }
        /**
         * Validate one element.
         * Full validation takes place.
         */
        public validateElementFully(ctrl: HTMLElement): void {
            let form = $YetaWF.Forms.getFormCond(ctrl);
            if (!form) return;
            YetaWF_ComponentsHTML_Validation.validateFieldFully(form, ctrl, true);
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
            // eliminate duplicate error messages
            let errs: string[] = [];
            for (let errImg of errImgs) {
                let err = $YetaWF.getAttribute(errImg, "data-tooltip");
                let e = errs.filter((entry: string): boolean => entry === err);
                if (!e.length)
                    errs.push(err);
            }
            // format message
            let s: string = "";
            for (let err of errs) {
                s += `${err}(+nl)`;
            }
            $YetaWF.error(YLocs.Forms.FormErrors + s);
        }

        /**
         * Serializes the form and returns an object
         */
        public serializeFormObject(form: HTMLFormElement): any {
            let obj = {};

            let elems = $YetaWF.getElementsBySelector("input,select,textarea", [form]) as (HTMLInputElement|HTMLSelectElement|HTMLTextAreaElement)[];
            for (let elem of elems) {
                let name = elem.name;
                if (!name ||
                    ($YetaWF.getAttributeCond(elem, "disabled") && !$YetaWF.elementHasClass(elem, "disabled-submit")) || // don't submit disabled fields
                    ($YetaWF.getAttributeCond(elem, "readonly") && !$YetaWF.elementHasClass(elem, "readonly-submit")) || // don't submit readonly fields
                    $YetaWF.elementHasClass(elem, YConfigs.Forms.CssFormNoSubmit) || // don't submit nosubmit fields
                    $YetaWF.elementClosestCond(elem, `.${YConfigs.Forms.CssFormNoSubmitContents}`)) // don't submit input fields in containers (usually grids)
                    continue;

                // translate HTML type arrays/objects into javascript objects
                // translate Prop[0].Key / Prop[0].Value into Prop: [] = [{ Key:.. Value:.. }, { Key:.. Value:.. }]
                // TODO: This is really only needed because we used to submit forms. Eventually we'll need to change this to
                // be easily collectable from each component in a form.
                const fieldValue = YetaWF_ComponentsHTML_Validation.getFieldValue(elem);
                this.parseField(obj, fieldValue, name);
            }
            return obj;
        }

        private parseField(obj: any, fieldValue: string|boolean, name: string): void {
            if (!name) return;
            const br = name.indexOf("[");
            const dot = name.indexOf(".");
            if (br > 0 && dot > 0) {
                if (br < dot) {
                    // Field[index].Value
                    this.parseFieldArray(obj, fieldValue, name, br);
                } else {
                    // Field.....
                    this.parseFieldObject(obj, fieldValue, name, dot);
                }
            } else if (br > 0) {
                // Field[index]
                this.parseFieldArray(obj, fieldValue, name, br);
            } else if (dot > 0) {
                // Field.....
                this.parseFieldObject(obj, fieldValue, name, dot);
            } else {
                obj[name] = fieldValue;
            }
        }
        private parseFieldArray(obj: any, fieldValue: string|boolean, name: string, br: number): void {
            const left = name.substring(0, br);
            let right = name.substring(br+1);
            const rbr = right.indexOf("]");
            if (rbr < 0) throw `Invalid field name ${name} - can't parse as array`;
            const index = right.substring(0, rbr);
            right = right.substring(rbr+1);
            if (right.startsWith("."))
                right = right.substring(1);
            if (!obj.hasOwnProperty(left))
                obj[left] = [];
            const ix = Number(index);
            while (obj[left].length <= ix)
                obj[left].push({});
            if (!right) {
                obj[left][ix] = fieldValue;
                return;
            } else {
                this.parseField(obj[left][ix], fieldValue, right);
            }
        }
        private parseFieldObject(obj: any, fieldValue: string|boolean, name: string, dot: number): void {
            const left = name.substring(0, dot);
            const right = name.substring(dot+1);
            if (!obj.hasOwnProperty(left))
                obj[left] = {};
            this.parseField(obj[left], fieldValue, right);
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
         * If there is a validation error in the specified tag, components can alter their state. For example, a tab control can activate the pane with the error.
         */
        public setErrorInNestedControls(tag: HTMLElement): void {

            // get the first field in error (if any)
            let errField = $YetaWF.getElement1BySelectorCond(".v-valerror", [tag]);
            if (errField) {
                if (YetaWF_ComponentsHTML.TabsComponent) {
                    let tabs = YetaWF.ComponentBaseDataImpl.getControlFromTagCond<YetaWF_ComponentsHTML.TabsComponent>(errField, TabsComponent.SELECTOR);
                    if (tabs)
                        tabs.activatePaneByTag(errField);
                }
            }
        }

        /**
         * Resequences array indexes in forms fields.
         * This is very much a work in progress and doesn't handle all controls.
         * All fields prefix[index].name are resequenced based on their position within the tags array.
         * This is typically used after adding/reordering entries.
         * @param rows Array of tags containing input fields to resequence.
         * @param prefix The name prefix used in input fields.
         */
        public resequenceFields(rows: HTMLElement[], prefix: string): void {

            let index = 0;
            let prefixEsc = prefix.replace("[", "\\[");
            let re1 = new RegExp(`${prefixEsc}\[[0-9]+\]`, "gim");
            for (let row of rows) {
                // input fields
                let fields = $YetaWF.getElementsBySelector(`[name^='${prefix}[']`, [row]);
                for (let field of fields) {
                    let name = $YetaWF.getAttribute(field, "name");
                    name = name.replace(re1, `${prefix}[${index.toString()}]`);
                    $YetaWF.setAttribute(field, "name", name);
                    let v = $YetaWF.getAttributeCond(field, "data-v");
                    if (v) {
                        v = v.replace(re1, `${prefix}[${index.toString()}]`);
                        $YetaWF.setAttribute(field, "data-v", v);
                    }
                }
                // validation fields
                fields = $YetaWF.getElementsBySelector(`[data-v-for^='${prefix}[']`, [row]);
                for (let field of fields) {
                    let name = $YetaWF.getAttribute(field, "data-v-for");
                    name = name.replace(re1, `${prefix}[${index.toString()}]`);
                    $YetaWF.setAttribute(field, "data-v-for", name);
                    let img = $YetaWF.getElement1BySelectorCond("img", [field]);
                    if (img) {
                        name = $YetaWF.getAttribute(img, "name");
                        name = name.replace(re1, `${prefix}[${index.toString()}]`);
                        $YetaWF.setAttribute(field, "name", name);
                    }
                }
                ++index;
            }
        }
    }

    /* Page load */

    /**
     * Initialize the form when page/content is ready.
     * No external use.
     */
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        let forms = $YetaWF.getElementsBySelector("form",  ev.detail.containers) as HTMLFormElement[];
        for (let form of forms) {
            if ($YetaWF.elementHasClass(form, "yValidateImmediately"))
                YetaWF_ComponentsHTML_Validation.validateForm(form, true);
        }
        return true;
    });
}

var YetaWF_FormsImpl: YetaWF.IFormsImpl = new YetaWF_ComponentsHTML.FormsImpl();
