/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Dashboard.Controllers;

namespace YetaWF.Modules.Dashboard.Modules {

    public class StaticPagesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, StaticPagesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{21b15c5c-d999-424e-8bff-17d9919a9ce8}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class StaticPagesBrowseModule : ModuleDefinition {

        public StaticPagesBrowseModule() {
            Title = this.__ResStr("modTitle", "Static Pages");
            Name = this.__ResStr("modName", "Static Pages");
            Description = this.__ResStr("modSummary", "Displays and manages information about static pages. This can be accessed using Admin > Dashboard > Static Pages (standard YetaWF site).");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new StaticPagesBrowseModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
            return menuList;
        }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Static Pages"),
                MenuText = this.__ResStr("browseText", "Static Pages"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage static pages"),
                Legend = this.__ResStr("browseLegend", "Displays and manages static pages"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_Remove(string localUrl) {
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(StaticPagesBrowseModuleController), "Remove"),
                NeedsModuleContext = true,
                QueryArgs = new { LocalUrl = localUrl },
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Unload Static Page"),
                MenuText = this.__ResStr("removeMenu", "Unload Loaded Static Page"),
                Tooltip = this.__ResStr("removeTT", "Unload the static page so it is regenerated the next time it's requested"),
                Legend = this.__ResStr("removeLegend", "Unloads the static page so it is regenerated the next time it's requested"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to unload static page \"{0}\"?", localUrl),
            };
        }
        public ModuleAction GetAction_RemoveAll() {
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(StaticPagesBrowseModuleController), "RemoveAll"),
                NeedsModuleContext = true,
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeAllLink", "Unload All"),
                MenuText = this.__ResStr("removeAllMenu", "Unload All"),
                Tooltip = this.__ResStr("removeAllTT", "Unload all static pages and remove the saved data - Pages will be regenerated when they are accessed"),
                Legend = this.__ResStr("removeAllLegend", "Unloads all static pages and removes the saved data - Pages will be regenerated when they are accessed"),
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("toggleremoveAll", "Are you sure you want to unload all static pages?"),
            };
        }
    }
}
