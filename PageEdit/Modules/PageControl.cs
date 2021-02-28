/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.PageEdit.Controllers;
using YetaWF.Modules.PageEdit.DataProvider;

namespace YetaWF.Modules.PageEdit.Modules {

    public class PageControlModuleDataProvider : ModuleDefinitionDataProvider<Guid, PageControlModule>, IInstallableModel { }

    [ModuleGuid("{466C0CCA-3E63-43f3-8754-F4267767EED1}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class PageControlModule : ModuleDefinition {

        public PageControlModule() {
            Name = this.__ResStr("modName", "Control Panel (Skin)");
            Title = this.__ResStr("modTitle", "Control Panel");
            Description = this.__ResStr("modSummary", "Displays an icon opening/closing the Control Panel which supports adding new and existing modules to a page, supports importing a module and is used to create new pages, change page settings and remove the current page.");
            ShowTitleActions = false;
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
            Invokable = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new PageControlModuleDataProvider(); }

        public override bool ShowModuleMenu { get { return false; } }
        public override bool ModuleHasSettings { get { return false; } }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            List<ModuleAction> baseMenuList = await base.GetModuleMenuListAsync(renderMode, location);
            List<ModuleAction> menuList = new List<ModuleAction>();

            PageEditModule modEdit = new PageEditModule();
            menuList.New(await modEdit.GetAction_EditAsync(null), location);
            menuList.New(await GetAction_ExportPageAsync(null), location);
            menuList.New(await modEdit.GetAction_RemoveAsync(null), location);

            menuList.New(GetAction_SwitchToView(), location);
            menuList.New(GetAction_SwitchToEdit(), location);

            menuList.New(await GetAction_W3CValidationAsync(), location);
            menuList.New(await GetAction_RestartSite(), location);
            menuList.New(GetAction_ClearJsCssCache(), location);

            menuList.AddRange(baseMenuList);
            return menuList;
        }

        public async Task<ModuleAction> GetAction_PageControlAsync() {
            return new ModuleAction(this) {
                Category = ModuleAction.ActionCategoryEnum.Significant,
                CssClass = "y_button_outline",
                Image = await new SkinImages().FindIcon_PackageAsync("PageEdit.png", Package.GetCurrentPackage(this)),
                Location = ModuleAction.ActionLocationEnum.Any,
                Mode = ModuleAction.ActionModeEnum.Any,
                Style = ModuleAction.ActionStyleEnum.Nothing,
                LinkText = this.__ResStr("pageControlLink", "Control Panel"),
                MenuText = this.__ResStr("pageControlMenu", "Control Panel"),
                Tooltip = this.__ResStr("pageControlTT", "Control Panel - Add new or existing modules, add new pages, switch to edit mode, access page settings and other site management tasks"),
                Legend = this.__ResStr("pageControlLeg", "Control Panel - Adds new or existing modules, adds new pages, switches to edit mode, accesses page settings and other site management tasks"),
            };
        }

        public ModuleAction GetAction_SwitchToEdit() {
            if (!Manager.CurrentPage.IsAuthorized_Edit()) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.SwitchToEdit)),
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
                Url = Utility.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.SwitchToView)),
                QueryArgs = new { },
                NeedsModuleContext = true,
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
        public async Task<ModuleAction> GetAction_ExportPageAsync(Guid? pageGuid = null) {
            Guid guid;
            PageDefinition page;
            if (pageGuid == null) {
                page = Manager.CurrentPage;
                if (page == null) return null;
                guid = page.PageGuid;
            } else {
                guid = (Guid)pageGuid;
                page = await PageDefinition.LoadAsync(guid);
            }
            if (page == null) return null;
            if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_PageExport)) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.ExportPage)),
                QueryArgs = new { PageGuid = guid },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = await CustomIconAsync("ExportPage.png"),
                Name = "ExportPage",
                LinkText = this.__ResStr("modExportLink", "Export"),
                MenuText = this.__ResStr("modExportMenu", "Export Page"),
                Tooltip = this.__ResStr("modExportTT", "Export the page and modules by creating an importable ZIP file (using Control Panel, Import Page)"),
                Legend = this.__ResStr("modExportLegend", "Exports the page and modules by creating an importable ZIP file (using Control Panel, Import Page)"),
                Location = ModuleAction.ActionLocationEnum.NoAuto |
                            ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                Mode = ModuleAction.ActionModeEnum.Any,
                CookieAsDoneSignal = true,
                Style = ModuleAction.ActionStyleEnum.Normal,
            };
        }
        public async Task<ModuleAction> GetAction_W3CValidationAsync() {
            if (Manager.CurrentPage == null) return null;
            if (Manager.IsLocalHost) return null;
            ControlPanelConfigData config = await ControlPanelConfigDataProvider.GetConfigAsync();
            if (string.IsNullOrWhiteSpace(config.W3CUrl)) return null;
            if (!config.W3CUrl.Contains("{0}")) return null;
            return new ModuleAction(this) {
                Url = string.Format(config.W3CUrl, Manager.CurrentPage.EvaluatedCanonicalUrl),
                Image = await CustomIconAsync("W3CValidator.png"),
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
        public async Task<ModuleAction> GetAction_RestartSite() {
            if (!Manager.HasSuperUserRole) return null;
            if (YetaWF.Core.Support.Startup.MultiInstance) return null;
            return new ModuleAction(this) {
                Url = "/$restart",
                Image = await CustomIconAsync("RestartSite.png"),
                LinkText = this.__ResStr("restartLink", "Restart Site"),
                MenuText = this.__ResStr("restartText", "Restart Site"),
                Tooltip = this.__ResStr("restartTooltip", "Restart the site immediately (IIS restart)"),
                Legend = this.__ResStr("restartLegend", "Restarts the site immediately (IIS restart)"),
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto |
                            ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                DontFollow = true,
            };
        }
        public ModuleAction GetAction_ClearJsCssCache() {
            if (!Manager.HasSuperUserRole) return null;
            return new ModuleAction(this) {
                Style = ModuleAction.ActionStyleEnum.Post,
                NeedsModuleContext = true,
                Url = Utility.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.ClearJsCss)),
                Image = "#Remove",
                LinkText = this.__ResStr("clrCacheLink", "Clear JS/CSS/Statics Cache"),
                MenuText = this.__ResStr("clrCacheText", "Clear JS/CSS/Statics Cache"),
                Tooltip = this.__ResStr("clrCacheTooltip", "Clear the cached JavaScript/CSS bundles and cached static small objects"),
                Legend = this.__ResStr("clrCacheLegend", "Clears the cached JavaScript/CSS bundles and cached static small objects"),
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                ConfirmationText = this.__ResStr("clrCacheConfirm", "Are you sure you want to clear the JavaScript/CSS bundles and cached static small objects?"),
                Location = ModuleAction.ActionLocationEnum.NoAuto |
                            ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                DontFollow = true,
            };
        }
    }
}