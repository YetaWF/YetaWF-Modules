/* Copyright � 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Pages.Modules {

    public class UnifiedSetAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, UnifiedSetAddModule>, IInstallableModel { }

    [ModuleGuid("{e79b556b-6adf-485f-9464-c4be1aaccbcc}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class UnifiedSetAddModule : ModuleDefinition {

        public UnifiedSetAddModule() {
            Title = this.__ResStr("modTitle", "Add New Unified Page Set");
            Name = this.__ResStr("modName", "Add New Unified Page Set");
            Description = this.__ResStr("modSummary", "Creates a new unified page set");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UnifiedSetAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Create a new unified page set"),
                Legend = this.__ResStr("addLegend", "Creates a new unified page set"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}
