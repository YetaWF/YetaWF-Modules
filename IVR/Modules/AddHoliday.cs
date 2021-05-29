/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules {

    public class AddHolidayModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddHolidayModule>, IInstallableModel { }

    [ModuleGuid("{0411e732-cf74-4950-8a1e-545566105f7a}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class AddHolidayModule : ModuleDefinition {

        public AddHolidayModule() {
            Title = this.__ResStr("modTitle", "Add New Holiday");
            Name = this.__ResStr("modName", "Add New Holiday Entry");
            Description = this.__ResStr("modSummary", "Adds a new holiday.");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AddHolidayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add(string? url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Add a new holiday entry"),
                Legend = this.__ResStr("addLegend", "Adds a new holiday entry"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

