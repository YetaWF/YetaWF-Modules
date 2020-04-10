/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Languages.Modules {

    public class LanguageDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, LanguageDisplayModule>, IInstallableModel { }

    [ModuleGuid("{4cf7299c-1217-47d1-99bf-14b214f609b6}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class LanguageDisplayModule : ModuleDefinition {

        public LanguageDisplayModule() : base() {
            Title = this.__ResStr("modTitle", "Language");
            Name = this.__ResStr("modName", "Display Language");
            Description = this.__ResStr("modSummary", "Used to display information about a language. This is used by the Languages Module.");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LanguageDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, string id) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Id = id },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Displays an existing language"),
                Legend = this.__ResStr("displayLegend", "Displays an existing language"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}