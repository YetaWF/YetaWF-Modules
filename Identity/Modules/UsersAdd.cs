/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {
    public class UsersAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, UsersAddModule>, IInstallableModel { }

    [ModuleGuid("{55928a06-793e-46d1-929e-e403a59de98a}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class UsersAddModule : ModuleDefinition {

        public UsersAddModule() {
            Title = this.__ResStr("modTitle", "New User");
            Name = this.__ResStr("modName", "New User");
            Description = this.__ResStr("modSummary", "Creates a new user");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UsersAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Create a new user"),
                Legend = this.__ResStr("addLegend", "Creates a new user"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

