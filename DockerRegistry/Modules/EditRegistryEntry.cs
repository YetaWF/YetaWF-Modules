/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DockerRegistry.Modules {

    public class EditRegistryEntryModuleDataProvider : ModuleDefinitionDataProvider<Guid, EditRegistryEntryModule>, IInstallableModel { }

    [ModuleGuid("{8491ae0a-de75-4924-8a8d-2ca844dc818f}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)] // unique so we can set skin props without page
    public class EditRegistryEntryModule : ModuleDefinition {

        public EditRegistryEntryModule() {
            Title = this.__ResStr("modTitle", "Edit Registry Server");
            Name = this.__ResStr("modName", "Edit Registry Server");
            Description = this.__ResStr("modSummary", "Edits an existing registry server");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EditRegistryEntryModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(int id) {
            if (id == 0) return null;
            return new ModuleAction(this) {
                Url = ModulePermanentUrl,
                QueryArgs = new { RegistryId = id },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit registry server"),
                Legend = this.__ResStr("editLegend", "Edits an existing registry server"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
