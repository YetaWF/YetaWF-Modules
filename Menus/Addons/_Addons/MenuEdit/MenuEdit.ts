/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

namespace YetaWF {
    export interface ILocs {
        YetaWF_Menus: YetaWF_Menus.IPackageLocs;
    }
}

namespace YetaWF_Menus {

    export interface IPackageLocs {
        ChangedEntry: string;
        NewEntry: string;
        NewEntryText: string;
    }

    enum MenuEntryType {
        Entry = 0,
        Parent = 1,
        Separator = 2
    }
    interface Data {
        EntryType: MenuEntryType;
        Separator: boolean;
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

            let treeTag = $YetaWF.getElement1BySelector(`#${this.Setup.TreeId} ${YetaWF_ComponentsHTML.TreeComponent.SELECTOR}`);
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

            $YetaWF.registerCustomEventHandler(this.Tree.Control, YetaWF_ComponentsHTML.TreeComponent.EVENTCLICK, null, (ev: CustomEvent): boolean => {
                this.changeSelection();
                return false;
            });
            this.Tree.Control.addEventListener(YetaWF_ComponentsHTML.TreeComponent.EVENTSELECT, (evt: Event): void => {
                this.changeSelection();
            });
            this.Tree.Control.addEventListener(YetaWF_ComponentsHTML.TreeComponent.EVENTDROP, (evt: Event): void => {
                this.sendEntireMenu();
            });

            // After submit (by Save button), submit the entire menu
            let form = $YetaWF.Forms.getForm(this.Details);
            $YetaWF.registerCustomEventHandler(form, YetaWF.Forms.EVENTPOSTSUBMIT, null, (ev: CustomEvent<YetaWF.DetailsPostSubmit>): boolean => {
                if (ev.detail.success) {
                    this.getFormControls();

                    this.saveFields();
                    this.Tree.setSelectText(this.MenuText.defaultValue);
                    this.ActiveNew = false;

                    this.sendEntireMenu();
                    this.update();
                }
                return true;
            });

            $YetaWF.registerEventHandler(this.AddButton, "click", null, (ev: MouseEvent): boolean => {
                if (this.ActiveEntry && this.changeSelection()) {
                    let li = this.Tree.addEntry(this.ActiveEntry, YLocs.YetaWF_Menus.NewEntryText, this.Setup.NewEntry as any as YetaWF_ComponentsHTML.TreeEntry);
                    this.Tree.setSelect(li);
                    this.ActiveEntry = this.Tree.getSelect();
                    this.ActiveData = this.Tree.getSelectData() as Data|null;
                    this.ActiveNew = true;
                    this.update();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.InsertButton, "click", null, (ev: MouseEvent): boolean => {
                if (this.ActiveEntry && this.changeSelection()) {
                    let li = this.Tree.insertEntry(this.ActiveEntry, YLocs.YetaWF_Menus.NewEntryText, this.Setup.NewEntry as any as YetaWF_ComponentsHTML.TreeEntry);
                    this.Tree.setSelect(li);
                    this.ActiveEntry = this.Tree.getSelect();
                    this.ActiveData = this.Tree.getSelectData() as Data | null;
                    this.ActiveNew = true;
                    this.update();
                }
                return false;
            });

            $YetaWF.registerEventHandler(this.SaveButton, "click", null, (ev: MouseEvent): boolean => {
                let form = $YetaWF.Forms.getForm(this.Details);
                $YetaWF.Forms.submit(form, true, "ValidateCurrent=true");
                return false;
            });
            $YetaWF.registerEventHandler(this.DeleteButton, "click", null, (ev: MouseEvent): boolean => {
                if (this.ActiveEntry) {
                    if (this.Tree.canCollapse(this.ActiveEntry))
                        this.Tree.collapse(this.ActiveEntry);
                    let next = this.Tree.getNextVisibleEntry(this.ActiveEntry);
                    if (!next)
                        next = this.Tree.getPrevVisibleEntry(this.ActiveEntry);
                    this.Tree.removeEntry(this.ActiveEntry);
                    if (next) {
                        this.Tree.setSelect(next);
                    } else {
                        this.Tree.clearSelect();
                    }
                    this.ActiveEntry = this.Tree.getSelect();
                    this.ActiveData = this.Tree.getSelectData() as Data | null;
                    this.ActiveNew = false;

                    this.sendEntireMenu();
                    this.update();
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
                    let li = this.Tree.getFirstVisibleItem();
                    if (li) {
                        // this.Tree.expand(li);
                        this.Tree.setSelect(li);
                        this.ActiveEntry = this.Tree.getSelect();
                        this.ActiveData = this.Tree.getSelectData() as Data | null;
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

            this.ActiveEntry = this.Tree.getSelect();
            this.ActiveData = this.Tree.getSelectData() as Data | null;
            this.ActiveNew = false;
            this.update();
        }

        private buildHierarchy(): DataHierarchy[] {
            let hierarchy: DataHierarchy[] = [];

            let liElem = this.Tree.getFirstVisibleItem();
            if (liElem)
                this.addHierarchyEntry(hierarchy, liElem);

            return hierarchy;
        }
        private addHierarchyEntry(hierarchy: DataHierarchy[], liElem: HTMLLIElement): void {

            for (; ;) {
                let data = this.Tree.getElementData(liElem) as any as Data;
                let h = data as DataHierarchy;
                h.SubMenu = [];
                hierarchy.push(h);

                // child elems
                let childLi = this.Tree.getFirstDirectChild(liElem);
                if (childLi)
                    this.addHierarchyEntry(h.SubMenu, childLi);

                let li = this.Tree.getNextSibling(liElem);
                if (!li)
                    break;
                liElem = li;
            }
        }

        private getFormControls(): void {
            this.EntryType = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModEntry.EntryType']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.Url = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.Url']", YetaWF_ComponentsHTML.UrlEditComponent.SELECTOR, [this.Details]);
            this.SubModule = YetaWF.ComponentBaseDataImpl.getControlFromSelector("[name='ModEntry.SubModule']", YetaWF_ComponentsHTML.ModuleSelectionEditComponent.SELECTOR, [this.Details]);
            this.MenuText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.MenuText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.LinkText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.LinkText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.ImageUrlFinal = $YetaWF.getElement1BySelector("input[name='ModEntry.ImageUrlFinal']", [this.Details]) as HTMLInputElement;
            this.Tooltip = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.Tooltip']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.Legend = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.Legend']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.Enabled = $YetaWF.getElement1BySelector("input[name='ModEntry.Enabled']", [this.Details]) as HTMLInputElement;
            this.CssClass = $YetaWF.getElement1BySelector("input[name='ModEntry.CssClass']", [this.Details]) as HTMLInputElement;
            this.Style = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModEntry.Style']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.Mode = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModEntry.Mode']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.Category = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModEntry.Category']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.LimitToRole = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModEntry.LimitToRole']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.AuthorizationIgnore = $YetaWF.getElement1BySelector("input[name='ModEntry.AuthorizationIgnore']", [this.Details]) as HTMLInputElement;
            this.ConfirmationText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.ConfirmationText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.PleaseWaitText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.PleaseWaitText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.SaveReturnUrl = $YetaWF.getElement1BySelector("input[name='ModEntry.SaveReturnUrl']", [this.Details]) as HTMLInputElement;
            this.AddToOriginList = $YetaWF.getElement1BySelector("input[name='ModEntry.AddToOriginList']", [this.Details]) as HTMLInputElement;
            this.NeedsModuleContext = $YetaWF.getElement1BySelector("input[name='ModEntry.NeedsModuleContext']", [this.Details]) as HTMLInputElement;
            this.DontFollow = $YetaWF.getElement1BySelector("input[name='ModEntry.DontFollow']", [this.Details]) as HTMLInputElement;

            $YetaWF.registerCustomEventHandler(this.EntryType.Control, YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, null, (ev: Event): boolean => {
                if (this.ActiveData) {
                    let data = this.ActiveData;
                    data.EntryType = Number(this.EntryType.value);
                    switch (data.EntryType) {
                        case MenuEntryType.Entry:
                            data.Separator = false;
                            break;
                        case MenuEntryType.Separator:
                            data.Url = "";
                            data.SubModule = "";
                            data.Separator = true;
                            break;
                        case MenuEntryType.Parent:
                            data.Url = "";
                            data.SubModule = "";
                            data.Separator = false;
                            break;
                    }
                    this.ActiveData = data;
                    this.update();
                }
                return false;
            });
        }

        private changeSelection(): boolean {
            if (this.ActiveNew) {
                if (this.ActiveEntry)// restore original selection
                    this.Tree.setSelect(this.ActiveEntry);
                $YetaWF.error(YLocs.YetaWF_Menus.NewEntry);
                return false;
            }
            if (this.hasChanged()) {
                if (this.ActiveEntry)// restore original selection
                    this.Tree.setSelect(this.ActiveEntry);
                $YetaWF.error(YLocs.YetaWF_Menus.ChangedEntry);
                return false;
            }
            // set new selection
            if (this.ActiveEntry !== this.Tree.getSelect()) {
                this.ActiveEntry = this.Tree.getSelect();
                this.ActiveData = this.Tree.getSelectData() as any as Data;
                this.ActiveNew = false;
                this.update();
            }
            return true;
        }

        private hasChanged(): boolean {
            if (this.ActiveEntry && this.ActiveData) {
                let data = this.ActiveData;
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
            let data = this.ActiveData;
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
            this.Tree.setSelectData(data as any as YetaWF_ComponentsHTML.TreeEntry);
        }

        private update(): void {
            let data = this.ActiveData;
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
            let enable = (this.ActiveEntry && this.ActiveData) != null;
            $YetaWF.elementEnableToggle(this.SaveButton, enable);
            $YetaWF.elementEnableToggle(this.DeleteButton, enable);
            $YetaWF.elementEnableToggle(this.ResetButton, enable);
            $YetaWF.elementEnableToggle(this.AddButton, enable);
            $YetaWF.elementEnableToggle(this.InsertButton, enable);
        }

        private sendEntireMenu(): void {

            if ($YetaWF.isLoading) return;
            let form = $YetaWF.Forms.getForm(this.Details);
            let menuVersionInput = $YetaWF.getElement1BySelector("input[name='MenuVersion']", [form]) as HTMLInputElement;
            let menuVersion = menuVersionInput.value;
            let menuGuidInput = $YetaWF.getElement1BySelector("input[name='MenuGuid']", [form]) as HTMLInputElement;
            let menuGuid = menuGuidInput.value;

            let uri = $YetaWF.parseUrl(this.Setup.AjaxUrl);
            uri.addFormInfo(form);
            uri.addSearch("menuGuid", menuGuid);
            uri.addSearch("menuVersion", menuVersion);
            uri.addSearch("EntireMenu", JSON.stringify(this.buildHierarchy()));

            $YetaWF.post(this.Setup.AjaxUrl, uri.toFormData(), (success: boolean, sendResult: SendResult): void => {
                menuVersionInput.value = sendResult.NewVersion.toString();
            });
        }
    }
}
