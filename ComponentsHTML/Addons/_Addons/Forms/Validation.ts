/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */

namespace YetaWF_ComponentsHTML {

    export interface ValidationBase {
        M: string;
        Method: string;
    }
    export interface Validator {
        Name: string;
        Func: (form: HTMLFormElement, elem: HTMLElement, val: ValidationBase) => boolean;
    }

    interface ValidationRequiredExpr {
        M: string;
        Method: string;
        Op: OpEnum;
        Expr: string;
    }
    interface ValidationSameAs {
        M: string;
        Method: string;
        CondProp: string;
    }
    interface ValidationStringLength {
        M: string;
        Method: string;
        Min: any;
        Max: any;
    }
    interface ValidationRegexValidationBase {
        M: string;
        Method: string;
        Pattern: string;
    }
    interface ValidationRange {
        M: string;
        Method: string;
        Min: any;
        Max: any;
    }


    export class Validation {

        private static readonly DATAATTR: string = "data-v";

        constructor() {

            // Validators

            this.registerValidator("expr", (form: HTMLFormElement, elem: HTMLElement, val: ValidationBase): boolean => {
                return this.requiredExprValidator(form, elem, val as ValidationRequiredExpr);
            });
            this.registerValidator("sameas", (form: HTMLFormElement, elem: HTMLElement, val: ValidationBase): boolean => {
                return this.sameAsValidator(form, elem, val as ValidationSameAs);
            });
            this.registerValidator("listnoduplicates", (form: HTMLFormElement, elem: HTMLElement, val: ValidationBase): boolean => {
                return this.listNoDuplicatesValidator(form, elem, val as ValidationSameAs);
            });
            this.registerValidator("stringlength", (form: HTMLFormElement, elem: HTMLElement, val: ValidationBase): boolean => {
                return this.stringLengthValidator(form, elem, val as ValidationStringLength);
            });
            this.registerValidator("range", (form: HTMLFormElement, elem: HTMLElement, val: ValidationBase): boolean => {
                return this.rangeValidator(form, elem, val as ValidationRange);
            });
            this.registerValidator("regexvalidationbase", (form: HTMLFormElement, elem: HTMLElement, val: ValidationBase): boolean => {
                return this.regexValidationBaseValidator(form, elem, val as ValidationRegexValidationBase);
            });

            // focus handler
            $YetaWF.registerEventHandlerBody("focusout", "input[data-v],select[data-v],textarea[data-v]", (ev: FocusEvent): boolean => {
                let elem = ev.__YetaWFElem;
                let form = $YetaWF.Forms.getForm(elem);
                this.validateFieldFully(form, elem, true);
                return true;
            });
            $YetaWF.registerMultipleEventHandlers([document.body], ["input", "change", "paste"], "input[data-v],select[data-v],textarea[data-v]", (ev: Event): boolean => {
                let elem = ev.__YetaWFElem;
                let form = $YetaWF.Forms.getForm(elem);
                this.validateField(form, elem, true);
                return true;
            });
        }

        public validateForm(form: HTMLFormElement, setMessage?: boolean): boolean {
            let valid = true;
            this.clearValidation(form);
            let ctrls = $YetaWF.getElementsBySelector(`[${Validation.DATAATTR}]`, [form]);
            for (let ctrl of ctrls) {
                if (!this.validateFieldFully(form, ctrl, setMessage)) {
                    valid = false;
                    if (!setMessage)
                        break;
                }
            }
            return valid;
        }
        /**
         * Validate one element.
         * If the contents are empty the field will be fully validated. If contents are present, the error indicator is reset.
         * Full validation takes place on blur (or using validateElementFully).
         */
        public validateField(form: HTMLFormElement, elem: HTMLElement, setMessage?: boolean, hasValue?: (value: any) => boolean): boolean {
            let value = this.getFieldValueSimple(elem);
            let testFully: boolean;
            if (hasValue) {
                testFully = !hasValue(value);// only fully test if there is no value (for required, mostly)
            } else {
                testFully = !value;
            }
            if (testFully) {
                return this.validateFieldFully(form, elem, setMessage);
            } else {
                this.clearValidation1(elem, form);
                return true;
            }
        }
        /**
         * Validate one element.
         * Full validation takes place.
         */
        public validateFieldFully(form: HTMLFormElement, elem: HTMLElement, setMessage?: boolean): boolean {
            if ($YetaWF.getAttributeCond(elem, "disabled") || // don't validate disabled fields
                $YetaWF.getAttributeCond(elem, "readonly") || // don't validate readonly fields
                $YetaWF.elementHasClass(elem, ".yform-novalidate") /* || // don't validate novalidate fields
                $YetaWF.elementClosestCond(elem, `.${YConfigs.Forms.CssFormNoSubmitContents}`) */) {// don't validate input fields in containers (usually grids)
                return true;
            }
            let data = $YetaWF.getAttributeCond(elem, Validation.DATAATTR);
            if (!data)
                return true;
            let vals = JSON.parse(data) as ValidationBase[];
            let valid = true;
            for (let val of vals) {
                valid = this.evaluate(form, elem, val);
                if (setMessage) {
                    let name = $YetaWF.getAttribute(elem, "name");
                    let msgElem = $YetaWF.getElement1BySelectorCond(`span[data-v-for="${name}"]`, [form]);
                    $YetaWF.elementRemoveClasses(elem, ["v-valerror"]);
                    if (!valid)
                        $YetaWF.elementAddClass(elem, "v-valerror");
                    if (msgElem) {
                        $YetaWF.elementRemoveClasses(msgElem, ["v-error", "v-valid"]);
                        if (!valid) {
                            msgElem.innerHTML = `<img src="${$YetaWF.htmlAttrEscape(YConfigs.Forms.CssWarningIconUrl)}" name=${name} class="${YConfigs.Forms.CssWarningIcon}" ${YConfigs.Basics.CssTooltip}="${$YetaWF.htmlAttrEscape(val.M)}"/>`;
                            $YetaWF.elementAddClass(msgElem, "v-error");
                        } else {
                            msgElem.innerText = "";
                            $YetaWF.elementAddClass(msgElem, "v-valid");
                        }
                    }
                }
                if (!valid)
                    break;
            }
            return valid;
        }

        public isFormValid(form: HTMLFormElement): boolean {
            return this.validateForm(form);
        }
        public isFieldValid(form: HTMLFormElement, elem: HTMLElement): boolean {
            return this.validateFieldFully(form, elem);
        }

        private evaluate(form: HTMLFormElement, elem: HTMLElement, val: ValidationBase): boolean {
            let validators = this.Validators.filter((entry: Validator): boolean => entry.Name === val.Method);
            if (validators.length === 0)
                throw `No validator found for ${val.Method}`;
            else if (validators.length > 1)
                throw `Too many validators found for ${val.Method}`;
            return validators[0].Func(form, elem, val);
        }

        public getFieldValue(elem: HTMLElement): boolean | string {
            if (elem.tagName === "INPUT") {
                if (elem.getAttribute("type") === "checkbox")
                    return (elem as HTMLInputElement).checked;
                return (elem as HTMLInputElement).value;
            } else if (elem.tagName === "TEXTAREA") {
                return (elem as HTMLTextAreaElement).value;
            } else if (elem.tagName === "SELECT") {
                return (elem as HTMLSelectElement).value;
            }
            throw `Add support for ${elem.tagName}`;
        }
        public getFieldValueSimple(elem: HTMLElement): boolean | string {
            if (elem.tagName === "INPUT") {
                if (elem.getAttribute("type") === "checkbox")
                    return (elem as HTMLInputElement).checked;
                return (elem as HTMLInputElement).value;
            } else if (elem.tagName === "TEXTAREA") {
                return (elem as HTMLTextAreaElement).value;
            } else if (elem.tagName === "SELECT") {
                let value = (elem as HTMLSelectElement).value;
                if (value && value === "0") // special case for select==0 which means no selection
                    return "";
                return value;
            }
            throw `Add support for ${elem.tagName}`;
        }

        /**
         * Clear any validation errors within the div
         */
        public clearValidation(div: HTMLElement): void {
            let elems = $YetaWF.getElementsBySelector(`[${Validation.DATAATTR}]`, [div]);
            for (let elem of elems) {
                this.clearValidation1(elem, div);
            }
        }
        /**
         * Clear validation error for one element
         */
        public clearValidation1(elem: HTMLElement, div: HTMLElement): void {
            let name = $YetaWF.getAttribute(elem, "name");
            $YetaWF.elementRemoveClasses(elem, ["v-valerror"]);
            let msgElem = $YetaWF.getElement1BySelectorCond(`span[data-v-for="${name}"]`, [div]);
            if (msgElem) {
                $YetaWF.elementRemoveClasses(msgElem, ["v-error", "v-valid"]);
                $YetaWF.elementAddClass(msgElem, "v-valid");
                msgElem.innerText = "";
            }
        }

        // Registration
        // Registration
        // Registration

        public registerValidator(name: string, validator: (form: HTMLFormElement, elem: HTMLElement, val: ValidationBase) => boolean): void {
            let v = this.Validators.filter((entry: Validator): boolean => entry.Name === name);
            if (!v.length)
                this.Validators.push({ Name: name, Func: validator});
        }

        private Validators: Validator[] = [];

        // Default Validators
        // Default Validators
        // Default Validators

        private requiredExprValidator(form: HTMLFormElement, elem: HTMLElement, val: ValidationRequiredExpr): boolean  {
            let value = this.getFieldValue(elem);
            let exprs: YetaWF_ComponentsHTML.Expr[] = JSON.parse(val.Expr);
            switch (val.Op) {
                case YetaWF_ComponentsHTML.OpEnum.Required:
                case YetaWF_ComponentsHTML.OpEnum.RequiredIf:
                case YetaWF_ComponentsHTML.OpEnum.RequiredIfSupplied: {
                    let valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, val.Op, exprs);
                    if (!valid)
                        return true;
                    if (typeof value === "boolean")
                        return value;
                    if (value === undefined || value === null || value.trim().length === 0)
                        return false;
                    return true;
                }
                case YetaWF_ComponentsHTML.OpEnum.SelectionRequired:
                case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIf:
                case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfSupplied: {
                    let valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, val.Op, exprs);
                    if (!valid)
                        return true;
                    if (typeof value === "boolean")
                        return value;
                    if (value === undefined || value === null || value.trim().length === 0 || value.toString() === "0")
                        return false;
                    return true;
                }
                case YetaWF_ComponentsHTML.OpEnum.RequiredIfNot:
                case YetaWF_ComponentsHTML.OpEnum.RequiredIfNotSupplied: {
                    let valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, val.Op, exprs);
                    if (valid)
                        return true;
                    if (typeof value === "boolean")
                        return value;
                    if (value === undefined || value === null || value.trim().length === 0)
                        return false;
                    return true;
                }
                case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfNot:
                case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfNotSupplied: {
                    let valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, val.Op, exprs);
                    if (valid)
                        return true;
                    if (typeof value === "boolean")
                        return value;
                    if (value === undefined || value === null || value.trim().length === 0 || value.toString() === "0")
                        return false;
                    return true;
                }
                default:
                    throw `Invalid Op ${val.Op} in evaluateExpressionList`;
            }
        }
        private sameAsValidator(form: HTMLFormElement, elem: HTMLElement, val: ValidationSameAs): boolean {
            let value = this.getFieldValue(elem);
            if (!value) return true;

            let item = ControlsHelper.getControlItemByName(val.CondProp, form);
            let actualValue = ControlsHelper.getControlValue(item);

            if (typeof value === "boolean") {
                if (actualValue == null) {
                    return !value;
                } else {
                    return value === (actualValue.toLowerCase() === "true");
                }
            } else
                return value === actualValue;
        }
        private listNoDuplicatesValidator(form: HTMLFormElement, elem: HTMLElement, val: ValidationSameAs): boolean {
            // this is not currently needed - server-side validation verifies during add of new records
            //return false;// duplicate found
            return true;
        }
        private stringLengthValidator(form: HTMLFormElement, elem: HTMLElement, val: ValidationStringLength): boolean {
            let value = this.getFieldValue(elem);
            if (typeof value === "boolean")
                throw `StringLength attribute used with boolean type - ${elem.outerHTML}`;
            if (!value) return true;
            let len = value.length;
            return len <= val.Max && len >= val.Min;
        }
        private regexValidationBaseValidator(form: HTMLFormElement, elem: HTMLElement, val: ValidationRegexValidationBase): boolean {
            let value = this.getFieldValue(elem) as string;
            if (typeof value === "boolean")
                throw `Regex attribute used with boolean type - ${elem.outerHTML}`;
            if (!value) return true;
            var re = new RegExp(val.Pattern);
            return re.test(value);
        }
        private rangeValidator(form: HTMLFormElement, elem: HTMLElement, val: ValidationRange): boolean {
            let value = this.getFieldValue(elem);
            if (typeof value === "boolean")
                throw `Range attribute used with boolean type - ${elem.outerHTML}`;
            if (!value) return true;
            return value >= val.Min && value <= val.Max;
        }
    }
}

// tslint:disable-next-line:variable-name
var YetaWF_ComponentsHTML_Validation: YetaWF_ComponentsHTML.Validation = new YetaWF_ComponentsHTML.Validation();

