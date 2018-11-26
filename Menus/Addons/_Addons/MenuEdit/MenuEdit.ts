/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

namespace YetaWF {
    export interface ILocs {
        YetaWF_Menus: YetaWF_Menus.IPackageLocs;
    }
}

namespace YetaWF_Menus {

    export interface IPackageLocs {
        ChangedEntry: string;
        NewEntry: string;
    }

    enum MenuEntryType {
        Entry = 0,
        Parent = 1,
        Separator = 2
    }
    interface Data {
        EntryType: MenuEntryType;
        Text: string;
        Url: string;
        SubModule: string;
        MenuText: object;
        LinkText: object;
        ImageUrlFinal: string;
        Tooltip: object;
        Legend: object;
        Enabled: boolean;
        CssClass: string;
        Style: number;
        Mode: number;
        Category: number;
        LimitToRole: number;
        AuthorizationIgnore: boolean;
        ConfirmationText: object;
        PleaseWaitText: object;
        SaveReturnUrl: boolean;
        AddToOriginList: boolean;
        NeedsModuleContext: boolean;
        DontFollow: boolean;
    }
    interface DataHierarchy extends Data {
        SubMenu: DataHierarchy[] | null;
    }
    interface SendResult {
        NewVersion: number;
    }

    interface Setup {
        TreeId: string;
        DetailsId: string;
        AjaxUrl: string;
        NewEntry: Data;
    }

    export class MenuEditView {

        private Setup: Setup;

        private Tree: YetaWF_ComponentsHTML.TreeComponent;
        private Details: HTMLDivElement;

        private ActiveEntry: HTMLLIElement | null;
        private ActiveData: Data | null;
        private ActiveNew: boolean = false;

        private EntryType!: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private Url!: YetaWF_ComponentsHTML.UrlEditComponent;
        private SubModule!: YetaWF_ComponentsHTML.ModuleSelectionEditComponent;
        private MenuText!: YetaWF_ComponentsHTML.MultiStringEditComponent;
        private LinkText!: YetaWF_ComponentsHTML.MultiStringEditComponent;
        private ImageUrlFinal!: HTMLInputElement;
        private Tooltip!: YetaWF_ComponentsHTML.MultiStringEditComponent;
        private Legend!: YetaWF_ComponentsHTML.MultiStringEditComponent;
        private Enabled!: HTMLInputElement;
        private CssClass!: HTMLInputElement;
        private Style!: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private Mode!: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private Category!: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private LimitToRole!: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private AuthorizationIgnore!: HTMLInputElement;
        private ConfirmationText!: YetaWF_ComponentsHTML.MultiStringEditComponent;
        private PleaseWaitText!: YetaWF_ComponentsHTML.MultiStringEditComponent;
        private SaveReturnUrl!: HTMLInputElement;
        private AddToOriginList!: HTMLInputElement;
        private NeedsModuleContext!: HTMLInputElement;
        private DontFollow!: HTMLInputElement;

        private SaveButton: HTMLInputElement;
        private ResetButton: HTMLInputElement;
        private AddButton: HTMLInputElement;
        private InsertButton: HTMLInputElement;
        private DeleteButton: HTMLInputElement;
        private ExpandAllButton: HTMLInputElement;
        private CollapseAllButton: HTMLInputElement;

        constructor(setup: Setup) {
            this.Setup = setup;

            var treeTag = $YetaWF.getElement1BySelector(`#${this.Setup.TreeId} ${YetaWF_ComponentsHTML.TreeComponent.SELECTOR}`);
            this.Tree = YetaWF.ComponentBaseDataImpl.getControlFromTag<YetaWF_ComponentsHTML.TreeComponent>(treeTag, YetaWF_ComponentsHTML.TreeComponent.SELECTOR);
            this.Details = $YetaWF.getElementById(this.Setup.DetailsId) as HTMLDivElement;

            this.getFormControls();

            this.SaveButton = $YetaWF.getElement1BySelector("input[name='t_submit']", [this.Details]) as HTMLInputElement;
            this.ResetButton = $YetaWF.getElement1BySelector("input[name='t_reset']", [this.Details]) as HTMLInputElement;
            this.AddButton = $YetaWF.getElement1BySelector("input[name='t_add']", [this.Details]) as HTMLInputElement;
            this.InsertButton = $YetaWF.getElement1BySelector("input[name='t_insert']", [this.Details]) as HTMLInputElement;
            this.DeleteButton = $YetaWF.getElement1BySelector("input[name='t_delete']", [this.Details]) as HTMLInputElement;
            this.ExpandAllButton = $YetaWF.getElement1BySelector("input[name='t_expandall']", [this.Details]) as HTMLInputElement;
            this.CollapseAllButton = $YetaWF.getElement1BySelector("input[name='t_collapseall']", [this.Details]) as HTMLInputElement;

            this.Tree.Control.addEventListener("tree_click", (evt: Event): void => {
                this.changeSelection();
            });
            this.Tree.Control.addEventListener("tree_select", (evt: Event): void => {
                this.changeSelection();
            });
            this.Tree.Control.addEventListener("tree_drop", (evt: Event): void => {
                this.sendEntireMenu();
            });

            $YetaWF.registerEventHandler(this.AddButton, "click", null, (ev: MouseEvent): boolean => {
                if (this.ActiveEntry && this.changeSelection()) {
                    var li = this.Tree.addEntry(this.ActiveEntry, this.Setup.NewEntry.Text, this.Setup.NewEntry);
                    this.Tree.setSelect(li);
                    this.ActiveEntry = this.Tree.getSelect();
                    this.ActiveData = this.Tree.getSelectData();
                    this.ActiveNew = true;
                    this.update();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.InsertButton, "click", null, (ev: MouseEvent): boolean => {
                if (this.ActiveEntry && this.changeSelection()) {
                    var li = this.Tree.insertEntry(this.ActiveEntry, this.Setup.NewEntry.Text, this.Setup.NewEntry);
                    this.Tree.setSelect(li);
                    this.ActiveEntry = this.Tree.getSelect();
                    this.ActiveData = this.Tree.getSelectData();
                    this.ActiveNew = true;
                    this.update();
                }
                return false;
            });

            $YetaWF.registerEventHandler(this.SaveButton, "click", null, (ev: MouseEvent): boolean => {
                var form = $YetaWF.Forms.getForm(this.Details);
                $YetaWF.Forms.submit(form, true, "ValidateCurrent=true", (hasErrors: boolean): void => {
                    this.getFormControls();
                    if (!hasErrors) {
                        this.saveFields();
                        this.Tree.setSelectText(this.MenuText.defaultValue);
                        this.update();

                        this.sendEntireMenu();
                    }
                });
                return false;
            });
            $YetaWF.registerEventHandler(this.DeleteButton, "click", null, (ev: MouseEvent): boolean => {
                if (this.ActiveEntry) {
                    if (this.Tree.canCollapse(this.ActiveEntry))
                        this.Tree.collapse(this.ActiveEntry);
                    var next = this.Tree.getNextVisibleEntry(this.ActiveEntry);
                    if (!next)
                        next = this.Tree.getPrevVisibleEntry(this.ActiveEntry);
                    this.Tree.removeEntry(this.ActiveEntry);
                    if (next) {
                        this.Tree.setSelect(next);
                    } else {
                        this.Tree.clearSelect();
                    }
                    this.ActiveEntry = this.Tree.getSelect();
                    this.ActiveData = this.Tree.getSelectData();
                    this.ActiveNew = false;
                    this.update();

                    this.sendEntireMenu();
                }
                return false;
            });

            $YetaWF.registerEventHandler(this.ExpandAllButton, "click", null, (ev: MouseEvent): boolean => {
                this.Tree.expandAll();
                return false;
            });
            $YetaWF.registerEventHandler(this.CollapseAllButton, "click", null, (ev: MouseEvent): boolean => {
                if (this.changeSelection()) {
                    this.Tree.collapseAll();
                    var li = this.Tree.getFirstVisibleItem();
                    if (li) {
                        // this.Tree.expand(li);
                        this.Tree.setSelect(li);
                        this.ActiveEntry = this.Tree.getSelect();
                        this.ActiveData = this.Tree.getSelectData();
                        this.ActiveNew = false;
                    }
                    this.update();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.ResetButton, "click", null, (ev: MouseEvent): boolean => {
                this.update();
                return false;
            });

            this.EntryType.Control.addEventListener("dropdownlist_change", (ev: Event): void => {
                var data = this.Tree.getSelectData() as Data | null;
                if (data)
                    data.EntryType = Number(this.EntryType.value);
                this.update();
            });

            this.ActiveEntry = this.Tree.getSelect();
            this.ActiveData = this.Tree.getSelectData();
            this.ActiveNew = false;
            this.update();
        }

        private buildHierarchy(): DataHierarchy[] {
            var hierarchy: DataHierarchy[] = [];

            var liElem = this.Tree.getFirstVisibleItem();
            if (liElem)
                this.addHierarchyEntry(hierarchy, liElem);

            return hierarchy;
        }
        private addHierarchyEntry(hierarchy: DataHierarchy[], liElem: HTMLLIElement): void {

            for (; ;) {
                var data = this.Tree.getElementData(liElem);
                var h = data as DataHierarchy;
                h.SubMenu = [];
                hierarchy.push(h);

                // child elems
                var childLi = this.Tree.getFirstDirectChild(liElem);
                if (childLi)
                    this.addHierarchyEntry(h.SubMenu, childLi);

                var li = this.Tree.getNextSibling(liElem);
                if (!li)
                    break;
                liElem = li;
            }
        }

        private getFormControls(): void {
            this.EntryType = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModAction.EntryType']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.Url = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModAction.Url']", YetaWF_ComponentsHTML.UrlEditComponent.SELECTOR, [this.Details]);
            this.SubModule = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModAction.SubModule']", YetaWF_ComponentsHTML.ModuleSelectionEditComponent.SELECTOR, [this.Details]);
            this.MenuText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModAction.MenuText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.LinkText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModAction.LinkText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.ImageUrlFinal = $YetaWF.getElement1BySelector("input[name='ModAction.ImageUrlFinal']", [this.Details]) as HTMLInputElement;
            this.Tooltip = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModAction.Tooltip']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.Legend = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModAction.Legend']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.Enabled = $YetaWF.getElement1BySelector("input[name='ModAction.Enabled']", [this.Details]) as HTMLInputElement;
            this.CssClass = $YetaWF.getElement1BySelector("input[name='ModAction.CssClass']", [this.Details]) as HTMLInputElement;
            this.Style = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModAction.Style']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.Mode = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModAction.Mode']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.Category = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModAction.Category']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.LimitToRole = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModAction.LimitToRole']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.AuthorizationIgnore = $YetaWF.getElement1BySelector("input[name='ModAction.AuthorizationIgnore']", [this.Details]) as HTMLInputElement;
            this.ConfirmationText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModAction.ConfirmationText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.PleaseWaitText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModAction.PleaseWaitText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.SaveReturnUrl = $YetaWF.getElement1BySelector("input[name='ModAction.SaveReturnUrl']", [this.Details]) as HTMLInputElement;
            this.AddToOriginList = $YetaWF.getElement1BySelector("input[name='ModAction.AddToOriginList']", [this.Details]) as HTMLInputElement;
            this.NeedsModuleContext = $YetaWF.getElement1BySelector("input[name='ModAction.NeedsModuleContext']", [this.Details]) as HTMLInputElement;
            this.DontFollow = $YetaWF.getElement1BySelector("input[name='ModAction.DontFollow']", [this.Details]) as HTMLInputElement;
        }

        private changeSelection(): boolean {
            if (this.ActiveNew) {
                if (this.ActiveEntry)// restore original selection
                    this.Tree.setSelect(this.ActiveEntry);
                $YetaWF.alert(YLocs.YetaWF_Menus.NewEntry);
                return false;
            }
            if (this.hasChanged()) {
                if (this.ActiveEntry)// restore original selection
                    this.Tree.setSelect(this.ActiveEntry);
                $YetaWF.alert(YLocs.YetaWF_Menus.ChangedEntry);
                return false;
            }
            // set new selection
            if (this.ActiveEntry !== this.Tree.getSelect()) {
                this.ActiveEntry = this.Tree.getSelect();
                this.ActiveData = this.Tree.getSelectData();
                this.ActiveNew = false;
                this.update();
            }
            return true;
        }

        private hasChanged(): boolean {
            if (this.ActiveEntry && this.ActiveData) {
                var data = this.ActiveData;
                if (Number(this.EntryType.value) !== data.EntryType) return true;
                if (!$YetaWF.stringCompare(this.Url.value, data.Url)) return true;
                if (!$YetaWF.stringCompare(this.SubModule.value, data.SubModule)) return true;
                if (this.MenuText.hasChanged(data.MenuText)) return true;
                if (this.LinkText.hasChanged(data.LinkText)) return true;
                if (!$YetaWF.stringCompare(this.ImageUrlFinal.value, data.ImageUrlFinal)) return true;
                if (this.Tooltip.hasChanged(data.Tooltip)) return true;
                if (this.Legend.hasChanged(data.Legend)) return true;
                if (this.Enabled.checked !== data.Enabled) return true;
                if (!$YetaWF.stringCompare(this.CssClass.value, data.CssClass)) return true;
                if (Number(this.Style.value) !== data.Style) return true;
                if (Number(this.Mode.value) !== data.Mode) return true;
                if (Number(this.Category.value) !== data.Category) return true;
                if (Number(this.LimitToRole.value) !== data.LimitToRole) return true;
                if (this.AuthorizationIgnore.checked !== data.AuthorizationIgnore) return true;
                if (this.ConfirmationText.hasChanged(data.ConfirmationText)) return true;
                if (this.PleaseWaitText.hasChanged(data.PleaseWaitText)) return true;
                if (this.SaveReturnUrl.checked !== data.SaveReturnUrl) return true;
                if (this.AddToOriginList.checked !== data.AddToOriginList) return true;
                if (this.NeedsModuleContext.checked !== data.NeedsModuleContext) return true;
                if (this.DontFollow.checked !== data.DontFollow) return true;
            }
            return false;
        }
        private saveFields(): void {
            if (!this.ActiveData) return;
            var data = this.ActiveData;
            data.EntryType = Number(this.EntryType.value);
            data.Url = this.Url.value;
            data.SubModule = this.SubModule.value;
            data.MenuText = this.MenuText.value;
            data.LinkText = this.LinkText.value;
            data.ImageUrlFinal = this.ImageUrlFinal.value;
            data.Tooltip = this.Tooltip.value;
            data.Legend = this.Legend.value;
            data.Enabled = this.Enabled.checked;
            data.CssClass = this.CssClass.value;
            data.Style = Number(this.Style.value);
            data.Mode = Number(this.Mode.value);
            data.Category = Number(this.Category.value);
            data.LimitToRole = Number(this.LimitToRole.value);
            data.AuthorizationIgnore = this.AuthorizationIgnore.checked;
            data.ConfirmationText = this.ConfirmationText.value;
            data.PleaseWaitText = this.PleaseWaitText.value;
            data.SaveReturnUrl = this.SaveReturnUrl.checked;
            data.AddToOriginList = this.AddToOriginList.checked;
            data.NeedsModuleContext = this.NeedsModuleContext.checked;
            data.DontFollow = this.DontFollow.checked;
        }

        private update(): void {
            var data = this.Tree.getSelectData() as Data | null;
            if (data) {
                this.EntryType.value = data.EntryType.toString();
                this.Url.value = data.Url;
                this.SubModule.updateComplete(data.SubModule);
                this.MenuText.value = data.MenuText;
                this.LinkText.value = data.LinkText;
                this.ImageUrlFinal.value = data.ImageUrlFinal;
                this.Tooltip.value = data.Tooltip;
                this.Legend.value = data.Legend;
                this.Enabled.checked = data.Enabled;
                this.CssClass.value = data.CssClass;
                this.Style.value = data.Style.toString();
                this.Mode.value = data.Mode.toString();
                this.Category.value = data.Category.toString();
                this.LimitToRole.value = data.LimitToRole.toString();
                this.AuthorizationIgnore.checked = data.AuthorizationIgnore;
                this.ConfirmationText.value = data.ConfirmationText;
                this.PleaseWaitText.value = data.PleaseWaitText;
                this.SaveReturnUrl.checked = data.SaveReturnUrl;
                this.AddToOriginList.checked = data.AddToOriginList;
                this.NeedsModuleContext.checked = data.NeedsModuleContext;
                this.DontFollow.checked = data.DontFollow;
                this.enableFields(true);
            } else {
                this.EntryType.value = MenuEntryType.Entry.toString();
                this.Url.value = "";
                this.SubModule.clear();
                this.MenuText.clear();
                this.LinkText.clear();
                this.ImageUrlFinal.value = "";
                this.Tooltip.clear();
                this.Legend.clear();
                this.Enabled.checked = false;
                this.CssClass.value = "";
                this.Style.value = "";
                this.Mode.value = "";
                this.Category.value = "";
                this.LimitToRole.value = "";
                this.AuthorizationIgnore.checked = false;
                this.ConfirmationText.clear();
                this.PleaseWaitText.clear();
                this.SaveReturnUrl.checked = false;
                this.AddToOriginList.checked = false;
                this.NeedsModuleContext.checked = false;
                this.DontFollow.checked = false;
                this.enableFields(false);
            }
            this.updateButtons();
        }
        private enableFields(enable: boolean): void {
            if (enable) {
                this.EntryType.enable(true);
                switch (Number(this.EntryType.value)) {
                    case MenuEntryType.Entry:
                        this.Url.enable(true);
                        this.SubModule.enable(true);
                        this.MenuText.enable(true);
                        this.LinkText.enable(true);
                        $YetaWF.elementEnable(this.ImageUrlFinal);
                        this.Tooltip.enable(true);
                        this.Legend.enable(true);
                        $YetaWF.elementEnable(this.Enabled);
                        $YetaWF.elementEnable(this.CssClass);
                        this.Style.enable(true);
                        this.Mode.enable(true);
                        this.Category.enable(true);
                        this.LimitToRole.enable(true);
                        $YetaWF.elementEnable(this.AuthorizationIgnore);
                        this.ConfirmationText.enable(true);
                        this.PleaseWaitText.enable(true);
                        $YetaWF.elementEnable(this.SaveReturnUrl);
                        $YetaWF.elementEnable(this.AddToOriginList);
                        $YetaWF.elementEnable(this.NeedsModuleContext);
                        $YetaWF.elementEnable(this.DontFollow);
                        break;
                    case MenuEntryType.Parent:
                        this.Url.enable(false);
                        this.SubModule.enable(false);
                        this.MenuText.enable(true);
                        this.LinkText.enable(true);
                        $YetaWF.elementEnable(this.ImageUrlFinal);
                        this.Tooltip.enable(true);
                        this.Legend.enable(true);
                        $YetaWF.elementEnable(this.Enabled);
                        $YetaWF.elementEnable(this.CssClass);
                        this.Style.enable(true);
                        this.Mode.enable(true);
                        this.Category.enable(true);
                        this.LimitToRole.enable(true);
                        $YetaWF.elementDisable(this.AuthorizationIgnore);
                        this.ConfirmationText.enable(false);
                        this.PleaseWaitText.enable(false);
                        $YetaWF.elementDisable(this.SaveReturnUrl);
                        $YetaWF.elementDisable(this.AddToOriginList);
                        $YetaWF.elementDisable(this.NeedsModuleContext);
                        $YetaWF.elementDisable(this.DontFollow);
                        break;
                    case MenuEntryType.Separator:
                        this.Url.enable(false);
                        this.SubModule.enable(false);
                        this.MenuText.enable(false);
                        this.LinkText.enable(false);
                        $YetaWF.elementDisable(this.ImageUrlFinal);
                        this.Tooltip.enable(false);
                        this.Legend.enable(false);
                        $YetaWF.elementDisable(this.Enabled);
                        $YetaWF.elementDisable(this.CssClass);
                        this.Style.enable(false);
                        this.Mode.enable(false);
                        this.Category.enable(false);
                        this.LimitToRole.enable(false);
                        $YetaWF.elementDisable(this.AuthorizationIgnore);
                        this.ConfirmationText.enable(false);
                        this.PleaseWaitText.enable(false);
                        $YetaWF.elementDisable(this.SaveReturnUrl);
                        $YetaWF.elementDisable(this.AddToOriginList);
                        $YetaWF.elementDisable(this.NeedsModuleContext);
                        $YetaWF.elementDisable(this.DontFollow);
                        break;
                }
            } else {
                this.EntryType.enable(false);
                this.Url.enable(false);
                this.SubModule.enable(false);
                this.MenuText.enable(false);
                this.LinkText.enable(false);
                $YetaWF.elementDisable(this.ImageUrlFinal);
                this.Tooltip.enable(false);
                this.Legend.enable(false);
                $YetaWF.elementDisable(this.Enabled);
                $YetaWF.elementDisable(this.CssClass);
                this.Style.enable(false);
                this.Mode.enable(false);
                this.Category.enable(false);
                this.LimitToRole.enable(false);
                $YetaWF.elementDisable(this.AuthorizationIgnore);
                this.ConfirmationText.enable(false);
                this.PleaseWaitText.enable(false);
                $YetaWF.elementDisable(this.SaveReturnUrl);
                $YetaWF.elementDisable(this.AddToOriginList);
                $YetaWF.elementDisable(this.NeedsModuleContext);
                $YetaWF.elementDisable(this.DontFollow);
            }
        }
        private updateButtons(): void {
            // save, reset, delete, add new
            var enable = (this.ActiveEntry && this.ActiveData) != null;
            $YetaWF.elementEnableToggle(this.SaveButton, enable);
            $YetaWF.elementEnableToggle(this.DeleteButton, enable);
            $YetaWF.elementEnableToggle(this.ResetButton, enable);
            $YetaWF.elementEnableToggle(this.AddButton, enable);
            $YetaWF.elementEnableToggle(this.InsertButton, enable);
        }

        private sendEntireMenu(): void {

            if ($YetaWF.isLoading) return;
            var form = $YetaWF.Forms.getForm(this.Details);
            var menuVersionInput = $YetaWF.getElement1BySelector("input[name='MenuVersion']", [form]) as HTMLInputElement;
            var menuVersion = menuVersionInput.value;
            var menuGuidInput = $YetaWF.getElement1BySelector("input[name='MenuGuid']", [form]) as HTMLInputElement;
            var menuGuid = menuGuidInput.value;

            var uri = $YetaWF.parseUrl(this.Setup.AjaxUrl);
            uri.addFormInfo(form);
            uri.addSearch("menuGuid", menuGuid);
            uri.addSearch("menuVersion", menuVersion);
            uri.addSearch("EntireMenu", JSON.stringify(this.buildHierarchy()));

            $YetaWF.setLoading(true);

            var request: XMLHttpRequest = new XMLHttpRequest();
            request.open("POST", this.Setup.AjaxUrl);
            request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            request.onreadystatechange = (ev: Event): any => {
                if (request.readyState === 4 /*DONE*/) {
                    $YetaWF.setLoading(false);
                    $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, undefined, undefined, (result: string):void => {
                        var sendResult: SendResult = JSON.parse(result);
                        menuVersionInput.value = sendResult.NewVersion.toString();
                    });
                }
            };
            var data = uri.toFormData();
            request.send(data);
        }
    }
}
