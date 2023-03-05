/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Modules;

public class VisitorSummaryModuleDataProvider : ModuleDefinitionDataProvider<Guid, VisitorSummaryModule>, IInstallableModel { }

[ModuleGuid("{20b91de2-6bec-4790-8499-1da48fe405f7}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class VisitorSummaryModule : ModuleDefinition2 {

    public VisitorSummaryModule() {
        Title = this.__ResStr("modTitle", "Visitor Activity Summary");
        Name = this.__ResStr("modName", "Visitor Activity Summary");
        Description = this.__ResStr("modSummary", "Displays a summary of yesterday's and today's visitor activity. It is accessible using Admin > Dashboard > Visitor Activity (standard YetaWF site).");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new VisitorSummaryModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string url) {
        return new ModuleAction() {
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

    public class DisplayModel {

        [Caption("Today's Anonymous Visitors"), Description("Displays the number of anonymous visitors today (by reporting the number of distinct sessions without a logged on user)")]
        [UIHint("IntValue"), ReadOnly]
        public int TodaysAnonymous { get; set; }
        [Caption("Today's Visitors"), Description("Displays the number of logged on visitors today")]
        [UIHint("IntValue"), ReadOnly]
        public int TodaysUsers { get; set; }
        [Caption("Yesterday's Anonymous Visitors"), Description("Displays the number of anonymous visitors yesterday (by reporting the number of distinct sessions without a logged on user)")]
        [UIHint("IntValue"), ReadOnly]
        public int YesterdaysAnonymous { get; set; }
        [Caption("Yesterday's Visitors"), Description("Displays the number of logged on visitors yesterday")]
        [UIHint("IntValue"), ReadOnly]
        public int YesterdaysUsers { get; set; }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
            if (visitorDP.Usable) {

                await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.AreaName, ModuleName); // add module specific items

                VisitorEntryDataProvider.Info info = await visitorDP.GetStatsAsync();
                DisplayModel model = new DisplayModel { };
                model.TodaysAnonymous = info.TodaysAnonymous;
                model.TodaysUsers = info.TodaysUsers;
                model.YesterdaysAnonymous = info.YesterdaysAnonymous;
                model.YesterdaysUsers = info.YesterdaysUsers;
                return await RenderAsync(model);
            }
            return ActionInfo.Empty;
        }
    }
}
