/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class RolesEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, RolesEditModule>, IInstallableModel { }

    [ModuleGuid("{e35d6a55-b682-4b4c-9453-04951cc9b9b1}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class RolesEditModule : ModuleDefinition {

        public RolesEditModule() : base() {
            Title = this.__ResStr("modTitle", "Edit a Role");
            Name = this.__ResStr("modName", "Edit Role");
            Description = this.__ResStr("modSummary", "Edits an existing role");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new RolesEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, string name) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Name = name },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit an existing role"),
                Legend = this.__ResStr("editLegend", "Edits an existing role"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
