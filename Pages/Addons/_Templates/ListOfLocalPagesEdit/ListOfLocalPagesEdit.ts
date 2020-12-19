/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

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
        private ReloadInProgress: boolean = false;
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

                if (this.ReloadInProgress) return true;

                this.ReloadInProgress = true;
                $YetaWF.setLoading(true);

                var uri = $YetaWF.parseUrl(this.Setup.AddUrl);
                uri.addFormInfo(this.Control);
                let uniqueIdCounters: YetaWF.UniqueIdInfo = { UniqueIdPrefix: `${this.ControlId}ls`, UniqueIdPrefixCounter: 0, UniqueIdCounter: ++this.AddCounter };
                uri.addSearch(YConfigs.Forms.UniqueIdCounters, JSON.stringify(uniqueIdCounters));
                uri.addSearch("newUrl", this.SelectUrl.value.trim());
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
                        $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, undefined, undefined, (result: string): void => {
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

            this.SelectUrl.Control.addEventListener("url_change", (evt: Event): void => {
                this.toggleButton();
            });
            this.GridAll.Control.addEventListener("grid_selectionchange", (evt: Event): void => {
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

