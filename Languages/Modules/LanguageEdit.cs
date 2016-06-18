/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Languages.Modules {

    public class LanguageEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, LanguageEditModule>, IInstallableModel { }

    [ModuleGuid("{afa84780-77a9-4f5e-b88c-9bb02c4dc9b3}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class LanguageEditModule : ModuleDefinition {

        public LanguageEditModule() : base() {
            Title = this.__ResStr("modTitle", "Language");
            Name = this.__ResStr("modName", "Edit Language");
            Description = this.__ResStr("modSummary", "Edits an existing language");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LanguageEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, string id) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Id = id },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit an existing language"),
                Legend = this.__ResStr("editLegend", "Edits an existing language"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}