/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF {
    export interface IVolatile {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageVolatiles;
    }
    export interface IConfigs {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageConfigs;
    }
}
namespace YetaWF_ComponentsHTML {
    export interface IPackageVolatiles {
    }
    export interface IPackageConfigs {
    }
}

namespace YetaWF_ComponentsHTML {

    export interface ControlDefinition {
        ControlType: ControlTypeEnum;
        ChangeEvent: string | null;
        GetValue: (template: HTMLElement | YetaWF.ComponentBaseDataImpl) => string | null;
        Enable: (template: HTMLElement | YetaWF.ComponentBaseDataImpl, enable: boolean, clearOnDisable: boolean) => void | null;
    }
    export interface ControlItemDefinition extends ControlDefinition {
        TemplateName: string;
        Template: HTMLElement;
    }
    export enum ControlTypeEnum {
        Input = 0,
        Select = 1,
        TextArea = 2,
        Div = 3,

        Hidden = 10,

        Template = 20
    }

    export class Controls {

        public getControlItemByNameCond(name: string, container: HTMLElement): ControlItemDefinition | null {
            let template = YetaWF.ComponentBaseDataImpl.getTemplateFromControlNameCond(name, [container]);
            if (!template)
                return null;
            return this.getControlItemFromTemplate(template);
        }
        public getControlItemByName(name: string, container: HTMLElement): ControlItemDefinition {
            let template = YetaWF.ComponentBaseDataImpl.getTemplateFromControlName(name, [container]);
            return this.getControlItemFromTemplate(template);
        }
        public getControlItemFromTemplate(template: HTMLElement): ControlItemDefinition {
            let templateDef = YetaWF.ComponentBaseDataImpl.getTemplateDefinitionFromTemplate(template);
            let controlDef = templateDef.UserData as ControlDefinition;
            return {
                ControlType: controlDef.ControlType,
                ChangeEvent: controlDef.ChangeEvent,
                GetValue: controlDef.GetValue,
                Enable: controlDef.Enable,
                TemplateName: templateDef.Template,
                Template: template
            };
        }

        public getControlValue(item: ControlItemDefinition): string | null {
            if (!item.GetValue)
                throw `Control template ${item.TemplateName} has no GetValue function`;
            switch (item.ControlType) {
                default:
                    throw `Invalid control type ${item.ControlType} in getControlValue`;
                case ControlTypeEnum.Input:
                case ControlTypeEnum.Select:
                case ControlTypeEnum.TextArea:
                case ControlTypeEnum.Div:
                case ControlTypeEnum.Hidden:
                    return item.GetValue(item.Template);
                case ControlTypeEnum.Template: {
                    var obj = $YetaWF.getObjectData(item.Template) as YetaWF.ComponentBaseDataImpl;
                    return item.GetValue(obj);
                }
            }
        }
        public enableToggle(item: ControlItemDefinition, enable: boolean, clearOnDisable: boolean): void {
            if (!item.Enable)
                throw `Control template ${item.TemplateName} has no Enable function`;
            switch (item.ControlType) {
                default:
                    throw `Invalid control type ${item.ControlType} in enableToggle`;
                case ControlTypeEnum.Input:
                case ControlTypeEnum.Select:
                case ControlTypeEnum.TextArea:
                case ControlTypeEnum.Div:
                case ControlTypeEnum.Hidden:
                    item.Enable(item.Template, enable, clearOnDisable);
                    break;
                case ControlTypeEnum.Template: {
                    var obj = $YetaWF.getObjectData(item.Template) as YetaWF.ComponentBaseDataImpl;
                    item.Enable(obj, enable, clearOnDisable);
                }
            }
        }
    }
}

var ControlsHelper = new YetaWF_ComponentsHTML.Controls();