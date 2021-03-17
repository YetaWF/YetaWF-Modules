/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
#if MVC6
#else
using System.Web.Routing;
#endif

namespace YetaWF.Modules.ModuleEdit.Modules {

    public class ModuleEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, ModuleEditModule>, IInstallableModel { }

    [ModuleGuid("{ACDC1453-32BD-4de2-AB2B-7BF5CE217762}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class ModuleEditModule : ModuleDefinition {

        public ModuleEditModule() {
            Name = this.__ResStr("modName", "Module Edit");
            Title = this.__ResStr("modTitle", "Module Editing Features");
            Description = this.__ResStr("modSummary", "Implements editing features used in Site Edit Mode to edit module settings.");
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ModuleEditModuleDataProvider(); }

        public override bool ShowModuleMenu { get { return false; } }
        public override bool ModuleHasSettings { get { return false; } }

        public Task<ModuleAction> GetAction_SettingsAsync(Guid editGuid) {
            return GetAction_SettingsAsync(editGuid, false);
        }
        public Task<ModuleAction> GetAction_SettingsGenerateAsync(Guid editGuid) {
            return GetAction_SettingsAsync(editGuid, true);
        }
        private async Task<ModuleAction> GetAction_SettingsAsync(Guid editGuid, bool force) {
            ModuleDefinition editMod = await ModuleDefinition.LoadAsync(editGuid, AllowNone: true);
            if (editMod == null) {
                if (!force)
                    return null;
                editGuid = Guid.Empty;
            } else {
                if (!editMod.ModuleHasSettings) return null;
            }
            if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_ModuleSettings))
                return null;
            return new ModuleAction(this) {
                Url = ModulePermanentUrl,
                QueryArgs = new { ModuleGuid = editGuid },
                QueryArgsDict = new QueryHelper(new QueryDictionary { { Globals.Link_NoEditMode, "y" } }),
                NeedsModuleContext = true,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Settings"),
                MenuText = this.__ResStr("editText", "Settings"),
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