/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

// validation for all forms
// does not implement any required externally callable functions
// http://jqueryvalidation.org/reference/

// Form submit handling for all forms
// http://jqueryvalidation.org/documentation/
// http://bradwilson.typepad.com/blog/2010/10/mvc3-unobtrusive-validation.html

// Make sure all hidden fields are NOT ignored
$.validator.setDefaults({
    ignore: `.yform-novalidate`, // don't ignore hidden fields - ignore fields with no validate class
    onsubmit: false         // don't validate on submit, we want to see the submit event and validate things ourselves
});
($.validator.unobtrusive as any).options = {
    errorElement: "label"
};

// REQUIREDEXPR
// REQUIREDEXPR
// REQUIREDEXPR

$.validator.addMethod("requiredexpr", function (this: Function, value: any, element: HTMLElement, parameters: any): boolean {
    switch (parameters.op) {
        case YetaWF_ComponentsHTML.OpEnum.Required:
        case YetaWF_ComponentsHTML.OpEnum.RequiredIf:
        case YetaWF_ComponentsHTML.OpEnum.RequiredIfSupplied: {
            let form = $YetaWF.Forms.getForm(element);
            let exprs: YetaWF_ComponentsHTML.Expr[][] = parameters.exprList;
            let match = false;
            for (let expr of exprs) {
                let valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, parameters.op, expr);
                if (valid) {
                    match = true;
                    break;
                }
            }
            if (!match)
                return true;
            if (value === undefined || value === null || value.trim().length === 0)
                return false;
            return true;
        }
        case YetaWF_ComponentsHTML.OpEnum.SelectionRequired:
        case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIf:
        case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfSupplied: {
            let form = $YetaWF.Forms.getForm(element);
            let exprs: YetaWF_ComponentsHTML.Expr[][] = parameters.exprList;
            let match = false;
            for (let expr of exprs) {
                let valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, parameters.op, expr);
                if (valid) {
                    match = true;
                    break;
                }
            }
            if (!match)
                return true;
            if (value === undefined || value === null || value.trim().length === 0 || value.toString() === "0")
                return false;
            return true;
        }
        case YetaWF_ComponentsHTML.OpEnum.RequiredIfNot:
        case YetaWF_ComponentsHTML.OpEnum.RequiredIfNotSupplied: {
            let form = $YetaWF.Forms.getForm(element);
            let exprs: YetaWF_ComponentsHTML.Expr[][] = parameters.exprList;
            let match = false;
            for (let expr of exprs) {
                let valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, parameters.op, expr);
                if (valid) {
                    match = true;
                    break;
                }
            }
            if (match)
                return true;
            if (value === undefined || value === null || value.trim().length === 0)
                return false;
            return true;
        }
        case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfNot:
        case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfNotSupplied: {
            let form = $YetaWF.Forms.getForm(element);
            let exprs: YetaWF_ComponentsHTML.Expr[][] = parameters.exprList;
            let match = false;
            for (let expr of exprs) {
                let valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, parameters.op, expr);
                if (valid) {
                    match = true;
                    break;
                }
            }
            if (match)
                return true;
            if (value === undefined || value === null || value.trim().length === 0 || value.toString() === "0")
                return false;
            return true;
        }
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
            let rightVal : string | null;
            if (expr._Right)
                rightVal = ValidatorHelper.getPropertyVal(form, expr._Right);
            else
                rightVal = expr._RightVal == null ? null : expr._RightVal.toString();
            switch (expr.Cond) {
                case OpCond.Eq:
                    if (!leftVal && !rightVal) return true;
                    return ValidatorHelper.EqualStrings(leftVal, rightVal);
                case OpCond.NotEq:
                    if (!leftVal && !rightVal) return false;
                    return !ValidatorHelper.EqualStrings(leftVal, rightVal);
                default:
                    throw `Invalid Cond ${expr.Cond} in isExprValid`;
            }
        }
        private static EqualStrings(s1: string | null, s2: string | null): boolean {
            // special case bool handling, ignore case
            let s1L = (s1) ? s1.toLowerCase() : "";
            let s2L = (s2) ? s2.toLowerCase() : "";
            if ((s1L === "true" || s1L === "false") && (s2L === "true" || s2L === "false"))
                return s1L === s2L;
            else
                return s1 === s2;
        }
        public static isExprSupplied(expr: Expr, form: HTMLFormElement): boolean {
            let leftVal = ValidatorHelper.getPropertyVal(form, expr._Left);
            if (!leftVal) return false;
            return true;
        }
        public static getPropertyVal(form: HTMLFormElement, name: string): string | null {
            let item = ControlsHelper.getControlItemByName(name, form);
            let row = $YetaWF.elementClosestCond(item.Template, ".yt_propertylist .t_row") as HTMLDivElement | null;
            if (row) {
                if ($YetaWF.elementHasClass(row, "t_hidden")) return null;
                if ($YetaWF.elementHasClass(row, "t_disabled")) return null;
            }
            let value = ControlsHelper.getControlValue(item);
            return value;
        }

        public static isSameValue(value: any, element: HTMLElement, parameters: any): boolean {
            if ($YetaWF.elementHasClass(element, "yform-novalidate")) return true;

            // Get value of the target control - we can't use its Id because it could be non-unique, not predictable
            // use the name attribute instead
            // first, find the enclosing form
            let form = $YetaWF.Forms.getForm(element);
            let name = parameters["dependentproperty"];

            let item = ControlsHelper.getControlItemByName(name, form);
            let actualValue = ControlsHelper.getControlValue(item);

            if (value === actualValue)
                return true;
            return false;
        }
    }
}