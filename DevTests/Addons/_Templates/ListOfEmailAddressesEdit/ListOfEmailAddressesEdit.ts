/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

namespace YetaWF_DevTests {

    interface ListOfEmailAddressesSetup {
        GridId: string;
        AddUrl: string;
    }
    interface GridRecordResult {
        TR: string;
        StaticData: string;
    }

    export class ListOfEmailAddressesEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_yetawf_devtests_listofemailaddresses";
        public static readonly SELECTOR: string = ".yt_yetawf_devtests_listofemailaddresses.t_edit";

        private Setup: ListOfEmailAddressesSetup;
        private Grid: YetaWF_ComponentsHTML.Grid;
        private buttonAdd: HTMLInputElement;
        private inputEmail: HTMLInputElement;
        private AddCounter: number = 0;

        constructor(controlId: string, setup: ListOfEmailAddressesSetup) {
            super(controlId, ListOfEmailAddressesEditComponent.TEMPLATE, ListOfEmailAddressesEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });

            this.Setup = setup;

            this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [this.Control]) as HTMLInputElement;
            this.inputEmail = $YetaWF.getElement1BySelector("input[name$='.NewValue']", [this.Control]) as HTMLInputElement;

            $YetaWF.registerEventHandler(this.buttonAdd, "click", null, (ev: MouseEvent): boolean => {

                if ($YetaWF.isLoading) return true;

                var uri = $YetaWF.parseUrl(this.Setup.AddUrl);
                const query = {
                    NewEmailAddress: this.inputEmail.value.trim(),
                    FieldPrefix: this.Grid.FieldName,
                };

                let data = $YetaWF.Forms.getJSONInfo(this.Control);
                data.GridData = this.Grid.StaticData;
                data[YConfigs.Forms.UniqueIdCounters] = { UniqueIdPrefix: `${this.ControlId}ls`, UniqueIdPrefixCounter: 0, UniqueIdCounter: ++this.AddCounter };

                $YetaWF.postJSON(uri, query, data, (success: boolean, partial: GridRecordResult): void => {
                    if (success) {
                        this.Grid.AddRecord(partial.TR, partial.StaticData);
                        this.inputEmail.value = "";
                    }
                });
                return false;
            });
            $YetaWF.handleInputReturnKeyForButton(this.inputEmail, this.buttonAdd);
            $YetaWF.registerMultipleEventHandlers([this.inputEmail], ["input", "change", "click", "keyup", "paste"], null, (ev: Event): boolean => { this.toggleButton(); return true; });
        }
        private toggleButton() : void {
            var s = this.inputEmail.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        }
    }
}

