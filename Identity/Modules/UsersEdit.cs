/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class UsersEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, UsersEditModule>, IInstallableModel { }

    [ModuleGuid("{31e5b2ed-428c-451c-a25e-5e7e755ef53c}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class UsersEditModule : ModuleDefinition {

        public UsersEditModule() : base() {
            Title = this.__ResStr("modTitle", "Edit User");
            Name = this.__ResStr("modName", "Edit User");
            Description = this.__ResStr("modSummary", "Edits an existing user");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UsersEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, string userName) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { UserName = userName },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit an existing user"),
                Legend = this.__ResStr("editLegend", "Edits an existing user"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}