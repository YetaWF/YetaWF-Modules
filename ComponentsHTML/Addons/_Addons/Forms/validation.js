"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
// validation for all forms
// does not implement any required externally callable functions
// http://jqueryvalidation.org/reference/
// Form submit handling for all forms
// http://jqueryvalidation.org/documentation/
// http://bradwilson.typepad.com/blog/2010/10/mvc3-unobtrusive-validation.html
// Make sure all hidden fields are NOT ignored
$.validator.setDefaults({
    ignore: ".yform-novalidate",
    onsubmit: false // don't validate on submit, we want to see the submit event and validate things ourselves
});
$.validator.unobtrusive.options = {
    errorElement: "label"
};
// REQUIREDEXPR
// REQUIREDEXPR
// REQUIREDEXPR
$.validator.addMethod("requiredexpr", function (value, element, parameters) {
    switch (parameters.op) {
        case YetaWF_ComponentsHTML.OpEnum.Required:
        case YetaWF_ComponentsHTML.OpEnum.RequiredIf:
        case YetaWF_ComponentsHTML.OpEnum.RequiredIfSupplied: {
            var form = $YetaWF.Forms.getForm(element);
            var exprs = parameters.exprList;
            var match = false;
            for (var _i = 0, exprs_1 = exprs; _i < exprs_1.length; _i++) {
                var expr = exprs_1[_i];
                var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, parameters.op, expr);
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
            var form = $YetaWF.Forms.getForm(element);
            var exprs = parameters.exprList;
            var match = false;
            for (var _a = 0, exprs_2 = exprs; _a < exprs_2.length; _a++) {
                var expr = exprs_2[_a];
                var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, parameters.op, expr);
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
            var form = $YetaWF.Forms.getForm(element);
            var exprs = parameters.exprList;
            var match = false;
            for (var _b = 0, exprs_3 = exprs; _b < exprs_3.length; _b++) {
                var expr = exprs_3[_b];
                var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, parameters.op, expr);
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
            var form = $YetaWF.Forms.getForm(element);
            var exprs = parameters.exprList;
            var match = false;
            for (var _c = 0, exprs_4 = exprs; _c < exprs_4.length; _c++) {
                var expr = exprs_4[_c];
                var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, parameters.op, expr);
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
            throw "Invalid Op " + parameters.op + " in evaluateExpressionList";
    }
});
$.validator.unobtrusive.adapters.add("requiredexpr", ["op", "json"], function (options) {
    options.rules["requiredexpr"] = {
        op: Number(options.params["op"]),
        exprList: JSON.parse(options.params["json"])
    };
    options.messages["requiredexpr"] = options.message;
});
// SAMEAS
// SAMEAS
// SAMEAS
$.validator.addMethod("sameas", function (value, element, parameters) {
    if (YetaWF_ComponentsHTML.ValidatorHelper.isSameValue(value, element, parameters)) {
        return $.validator.methods.required.call(this, value, element, parameters);
    }
    return true;
});
$.validator.unobtrusive.adapters.add("sameas", [YConfigs.Forms.ConditionPropertyName], function (options) {
    options.rules["sameas"] = {
        dependentproperty: options.params[YConfigs.Forms.ConditionPropertyName],
    };
    options.messages["sameas"] = options.message;
});
// LISTNODUPLICATES
// LISTNODUPLICATES
// LISTNODUPLICATES
$.validator.addMethod("listnoduplicates", function (value, element, parameters) {
    // this is not currently needed - server-side validation verifies during add of new records
    //return false;// duplicate found
    return true;
});
$.validator.unobtrusive.adapters.add("listnoduplicates", [YConfigs.Forms.ConditionPropertyName, YConfigs.Forms.ConditionPropertyValue], function (options) {
    options.rules["listnoduplicates"] = {};
    options.messages["listnoduplicates"] = options.message;
});
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
        ValidatorHelper.isSameValue = function (value, element, parameters) {
            if ($YetaWF.elementHasClass(element, "yform-novalidate"))
                return true;
            // Get value of the target control - we can't use its Id because it could be non-unique, not predictable
            // use the name attribute instead
            // first, find the enclosing form
            var form = $YetaWF.Forms.getForm(element);
            var name = parameters["dependentproperty"];
            var item = ControlsHelper.getControlItemByName(name, form);
            var actualValue = ControlsHelper.getControlValue(item);
            if (value === actualValue)
                return true;
            return false;
        };
        return ValidatorHelper;
    }());
    YetaWF_ComponentsHTML.ValidatorHelper = ValidatorHelper;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=validation.js.map
