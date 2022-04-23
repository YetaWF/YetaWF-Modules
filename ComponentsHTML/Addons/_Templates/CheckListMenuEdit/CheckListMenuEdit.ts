/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface CheckListSetup {
        FieldName: string;
    }
    export interface ValueEntry {
        Name: string;
        Checked: boolean;
    }


    export class CheckListMenuEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_checklistmenu";
        public static readonly SELECTOR: string = ".yt_checklistmenu.t_edit";

        public static readonly EVENTCHANGE: string = "checklistmenu_change";

        //private Setup: CheckListSetup;
        private Button: HTMLButtonElement;
        private Menu: HTMLElement;

        constructor(controlId: string, setup: CheckListSetup) {
            super(controlId, CheckListMenuEditComponent.TEMPLATE, CheckListMenuEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            //this.Setup = setup;
            this.Button = $YetaWF.getElement1BySelector("button", [this.Control]) as HTMLButtonElement;
            this.Menu = $YetaWF.getElement1BySelector("ul", [this.Control]);

            $YetaWF.registerEventHandler(this.Button, "mousedown", null, (ev: MouseEvent): boolean => {
                if (!YetaWF_ComponentsHTML.MenuULComponent.closeMenus())
                    this.openMenu();
                return false;
            });
            $YetaWF.registerEventHandler(this.Button, "click", null, (ev: MouseEvent): boolean => {
                return false;
            });
        }

        private openMenu(): void {

            let menu = this.Menu.cloneNode(true) as HTMLElement;
            menu.id = `${this.Menu.id}_live`;
            // update checkboxes from hidden fields
            let lis = $YetaWF.getElementsBySelector("li", [menu]) as HTMLLIElement[];
            for (let li of lis) {
                let name = $YetaWF.getAttribute(li, "data-name");
                let input = $YetaWF.getElement1BySelector(`input[name$='.Value'][data-name='${name}']`, [this.Control]) as HTMLInputElement;
                let check = $YetaWF.getElement1BySelector(`li[data-name='${name}'] input[type="checkbox"]`, [menu]) as HTMLInputElement;
                check.checked = input.value === "True";
            }
            document.body.appendChild(menu);

            new YetaWF_ComponentsHTML.MenuULComponent(menu.id,
                {
                    "Owner": this.Menu,
                    "AutoOpen": true,
                    "AutoRemove": true,
                    "AttachTo": this.Button,
                    "Dynamic": true,
                    "RightAlign": true,
                    "CloseOnClick": false,
                    "Click": (liElem: HTMLLIElement, target?: HTMLElement|null): void => {
                        setTimeout((): void => {// checkboxes are weird
                            let check = $YetaWF.getElement1BySelector("input", [liElem]) as HTMLInputElement;
                            check.checked = !check.checked;
                            // update hidden field
                            let name = $YetaWF.getAttribute(liElem, "data-name");
                            let input = $YetaWF.getElement1BySelector(`input[name$='.Value'][data-name='${name}']`, [this.Control]) as HTMLInputElement;
                            input.value = check.checked ? "True" : "False";
                            this.sendChangeEvent();
                        }, 1);
                    }
                });
        }

        private sendChangeEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, CheckListMenuEditComponent.EVENTCHANGE);
        }

        // API

        public getValueEntries(): ValueEntry[] {
            let entries: ValueEntry[] = [];
            let lis = $YetaWF.getElementsBySelector("li", [this.Menu]) as HTMLLIElement[];
            for (let li of lis) {
                let name = $YetaWF.getAttribute(li, "data-name");
                let input = $YetaWF.getElement1BySelector(`input[name$='.Value'][data-name='${name}']`, [this.Control]) as HTMLInputElement;
                entries.push({Name: name, Checked: input.value === "True" });
            }
            return entries;
        }

        public replaceValues(values: boolean[]): void {

            let lis = $YetaWF.getElementsBySelector("li", [this.Menu]) as HTMLLIElement[];
            if (lis.length !== values.length)
                throw `replaceValues expected ${lis.length} values, received ${values.length}`;

            let valIndex = 0;
            for (let li of lis) {
                let name = $YetaWF.getAttribute(li, "data-name");
                let input = $YetaWF.getElement1BySelector(`input[name$='.Value'][data-name='${name}']`, [this.Control]) as HTMLInputElement;
                input.value = values[valIndex] ? "True" : "False";
                ++valIndex;
            }
        }
    }
}

