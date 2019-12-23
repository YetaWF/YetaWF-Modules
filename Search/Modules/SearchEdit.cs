/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Search.Modules {

    public class SearchEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchEditModule>, IInstallableModel { }

    [ModuleGuid("{dfa30d65-52b8-4b7f-a2f4-1e4e73477b04}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SearchEditModule : ModuleDefinition {

        public SearchEditModule() {
            Title = this.__ResStr("modTitle", "Search Item");
            Name = this.__ResStr("modName", "Edit Search Item");
            Description = this.__ResStr("modSummary", "Edits an existing search item");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SearchEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, int searchDataId) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { searchDataId = searchDataId },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit an existing search item"),
                Legend = this.__ResStr("editLegend", "Edits an existing search item"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}