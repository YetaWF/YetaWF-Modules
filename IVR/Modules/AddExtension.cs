/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules {

    public class AddExtensionModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddExtensionModule>, IInstallableModel { }

    [ModuleGuid("{cb461097-1109-4a5b-8514-5af0260b98c7}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class AddExtensionModule : ModuleDefinition {

        public AddExtensionModule() {
            Title = this.__ResStr("modTitle", "Add New Extension");
            Name = this.__ResStr("modName", "Add New Extension");
            Description = this.__ResStr("modSummary", "Adds a new extension.");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AddExtensionModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction? GetAction_Add(string? url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Add a new extension"),
                Legend = this.__ResStr("addLegend", "Adds a new extension"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

