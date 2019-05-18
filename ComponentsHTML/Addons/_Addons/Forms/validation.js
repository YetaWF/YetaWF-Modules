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
// SELECTIONREQUIRED
// SELECTIONREQUIRED
// SELECTIONREQUIRED
$.validator.addMethod("selectionrequired", function (value, element, parameters) {
    if ($YetaWF.elementHasClass(element, YConfigs.Forms.CssFormNoValidate))
        return true;
    if (value === undefined || value === null || value.trim().length === 0 || value.trim() === "0")
        return false;
    return true;
});
$.validator.unobtrusive.adapters.add("selectionrequired", [YConfigs.Forms.ConditionPropertyName, YConfigs.Forms.ConditionPropertyValue], function (options) {
    options.rules["selectionrequired"] = {};
    options.messages["selectionrequired"] = options.message;
});
// SELECTIONREQUIREDIF
// SELECTIONREQUIREDIF
$.validator.addMethod("selectionrequiredif", function (value, element, parameters) {
    if (YetaWF_ComponentsHTML.ValidatorHelper.isCondAndDependentValue(value, element, parameters)) {
        // if the condition is true, reuse the existing
        // required field validator functionality
        if (value === undefined || value === null || value.trim().length === 0 || value.trim() === "0")
            return false;
    }
    return true;
});
$.validator.unobtrusive.adapters.add("selectionrequiredif", [YConfigs.Forms.ConditionPropertyName, YConfigs.Forms.ConditionPropertyValue], function (options) {
    options.rules["selectionrequiredif"] = {
        dependentproperty: options.params[YConfigs.Forms.ConditionPropertyName],
        targetvalue: options.params[YConfigs.Forms.ConditionPropertyValue]
    };
    options.messages["selectionrequiredif"] = options.message;
});
// REQUIREDEXPR
// REQUIREDEXPR
// REQUIREDEXPR
$.validator.addMethod("requiredexpr", function (value, element, parameters) {
    return YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, element, parameters);
});
$.validator.unobtrusive.adapters.add("requiredexpr", ["op", "json"], function (options) {
    options.rules["requiredexpr"] = {
        op: Number(options.params["op"]),
        exprList: JSON.parse(options.params["json"])
    };
    options.messages["requiredexpr"] = options.message;
});
// REQUIREDIFINRANGE
// REQUIREDIFINRANGE
// REQUIREDIFINRANGE
$.validator.addMethod("requiredifinrange", function (value, element, parameters) {
    if (YetaWF_ComponentsHTML.ValidatorHelper.isRangeValue(value, element, parameters)) {
        // if the condition is true, reuse the existing
        // required field validator functionality
        return $.validator.methods.required.call(this, value, element, parameters);
    }
    return true;
});
$.validator.unobtrusive.adapters.add("requiredifinrange", [YConfigs.Forms.ConditionPropertyName, YConfigs.Forms.ConditionPropertyValueLow, YConfigs.Forms.ConditionPropertyValueHigh], function (options) {
    options.rules["requiredifinrange"] = {
        dependentproperty: options.params[YConfigs.Forms.ConditionPropertyName],
        targetvaluelow: options.params[YConfigs.Forms.ConditionPropertyValueLow],
        targetvaluehigh: options.params[YConfigs.Forms.ConditionPropertyValueHigh]
    };
    options.messages["requiredifinrange"] = options.message;
});
// REQUIREDIFSUPPLIED
// REQUIREDIFSUPPLIED
// REQUIREDIFSUPPLIED
$.validator.addMethod("requiredifsupplied", function (value, element, parameters) {
    if (YetaWF_ComponentsHTML.ValidatorHelper.isSuppliedValue(value, element, parameters)) {
        return $.validator.methods.required.call(this, value, element, parameters);
    }
    return false;
});
$.validator.unobtrusive.adapters.add("requiredifsupplied", [YConfigs.Forms.ConditionPropertyName], function (options) {
    options.rules["requiredifsupplied"] = {
        dependentproperty: options.params[YConfigs.Forms.ConditionPropertyName]
    };
    options.messages["requiredifsupplied"] = options.message;
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
        OpEnum[OpEnum["RequiredIf"] = 0] = "RequiredIf";
        OpEnum[OpEnum["RequiredIfNot"] = 1] = "RequiredIfNot";
        OpEnum[OpEnum["RequiredIfSupplied"] = 2] = "RequiredIfSupplied";
        OpEnum[OpEnum["RequiredIfNotSupplied"] = 3] = "RequiredIfNotSupplied";
        OpEnum[OpEnum["ProcessIf"] = 4] = "ProcessIf";
        OpEnum[OpEnum["ProcessIfNot"] = 5] = "ProcessIfNot";
        OpEnum[OpEnum["ProcessIfSupplied"] = 6] = "ProcessIfSupplied";
        OpEnum[OpEnum["ProcessIfNotSupplied"] = 7] = "ProcessIfNotSupplied";
        OpEnum[OpEnum["SuppressIf"] = 8] = "SuppressIf";
        OpEnum[OpEnum["SuppressIfNot"] = 9] = "SuppressIfNot";
        OpEnum[OpEnum["SuppressIfSupplied"] = 10] = "SuppressIfSupplied";
        OpEnum[OpEnum["SuppressIfNotSupplied"] = 11] = "SuppressIfNotSupplied";
        OpEnum[OpEnum["HideIfNotSupplied"] = 12] = "HideIfNotSupplied";
    })(OpEnum || (OpEnum = {}));
    var OpCond;
    (function (OpCond) {
        OpCond[OpCond["Eq"] = 0] = "Eq";
        OpCond[OpCond["NotEq"] = 1] = "NotEq";
    })(OpCond || (OpCond = {}));
    var ValidatorHelper = /** @class */ (function () {
        function ValidatorHelper() {
        }
        ValidatorHelper.evaluateExpressionList = function (value, element, parameters) {
            var form = $YetaWF.Forms.getForm(element);
            var exprList = parameters.exprList;
            for (var _i = 0, exprList_1 = exprList; _i < exprList_1.length; _i++) {
                var expr = exprList_1[_i];
                switch (parameters.op) {
                    case OpEnum.RequiredIf:
                        if (!ValidatorHelper.isExprValid(expr, form))
                            return true;
                        break;
                    case OpEnum.RequiredIfNot:
                        if (ValidatorHelper.isExprValid(expr, form))
                            return true;
                        break;
                    default:
                        throw "Invalid Op " + parameters.op + " in evaluateExpressionList";
                }
            }
            switch (parameters.op) {
                case OpEnum.RequiredIf:
                case OpEnum.RequiredIfNot:
                    if (value === undefined || value === null || value.trim().length === 0)
                        return false;
                    break;
                default:
                    throw "Invalid Op " + parameters.op + " in evaluateExpressionList";
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
                    return leftVal === rightVal;
                default:
                    throw "Invalid Cond " + expr.Cond + " in isExprValid";
            }
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
        //$$$$$$$$$$$$$$$
        ValidatorHelper.isCondAndDependentValue = function (value, element, parameters) {
            if ($YetaWF.elementHasClass(element, YConfigs.Forms.CssFormNoValidate))
                return true;
            // get the target value (as a string)
            var condValue = parameters["targetvalue"];
            var conditionvalue = (condValue === null ? "" : condValue).toString();
            // Get value of the target control - we can"t use its Id because it could be non-unique, not predictable
            // use the name attribute instead
            // first, find the enclosing form
            var form = $YetaWF.Forms.getForm(element);
            var name = parameters["dependentproperty"];
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
                    conditionvalue = conditionvalue.toLowerCase();
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
            if (conditionvalue.toLowerCase() === actualValue.toLowerCase())
                return true;
            return false;
        };
        ValidatorHelper.isRangeValue = function (value, element, parameters) {
            if ($YetaWF.elementHasClass(element, YConfigs.Forms.CssFormNoValidate))
                return true;
            // get the target value (as a int as that's what the actual value will be)
            var conditionvaluelow = parseInt(parameters["targetvaluelow"], 10);
            var conditionvaluehigh = parseInt(parameters["targetvaluehigh"], 10);
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
                // regular input control
                actualValue = Number(ctrl.value);
            }
            else if (tag === "SELECT") {
                actualValue = Number(ctrl.value);
            }
            else {
                throw "Unsupported tag " + tag; /*DEBUG*/
            }
            if (actualValue >= conditionvaluelow && actualValue <= conditionvaluehigh)
                return true;
            return false;
        };
        ValidatorHelper.isSuppliedValue = function (value, element, parameters) {
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
                actualValue = ctrl.value.trim();
            }
            else if (tag === "SELECT") {
                actualValue = ctrl.value;
                if (actualValue === "0")
                    actualValue = "";
            }
            else {
                throw "Unsupported tag " + tag; /*DEBUG*/
            }
            if (actualValue !== undefined && actualValue !== "")
                return true;
            return false;
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
