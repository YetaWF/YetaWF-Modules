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
    ignore: "." + YConfigs.Forms.CssFormNoValidate,
    onsubmit: false // don't validate on submit, we want to see the submit event and validate things ourselves
});
$.validator.unobtrusive.options = {
    errorElement: "label"
};
// REQUIREDEXPR
// REQUIREDEXPR
// REQUIREDEXPR
$.validator.addMethod("requiredexpr", function (value, element, parameters) {
    var form = $YetaWF.Forms.getForm(element);
    var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, parameters.op, parameters.exprList);
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
                    return leftVal === rightVal;
                case OpCond.NotEq:
                    if (!leftVal && !rightVal)
                        return false;
                    return leftVal !== rightVal;
                default:
                    throw "Invalid Cond " + expr.Cond + " in isExprValid";
            }
        };
        ValidatorHelper.isExprSupplied = function (expr, form) {
            var leftVal = ValidatorHelper.getPropertyVal(form, expr._Left);
            if (!leftVal)
                return false;
            return true;
        };
        ValidatorHelper.getPropertyVal = function (form, name) {
            var ctrls = $YetaWF.getElementsBySelector("input[name=\"" + name + "\"],select[name=\"" + name + "\"]", [form]);
            if (ctrls.length < 1)
                throw "No control found for name " + name; /*DEBUG*/
            var ctrl = ctrls[0];
            var tag = ctrl.tagName;
            var controltype = ctrl.getAttribute("type");
            if (ctrls.length >= 2) {
                // for checkboxes there can be two controls by the same name (the second is a hidden control)
                if (tag !== "INPUT" || controltype !== "checkbox")
                    throw "Multiple controls found for name " + name; /*DEBUG*/
            }
            // handle all control types, e.g. radios, checkbox, input, etc.
            var actualValue;
            if (tag === "INPUT") {
                // regular input control
                if (controltype === "checkbox") {
                    // checkbox
                    actualValue = ctrl.checked ? "true" : "false";
                }
                else {
                    // other
                    actualValue = ctrl.value.toLowerCase();
                }
            }
            else if (tag === "SELECT") {
                actualValue = ctrl.value;
            }
            else {
                throw "Unsupported tag " + tag; /*DEBUG*/
            }
            return actualValue;
        };
        ValidatorHelper.isSameValue = function (value, element, parameters) {
            if ($YetaWF.elementHasClass(element, YConfigs.Forms.CssFormNoValidate))
                return true;
            // Get value of the target control - we can't use its Id because it could be non-unique, not predictable
            // use the name attribute instead
            // first, find the enclosing form
            var form = $YetaWF.Forms.getForm(element);
            var name = parameters["dependentproperty"];
            var ctrls = $YetaWF.getElementsBySelector("input[name=\"" + name + "\"],select[name=\"" + name + "\"]", [form]);
            if (ctrls.length < 1)
                throw "No control found for name " + name; /*DEBUG*/
            if (ctrls.length > 1)
                throw "Multiple controls found for name " + name; /*DEBUG*/
            var ctrl = ctrls[0];
            var tag = ctrl.tagName;
            // handle all control types, e.g. radios, checkbox, input, etc.
            var actualValue;
            if (tag === "INPUT") {
                actualValue = ctrl.value;
            }
            else if (tag === "SELECT") {
                actualValue = ctrl.value;
            }
            else {
                throw "Unsupported tag " + tag; /*DEBUG*/
            }
            if (value === actualValue)
                return true;
            return false;
        };
        return ValidatorHelper;
    }());
    YetaWF_ComponentsHTML.ValidatorHelper = ValidatorHelper;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=validation.js.map
