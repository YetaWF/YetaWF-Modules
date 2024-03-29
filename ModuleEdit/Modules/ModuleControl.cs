/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.ModuleEdit.Controllers;
#if MVC6
#else
#endif

namespace YetaWF.Modules.ModuleEdit.Modules {

    public class ModuleControlModuleDataProvider : ModuleDefinitionDataProvider<Guid, ModuleControlModule>, IInstallableModel { }

    [ModuleGuid("{96CAEAD9-068D-4b83-8F46-5269834F3B16}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class ModuleControlModule : ModuleDefinition {

        public ModuleControlModule() {
            Name = this.__ResStr("modName", "Module Control");
            Title = this.__ResStr("modTitle", "Module Control");
            Description = this.__ResStr("modSummary", "Implements editing services used in Site Edit Mode to move modules within panes, change their position, remove modules from a page and to export module data.");
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ModuleControlModuleDataProvider(); }

        [Category("Variables")]
        [Description("Shows whether the module menu is shown for this module")]
        [UIHint("Boolean")]
        public override bool ShowModuleMenu { get { return false; } }

        public override bool ModuleHasSettings { get { return false; } }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction?> GetAction_ExportModuleAsync(ModuleDefinition mod) {
            if (!mod.ModuleHasSettings) return null;
            if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_ModuleExport)) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModuleControlModuleController), nameof(ModuleControlModuleController.ExportModuleData)),
                QueryArgs = new { ModuleGuid = mod.ModuleGuid },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = await CustomIconAsync("ExportModule.png"),
                LinkText = this.__ResStr("modExportLink", "Export"),
                MenuText = this.__ResStr("modExportMenu", "Export Module"),
                Tooltip = this.__ResStr("modExportTT", "Export the module data by creating an importable ZIP file (using Control Panel, Import Module)"),
                Legend = this.__ResStr("modExportLegend", "Exports the module data by creating an importable ZIP file (using Control Panel, Import Module)"),
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                Mode = ModuleAction.ActionModeEnum.Edit,
                CookieAsDoneSignal = true,
                Style = ModuleAction.ActionStyleEnum.Normal,
            };
        }

        public ModuleAction? GetAction_Help(ModuleDefinition mod) {
            string? url = mod.HelpURL;
            if (string.IsNullOrWhiteSpace(url)) {
                Package package = Package.GetCurrentPackage(mod);
                url = package.InfoLink;
            }
            if (string.IsNullOrWhiteSpace(url)) 
                return null;
            return new ModuleAction(this) {
                Url = url,
                Image = "#Help",
                LinkText = this.__ResStr("modHelpLink", "Help"),
                MenuText = this.__ResStr("modHelpMenu", "Help"),
                Tooltip = this.__ResStr("modHelpTT", "Display help information"),
                Legend = this.__ResStr("modHelpLegend", "Displays help information"),
                Location = Manager.EditMode ? ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu : ModuleAction.ActionLocationEnum.ModuleLinks,
                Mode = ModuleAction.ActionModeEnum.Any,
                Style = ModuleAction.ActionStyleEnum.NewWindow,
            };
        }

        public async Task<ModuleAction?> GetAction_MoveToPaneAsync(PageDefinition? page, ModuleDefinition mod, string? oldPane, string? newPane) {
            if (page == null) return null;
            if (oldPane == null) return null;
            if (newPane == null) return null;
            if (!page.IsAuthorized_Edit()) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModuleControlModuleController), nameof(ModuleControlModuleController.MoveToPane)),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = mod.ModuleGuid, OldPane = oldPane, NewPane = newPane },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = await CustomIconAsync("MoveToPane.png"),
                LinkText = this.__ResStr("modMoveToLink", "Move To {0}", newPane),
                Style = ModuleAction.ActionStyleEnum.Post,
                MenuText = newPane,
                Tooltip = this.__ResStr("ttMoveToPane", "Move the module to the {0} pane", newPane),
                Legend = this.__ResStr("modMoveToLegend", "Moves the module to the {0} pane", newPane),
                Enabled = string.Compare(oldPane, newPane, true) != 0,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Edit,
                Location = ModuleAction.ActionLocationEnum.ModuleMenu,
            };
        }
        public async Task<ModuleAction?> GetAction_MoveUpAsync(PageDefinition? page, ModuleDefinition mod, string? pane) {
            if (page == null) return null;
            if (pane == null) return null;
            if (!page.IsAuthorized_Edit()) return null;
            PageDefinition.ModuleList modList = page.ModuleDefinitions.GetModulesForPane(pane);
            int modIndex = modList.IndexInPane(mod, pane);
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModuleControlModuleController), nameof(ModuleControlModuleController.MoveUp)),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = mod.ModuleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = await CustomIconAsync("MoveUp.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("modMoveUpLink", "Move Up"),
                MenuText = this.__ResStr("modMoveUpText", "Up"),
                Tooltip = this.__ResStr("modMoveUpTooltip", "Move the module up within the pane"),
                Legend = this.__ResStr("modMoveUpLegend", "Moves the module up within the pane"),
                Enabled = modIndex > 0,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Edit,
                Location = ModuleAction.ActionLocationEnum.ModuleMenu,
            };
        }
        public async Task<ModuleAction?> GetAction_MoveDownAsync(PageDefinition? page, ModuleDefinition mod, string? pane) {
            if (page == null) return null;
            if (pane == null) return null;
            if (!page.IsAuthorized_Edit()) return null;
            PageDefinition.ModuleList modList = page.ModuleDefinitions.GetModulesForPane(pane);
            int modIndex = modList.IndexInPane(mod, pane);
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModuleControlModuleController), nameof(ModuleControlModuleController.MoveDown)),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = mod.ModuleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = await CustomIconAsync("MoveDown.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("modMoveDownLink", "Move Down"),
                MenuText = this.__ResStr("modMoveDownText", "Down"),
                Tooltip = this.__ResStr("modMoveDownTooltip", "Move the module down within the pane"),
                Legend = this.__ResStr("modMoveDownLegend", "Moves the module down within the pane"),
                Enabled = modIndex < modList.Count - 1,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Edit,
                Location = ModuleAction.ActionLocationEnum.ModuleMenu,
            };
        }

        public async Task<ModuleAction?> GetAction_MoveTopAsync(PageDefinition? page, ModuleDefinition mod, string? pane) {
            if (page == null) return null;
            if (pane == null) return null;
            if (!page.IsAuthorized_Edit()) return null;
            PageDefinition.ModuleList modList = page.ModuleDefinitions.GetModulesForPane(pane);
            int modIndex = modList.IndexInPane(mod, pane);
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModuleControlModuleController), nameof(ModuleControlModuleController.MoveTop)),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = mod.ModuleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = await CustomIconAsync("MoveTop.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("modMoveTopLink", "Move To Top"),
                MenuText = this.__ResStr("modMoveTopText", "Top"),
                Tooltip = this.__ResStr("modMoveTopTooltip", "Move the module to the top of the pane"),
                Legend = this.__ResStr("modMoveTopLegend", "Moves the module to the top of the pane"),
                Enabled = modIndex > 0,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Edit,
                Location = ModuleAction.ActionLocationEnum.ModuleMenu,
            };
        }

        public async Task<ModuleAction?> GetAction_MoveBottomAsync(PageDefinition? page, ModuleDefinition mod, string? pane) {
            if (page == null) return null;
            if (pane == null) return null;
            if (!page.IsAuthorized_Edit()) return null;
            PageDefinition.ModuleList modList = page.ModuleDefinitions.GetModulesForPane(pane);
            int modIndex = modList.IndexInPane(mod, pane);
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModuleControlModuleController), nameof(ModuleControlModuleController.MoveBottom)),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = mod.ModuleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = await CustomIconAsync("MoveBottom.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("modMoveBottomLink", "Move To Bottom"),
                MenuText = this.__ResStr("modMoveBottomText", "Bottom"),
                Tooltip = this.__ResStr("modMoveBottomTooltip", "Move the module to the bottom of the pane"),
                Legend = this.__ResStr("modMoveBottomLegend", "Moves the module to the bottom of the pane"),
                Enabled = modIndex < modList.Count - 1,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Edit,
                Location = ModuleAction.ActionLocationEnum.ModuleMenu,
            };
        }

        public ModuleAction? GetAction_Remove(PageDefinition? page, ModuleDefinition? mod, Guid moduleGuid, string? pane) {
            if (mod != null && moduleGuid != Guid.Empty) throw new InternalError("Can't use module definition and module guid at the same time to remove the module");
            if (page == null) return null;
            if (pane == null) return null;
            if (mod != null) {
                if (!mod.IsAuthorized(RoleDefinition.Remove)) return null;
                moduleGuid = mod.ModuleGuid;
            }
            PageDefinition.ModuleList modList = page.ModuleDefinitions.GetModulesForPane(pane);
            int modIndex = modList.IndexInPane(moduleGuid, pane);
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModuleControlModuleController), nameof(ModuleControlModuleController.Remove)),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = moduleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("modRemoveLink", "Remove"),
                MenuText = this.__ResStr("modRemoveText", "Remove"),
                Tooltip = this.__ResStr("modRemoveTooltip", "Remove the module from this page (the module and its data are NOT deleted and can still be used on other pages or added again)"),
                Legend = this.__ResStr("modRemoveLegend", "Removes the module from this page (the module itself and its data are NOT deleted and can still be used on other pages or added again)"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Edit,
                Location = ModuleAction.ActionLocationEnum.ModuleMenu | ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("confirmRemove", "Are you sure you want to remove this module?"),
            };
        }
        public ModuleAction? GetAction_RemovePermanent(PageDefinition? page, ModuleDefinition? mod, Guid moduleGuid, string? pane) {
            if (mod != null && moduleGuid != Guid.Empty) throw new InternalError("Can't use module definition and module guid at the same time to remove the module");
            if (page == null) return null;
            if (pane == null) return null;
            if (mod != null) {
                if (!mod.IsAuthorized(RoleDefinition.Remove)) return null;
                moduleGuid = mod.ModuleGuid;
            }
            PageDefinition.ModuleList modList = page.ModuleDefinitions.GetModulesForPane(pane);
            int modIndex = modList.IndexInPane(moduleGuid, pane);
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModuleControlModuleController), nameof(ModuleControlModuleController.RemovePermanent)),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = moduleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("modRemovePermLink", "Remove PERMANENTLY"),
                MenuText = this.__ResStr("modRemovePermText", "Remove PERMANENTLY"),
                Tooltip = this.__ResStr("modRemovePermTooltip", "Remove the module permanently - The module and its data are PERMANENTLY deleted and can no longer be used on any pages"),
                Legend = this.__ResStr("modRemovePermLegend", "Removes the module permanently - The module and its data are PERMANENTLY deleted and can no longer be used on any pages"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Edit,
                Location = ModuleAction.ActionLocationEnum.ModuleMenu | ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("confirmPermRemove", "Are you sure you want to PERMANENTLY remove this module?"),
            };
        }
    }

}