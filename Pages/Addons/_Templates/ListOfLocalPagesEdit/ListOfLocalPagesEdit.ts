/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

namespace YetaWF_Pages {

    interface ListOfLocalPagesSetup {
        GridId: string;
        GridAllId: string;
        AddUrl: string;
    }
    interface GridRecordResult {
        TR: string;
        StaticData: string;
    }

    export class ListOfLocalPagesEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_yetawf_pages_listoflocalpages";
        public static readonly SELECTOR: string = ".yt_yetawf_pages_listoflocalpages.t_edit";

        private Setup: ListOfLocalPagesSetup;
        private Grid: YetaWF_ComponentsHTML.Grid;
        private GridAll: YetaWF_ComponentsHTML.Grid;
        private buttonAdd: HTMLInputElement;
        private SelectUrl: YetaWF_ComponentsHTML.UrlEditComponent;
        private AddCounter: number = 0;

        constructor(controlId: string, setup: ListOfLocalPagesSetup) {
            super(controlId, ListOfLocalPagesEditComponent.TEMPLATE, ListOfLocalPagesEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            this.Setup = setup;

            this.Grid = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.GridAll = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridAllId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            this.buttonAdd = $YetaWF.getElement1BySelector("input[name='btnAdd']", [this.Control]) as HTMLInputElement;
            this.SelectUrl = YetaWF.ComponentBaseDataImpl.getControlFromSelector("[name$='.NewValue']", YetaWF_ComponentsHTML.UrlEditComponent.SELECTOR, [this.Control]);

            $YetaWF.registerEventHandler(this.buttonAdd, "click", null, (ev: MouseEvent): boolean => {

                if ($YetaWF.isLoading) return true;

                var uri = $YetaWF.parseUrl(this.Setup.AddUrl);
                const query = {
                    NewUrl: this.SelectUrl.value.trim(),
                    FieldPrefix: this.Grid.FieldName,
                };
                const formJson =  $YetaWF.Forms.getJSONInfo(this.Control, { UniqueIdPrefix: `${this.ControlId}ls`, UniqueIdPrefixCounter: 0, UniqueIdCounter: ++this.AddCounter });
                const data : YetaWF_ComponentsHTML.GridAdditionPartialViewData = {
                    GridData: this.Grid.StaticData,
                    __ModuleGuid: formJson.ModuleGuid,
                    __UniqueIdCounters: formJson.UniqueIdCounters,
                };
                $YetaWF.postJSON(uri, formJson, query, data, (success: boolean, partial: GridRecordResult): void => {
                    if (success)
                        this.Grid.AddRecord(partial.TR, partial.StaticData);
                });
                return false;
            });

            this.SelectUrl.Control.addEventListener(YetaWF_ComponentsHTML.UrlEditComponent.EVENTCHANGE, (evt: Event): void => {
                this.toggleButton();
            });
            this.GridAll.Control.addEventListener(YetaWF_ComponentsHTML.Grid.EVENTSELECT, (evt: Event): void => {
                var index = this.GridAll.SelectedIndex();
                if (index < 0) return;
                var td = $YetaWF.getElement1BySelector("td", [this.GridAll.GetTR(index)]);
                var url = td.innerText.trim();
                this.SelectUrl.value = url;
                this.toggleButton();
            });
        }
        private toggleButton() : void {
            var s = this.SelectUrl.value;
            s = s.trim();
            $YetaWF.elementEnableToggle(this.buttonAdd, s.length > 0);
        }
    }
}

