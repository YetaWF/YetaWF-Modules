/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules {

    public class EditExtensionModuleDataProvider : ModuleDefinitionDataProvider<Guid, EditExtensionModule>, IInstallableModel { }

    [ModuleGuid("{2bd2f6c5-daf4-48c0-bdc4-4eb20f1bca8a}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class EditExtensionModule : ModuleDefinition {

        public EditExtensionModule() {
            Title = this.__ResStr("modTitle", "Extension");
            Name = this.__ResStr("modName", "Edit Extension");
            Description = this.__ResStr("modSummary", "Edits an existing extension.");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EditExtensionModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, string extension) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Extension = extension },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit extension"),
                Legend = this.__ResStr("editLegend", "Edits an existing extension"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
