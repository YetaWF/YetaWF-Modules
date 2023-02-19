"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */
var YetaWF_Menus;
(function (YetaWF_Menus) {
    var MenuEntryType;
    (function (MenuEntryType) {
        MenuEntryType[MenuEntryType["Entry"] = 0] = "Entry";
        MenuEntryType[MenuEntryType["Parent"] = 1] = "Parent";
        MenuEntryType[MenuEntryType["Separator"] = 2] = "Separator";
    })(MenuEntryType || (MenuEntryType = {}));
    var MenuEditView = /** @class */ (function () {
        function MenuEditView(setup) {
            var _this = this;
            this.ActiveNew = false;
            this.Setup = setup;
            var treeTag = $YetaWF.getElement1BySelector("#".concat(this.Setup.TreeId, " ").concat(YetaWF_ComponentsHTML.TreeComponent.SELECTOR));
            this.Tree = YetaWF.ComponentBaseDataImpl.getControlFromTag(treeTag, YetaWF_ComponentsHTML.TreeComponent.SELECTOR);
            this.Details = $YetaWF.getElementById(this.Setup.DetailsId);
            this.getFormControls();
            this.SaveButton = $YetaWF.getElement1BySelector("input[name='t_submit']", [this.Details]);
            this.ResetButton = $YetaWF.getElement1BySelector("input[name='t_reset']", [this.Details]);
            this.AddButton = $YetaWF.getElement1BySelector("input[name='t_add']", [this.Details]);
            this.InsertButton = $YetaWF.getElement1BySelector("input[name='t_insert']", [this.Details]);
            this.DeleteButton = $YetaWF.getElement1BySelector("input[name='t_delete']", [this.Details]);
            this.ExpandAllButton = $YetaWF.getElement1BySelector("input[name='t_expandall']", [this.Details]);
            this.CollapseAllButton = $YetaWF.getElement1BySelector("input[name='t_collapseall']", [this.Details]);
            $YetaWF.registerCustomEventHandler(this.Tree.Control, YetaWF_ComponentsHTML.TreeComponent.EVENTCLICK, null, function (ev) {
                _this.changeSelection();
                return false;
            });
            this.Tree.Control.addEventListener(YetaWF_ComponentsHTML.TreeComponent.EVENTSELECT, function (evt) {
                _this.changeSelection();
            });
            this.Tree.Control.addEventListener(YetaWF_ComponentsHTML.TreeComponent.EVENTDROP, function (evt) {
                _this.sendEntireMenu();
            });
            // After submit (by Save button), submit the entire menu
            var form = $YetaWF.Forms.getForm(this.Details);
            $YetaWF.registerCustomEventHandler(form, YetaWF.Forms.EVENTPOSTSUBMIT, null, function (ev) {
                if (ev.detail.success) {
                    _this.getFormControls();
                    _this.saveFields();
                    _this.Tree.setSelectText(_this.MenuText.defaultValue);
                    _this.ActiveNew = false;
                    _this.sendEntireMenu();
                    _this.update();
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.AddButton, "click", null, function (ev) {
                if (_this.ActiveEntry && _this.changeSelection()) {
                    var li = _this.Tree.addEntry(_this.ActiveEntry, YLocs.YetaWF_Menus.NewEntryText, _this.Setup.NewEntry);
                    _this.Tree.setSelect(li);
                    _this.ActiveEntry = _this.Tree.getSelect();
                    _this.ActiveData = _this.Tree.getSelectData();
                    _this.ActiveNew = true;
                    _this.update();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.InsertButton, "click", null, function (ev) {
                if (_this.ActiveEntry && _this.changeSelection()) {
                    var li = _this.Tree.insertEntry(_this.ActiveEntry, YLocs.YetaWF_Menus.NewEntryText, _this.Setup.NewEntry);
                    _this.Tree.setSelect(li);
                    _this.ActiveEntry = _this.Tree.getSelect();
                    _this.ActiveData = _this.Tree.getSelectData();
                    _this.ActiveNew = true;
                    _this.update();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.SaveButton, "click", null, function (ev) {
                var form = $YetaWF.Forms.getForm(_this.Details);
                $YetaWF.Forms.submit(form, true, { ValidateCurrent: true });
                return false;
            });
            $YetaWF.registerEventHandler(this.DeleteButton, "click", null, function (ev) {
                if (_this.ActiveEntry) {
                    if (_this.Tree.canCollapse(_this.ActiveEntry))
                        _this.Tree.collapse(_this.ActiveEntry);
                    var next = _this.Tree.getNextVisibleEntry(_this.ActiveEntry);
                    if (!next)
                        next = _this.Tree.getPrevVisibleEntry(_this.ActiveEntry);
                    _this.Tree.removeEntry(_this.ActiveEntry);
                    if (next) {
                        _this.Tree.setSelect(next);
                    }
                    else {
                        _this.Tree.clearSelect();
                    }
                    _this.ActiveEntry = _this.Tree.getSelect();
                    _this.ActiveData = _this.Tree.getSelectData();
                    _this.ActiveNew = false;
                    _this.sendEntireMenu();
                    _this.update();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.ExpandAllButton, "click", null, function (ev) {
                _this.Tree.expandAll();
                return false;
            });
            $YetaWF.registerEventHandler(this.CollapseAllButton, "click", null, function (ev) {
                if (_this.changeSelection()) {
                    _this.Tree.collapseAll();
                    var li = _this.Tree.getFirstVisibleItem();
                    if (li) {
                        // this.Tree.expand(li);
                        _this.Tree.setSelect(li);
                        _this.ActiveEntry = _this.Tree.getSelect();
                        _this.ActiveData = _this.Tree.getSelectData();
                        _this.ActiveNew = false;
                    }
                    _this.update();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.ResetButton, "click", null, function (ev) {
                _this.update();
                return false;
            });
            this.ActiveEntry = this.Tree.getSelect();
            this.ActiveData = this.Tree.getSelectData();
            this.ActiveNew = false;
            this.update();
        }
        MenuEditView.prototype.buildHierarchy = function () {
            var hierarchy = [];
            var liElem = this.Tree.getFirstVisibleItem();
            if (liElem)
                this.addHierarchyEntry(hierarchy, liElem);
            return hierarchy;
        };
        MenuEditView.prototype.addHierarchyEntry = function (hierarchy, liElem) {
            for (;;) {
                var data = this.Tree.getElementData(liElem);
                var h = data;
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
        };
        MenuEditView.prototype.getFormControls = function () {
            var _this = this;
            this.EntryType = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModEntry.EntryType']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.Url = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.Url']", YetaWF_ComponentsHTML.UrlEditComponent.SELECTOR, [this.Details]);
            this.SubModule = YetaWF.ComponentBaseDataImpl.getControlFromSelector("[name='ModEntry.SubModule']", YetaWF_ComponentsHTML.ModuleSelectionEditComponent.SELECTOR, [this.Details]);
            this.MenuText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.MenuText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.LinkText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.LinkText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.ImageUrlFinal = $YetaWF.getElement1BySelector("input[name='ModEntry.ImageUrlFinal']", [this.Details]);
            this.Tooltip = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.Tooltip']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.Legend = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.Legend']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.Enabled = $YetaWF.getElement1BySelector("input[name='ModEntry.Enabled']", [this.Details]);
            this.CssClass = $YetaWF.getElement1BySelector("input[name='ModEntry.CssClass']", [this.Details]);
            this.Style = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModEntry.Style']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.Mode = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModEntry.Mode']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.Category = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModEntry.Category']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.LimitToRole = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name='ModEntry.LimitToRole']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Details]);
            this.AuthorizationIgnore = $YetaWF.getElement1BySelector("input[name='ModEntry.AuthorizationIgnore']", [this.Details]);
            this.ConfirmationText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.ConfirmationText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.PleaseWaitText = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='ModEntry.PleaseWaitText']", YetaWF_ComponentsHTML.MultiStringEditComponent.SELECTOR, [this.Details]);
            this.SaveReturnUrl = $YetaWF.getElement1BySelector("input[name='ModEntry.SaveReturnUrl']", [this.Details]);
            this.AddToOriginList = $YetaWF.getElement1BySelector("input[name='ModEntry.AddToOriginList']", [this.Details]);
            this.NeedsModuleContext = $YetaWF.getElement1BySelector("input[name='ModEntry.NeedsModuleContext']", [this.Details]);
            this.DontFollow = $YetaWF.getElement1BySelector("input[name='ModEntry.DontFollow']", [this.Details]);
            $YetaWF.registerCustomEventHandler(this.EntryType.Control, YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, null, function (ev) {
                if (_this.ActiveData) {
                    var data = _this.ActiveData;
                    data.EntryType = Number(_this.EntryType.value);
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
                    _this.ActiveData = data;
                    _this.update();
                }
                return false;
            });
        };
        MenuEditView.prototype.changeSelection = function () {
            if (this.ActiveNew) {
                if (this.ActiveEntry) // restore original selection
                    this.Tree.setSelect(this.ActiveEntry);
                $YetaWF.error(YLocs.YetaWF_Menus.NewEntry);
                return false;
            }
            if (this.hasChanged()) {
                if (this.ActiveEntry) // restore original selection
                    this.Tree.setSelect(this.ActiveEntry);
                $YetaWF.error(YLocs.YetaWF_Menus.ChangedEntry);
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
        };
        MenuEditView.prototype.hasChanged = function () {
            if (this.ActiveEntry && this.ActiveData) {
                var data = this.ActiveData;
                if (Number(this.EntryType.value) !== data.EntryType)
                    return true;
                if (!$YetaWF.stringCompare(this.Url.value, data.Url))
                    return true;
                if (!$YetaWF.stringCompare(this.SubModule.value, data.SubModule))
                    return true;
                if (this.MenuText.hasChanged(data.MenuText))
                    return true;
                if (this.LinkText.hasChanged(data.LinkText))
                    return true;
                if (!$YetaWF.stringCompare(this.ImageUrlFinal.value, data.ImageUrlFinal))
                    return true;
                if (this.Tooltip.hasChanged(data.Tooltip))
                    return true;
                if (this.Legend.hasChanged(data.Legend))
                    return true;
                if (this.Enabled.checked !== data.Enabled)
                    return true;
                if (!$YetaWF.stringCompare(this.CssClass.value, data.CssClass))
                    return true;
                if (Number(this.Style.value) !== data.Style)
                    return true;
                if (Number(this.Mode.value) !== data.Mode)
                    return true;
                if (Number(this.Category.value) !== data.Category)
                    return true;
                if (Number(this.LimitToRole.value) !== data.LimitToRole)
                    return true;
                if (this.AuthorizationIgnore.checked !== data.AuthorizationIgnore)
                    return true;
                if (this.ConfirmationText.hasChanged(data.ConfirmationText))
                    return true;
                if (this.PleaseWaitText.hasChanged(data.PleaseWaitText))
                    return true;
                if (this.SaveReturnUrl.checked !== data.SaveReturnUrl)
                    return true;
                if (this.AddToOriginList.checked !== data.AddToOriginList)
                    return true;
                if (this.NeedsModuleContext.checked !== data.NeedsModuleContext)
                    return true;
                if (this.DontFollow.checked !== data.DontFollow)
                    return true;
            }
            return false;
        };
        MenuEditView.prototype.saveFields = function () {
            if (!this.ActiveData)
                return;
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
            this.Tree.setSelectData(data);
        };
        MenuEditView.prototype.update = function () {
            var data = this.ActiveData;
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
            }
            else {
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
        };
        MenuEditView.prototype.enableFields = function (enable) {
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
            }
            else {
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
        };
        MenuEditView.prototype.updateButtons = function () {
            // save, reset, delete, add new
            var enable = (this.ActiveEntry && this.ActiveData) != null;
            $YetaWF.elementEnableToggle(this.SaveButton, enable);
            $YetaWF.elementEnableToggle(this.DeleteButton, enable);
            $YetaWF.elementEnableToggle(this.ResetButton, enable);
            $YetaWF.elementEnableToggle(this.AddButton, enable);
            $YetaWF.elementEnableToggle(this.InsertButton, enable);
        };
        MenuEditView.prototype.sendEntireMenu = function () {
            if ($YetaWF.isLoading)
                return;
            var form = $YetaWF.Forms.getForm(this.Details);
            var menuVersionInput = $YetaWF.getElement1BySelector("input[name='MenuVersion']", [form]);
            var menuVersion = menuVersionInput.value;
            var menuGuidInput = $YetaWF.getElement1BySelector("input[name='MenuGuid']", [form]);
            var menuGuid = menuGuidInput.value;
            var uri = $YetaWF.parseUrl(this.Setup.AjaxUrl);
            uri.addFormInfo(form);
            uri.addSearch("menuGuid", menuGuid);
            uri.addSearch("menuVersion", menuVersion);
            uri.addSearch("EntireMenu", JSON.stringify(this.buildHierarchy()));
            $YetaWF.post(this.Setup.AjaxUrl, uri.toFormData(), function (success, sendResult) {
                menuVersionInput.value = sendResult.NewVersion.toString();
            });
        };
        return MenuEditView;
    }());
    YetaWF_Menus.MenuEditView = MenuEditView;
})(YetaWF_Menus || (YetaWF_Menus = {}));

//# sourceMappingURL=MenuEdit.js.map
