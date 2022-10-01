/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

namespace Softelvdm_IVR {

    interface ListOfPhoneNumbersSetup {
        GridId: string;
        AddUrl: string;
    }
    interface GridRecordResult {
        TR: string;
        StaticData: string;
    }

    export class ListOfPhoneNumbersEditComponent extends YetaWF.ComponentBaseNoDataImpl {

        public static readonly TEMPLATE: string = "yt_softelvdm_ivr_listofphonenumbers";
        public static readonly SELECTOR: string = ".yt_softelvdm_ivr_listofphonenumbers.t_edit";

        private Setup: ListOfPhoneNumbersSetup;
        private Grid: YetaWF_ComponentsHTML.Grid;
        private buttonAdd: HTMLInputElement;
        private inputPhoneNumber: HTMLInputElement;
        private AddCounter: number = 0;

        constructor(controlId: string, setup: ListOfPhoneNumbersSetup) {
            super(controlId, ListOfPhoneNumbersEditComponent.TEMPLATE, ListOfPhoneNumbersEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            this.Setup = setup;

            this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [this.Control]) as HTMLInputElement;
            this.inputPhoneNumber = $YetaWF.getElement1BySelector("input[name$='.NewPhoneNumber']", [this.Control]) as HTMLInputElement;

            $YetaWF.registerEventHandler(this.buttonAdd, "click", null, (ev: MouseEvent): boolean => {

                if ($YetaWF.isLoading) return true;

                var uri = $YetaWF.parseUrl(this.Setup.AddUrl);
                uri.addFormInfo(this.Control);
                let uniqueIdCounters: YetaWF.UniqueIdInfo = { UniqueIdPrefix: `${this.ControlId}ls`, UniqueIdPrefixCounter: 0, UniqueIdCounter: ++this.AddCounter };
                uri.addSearch(YConfigs.Forms.UniqueIdCounters, JSON.stringify(uniqueIdCounters));
                uri.addSearch("newPhoneNumber", this.inputPhoneNumber.value);
                uri.addSearch("fieldPrefix", this.Grid.FieldName);
                uri.addSearch("data", JSON.stringify(this.Grid.StaticData));
                if (this.Grid.ExtraData) uri.addSearchSimpleObject(this.Grid.ExtraData);

                $YetaWF.post(this.Setup.AddUrl, uri.toFormData(), (success: boolean, partial: GridRecordResult): void => {
                    if (success) {
                        this.Grid.AddRecord(partial.TR, partial.StaticData);
                        this.inputPhoneNumber.value = "";
                        this.toggleButton();
                    }
                });
                return false;
            });
            $YetaWF.handleInputReturnKeyForButton(this.inputPhoneNumber, this.buttonAdd);
            $YetaWF.registerMultipleEventHandlers([this.inputPhoneNumber], ["input", "change", "click", "keyup", "paste"], null, (ev: Event): boolean => { this.toggleButton(); return true; });
        }
        private toggleButton() : void {
            var s = this.inputPhoneNumber.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        }
    }
}

