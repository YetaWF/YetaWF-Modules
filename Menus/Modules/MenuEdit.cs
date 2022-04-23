/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Menus.Modules {

    public class MenuEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, MenuEditModule>, IInstallableModel { }

    [ModuleGuid("{28CCB0EB-0B46-4e78-A80F-F98DA875EE82}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class MenuEditModule : ModuleDefinition {

        public MenuEditModule() : base() {
            Title = this.__ResStr("modTitle", "Menu");
            Name = this.__ResStr("modName", "Menu Edit");
            Description = this.__ResStr("modSummary", "Implements menu editing. This is used as part of the Main Menu Module and the Menu Module to edit menu settings when in Site Edit Mode.");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new MenuEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, Guid menuGuid) {
            if (!IsAuthorized(RoleDefinition.Edit)) return null;
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                QueryArgs = new { MenuGuid = menuGuid },
                LinkText = this.__ResStr("editLink", "Menu Settings"),
                MenuText = this.__ResStr("editText", "Menu Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit menu settings"),
                Legend = this.__ResStr("editLegend", "Edits menu settings"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | (Manager.EditMode ? ModuleAction.ActionLocationEnum.ModuleMenu : 0),
                SaveReturnUrl = true,
            };
        }
    }
}