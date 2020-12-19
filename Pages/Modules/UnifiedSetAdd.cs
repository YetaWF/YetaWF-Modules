/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

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
            Description = this.__ResStr("modSummary", "Adds a new Unified Page Set. This is used by the Unified Page Sets Module to add a new Unified Page Set.");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UnifiedSetAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add(string url) {
            if (YetaWF.Core.Support.Startup.MultiInstance) return null;
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Create a new Unified Page Set"),
                Legend = this.__ResStr("addLegend", "Creates a new Unified Page Set"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

