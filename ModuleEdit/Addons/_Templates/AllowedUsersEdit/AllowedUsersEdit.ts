/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

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

    export class AllowedUsersEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_yetawf_moduleedit_allowedusers";
        public static readonly SELECTOR: string = ".yt_yetawf_moduleedit_allowedusers.t_edit";

        private Setup: AllowedUsersSetup;
        private Grid: YetaWF_ComponentsHTML.Grid;
        private GridAll: YetaWF_ComponentsHTML.Grid;
        private buttonAdd: HTMLInputElement;
        private inputUserName: HTMLInputElement;
        private AddCounter: number = 0;

        constructor(controlId: string, setup: AllowedUsersSetup) {
            super(controlId, AllowedUsersEditComponent.TEMPLATE, AllowedUsersEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });

            this.Setup = setup;

            this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.GridAll = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridAllId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [this.Control]) as HTMLInputElement;
            this.inputUserName = $YetaWF.getElement1BySelector("input[name$='.NewValue']", [this.Control]) as HTMLInputElement;

            $YetaWF.registerEventHandler(this.buttonAdd, "click", null, (ev: MouseEvent): boolean => {

                if ($YetaWF.isLoading) return true;

                var uri = $YetaWF.parseUrl(this.Setup.AddUrl);
                const query = {
                    NewUser: this.inputUserName.value,
                    FieldPrefix: this.Grid.FieldName,
                };
                const formJson = $YetaWF.Forms.getJSONInfo(this.Control, { UniqueIdPrefix: `${this.ControlId}ls`, UniqueIdPrefixCounter: 0, UniqueIdCounter: ++this.AddCounter });
                const data : YetaWF_ComponentsHTML.GridAdditionPartialViewData = {
                    GridData: this.Grid.StaticData,
                    __ModuleGuid: formJson.ModuleGuid,
                    __UniqueIdCounters: formJson.UniqueIdCounters!,
                    __RequestVerificationToken: formJson.RequestVerificationToken,
                };  
                if (this.Grid.ExtraData)
                    uri.addSearchSimpleObject(this.Grid.ExtraData);

                $YetaWF.postJSON(uri, formJson, query, data, (success: boolean, partial: GridRecordResult): void => {
                    if (success) {
                        this.Grid.AddRecord(partial.TR, partial.StaticData);
                        this.inputUserName.value = "";
                        this.toggleButton();
                    }
                });
                return false;
            });
            $YetaWF.handleInputReturnKeyForButton(this.inputUserName, this.buttonAdd);
            $YetaWF.registerMultipleEventHandlers([this.inputUserName], ["input", "change", "click", "keyup", "paste"], null, (ev: Event): boolean => { this.toggleButton(); return true; });

            this.GridAll.Control.addEventListener(YetaWF_ComponentsHTML.Grid.EVENTSELECT, (evt: Event): void => {
                var index = this.GridAll.SelectedIndex();
                if (index < 0) return;
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

