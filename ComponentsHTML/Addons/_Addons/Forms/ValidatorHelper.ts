/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* eslint-disable no-underscore-dangle */

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
    }
}