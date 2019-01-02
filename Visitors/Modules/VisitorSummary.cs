/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Visitors.Modules {

    public class VisitorSummaryModuleDataProvider : ModuleDefinitionDataProvider<Guid, VisitorSummaryModule>, IInstallableModel { }

    [ModuleGuid("{20b91de2-6bec-4790-8499-1da48fe405f7}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class VisitorSummaryModule : ModuleDefinition {

        public VisitorSummaryModule() {
            Title = this.__ResStr("modTitle", "Visitor Activity Summary");
            Name = this.__ResStr("modName", "Visitor Activity Summary");
            Description = this.__ResStr("modSummary", "Displays a visitor summary");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new VisitorSummaryModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display a visitor summary"),
                Legend = this.__ResStr("displayLegend", "Displays a visitor summary"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
