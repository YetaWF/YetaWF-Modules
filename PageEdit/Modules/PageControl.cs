/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
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

            PageEditModule modEdit = new PageEditModule();
            menuList.New(await modEdit.GetAction_EditAsync(null), location);
            menuList.New(await GetAction_ExportPageAsync(null), location);
            menuList.New(await modEdit.GetAction_RemoveAsync(null), location);

            menuList.New(GetAction_SwitchToView(), location);
            menuList.New(GetAction_SwitchToEdit(), location);

            menuList.New(await GetAction_W3CValidationAsync(), location);
            menuList.New(await GetAction_RestartSite(), location);

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
                Url = YetaWFManager.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.ExportPage)),
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
    }
}