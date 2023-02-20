/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

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
using YetaWF.Modules.LoggingDataProvider.DataProvider;

namespace YetaWF.Modules.Logging.Modules;

public class DisplayLogModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisplayLogModule>, IInstallableModel { }

[ModuleGuid("{8327e155-409c-438e-83ef-1f7f7ac1e951}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class DisplayLogModule : ModuleDefinition2 {

    public DisplayLogModule() : base() {
        Title = this.__ResStr("modTitle", "Log Record");
        Name = this.__ResStr("modName", "Display Log Record");
        Description = this.__ResStr("modSummary", "Displays an individual log entry. This is used by the Log Module to display a log entry and is only available if the log file is PostgreSQL/SQL Server hosted.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new DisplayLogModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Display(string? url, int record) {
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

    public class DisplayModel {

        [UIHint("Hidden")]
        public int Key { get; set; }

        [Caption("Date/Time"), Description("The time this record was logged")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime TimeStamp { get; set; }

        [Caption("Session Id"), Description("The session id used to identify the visitor")]
        [UIHint("String"), ReadOnly]
        public string SessionId { get; set; } = null!;

        [Caption("Level"), Description("The error level of this log record")]
        [UIHint("Enum"), ReadOnly]
        public Core.Log.Logging.LevelEnum Level { get; set; }

        [Caption("Info"), Description("The information logged in this record")]
        [UIHint("String"), ReadOnly]
        public string Info { get; set; } = null!;

        [Caption("Site Id"), Description("The site which logged this record")]
        [UIHint("IntValue"), ReadOnly]
        public int SiteIdentity { get; set; }

        [Caption("IP Address"), Description("The IP address associated with this log entry")]
        [UIHint("IPAddress"), ReadOnly]
        public string IPAddress { get; set; } = null!;
        [Caption("Url"), Description("The requested Url")]
        [UIHint("Url"), ReadOnly]
        public string RequestedUrl { get; set; } = null!;
        [UIHint("Url"), ReadOnly]
        [Caption("Referrer"), Description("The referring Url associated with this log entry")]
        public string ReferrerUrl { get; set; } = null!;

        [Caption("User"), Description("The user's name/email address (if available)")]
        [UIHint("YetaWF_Identity_UserId"), ReadOnly]
        public int UserId { get; set; }

        [Caption("Module Name"), Description("The module logging this record")]
        [UIHint("String"), ReadOnly]
        public string ModuleName { get; set; } = null!;
        [Caption("Class"), Description("The class logging this record")]
        [UIHint("String"), ReadOnly]
        public string Class { get; set; } = null!;
        [Caption("Method"), Description("The method logging this record")]
        [UIHint("String"), ReadOnly]
        public string Method { get; set; } = null!;
        [Caption("Namespace"), Description("The namespace logging this record")]
        [UIHint("String"), ReadOnly]
        public string Namespace { get; set; } = null!;

        public void SetData(LogRecord data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!int.TryParse(Manager.RequestQueryString["Key"], out int key)) key = 0;
        using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
            LogRecord? data = await dataProvider.GetItemAsync(key);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Record \"{0}\" not found."), key);
            DisplayModel model = new DisplayModel();
            model.SetData(data);
            return await RenderAsync(model);
        }
    }
}
