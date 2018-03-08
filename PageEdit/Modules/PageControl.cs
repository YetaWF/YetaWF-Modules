﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.DataProvider;

namespace YetaWF.Modules.PageEdit.Modules {

    public class PageControlModuleDataProvider : ModuleDefinitionDataProvider<Guid, PageControlModule>, IInstallableModel { }

    [ModuleGuid("{466C0CCA-3E63-43f3-8754-F4267767EED1}")] // Published Guid
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class PageControlModule : ModuleDefinition {

        public PageControlModule() {
            Name = this.__ResStr("modName", "Control Panel");
            Title = this.__ResStr("modTitle", "Control Panel");
            Description = this.__ResStr("modSummary", "Control Panel with support for adding modules, new pages, importing modules and editing page settings");
            ShowTitleActions = false;
            WantFocus = false;
            WantSearch = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new PageControlModuleDataProvider(); }

        [Category("Variables")]
        [Description("The HTML id used by this module - this id doesn't change for this module and can be used for skinning purposes")]
        [Caption("ModuleHtmlId")]
        public override string ModuleHtmlId { get { return Addons.Info.PageControlMod; } }

        public override bool ShowModuleMenu { get { return false; } }
        public override bool ModuleHasSettings { get { return false; } }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList baseMenuList = await base.GetModuleMenuListAsync(renderMode, location);

            MenuList menuList = new MenuList();
            menuList.New(this.GetAction_SwitchToView(), location);
            menuList.New(this.GetAction_SwitchToEdit(), location);

            PageEditModule modEdit = new PageEditModule();
            menuList.New(modEdit.GetAction_Edit(null), location);
            menuList.New(await modEdit.GetAction_RemoveAsync(null), location);

            menuList.New(await this.GetAction_W3CValidationAsync(), location);

            menuList.AddRange(baseMenuList);
            return menuList;
        }

        public ModuleAction GetAction_SwitchToEdit() {
            if (!Manager.CurrentPage.IsAuthorized_Edit()) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.SwitchToEdit)),
                QueryArgs = new { },
                NeedsModuleContext = true,
                Image = "#Edit",
                LinkText = this.__ResStr("modSwitchToEditLink", "Switch To Site Edit Mode"),
                MenuText = this.__ResStr("modSwitchToEditText", "Switch To Site Edit Mode"),
                Tooltip = this.__ResStr("modSwitchToEditTooltip", "Switch to Site Edit Mode"),
                Legend = this.__ResStr("modSwitchToEditLegend", "Switches to Site Edit Mode"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.View,
                Location = ModuleAction.ActionLocationEnum.NoAuto |
                        ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
                DontFollow = true,
            };

        }
        public ModuleAction GetAction_SwitchToView() {
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.SwitchToView)),
                NeedsModuleContext = true,
                QueryArgs = new { },
                Image = "#Display",
                LinkText = this.__ResStr("modSwitchToViewLink", "Switch To Site View Mode"),
                MenuText = this.__ResStr("modSwitchToViewText", "Switch To Site View Mode"),
                Tooltip = this.__ResStr("modSwitchToViewTooltip", "Switch to Site View Mode"),
                Legend = this.__ResStr("modSwitchToViewLegend", "Switches to Site View Mode"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Edit,
                Location = ModuleAction.ActionLocationEnum.NoAuto |
                            ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
                DontFollow = true,
            };
        }
        public async Task<ModuleAction> GetAction_W3CValidationAsync() {
            if (Manager.CurrentPage == null) return null;
            ControlPanelConfigData config = await ControlPanelConfigDataProvider.GetConfigAsync();
            if (string.IsNullOrWhiteSpace(config.W3CUrl)) return null;
            if (!config.W3CUrl.Contains("{0}")) return null;
            return new ModuleAction(this) {
                Url = string.Format(config.W3CUrl, Manager.CurrentPage.EvaluatedCanonicalUrl),
                Image = "W3CValidator.png",
                Name = "W3CValidate",
                LinkText = this.__ResStr("modW3CValLink", "W3C Validation"),
                MenuText = this.__ResStr("modW3CValText", "W3C Validation"),
                Tooltip = this.__ResStr("modW3CValTooltip", "Use W3C Validation service to validate the current page - The page must be accessible to the remote service as an anonymous user"),
                Legend = this.__ResStr("modW3CValLegend", "Uses the defined W3C Validation service to validate a page - The page must be accessible to the remote service as an anonymous user"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto |
                            ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                DontFollow = true,
            };
        }
    }
}