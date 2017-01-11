/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.DataProvider;

namespace YetaWF.Modules.ModuleEdit.Modules {

    public class ModuleEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, ModuleEditModule>, IInstallableModel { }

    [ModuleGuid("{ACDC1453-32BD-4de2-AB2B-7BF5CE217762}")] // Published Guid
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class ModuleEditModule : ModuleDefinition {

        public ModuleEditModule() {
            Name = this.__ResStr("modName", "Module Edit");
            Title = this.__ResStr("modTitle", "Module Editing Features");
            Description = this.__ResStr("modSummary", "Editing features for modules");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ModuleEditModuleDataProvider(); }

        public override bool ShowModuleMenu { get { return false; } }
        public override bool ModuleHasSettings { get { return false; } }

        public ModuleAction GetAction_Settings(Guid editGuid) {
            ModuleDefinition editMod = ModuleDefinition.Load(editGuid, AllowNone: true);
            if (editMod == null) return null;
            if (!editMod.ModuleHasSettings) return null;
            if (!editMod.IsAuthorized(RoleDefinition.Edit)) return null;
            return new ModuleAction(this) {
                Url = ModulePermanentUrl,
                QueryArgs = new { ModuleGuid = editGuid },
                QueryArgsRvd = new System.Web.Routing.RouteValueDictionary{
                    { Globals.Link_TempNoEditMode, "y" },
                },
                NeedsModuleContext = true,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Module Settings"),
                MenuText = this.__ResStr("editText", "Module Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the module's settings"),
                Legend = this.__ResStr("editLegend", "Edits the module's settings"),
                Category = ModuleAction.ActionCategoryEnum.Update,

                Mode = ModuleAction.ActionModeEnum.Any,
                Style = ModuleAction.ActionStyleEnum.PopupEdit,
                Location = ModuleAction.ActionLocationEnum.ModuleMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.GridLinks,
                SaveReturnUrl = true,
            };
        }
    }
}