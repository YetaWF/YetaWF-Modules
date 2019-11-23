"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var Validation = /** @class */ (function () {
        function Validation() {
            // Validators
            var _this = this;
            this.Validators = [];
            this.registerValidator("expr", function (form, elem, val) {
                return _this.requiredExprValidator(form, elem, val);
            });
            this.registerValidator("sameas", function (form, elem, val) {
                return _this.sameAsValidator(form, elem, val);
            });
            this.registerValidator("listnoduplicates", function (form, elem, val) {
                return _this.listNoDuplicatesValidator(form, elem, val);
            });
            this.registerValidator("stringlength", function (form, elem, val) {
                return _this.stringLengthValidator(form, elem, val);
            });
            this.registerValidator("range", function (form, elem, val) {
                return _this.rangeValidator(form, elem, val);
            });
            this.registerValidator("regexvalidationbase", function (form, elem, val) {
                return _this.regexValidationBaseValidator(form, elem, val);
            });
            // focus handler
            $YetaWF.registerEventHandlerBody("focusout", "input[data-v],select[data-v],textarea[data-v]", function (ev) {
                var elem = ev.__YetaWFElem;
                var form = $YetaWF.elementClosest(elem, "form");
                _this.validateField(form, elem, true);
                return true;
            });
        }
        Validation.prototype.validateForm = function (form, setMessage) {
            var valid = true;
            this.clearValidation(form);
            var ctrls = $YetaWF.getElementsBySelector("[" + Validation.DATAATTR + "]", [form]);
            for (var _i = 0, ctrls_1 = ctrls; _i < ctrls_1.length; _i++) {
                var ctrl = ctrls_1[_i];
                if (!this.validateField(form, ctrl, setMessage)) {
                    valid = false;
                    if (!setMessage)
                        break;
                }
            }
            return valid;
        };
        Validation.prototype.validateField = function (form, elem, setMessage) {
            if ($YetaWF.getAttributeCond(elem, "disabled") || // don't validate disabled fields
                $YetaWF.getAttributeCond(elem, "readonly") || // don't validate readonly fields
                $YetaWF.elementHasClass(elem, ".yform-novalidate") || // don't validate novalidate fields
                $YetaWF.elementClosestCond(elem, YConfigs.Forms.CssFormNoSubmitContents)) { // don't validate input fields in containers (usually grids)
                return true;
            }
            var data = $YetaWF.getAttributeCond(elem, Validation.DATAATTR);
            if (!data)
                return true;
            var vals = JSON.parse(data);
            var valid = true;
            for (var _i = 0, vals_1 = vals; _i < vals_1.length; _i++) {
                var val = vals_1[_i];
                valid = this.evaluate(form, elem, val);
                if (setMessage) {
                    var name_1 = $YetaWF.getAttribute(elem, "name");
                    var msgElem = $YetaWF.getElement1BySelector("span[data-v-for=\"" + name_1 + "\"]", [form]);
                    $YetaWF.elementRemoveClasses(elem, ["v-valerror"]);
                    $YetaWF.elementRemoveClasses(msgElem, ["v-error", "v-valid"]);
                    if (!valid) {
                        $YetaWF.elementAddClass(elem, "v-valerror");
                        msgElem.innerHTML = "<img src=\"" + $YetaWF.htmlAttrEscape(YConfigs.Forms.CssWarningIconUrl) + "\" name=" + name_1 + " class=\"" + YConfigs.Forms.CssWarningIcon + "\" " + YConfigs.Basics.CssTooltip + "=\"" + $YetaWF.htmlAttrEscape(val.M) + "\"/>";
                        $YetaWF.elementAddClass(msgElem, "v-error");
                    }
                    else {
                        msgElem.innerText = "";
                        $YetaWF.elementAddClass(msgElem, "v-valid");
                    }
                }
                if (!valid)
                    break;
            }
            return valid;
        };
        Validation.prototype.isFormValid = function (form) {
            return this.validateForm(form);
        };
        Validation.prototype.isFieldValid = function (form, elem) {
            return this.validateField(form, elem);
        };
        Validation.prototype.evaluate = function (form, elem, val) {
            var validators = this.Validators.filter(function (entry) { return entry.Name === val.Method; });
            if (validators.length === 0)
                throw "No validator found for " + val.Method;
            else if (validators.length > 1)
                throw "Too many validators found for " + val.Method;
            return validators[0].Func(form, elem, val);
        };
        Validation.prototype.getFieldValue = function (elem) {
            if (elem.tagName === "INPUT") {
                if (elem.getAttribute("type") === "checkbox")
                    return elem.checked;
                return elem.value;
            }
            if (elem.tagName === "TEXTAREA")
                return elem.value;
            if (elem.tagName === "SELECT")
                return elem.value;
            throw "Add support for " + elem.tagName;
        };
        /**
         * Clear any validation errors within the div
         */
        Validation.prototype.clearValidation = function (div) {
            var elems = $YetaWF.getElementsBySelector("[" + Validation.DATAATTR + "]", [div]);
            for (var _i = 0, elems_1 = elems; _i < elems_1.length; _i++) {
                var elem = elems_1[_i];
                var name_2 = $YetaWF.getAttribute(elem, "name");
                var msgElem = $YetaWF.getElement1BySelector("span[data-v-for=\"" + name_2 + "\"]", [div]);
                $YetaWF.elementRemoveClasses(elem, ["v-valerror"]);
                $YetaWF.elementRemoveClasses(msgElem, ["v-error", "v-valid"]);
                $YetaWF.elementAddClass(msgElem, "v-valid");
                msgElem.innerText = "";
            }
        };
        // Registration
        // Registration
        // Registration
        Validation.prototype.registerValidator = function (name, validator) {
            var v = this.Validators.filter(function (entry) { return entry.Name === name; });
            if (!v.length)
                this.Validators.push({ Name: name, Func: validator });
        };
        // Default Validators
        // Default Validators
        // Default Validators
        Validation.prototype.requiredExprValidator = function (form, elem, val) {
            var value = this.getFieldValue(elem);
            var exprs = JSON.parse(val.Expr);
            switch (val.Op) {
                case YetaWF_ComponentsHTML.OpEnum.Required:
                case YetaWF_ComponentsHTML.OpEnum.RequiredIf:
                case YetaWF_ComponentsHTML.OpEnum.RequiredIfSupplied: {
                    var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, val.Op, exprs);
                    if (!valid)
                        return true;
                    if (value === undefined || value === null || value.trim().length === 0)
                        return false;
                    return true;
                }
                case YetaWF_ComponentsHTML.OpEnum.SelectionRequired:
                case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIf:
                case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfSupplied: {
                    var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, val.Op, exprs);
                    if (!valid)
                        return true;
                    if (value === undefined || value === null || value.trim().length === 0 || value.toString() === "0")
                        return false;
                    return true;
                }
                case YetaWF_ComponentsHTML.OpEnum.RequiredIfNot:
                case YetaWF_ComponentsHTML.OpEnum.RequiredIfNotSupplied: {
                    var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, val.Op, exprs);
                    if (valid)
                        return true;
                    if (value === undefined || value === null || value.trim().length === 0)
                        return false;
                    return true;
                }
                case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfNot:
                case YetaWF_ComponentsHTML.OpEnum.SelectionRequiredIfNotSupplied: {
                    var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(value, form, val.Op, exprs);
                    if (valid)
                        return true;
                    if (value === undefined || value === null || value.trim().length === 0 || value.toString() === "0")
                        return false;
                    return true;
                }
                default:
                    throw "Invalid Op " + val.Op + " in evaluateExpressionList";
            }
        };
        Validation.prototype.sameAsValidator = function (form, elem, val) {
            var value = this.getFieldValue(elem);
            if (!value)
                return true;
            var item = ControlsHelper.getControlItemByName(val.CondProp, form);
            var actualValue = ControlsHelper.getControlValue(item);
            return value === actualValue;
        };
        Validation.prototype.listNoDuplicatesValidator = function (form, elem, val) {
            // this is not currently needed - server-side validation verifies during add of new records
            //return false;// duplicate found
            return true;
        };
        Validation.prototype.stringLengthValidator = function (form, elem, val) {
            var value = this.getFieldValue(elem);
            if (!value)
                return true;
            var len = value.length;
            return len <= val.Max && len >= val.Min;
        };
        Validation.prototype.regexValidationBaseValidator = function (form, elem, val) {
            var value = this.getFieldValue(elem);
            if (!value)
                return true;
            var re = new RegExp(val.Pattern);
            return re.test(value);
        };
        Validation.prototype.rangeValidator = function (form, elem, val) {
            var value = this.getFieldValue(elem);
            if (!value)
                return true;
            return value <= val.Max && value >= val.Min;
        };
        Validation.DATAATTR = "data-v";
        return Validation;
    }());
    YetaWF_ComponentsHTML.Validation = Validation;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
// tslint:disable-next-line:variable-name
var YetaWF_ComponentsHTML_Validation = new YetaWF_ComponentsHTML.Validation();

//# sourceMappingURL=validation.js.map
