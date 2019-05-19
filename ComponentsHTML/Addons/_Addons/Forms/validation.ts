/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

// validation for all forms
// does not implement any required externally callable functions
// http://jqueryvalidation.org/reference/

// Form submit handling for all forms
// http://jqueryvalidation.org/documentation/
// http://bradwilson.typepad.com/blog/2010/10/mvc3-unobtrusive-validation.html

// Make sure all hidden fields are NOT ignored
$.validator.setDefaults({
    ignore: "." + YConfigs.Forms.CssFormNoValidate, // don't ignore hidden fields - ignore fields with no validate class
    onsubmit: false         // don't validate on submit, we want to see the submit event and validate things ourselves
});
($.validator.unobtrusive as any).options = {
    errorElement: "label"
};

// REQUIREDEXPR
// REQUIREDEXPR
// REQUIREDEXPR

$.validator.addMethod("requiredexpr", function (this: Function, value: any, element: HTMLElement, parameters: any): boolean {
    let form = $YetaWF.Forms.getForm(element);
    let valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, parameters.op, parameters.exprList);
    switch (parameters.op) {
        case YetaWF_ComponentsHTML.OpEnum.Required:
        case YetaWF_ComponentsHTML.OpEnum.RequiredIf:
        case YetaWF_ComponentsHTML.OpEnum.RequiredIfSupplied:
            if (!valid)
                return true;
            if (value === undefined || value === null || value.trim().length === 0)
                return false;
            return true;
        case YetaWF_ComponentsHTML.OpEnum.SelectionRequired:
        case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIf:
        case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfSupplied:
            if (!valid)
                return true;
            if (value === undefined || value === null || value.trim().length === 0 || value.toString() === "0")
                return false;
            return true;
        case YetaWF_ComponentsHTML.OpEnum.RequiredIfNot:
        case YetaWF_ComponentsHTML.OpEnum.RequiredIfNotSupplied:
            if (valid)
                return true;
            if (value === undefined || value === null || value.trim().length === 0)
                return false;
            return true;
        case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfNot:
        case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfNotSupplied:
            if (valid)
                return true;
            if (value === undefined || value === null || value.trim().length === 0 || value.toString() === "0")
                return false;
            return true;
        default:
            throw `Invalid Op ${parameters.op} in evaluateExpressionList`;
    }
});

$.validator.unobtrusive.adapters.add("requiredexpr", ["op", "json"], (options: any): void => {
    options.rules["requiredexpr"] = {
        op: Number(options.params["op"]),
        exprList: JSON.parse(options.params["json"])
    };
    options.messages["requiredexpr"] = options.message;
});

// SAMEAS
// SAMEAS
// SAMEAS

$.validator.addMethod("sameas", function (this: Function, value: any, element: HTMLElement, parameters: any): boolean {

    if (YetaWF_ComponentsHTML.ValidatorHelper.isSameValue(value, element, parameters)) {
        return $.validator.methods.required.call((this as unknown) as Function, value, element, parameters);
    }
    return true;
});

$.validator.unobtrusive.adapters.add("sameas", [YConfigs.Forms.ConditionPropertyName], (options:any):void => {
    options.rules["sameas"] = {
        dependentproperty: options.params[YConfigs.Forms.ConditionPropertyName],
    };
    options.messages["sameas"] = options.message;
});

// LISTNODUPLICATES
// LISTNODUPLICATES
// LISTNODUPLICATES

$.validator.addMethod("listnoduplicates", function (this: Function, value: any, element: HTMLElement, parameters: any): boolean {
    // this is not currently needed - server-side validation verifies during add of new records
    //return false;// duplicate found
    return true;
});

$.validator.unobtrusive.adapters.add("listnoduplicates", [YConfigs.Forms.ConditionPropertyName, YConfigs.Forms.ConditionPropertyValue], (options:any):void => {
    options.rules["listnoduplicates"] = {};
    options.messages["listnoduplicates"] = options.message;
});

namespace YetaWF_ComponentsHTML {

    export enum OpEnum {
        Required,
        RequiredIf,
        RequiredIfNot,
        RequiredIfSupplied,
        RequiredIfNotSupplied,
        SelectionRequired,
        SelectionRequiredIf,
        SelectionRequiredIfNot,
        SelectionRequiredIfSupplied,
        SelectionRequiredIfNotSupplied,
        SuppressIf,
        SuppressIfNot,
        SuppressIfSupplied,
        SuppressIfNotSupplied,

        ProcessIf,
        ProcessIfNot,
        ProcessIfSupplied,
        ProcessIfNotSupplied,

        HideIf,
        HideIfNot,
        HideIfSupplied,
        HideIfNotSupplied,
    }
    export enum OpCond {
        None,
        Eq,
        NotEq,
    }

    export interface Expr {
        Cond: OpCond;
        _Left: string;
        _Right: string;
        _RightVal: any;
    }

    export class ValidatorHelper {

        public static evaluateExpressionList(value: any, form: HTMLFormElement, op: OpEnum, exprList: Expr[]): boolean {

            let func: (expr: Expr, form: HTMLFormElement) => boolean = ValidatorHelper.isExprValid;
            switch (op) {
                case OpEnum.RequiredIfSupplied:
                case OpEnum.RequiredIfNotSupplied:
                case OpEnum.SelectionRequiredIfSupplied:
                case OpEnum.SelectionRequiredIfNotSupplied:
                case OpEnum.SuppressIfSupplied:
                case OpEnum.SuppressIfNotSupplied:
                case OpEnum.ProcessIfSupplied:
                case OpEnum.ProcessIfNotSupplied:
                case OpEnum.HideIfSupplied:
                case OpEnum.HideIfNotSupplied:
                    func = ValidatorHelper.isExprSupplied;
            }
            for (let expr of exprList) {
                if (!func(expr, form))
                    return false;
            }
            return true;
        }
        public static isExprValid(expr: Expr, form: HTMLFormElement): boolean {
            let leftVal = ValidatorHelper.getPropertyVal(form, expr._Left);
            let rightVal : string;
            if (expr._Right)
                rightVal = ValidatorHelper.getPropertyVal(form, expr._Right);
            else
                rightVal = expr._RightVal == null ? null : expr._RightVal.toString();
            switch (expr.Cond) {
                case OpCond.Eq:
                    if (!leftVal && !rightVal) return true;
                    return leftVal === rightVal;
                case OpCond.NotEq:
                    if (!leftVal && !rightVal) return false;
                    return leftVal !== rightVal;
                default:
                    throw `Invalid Cond ${expr.Cond} in isExprValid`;
            }
        }
        public static isExprSupplied(expr: Expr, form: HTMLFormElement): boolean {
            let leftVal = ValidatorHelper.getPropertyVal(form, expr._Left);
            if (!leftVal) return false;
            return true;
        }
        public static getPropertyVal(form: HTMLFormElement, name: string): string {
            let ctrls = $YetaWF.getElementsBySelector(`input[name="${name}"],select[name="${name}"]`, [form]);
            if (ctrls.length < 1) throw `No control found for name ${name}`;/*DEBUG*/
            let ctrl = ctrls[0];
            let tag = ctrl.tagName;
            let controltype = ctrl.getAttribute("type");

            if (ctrls.length >= 2) {
                // for checkboxes there can be two controls by the same name (the second is a hidden control)
                if (tag !== "INPUT" || controltype !== "checkbox") throw `Multiple controls found for name ${name}`;/*DEBUG*/
            }

            // handle all control types, e.g. radios, checkbox, input, etc.
            let actualValue: string;
            if (tag === "INPUT") {
                // regular input control
                if (controltype === "checkbox") {
                    // checkbox
                    actualValue = (ctrl as HTMLInputElement).checked ? "true" : "false";
                } else {
                    // other
                    actualValue = (ctrl as HTMLInputElement).value.toLowerCase();
                }
            } else if (tag === "SELECT") {
                actualValue = (ctrl as HTMLSelectElement).value;
            } else {
                throw `Unsupported tag ${tag}`;/*DEBUG*/
            }
            return actualValue;
        }

        public static isSameValue(value: any, element: HTMLElement, parameters: any): boolean {
            if ($YetaWF.elementHasClass(element, YConfigs.Forms.CssFormNoValidate)) return true;

            // Get value of the target control - we can't use its Id because it could be non-unique, not predictable
            // use the name attribute instead
            // first, find the enclosing form
            let form = $YetaWF.Forms.getForm(element);
            let name = parameters["dependentproperty"];

            let ctrls = $YetaWF.getElementsBySelector(`input[name="${name}"],select[name="${name}"]`, [form]);
            if (ctrls.length < 1) throw `No control found for name ${name}`;/*DEBUG*/
            if (ctrls.length > 1) throw `Multiple controls found for name ${name}`;/*DEBUG*/
            let ctrl = ctrls[0];
            let tag = ctrl.tagName;

            // handle all control types, e.g. radios, checkbox, input, etc.
            let actualValue: string;
            if (tag === "INPUT") {
                actualValue = (ctrl as HTMLInputElement).value;
            } else if (tag === "SELECT") {
                actualValue = (ctrl as HTMLSelectElement).value;
            } else {
                throw `Unsupported tag ${tag}`;/*DEBUG*/
            }
            if (value === actualValue)
                return true;
            return false;
        }
    }
}