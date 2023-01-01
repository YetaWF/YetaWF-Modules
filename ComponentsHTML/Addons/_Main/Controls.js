"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ControlTypeEnum;
    (function (ControlTypeEnum) {
        ControlTypeEnum[ControlTypeEnum["Input"] = 0] = "Input";
        ControlTypeEnum[ControlTypeEnum["Select"] = 1] = "Select";
        ControlTypeEnum[ControlTypeEnum["TextArea"] = 2] = "TextArea";
        ControlTypeEnum[ControlTypeEnum["Div"] = 3] = "Div";
        ControlTypeEnum[ControlTypeEnum["Hidden"] = 10] = "Hidden";
        ControlTypeEnum[ControlTypeEnum["Template"] = 20] = "Template";
    })(ControlTypeEnum = YetaWF_ComponentsHTML.ControlTypeEnum || (YetaWF_ComponentsHTML.ControlTypeEnum = {}));
    var Controls = /** @class */ (function () {
        function Controls() {
        }
        Controls.prototype.getControlItemByNameCond = function (name, container) {
            var template = YetaWF.ComponentBaseDataImpl.getTemplateFromControlNameCond(name, [container]);
            if (!template)
                return null;
            return this.getControlItemFromTemplate(template);
        };
        Controls.prototype.getControlItemByName = function (name, container) {
            var template = YetaWF.ComponentBaseDataImpl.getTemplateFromControlName(name, [container]);
            return this.getControlItemFromTemplate(template);
        };
        Controls.prototype.getControlItemFromTemplate = function (template) {
            var templateDef = YetaWF.ComponentBaseDataImpl.getTemplateDefinitionFromTemplate(template);
            var controlDef = templateDef.UserData;
            return {
                ControlType: controlDef.ControlType,
                ChangeEvent: controlDef.ChangeEvent,
                GetValue: controlDef.GetValue,
                Enable: controlDef.Enable,
                TemplateName: templateDef.Template,
                Template: template
            };
        };
        Controls.prototype.getControlValue = function (item) {
            if (!item.GetValue)
                throw "Control template ".concat(item.TemplateName, " has no GetValue function");
            switch (item.ControlType) {
                default:
                    throw "Invalid control type ".concat(item.ControlType, " in getControlValue");
                case ControlTypeEnum.Input:
                case ControlTypeEnum.Select:
                case ControlTypeEnum.TextArea:
                case ControlTypeEnum.Div:
                case ControlTypeEnum.Hidden:
                    return item.GetValue(item.Template);
                case ControlTypeEnum.Template: {
                    var obj = $YetaWF.getObjectData(item.Template);
                    return item.GetValue(obj);
                }
            }
        };
        Controls.prototype.enableToggle = function (item, enable, clearOnDisable) {
            if (!item.Enable)
                throw "Control template ".concat(item.TemplateName, " has no Enable function");
            switch (item.ControlType) {
                default:
                    throw "Invalid control type ".concat(item.ControlType, " in enableToggle");
                case ControlTypeEnum.Input:
                case ControlTypeEnum.Select:
                case ControlTypeEnum.TextArea:
                case ControlTypeEnum.Div:
                case ControlTypeEnum.Hidden:
                    item.Enable(item.Template, enable, clearOnDisable);
                    break;
                case ControlTypeEnum.Template: {
                    var obj = $YetaWF.getObjectData(item.Template);
                    item.Enable(obj, enable, clearOnDisable);
                }
            }
        };
        return Controls;
    }());
    YetaWF_ComponentsHTML.Controls = Controls;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var ControlsHelper = new YetaWF_ComponentsHTML.Controls();

//# sourceMappingURL=Controls.js.map
