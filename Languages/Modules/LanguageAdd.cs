/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Languages.Modules {
    public class LanguageAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, LanguageAddModule>, IInstallableModel { }

    [ModuleGuid("{fe177233-0a29-419e-924f-545ec48a7bb5}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class LanguageAddModule : ModuleDefinition {

        public LanguageAddModule() {
            Title = this.__ResStr("modTitle", "Add New Language");
            Name = this.__ResStr("modName", "Add Language");
            Description = this.__ResStr("modSummary", "Adds a new language");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LanguageAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add Language"),
                MenuText = this.__ResStr("addText", "Add Language"),
                Tooltip = this.__ResStr("addTooltip", "Adds a new language"),
                Legend = this.__ResStr("addLegend", "Adds a new language"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

