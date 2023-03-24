/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Modules;

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
        return new ModuleAction() {
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

    public class DisplayModel {

        [UIHint("Hidden")]
        public int Key { get; set; }

        [Caption("Accessed"), Description("The date and time the visitor visited the site")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime AccessDateTime { get; set; }

        [Caption("Session Id"), Description("The session id used to identify the visitor")]
        [UIHint("String"), ReadOnly]
        public string? SessionId { get; set; }

        [Caption("User Id"), Description("The user's email address (if available)")]
        [UIHint("YetaWF_Identity_UserId"), ReadOnly]
        public int UserId { get; set; }

        [Caption("IP Address"), Description("The IP address of the site visitor")]
        [UIHint("IPAddress"), ReadOnly]
        public string? IPAddress { get; set; }
        [Caption("Url"), Description("The Url accessed by the site visitor")]
        [UIHint("Url"), ReadOnly]
        public string? Url { get; set; }
        [Caption("Referrer"), Description("The Url where the site visitor came from")]
        [UIHint("Url"), ReadOnly]
        public string? Referrer { get; set; }
        [Caption("User Agent"), Description("The web browser's user agent")]
        [UIHint("String"), ReadOnly]
        public string? UserAgent { get; set; }
        [Caption("Error"), Description("Shows any error that may have occurred")]
        [UIHint("String"), ReadOnly]
        public string? Error { get; set; }

        public void SetData(VisitorEntry data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(int key) {
        using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
            VisitorEntry? data = await visitorDP.GetItemAsync(key);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Visitor Entry {0} not found."), key);
            DisplayModel model = new DisplayModel();
            model.SetData(data);
            Title = this.__ResStr("title", "Visitor Entry {0}", key);
            return await RenderAsync(model);
        }
    }
}