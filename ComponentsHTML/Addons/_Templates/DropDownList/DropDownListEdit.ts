/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface DropDownListEditSetup {
        ToolTips: string[] | null;
        AdjustWidth: boolean;
    }

    interface AjaxData {
        data: kendo.data.DataSource;
        tooltips: string[] | null;
    }

    export class DropDownListEditComponent extends YetaWF.ComponentBase<HTMLSelectElement> {

        public static readonly SELECTOR: string = "select.yt_dropdownlist_base.t_edit.t_kendo";

        private Setup: DropDownListEditSetup;
        private KendoDropDownList: kendo.ui.DropDownList | null = null;

        constructor(controlId: string, setup: DropDownListEditSetup) {
            super(controlId);
            this.Setup = setup;

            $YetaWF.addObjectDataById(controlId, this);

            this.updateWidth();
        }

        public updateWidth(): void {
            var w = this.Control.clientWidth;
            if (w > 0 && this.KendoDropDownList == null) {
                var thisObj = this;
                $(this.Control).kendoDropDownList({
                    autoWidth: true,
                    // tslint:disable-next-line:only-arrow-functions
                    change: function (): void {
                        var event = document.createEvent("Event");
                        event.initEvent("dropdownlist_change", true, true);
                        thisObj.Control.dispatchEvent(event);
                        FormsSupport.validateElement(thisObj.Control);
                    }
                });
                this.KendoDropDownList = $(this.Control).data("kendoDropDownList");

                if (this.Setup.AdjustWidth) {
                    var avgw = Number($YetaWF.getAttribute(this.Control, "data-charavgw"));
                    var container = $YetaWF.elementClosest(this.Control, ".k-widget.yt_dropdownlist,.k-widget.yt_dropdownlist_base,.k-widget.yt_enum");
                    $(container).width(w + 3 * avgw);
                }
            }
        }

        get value(): string {
            return this.Control.value;
        }
        set value(val: string) {
            if (this.KendoDropDownList == null) {
                this.Control.value = val;
            } else {
                this.KendoDropDownList.value(val);
                if (this.KendoDropDownList.select() < 0)
                    this.KendoDropDownList.select(0);
            }
        }
        get selectedIndex(): number {
            if (this.KendoDropDownList == null) return -1;
            return this.KendoDropDownList.select();
        }

        // retrieve the tooltip for the nth item (index) in the dropdown list
        public getToolTip(index: number): string | null {
            if (!this.Setup.ToolTips) return null;
            if (index < 0 || index >= this.Setup.ToolTips.length) return null;
            return this.Setup.ToolTips[index];
        }
        public clear(): void {
            if (this.KendoDropDownList == null) {
                this.Control.selectedIndex = 0;
            } else {
                this.KendoDropDownList.select(0);
            }
        }
        public enable(enabled: boolean): void {
            if (this.KendoDropDownList == null) {
                $YetaWF.elementEnableToggle(this.Control, enabled);
            } else {
                this.KendoDropDownList.enable(enabled);
            }
        }
        public destroy(): void {
            if (this.KendoDropDownList)
                this.KendoDropDownList.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        }

        public ajaxUpdate(data: any, ajaxUrl: string, onSuccess?: (data: any) => void, onFailure?: (result: string) => void): void {

            var uri = $YetaWF.parseUrl(ajaxUrl);
            uri.addSearchSimpleObject(data);

            var request: XMLHttpRequest = new XMLHttpRequest();
            request.open("POST", uri.toUrl());
            request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            request.onreadystatechange = (ev: Event): any => {
                if (request.readyState === 4 /*DONE*/) {
                    $YetaWF.setLoading(false);
                    var retVal = $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, this.Control, undefined, undefined, (data: AjaxData): void => {

                        // $(this.Control).val(null);
                        var thisObj = this;
                        $(this.Control).kendoDropDownList({
                            dataTextField: "t",
                            dataValueField: "v",
                            dataSource: data.data,
                            autoWidth: true,
                            // tslint:disable-next-line:only-arrow-functions
                            change: function (): void {
                                var event = document.createEvent("Event");
                                event.initEvent("dropdownlist_change", true, true);
                                thisObj.Control.dispatchEvent(event);
                                FormsSupport.validateElement(thisObj.Control);
                            }
                        });
                        this.Setup.ToolTips = data.tooltips;
                        this.KendoDropDownList = $(this.Control).data("kendoDropDownList");

                        if (onSuccess) {
                            onSuccess(data);
                        } else {
                            this.value = "";
                            $(this.Control).trigger("change");
                            var event = document.createEvent("Event");
                            event.initEvent("dropdownlist_change", true, true);
                            this.Control.dispatchEvent(event);
                        }
                    });
                    if (!retVal) {
                        if (onFailure)
                            onFailure(request.responseText);
                        throw "Unexpected data returned";
                    }
                }
            };
            request.send();
        }

        public static getControlFromTag(elem: HTMLElement): DropDownListEditComponent { return super.getControlBaseFromTag<DropDownListEditComponent>(elem, DropDownListEditComponent.SELECTOR); }
        public static getControlFromSelector(selector: string | null, tags: HTMLElement[]): DropDownListEditComponent { return super.getControlBaseFromSelector<DropDownListEditComponent>(selector || DropDownListEditComponent.SELECTOR, DropDownListEditComponent.SELECTOR, tags); }
        public static getControlById(id: string): DropDownListEditComponent { return super.getControlBaseById<DropDownListEditComponent>(id, DropDownListEditComponent.SELECTOR); }
    }

    // We need to delay initialization until divs become visible so we can calculate the dropdown width
    $YetaWF.registerActivateDiv((tag: HTMLElement): void => {
        var ctls = $YetaWF.getElementsBySelector(DropDownListEditComponent.SELECTOR, [tag]);
        for (let ctl of ctls) {
            var control = DropDownListEditComponent.getControlFromTag(ctl);
            control.updateWidth();
        }
    });

    // A <div> is being emptied. Destroy all dropdownlists the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBase.clearDiv<DropDownListEditComponent>(tag, DropDownListEditComponent.SELECTOR, (control: DropDownListEditComponent): void => {
            control.destroy();
        });
    });

    // handle submit/apply
    $YetaWF.registerCustomEventHandlerDocument("dropdownlist_change", ".ysubmitonchange .k-dropdown select.yt_dropdownlist_base", (ev: Event): boolean => {
        $YetaWF.Forms.submitOnChange(ev.target as HTMLElement);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument("dropdownlist_change", ".yapplyonchange .k-dropdown select.yt_dropdownlist_base", (ev: Event): boolean => {
        $YetaWF.Forms.applyOnChange(ev.target as HTMLElement);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument("dropdownlist_change", ".yreloadonchange .k-dropdown select.yt_dropdownlist_base", (ev: Event): boolean => {
        $YetaWF.Forms.reloadOnChange(ev.target as HTMLElement);
        return false;
    });
}

