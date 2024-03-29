/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

namespace YetaWF_Identity {

    interface ResourceUsersSetup {
        GridId: string;
        GridAllId: string;
        AddUrl: string;
    }
    interface GridRecordResult {
        TR: string;
        StaticData: string;
    }

    export class ResourceUsersEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_yetawf_identity_resourceusers";
        public static readonly SELECTOR: string = ".yt_yetawf_identity_resourceusers.t_edit";

        private Setup: ResourceUsersSetup;
        private Grid: YetaWF_ComponentsHTML.Grid;
        private GridAll: YetaWF_ComponentsHTML.Grid;
        private ButtonAdd: HTMLInputElement;
        private InputUserName: HTMLInputElement;
        private AddCounter: number = 0;

        constructor(controlId: string, setup: ResourceUsersSetup) {
            super(controlId, ResourceUsersEditComponent.TEMPLATE, ResourceUsersEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });

            this.Setup = setup;

            this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.GridAll = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridAllId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.ButtonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [this.Control]) as HTMLInputElement;
            this.InputUserName = $YetaWF.getElement1BySelector("input[name$='.NewValue']", [this.Control]) as HTMLInputElement;

            $YetaWF.registerEventHandler(this.ButtonAdd, "click", null, (ev: MouseEvent): boolean => {

                if ($YetaWF.isLoading) return true;

                var uri = $YetaWF.parseUrl(this.Setup.AddUrl);
                uri.addFormInfo(this.Control);
                let uniqueIdCounters: YetaWF.UniqueIdInfo = { UniqueIdPrefix: `${this.ControlId}ls`, UniqueIdPrefixCounter: 0, UniqueIdCounter: ++this.AddCounter };
                uri.addSearch(YConfigs.Forms.UniqueIdCounters, JSON.stringify(uniqueIdCounters));
                uri.addSearch("newUser", this.InputUserName.value);
                uri.addSearch("fieldPrefix", this.Grid.FieldName);
                uri.addSearch("data", JSON.stringify(this.Grid.StaticData));
                if (this.Grid.ExtraData) uri.addSearchSimpleObject(this.Grid.ExtraData);

                $YetaWF.post(this.Setup.AddUrl, uri.toFormData(), (success: boolean, partial: GridRecordResult): void => {
                    if (success) {
                        this.Grid.AddRecord(partial.TR, partial.StaticData);
                    }
                });

                return false;
            });
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
            $YetaWF.elementEnableToggle(this.ButtonAdd, s.length > 0);
        }
    }
}

