/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {

    public class EntryEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, EntryEditModule>, IInstallableModel { }

    [ModuleGuid("{64aae940-f2a8-4fb5-a2ab-fb3edfc9d6ac}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Entries")]
    public class EntryEditModule : ModuleDefinition {

        public EntryEditModule() {
            Title = this.__ResStr("modTitle", "Blog Entry");
            Name = this.__ResStr("modName", "Edit Blog Entry");
            Description = this.__ResStr("modSummary", "Edits an existing blog entry.");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EntryEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction?> GetAction_EditAsync(string? url, int blogEntry) {
            if (blogEntry == 0) return null;
            ModuleAction action = new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { BlogEntry = blogEntry },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit this blog entry"),
                Legend = this.__ResStr("editLegend", "Edits an existing blog entry"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
            if (!await action.IsAuthorizedAsync()) return null;
            return action;
        }
    }
}