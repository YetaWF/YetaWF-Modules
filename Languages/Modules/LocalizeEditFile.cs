/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Languages.Modules {

    public class LocalizeEditFileModuleDataProvider : ModuleDefinitionDataProvider<Guid, LocalizeEditFileModule>, IInstallableModel { }

    [ModuleGuid("{1b17a5d7-2b3a-4759-919e-f6509403a16b}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class LocalizeEditFileModule : ModuleDefinition {

        public LocalizeEditFileModule() {
            Title = this.__ResStr("modTitle", "Localization Resource");
            Name = this.__ResStr("modName", "Edit Localization Resource");
            Description = this.__ResStr("modSummary", "Used to edit an existing localization resource. This is used by the Languages Module.");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LocalizeEditFileModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, string packageName, string typeName) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { PackageName = packageName, TypeName = typeName },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit localization resource"),
                Legend = this.__ResStr("editLegend", "Edits an existing localization resource"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}