/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Pages.Modules {

    public class UnifiedSetEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, UnifiedSetEditModule>, IInstallableModel { }

    [ModuleGuid("{ab391cf1-bad3-438f-a7fd-d4159631289f}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class UnifiedSetEditModule : ModuleDefinition {

        public UnifiedSetEditModule() {
            Title = this.__ResStr("modTitle", "Unified Page Set");
            Name = this.__ResStr("modName", "Edit Unified Page Set");
            Description = this.__ResStr("modSummary", "Edits an existing Unified Page Set");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UnifiedSetEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, Guid unifiedSetGuid) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { UnifiedSetGuid = unifiedSetGuid },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit Unified Page Set"),
                Legend = this.__ResStr("editLegend", "Edits an existing Unified Page Set"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
