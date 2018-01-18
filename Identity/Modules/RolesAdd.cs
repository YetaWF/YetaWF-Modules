/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {
    public class RolesAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, RolesAddModule>, IInstallableModel { }

    [ModuleGuid("{97285509-fb4e-4f13-a3bc-cd4957f1cff0}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class RolesAddModule : ModuleDefinition {

        public RolesAddModule() {
            Title = this.__ResStr("modTitle", "New Role");
            Name = this.__ResStr("modName", "New Role");
            Description = this.__ResStr("modSummary", "Adds a new role");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new RolesAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Create a new role"),
                Legend = this.__ResStr("addLegend", "Creates a new role"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

