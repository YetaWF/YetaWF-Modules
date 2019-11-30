/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.DockerRegistry.Controllers;

namespace YetaWF.Modules.DockerRegistry.Modules {

    public class AddRegistryEntryModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddRegistryEntryModule>, IInstallableModel { }

    [ModuleGuid("{8a115c41-7508-4279-8d69-c436bf808c61}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)] // unique so we can set skin props without page and so NeedsModuleContext for remove works
    public class AddRegistryEntryModule : ModuleDefinition {

        public AddRegistryEntryModule() {
            Title = this.__ResStr("modTitle", "Add New Registry Server");
            Name = this.__ResStr("modName", "Add New Registry Server");
            Description = this.__ResStr("modSummary", "Adds a new registry server");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AddRegistryEntryModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add() {
            return new ModuleAction(this) {
                Url = ModulePermanentUrl,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Add a new registry server"),
                Legend = this.__ResStr("addLegend", "Adds a new registry server"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
        public ModuleAction GetAction_Remove(int registryId) {
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(AddRegistryEntryModuleController), nameof(AddRegistryEntryModuleController.Remove)),
                NeedsModuleContext = true,
                QueryArgs = new { RegistryId = registryId },
                Image = "#Remove",
                LinkText = this.__ResStr("remLink", "Remove"),
                MenuText = this.__ResStr("remText", "Remove"),
                Tooltip = this.__ResStr("remTooltip", "Remove the selected registry server"),
                Legend = this.__ResStr("remLegend", "Removes the selected registry server"),
                Style = ModuleAction.ActionStyleEnum.Post,
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this registry server?"),
            };
        }
    }
}

