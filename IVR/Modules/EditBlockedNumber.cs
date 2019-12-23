/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules {

    public class EditBlockedNumberModuleDataProvider : ModuleDefinitionDataProvider<Guid, EditBlockedNumberModule>, IInstallableModel { }

    [ModuleGuid("{6f29aca3-e0c3-4e92-aa65-1d9ca8596bfe}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class EditBlockedNumberModule : ModuleDefinition {

        public EditBlockedNumberModule() {
            Title = this.__ResStr("modTitle", "Blocked Number");
            Name = this.__ResStr("modName", "Edit Blocked Number");
            Description = this.__ResStr("modSummary", "Edits an existing blocked number");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EditBlockedNumberModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, string number) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Number = number },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit blocked number"),
                Legend = this.__ResStr("editLegend", "Edits an existing blocked number"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
