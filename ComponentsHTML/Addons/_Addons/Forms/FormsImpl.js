"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
/* Forms implementation required by YetaWF */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var FormsImpl = /** @class */ (function () {
        function FormsImpl() {
        }
        // Partialform Initialization
        /**
         * Initializes a partialform.
         */
        FormsImpl.prototype.initPartialForm = function (partialForm) {
            // find the first field in each tab control that has an input validation error and activate that tab
            // This will not work for nested tabs. Only the lowermost tab will be activated.
            var elems = $YetaWF.getElementsBySelector("div.yt_propertylist.t_tabbed", [partialForm]);
            elems.forEach(function (tabctrl, index) {
                YetaWF_FormsImpl.setErrorInTab(tabctrl);
            });
        };
        // Validation
        /**
         * Re-validate all fields within the div, typically used after paging in a grid to let jquery.validate update all fields
         */
        FormsImpl.prototype.updateValidation = function (div) {
            // apparently not used
            throw "updateValidation not implemented";
        };
        /**
         * Clear any validation errors within the div
         */
        FormsImpl.prototype.clearValidation = function (div) {
            return YetaWF_ComponentsHTML_Validation.clearValidation(div);
        };
        /**
         * Clear any validation errors for one element.
         */
        FormsImpl.prototype.clearValidation1 = function (elem) {
            var form = $YetaWF.Forms.getForm(elem);
            return YetaWF_ComponentsHTML_Validation.clearValidation1(elem, form);
        };
        /**
         * Validate one element.
         * If the contents are empty the field will be fully validated. If contents are present, the error indicator is reset.
         * Full validation takes place on blur (or using validateElementFully).
         */
        FormsImpl.prototype.validateElement = function (ctrl, hasValue) {
            var form = $YetaWF.Forms.getFormCond(ctrl);
            if (!form)
                return;
            YetaWF_ComponentsHTML_Validation.validateField(form, ctrl, true, hasValue);
        };
        /**
         * Validate one element.
         * Full validation takes place.
         */
        FormsImpl.prototype.validateElementFully = function (ctrl) {
            var form = $YetaWF.Forms.getFormCond(ctrl);
            if (!form)
                return;
            YetaWF_ComponentsHTML_Validation.validateFieldFully(form, ctrl, true);
        };
        /**
         * Returns whether a div has form errors.
         */
        FormsImpl.prototype.hasErrors = function (div) {
            var errImgs = $YetaWF.getElementsBySelector("span[data-v-for] img", [div]);
            return errImgs.length > 0;
        };
        /**
         * Shows all div form errors in a popup.
         */
        FormsImpl.prototype.showErrors = function (div) {
            var errImgs = $YetaWF.getElementsBySelector("span[data-v-for] img", [div]);
            // eliminate duplicate error messages
            var errs = [];
            var _loop_1 = function (errImg) {
                var err = $YetaWF.getAttribute(errImg, "data-tooltip");
                var e = errs.filter(function (entry) { return entry === err; });
                if (!e.length)
                    errs.push(err);
            };
            for (var _i = 0, errImgs_1 = errImgs; _i < errImgs_1.length; _i++) {
                var errImg = errImgs_1[_i];
                _loop_1(errImg);
            }
            // format message
            var s = "";
            for (var _a = 0, errs_1 = errs; _a < errs_1.length; _a++) {
                var err = errs_1[_a];
                s += err + "(+nl)";
            }
            $YetaWF.error(YLocs.Forms.FormErrors + s);
        };
        /**
         * Serializes the form and returns a name/value pairs array
         */
        FormsImpl.prototype.serializeFormArray = function (form) {
            var array = [];
            var elems = $YetaWF.getElementsBySelector("input,select,textarea", [form]);
            for (var _i = 0, elems_1 = elems; _i < elems_1.length; _i++) {
                var elem = elems_1[_i];
                var name_1 = $YetaWF.getAttributeCond(elem, "name");
                if (!name_1 ||
                    $YetaWF.getAttributeCond(elem, "disabled") || // don't submit disabled fields
                    $YetaWF.getAttributeCond(elem, "readonly") || // don't submit readonly fields
                    $YetaWF.elementHasClass(elem, YConfigs.Forms.CssFormNoSubmit) || // don't submit nosubmit fields
                    $YetaWF.elementClosestCond(elem, "." + YConfigs.Forms.CssFormNoSubmitContents)) // don't submit input fields in containers (usually grids)
                    continue;
                array.push({
                    name: name_1,
                    value: YetaWF_ComponentsHTML_Validation.getFieldValue(elem).toString()
                });
            }
            return array;
        };
        /**
         * Validate all fields in the current form.
         */
        FormsImpl.prototype.validate = function (form) {
            return YetaWF_ComponentsHTML_Validation.validateForm(form, true);
        };
        /**
         * Returns whether all fields in the current form are valid.
         */
        FormsImpl.prototype.isValid = function (form) {
            return YetaWF_ComponentsHTML_Validation.isFormValid(form);
        };
        /**
         * If there is a validation in the specified tab control, the tab is activated.
         */
        FormsImpl.prototype.setErrorInTab = function (tabctrl) {
            // get the first field in error (if any)
            var errField = $YetaWF.getElement1BySelectorCond(".v-valerror", [tabctrl]);
            if (errField) {
                // find out which tab panel we're on
                var ttabpanel = $YetaWF.elementClosest(errField, "div.t_tabpanel");
                var panel = ttabpanel.getAttribute("data-tab");
                if (!panel)
                    throw "We found a panel in a tab control without panel number (data-tab attribute)."; /*DEBUG*/
                // get the tab entry
                // TODO: MOVE TO TAB CONTROL
                var $tabctrl = $(tabctrl);
                var $te = $("ul.t_tabstrip > li", $tabctrl).eq(panel);
                if ($te.length === 0)
                    throw "We couldn't find the tab entry for panel " + panel; /*DEBUG*/
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery)
                    $tabctrl.tabs("option", "active", panel);
                else if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.Kendo)
                    $tabctrl.data("kendoTabStrip").activateTab($te);
                else
                    throw "Unknown tab style"; /*DEBUG*/
            }
        };
        // Forms initialization
        /**
         * Initialize the form when page/content is ready.
         * No external use.
         */
        FormsImpl.prototype.initForm = function (tag) {
            var forms = $YetaWF.getElementsBySelector("form", [tag]);
            for (var _i = 0, forms_1 = forms; _i < forms_1.length; _i++) {
                var form = forms_1[_i];
                if ($YetaWF.elementHasClass(form, "yValidateImmediately")) {
                    YetaWF_ComponentsHTML_Validation.validateForm(form, true);
                }
            }
        };
        return FormsImpl;
    }());
    YetaWF_ComponentsHTML.FormsImpl = FormsImpl;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
// tslint:disable-next-line:variable-name
var YetaWF_FormsImpl = new YetaWF_ComponentsHTML.FormsImpl();
/* Page load */
$YetaWF.addWhenReady(YetaWF_FormsImpl.initForm);

//# sourceMappingURL=FormsImpl.js.map
