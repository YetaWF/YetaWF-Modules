/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules {

    public class AddBlockedNumberModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddBlockedNumberModule>, IInstallableModel { }

    [ModuleGuid("{f57c96c9-f3aa-43f7-b9a6-4e30282889e4}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class AddBlockedNumberModule : ModuleDefinition {

        public AddBlockedNumberModule() {
            Title = this.__ResStr("modTitle", "Add New Blocked Number");
            Name = this.__ResStr("modName", "Add New Blocked Number");
            Description = this.__ResStr("modSummary", "Adds a new blocked number");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AddBlockedNumberModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Add a new blocked number"),
                Legend = this.__ResStr("addLegend", "Adds a new blocked number"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

