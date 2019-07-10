/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

namespace YetaWF_Identity {

    interface ListOfUserNamesSetup {
        GridId: string;
        GridAllId: string;
        AddUrl: string;
    }
    interface GridRecordResult {
        TR: string;
        StaticData: string;
    }

    export class ListOfUserNamesEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_yetawf_identity_listofusernames";
        public static readonly SELECTOR: string = ".yt_yetawf_identity_listofusernames.t_edit";

        private Setup: ListOfUserNamesSetup;
        private Grid: YetaWF_ComponentsHTML.Grid;
        private GridAll: YetaWF_ComponentsHTML.Grid;
        private buttonAdd: HTMLInputElement;
        private InputUserName: HTMLInputElement;
        private ReloadInProgress: boolean = false;
        private AddCounter: number = 0;

        constructor(controlId: string, setup: ListOfUserNamesSetup) {
            super(controlId, ListOfUserNamesEditComponent.TEMPLATE, ListOfUserNamesEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            this.Setup = setup;

            this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.GridAll = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridAllId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [this.Control]) as HTMLInputElement;
            this.InputUserName = $YetaWF.getElement1BySelector("input[name$='.NewValue']", [this.Control]) as HTMLInputElement;

            $YetaWF.registerEventHandler(this.buttonAdd, "click", null, (ev: MouseEvent): boolean => {

                if (this.ReloadInProgress) return true;

                this.ReloadInProgress = true;
                $YetaWF.setLoading(true);

                var uri = $YetaWF.parseUrl(this.Setup.AddUrl);
                uri.addFormInfo(this.Control);
                let uniqueIdCounters: YetaWF.UniqueIdInfo = { UniqueIdPrefix: `${this.ControlId}ls`, UniqueIdPrefixCounter: 0, UniqueIdCounter: ++this.AddCounter };
                uri.addSearch(YConfigs.Forms.UniqueIdCounters, JSON.stringify(uniqueIdCounters));
                uri.addSearch("newUser", this.InputUserName.value);
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
            $YetaWF.handleInputReturnKeyForButton(this.InputUserName, this.buttonAdd);
            $YetaWF.registerMultipleEventHandlers([this.InputUserName], ["input", "change", "click", "keyup", "paste"], null, (ev: Event): boolean => { this.toggleButton(); return true; });

            this.GridAll.Control.addEventListener("grid_selectionchange", (evt: Event): void => {
                var index = this.GridAll.SelectedIndex();
                if (index < 0) return;
                var td = $YetaWF.getElement1BySelector("td", [this.GridAll.GetTR(index)]);
                var name = td.innerText.trim();
                this.InputUserName.value = name;
                this.toggleButton();
            });
        }
        private toggleButton() : void {
            var s = this.InputUserName.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        }
    }
}

