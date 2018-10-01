 /* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ModuleEdit {

    interface AllowedUsersSetup {
        GridId: string;
        GridAllId: string;
        AddUrl: string;
    }
    interface GridRecordResult {
        TR: string;
        StaticData: string;
    }

    export class AllowedUsersEditComponent extends YetaWF.ComponentBase<HTMLDivElement> {

        private Setup: AllowedUsersSetup;
        private Grid: Softelvdm_Grid.Grid;
        private GridAll: Softelvdm_Grid.Grid;
        private buttonAdd: HTMLInputElement;
        private inputUserName: HTMLInputElement;
        private ReloadInProgress: boolean = false;
        private AddCounter: number = 0;

        constructor(controlId: string, setup: AllowedUsersSetup) {
            super(controlId);
            this.Setup = setup;

            this.Grid = Softelvdm_Grid.Grid.getControlById(this.Setup.GridId);
            this.GridAll = Softelvdm_Grid.Grid.getControlById(this.Setup.GridAllId);
            this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [this.Control]) as HTMLInputElement;
            this.inputUserName = $YetaWF.getElement1BySelector("input[name$='.NewValue']", [this.Control]) as HTMLInputElement;

            $YetaWF.registerEventHandler(this.buttonAdd, "click", null, (ev: MouseEvent): boolean => {

                if (this.ReloadInProgress) return true;

                this.ReloadInProgress = true;
                $YetaWF.setLoading(true);

                var uri = $YetaWF.parseUrl(this.Setup.AddUrl);
                uri.addFormInfo(this.Control, ++this.AddCounter);
                uri.addSearch("newUser", this.inputUserName.value);
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
                        });
                    }
                };
                request.send(uri.toFormData());

                return false;
            });
            $YetaWF.handleInputReturnKeyForButton(this.inputUserName, this.buttonAdd);
            $YetaWF.registerMultipleEventHandlers(this.inputUserName, ["input", "change", "click", "keyup", "paste"], null, (ev: Event): boolean => { this.toggleButton(); return true; });

            this.GridAll.Control.addEventListener("grid_selectionchange", (evt: Event): void => {
                var index = this.GridAll.SelectedIndex();
                var td = $YetaWF.getElement1BySelector("td", [this.GridAll.GetTR(index)]);
                var name = td.innerText.trim();
                this.inputUserName.value = name;
                this.toggleButton();
            });
        }
        private toggleButton() : void {
            var s = this.inputUserName.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        }
    }
}

