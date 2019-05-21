/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

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
        private ReloadInProgress: boolean = false;
        private AddCounter: number = 0;

        constructor(controlId: string, setup: ListOfEmailAddressesSetup) {
            super(controlId, ListOfEmailAddressesEditComponent.TEMPLATE, ListOfEmailAddressesEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: "",//$$$$
                GetValue: (control: ListOfEmailAddressesEditComponent): string | null => {
                    return null;//$$$$control.value;
                },
                Enable: (control: ListOfEmailAddressesEditComponent, enable: boolean): void => {
                    //$$$control.enable(enable)
                },
            });

            this.Setup = setup;

            this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [this.Control]) as HTMLInputElement;
            this.inputEmail = $YetaWF.getElement1BySelector("input[name$='.NewValue']", [this.Control]) as HTMLInputElement;

            $YetaWF.registerEventHandler(this.buttonAdd, "click", null, (ev: MouseEvent): boolean => {

                if (this.ReloadInProgress) return true;

                this.ReloadInProgress = true;
                $YetaWF.setLoading(true);

                var uri = $YetaWF.parseUrl(this.Setup.AddUrl);
                uri.addFormInfo(this.Control, ++this.AddCounter);
                uri.addSearch("newEmailAddress", this.inputEmail.value.trim());
                uri.addSearch("fieldPrefix", this.Grid.FieldName);
                uri.addSearch("data", JSON.stringify(this.Grid.StaticData));
                if (this.Grid.ExtraData) uri.addSearchSimpleObject(this.Grid.ExtraData);
                var request: XMLHttpRequest = new XMLHttpRequest();
                request.open("POST", this.Setup.AddUrl, true);
                request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                request.onreadystatechange = (ev: Event): any => {
                    if (request.readyState === 4 /*DONE*/) {
                        this.ReloadInProgress = false;
                        $YetaWF.setLoading(false);
                        $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, undefined, undefined, (result: string) => {
                            var partial: GridRecordResult = JSON.parse(request.responseText);
                            this.ReloadInProgress = false;
                            $YetaWF.setLoading(false);
                            this.Grid.AddRecord(partial.TR, partial.StaticData);
                            this.inputEmail.value = "";
                        });
                    }
                };
                request.send(uri.toFormData());
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

