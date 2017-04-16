/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.Addons;
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
using Microsoft.AspNetCore.Routing;
#else
using System.Web.Routing;
#endif

namespace YetaWF.Modules.ModuleEdit.Modules {

    public class ModuleControlModuleDataProvider : ModuleDefinitionDataProvider<Guid, ModuleControlModule>, IInstallableModel { }

    [ModuleGuid("{96CAEAD9-068D-4b83-8F46-5269834F3B16}")] // Published Guid
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class ModuleControlModule : ModuleDefinition {

        public ModuleControlModule() {
            Name = this.__ResStr("modName", "Module Control");
            Title = this.__ResStr("modTitle", "Module Control");
            Description = this.__ResStr("modSummary", "Module editing services");
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ModuleControlModuleDataProvider(); }

        [Category("Variables")]
        [Description("Shows whether the module menu is shown for this module")]
        [UIHint("Boolean")]
        public override bool ShowModuleMenu { get { return false; } }

        public override bool ModuleHasSettings { get { return false; } }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("Exports",
                        this.__ResStr("roleExportsC", "Export Module Data"), this.__ResStr("roleExports", "The role has permission to export module data"),
                        this.__ResStr("userExportsC", "Export Module Data"), this.__ResStr("userExports", "The user has permission to export module data")),
                };
            }
        }

        public ModuleAction GetAction_ExportModule(ModuleDefinition mod) {
            if (!mod.ModuleHasSettings) return null;
            if (!mod.IsAuthorized(RoleDefinition.Edit)) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(ModuleControlModuleController), "ExportModuleData"),
                QueryArgs = new { ModuleGuid = mod.ModuleGuid },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = "ExportModule.png",
                LinkText = this.__ResStr("modExportLink", "Export"),
                MenuText = this.__ResStr("modExportMenu", "Export Module"),
                Legend = this.__ResStr("modExportLegend", "Export the module data by creating an importable ZIP file (using Control Panel, Import Module)"),
                Tooltip = this.__ResStr("modExportTT", "Exports the module data by creating an importable ZIP file (using Control Panel, Import Module)"),
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                Mode = ModuleAction.ActionModeEnum.Edit,
                CookieAsDoneSignal = true,
                Style = ModuleAction.ActionStyleEnum.Normal,
            };
        }

        public ModuleAction GetAction_Help(ModuleDefinition mod) {
            Package package = Package.GetCurrentPackage(mod);
            return new ModuleAction(this) {
                Url = package.InfoLink,
                QueryArgsDict = new QueryHelper(new QueryDictionary { { Globals.Link_NoEditMode, "y" }, { Globals.Link_NoPageControl, "y" } }),
                Image = "Help.png",
                LinkText = this.__ResStr("modHelpLink", "Help"),
                MenuText = this.__ResStr("modHelpMenu", "Help"),
                Legend = this.__ResStr("modHelpLegend", "Display help information for the package implementing this module"),
                Tooltip = this.__ResStr("modHelpTT", "Displays help information for the package implementing this module"),
                Location = Manager.EditMode ? ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu : ModuleAction.ActionLocationEnum.ModuleLinks,
                Mode = ModuleAction.ActionModeEnum.Any,
                Style = ModuleAction.ActionStyleEnum.NewWindow,
            };
        }

        public ModuleAction GetAction_MoveToPane(PageDefinition page, ModuleDefinition mod, string oldPane, string newPane) {
            if (page == null) return null;
            if (oldPane == null) return null;
            if (newPane == null) return null;
            if (!page.IsAuthorized_Edit()) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(ModuleControlModuleController), "MoveToPane"),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = mod.ModuleGuid, OldPane = oldPane, NewPane = newPane },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = "MoveToPane.png",
                LinkText = string.Format(this.__ResStr("modMoveToLink", "Move To {0}"), newPane),
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
        public ModuleAction GetAction_MoveUp(PageDefinition page, ModuleDefinition mod, string pane) {
            if (page == null) return null;
            if (pane == null) return null;
            if (!page.IsAuthorized_Edit()) return null;
            PageDefinition.ModuleList modList = page.ModuleDefinitions.GetModulesForPane(pane);
            int modIndex = modList.IndexInPane(mod, pane);
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(ModuleControlModuleController), "MoveUp"),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = mod.ModuleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = "MoveUp.png",
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
        public ModuleAction GetAction_MoveDown(PageDefinition page, ModuleDefinition mod, string pane) {
            if (page == null) return null;
            if (pane == null) return null;
            if (!page.IsAuthorized_Edit()) return null;
            PageDefinition.ModuleList modList = page.ModuleDefinitions.GetModulesForPane(pane);
            int modIndex = modList.IndexInPane(mod, pane);
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(ModuleControlModuleController), "MoveDown"),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = mod.ModuleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = "MoveDown.png",
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

        public ModuleAction GetAction_MoveTop(PageDefinition page, ModuleDefinition mod, string pane) {
            if (page == null) return null;
            if (pane == null) return null;
            if (!page.IsAuthorized_Edit()) return null;
            PageDefinition.ModuleList modList = page.ModuleDefinitions.GetModulesForPane(pane);
            int modIndex = modList.IndexInPane(mod, pane);
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(ModuleControlModuleController), "MoveTop"),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = mod.ModuleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = "MoveTop.png",
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

        public ModuleAction GetAction_MoveBottom(PageDefinition page, ModuleDefinition mod, string pane) {
            if (page == null) return null;
            if (pane == null) return null;
            if (!page.IsAuthorized_Edit()) return null;
            PageDefinition.ModuleList modList = page.ModuleDefinitions.GetModulesForPane(pane);
            int modIndex = modList.IndexInPane(mod, pane);
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(ModuleControlModuleController), "MoveBottom"),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = mod.ModuleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = "MoveBottom.png",
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

        public ModuleAction GetAction_Remove(PageDefinition page, ModuleDefinition mod, Guid moduleGuid, string pane) {
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
                Url = YetaWFManager.UrlFor(typeof(ModuleControlModuleController), "Remove"),
                QueryArgs = new { PageGuid = page.PageGuid, ModuleGuid = moduleGuid, Pane = pane, ModuleIndex = modIndex },
                QueryArgsDict = new QueryHelper(new QueryDictionary {
                    { Basics.ModuleGuid, this.ModuleGuid }, // the module authorizing this
                }),
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("modRemoveLink", "Remove Module"),
                MenuText = this.__ResStr("modRemoveText", "Remove"),
                Tooltip = this.__ResStr("modRemoveTooltip", "Remove the module from the page"),
                Legend = this.__ResStr("modRemoveLegend", "Removes the module from the page"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Edit,
                Location = ModuleAction.ActionLocationEnum.ModuleMenu | ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("confirmRemove", "Are you sure you want to remove this module?"),
            };
        }
    }

}