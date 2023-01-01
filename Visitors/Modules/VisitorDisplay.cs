/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Modules {

    public class VisitorDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, VisitorDisplayModule>, IInstallableModel { }

    [ModuleGuid("{47362675-fa57-4a47-899d-6a60c263f5c3}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class VisitorDisplayModule : ModuleDefinition {

        public VisitorDisplayModule() {
            Title = this.__ResStr("modTitle", "Visitor Entry Details");
            Name = this.__ResStr("modName", "Visitor Entry Details");
            Description = this.__ResStr("modSummary", "Displays detailed information about a visitor. This is used by the Visitor Activity Module to display a record's detail information.");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new VisitorDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction? GetAction_Display(string? url, int key) {
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                if (!visitorDP.Usable) return null;
            }
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Key = key },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the visitor entry details"),
                Legend = this.__ResStr("displayLegend", "Displays visitor entry details"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
                DontFollow = true,
            };
        }
    }
}