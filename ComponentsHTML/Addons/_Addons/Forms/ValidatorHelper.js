"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var OpEnum;
    (function (OpEnum) {
        OpEnum[OpEnum["Required"] = 0] = "Required";
        OpEnum[OpEnum["RequiredIf"] = 1] = "RequiredIf";
        OpEnum[OpEnum["RequiredIfNot"] = 2] = "RequiredIfNot";
        OpEnum[OpEnum["RequiredIfSupplied"] = 3] = "RequiredIfSupplied";
        OpEnum[OpEnum["RequiredIfNotSupplied"] = 4] = "RequiredIfNotSupplied";
        OpEnum[OpEnum["SelectionRequired"] = 5] = "SelectionRequired";
        OpEnum[OpEnum["SelectionRequiredIf"] = 6] = "SelectionRequiredIf";
        OpEnum[OpEnum["SelectionRequiredIfNot"] = 7] = "SelectionRequiredIfNot";
        OpEnum[OpEnum["SelectionRequiredIfSupplied"] = 8] = "SelectionRequiredIfSupplied";
        OpEnum[OpEnum["SelectionRequiredIfNotSupplied"] = 9] = "SelectionRequiredIfNotSupplied";
        OpEnum[OpEnum["SuppressIf"] = 10] = "SuppressIf";
        OpEnum[OpEnum["SuppressIfNot"] = 11] = "SuppressIfNot";
        OpEnum[OpEnum["SuppressIfSupplied"] = 12] = "SuppressIfSupplied";
        OpEnum[OpEnum["SuppressIfNotSupplied"] = 13] = "SuppressIfNotSupplied";
        OpEnum[OpEnum["ProcessIf"] = 14] = "ProcessIf";
        OpEnum[OpEnum["ProcessIfNot"] = 15] = "ProcessIfNot";
        OpEnum[OpEnum["ProcessIfSupplied"] = 16] = "ProcessIfSupplied";
        OpEnum[OpEnum["ProcessIfNotSupplied"] = 17] = "ProcessIfNotSupplied";
        OpEnum[OpEnum["HideIf"] = 18] = "HideIf";
        OpEnum[OpEnum["HideIfNot"] = 19] = "HideIfNot";
        OpEnum[OpEnum["HideIfSupplied"] = 20] = "HideIfSupplied";
        OpEnum[OpEnum["HideIfNotSupplied"] = 21] = "HideIfNotSupplied";
    })(OpEnum = YetaWF_ComponentsHTML.OpEnum || (YetaWF_ComponentsHTML.OpEnum = {}));
    var OpCond;
    (function (OpCond) {
        OpCond[OpCond["None"] = 0] = "None";
        OpCond[OpCond["Eq"] = 1] = "Eq";
        OpCond[OpCond["NotEq"] = 2] = "NotEq";
    })(OpCond = YetaWF_ComponentsHTML.OpCond || (YetaWF_ComponentsHTML.OpCond = {}));
    var ValidatorHelper = /** @class */ (function () {
        function ValidatorHelper() {
        }
        ValidatorHelper.evaluateExpressionList = function (value, form, op, exprList) {
            var func = ValidatorHelper.isExprValid;
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
            for (var _i = 0, exprList_1 = exprList; _i < exprList_1.length; _i++) {
                var expr = exprList_1[_i];
                if (!func(expr, form))
                    return false;
            }
            return true;
        };
        ValidatorHelper.isExprValid = function (expr, form) {
            var leftVal = ValidatorHelper.getPropertyVal(form, expr._Left);
            var rightVal;
            if (expr._Right)
                rightVal = ValidatorHelper.getPropertyVal(form, expr._Right);
            else
                rightVal = expr._RightVal == null ? null : expr._RightVal.toString();
            switch (expr.Cond) {
                case OpCond.Eq:
                    if (!leftVal && !rightVal)
                        return true;
                    return ValidatorHelper.EqualStrings(leftVal, rightVal);
                case OpCond.NotEq:
                    if (!leftVal && !rightVal)
                        return false;
                    return !ValidatorHelper.EqualStrings(leftVal, rightVal);
                default:
                    throw "Invalid Cond " + expr.Cond + " in isExprValid";
            }
        };
        ValidatorHelper.EqualStrings = function (s1, s2) {
            // special case bool handling, ignore case
            var s1L = (s1) ? s1.toLowerCase() : "";
            var s2L = (s2) ? s2.toLowerCase() : "";
            if ((s1L === "true" || s1L === "false") && (s2L === "true" || s2L === "false"))
                return s1L === s2L;
            else
                return s1 === s2;
        };
        ValidatorHelper.isExprSupplied = function (expr, form) {
            var leftVal = ValidatorHelper.getPropertyVal(form, expr._Left);
            if (!leftVal)
                return false;
            return true;
        };
        ValidatorHelper.getPropertyVal = function (form, name) {
            var item = ControlsHelper.getControlItemByName(name, form);
            var row = $YetaWF.elementClosestCond(item.Template, ".yt_propertylist .t_row");
            if (row) {
                if ($YetaWF.elementHasClass(row, "t_hidden"))
                    return null;
                if ($YetaWF.elementHasClass(row, "t_disabled"))
                    return null;
            }
            var value = ControlsHelper.getControlValue(item);
            return value;
        };
        return ValidatorHelper;
    }());
    YetaWF_ComponentsHTML.ValidatorHelper = ValidatorHelper;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
