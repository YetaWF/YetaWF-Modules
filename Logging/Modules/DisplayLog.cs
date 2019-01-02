/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Logging.Modules {

    public class DisplayLogModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisplayLogModule>, IInstallableModel { }

    [ModuleGuid("{8327e155-409c-438e-83ef-1f7f7ac1e951}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class DisplayLogModule : ModuleDefinition {

        public DisplayLogModule() : base() {
            Title = this.__ResStr("modTitle", "Log Record");
            Name = this.__ResStr("modName", "Display Log Record");
            Description = this.__ResStr("modSummary", "A module used to display a log record.");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new DisplayLogModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, int record) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Key = record.ToString() },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the log record"),
                Legend = this.__ResStr("displayLegend", "Displays the log record"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
                DontFollow = true,
            };
        }
    }
}