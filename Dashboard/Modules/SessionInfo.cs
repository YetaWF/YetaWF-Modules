/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Dashboard.Endpoints;

namespace YetaWF.Modules.Dashboard.Modules;

public class SessionInfoModuleDataProvider : ModuleDefinitionDataProvider<Guid, SessionInfoModule>, IInstallableModel { }

[ModuleGuid("{FDC457A6-EAF7-4874-949F-67AB6DDD5343}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SessionInfoModule : ModuleDefinition2 {

    public SessionInfoModule() {
        Title = this.__ResStr("modTitle", "SessionState Information (HttpContext.Current.Session)");
        Name = this.__ResStr("modName", "SessionState Information (HttpContext.Current.Session)");
        Description = this.__ResStr("modSummary", "Displays SessionState information (HttpContext.Current.Session). Session state information can be accessed using Admin > Dashboard > HttpContext.Current.Session (standard YetaWF site).");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SessionInfoModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "HttpContext.Current.Session"),
            MenuText = this.__ResStr("displayText", "HttpContext.Current.Session"),
            Tooltip = this.__ResStr("displayTooltip", "Display SessionState information (HttpContext.Current.Session)"),
            Legend = this.__ResStr("displayLegend", "Displays SessionState information (HttpContext.Current.Session)"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }
    public ModuleAction GetAction_ClearAll() {
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(SessionInfoModuleEndpoints), SessionInfoModuleEndpoints.ClearAll),
            QueryArgs = new { __ModuleGuid = ModuleGuid },
            Image = "#Remove",
            LinkText = this.__ResStr("removeLink", "Remove Session Settings"),
            MenuText = this.__ResStr("removeText", "Remove Session Settings"),
            Tooltip = this.__ResStr("removeTooltip", "Remove all SessionState information"),
            Legend = this.__ResStr("removeLegend", "Removes all SessionState information"),
            Style = ModuleAction.ActionStyleEnum.Post,
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove all session settings for all users?"),
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.InPopup,
        };
    }

    public class BrowseItem {
        [Caption("Key"), Description("The SessionState key")]
        [UIHint("String"), ReadOnly]
        public string Key { get; set; } = null!;
        [Caption("Value"), Description("The first 100 bytes of the SessionState value")]
        [UIHint("String"), ReadOnly]
        public string Value { get; set; } = null!;
        [Caption("Size"), Description("The size of the value (if available)")]
        [UIHint("FileFolderSize"), ReadOnly]
        public long Size { get; set; }
    }

    public class DisplayModel {

        [Caption("Total Size"), Description("The approximate size of all SessionState items")]
        [UIHint("FileFolderSize"), ReadOnly]
        public long TotalSize { get; set; }

        [Caption("SessionState Items"), Description("The SessionState keys and the values (either the data type or the first 100 bytes of data are shown)")]
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;

        public void SetData(SessionState session) { }
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<SessionInfoModuleEndpoints>(GridSupport.BrowseGridData),
            SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                return new DataSourceResult {
                    Data = recs.Data.ToList<object>(),
                    Total = recs.Total,
                };
            },
            DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                DataProviderGetRecords<BrowseItem> items = DataProviderImpl<BrowseItem>.GetRecords(GetAllItems(), skip, take, sort, filters);
                foreach (BrowseItem item in items.Data)
                    item.Value = item.Value.PadRight(100, ' ').Substring(0, 100).TrimEnd();

                DataSourceResult data = new DataSourceResult {
                    Data = items.Data.ToList<object>(),
                    Total = items.Total,
                };
                return Task.FromResult(data);
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        DisplayModel model = new DisplayModel();
        model.SetData(Manager.CurrentSession);
        model.GridDef = GetGridModel();

        List<BrowseItem> items = GetAllItems();
        model.TotalSize = items.Sum(m => m.Size);
        return await RenderAsync(model);
    }

    private static List<BrowseItem> GetAllItems() {
        SessionState session = Manager.CurrentSession;
        List<BrowseItem> items = (from string item in session.Keys select new BrowseItem { Key = item, Value = (session[item] ?? string.Empty).ToString()!, Size = -1 }).ToList();
        foreach (BrowseItem item in items) {
            object? o = null;
            try {
                o = session[item.Key];
            } catch (Exception) { }
            if (o != null) {
                if (o as byte[] != null)
                    item.Size = ((byte[])o).Length;
                else if (o as string != null)
                    item.Size = item.Value.Length;
                // add more types as needed
            }
        }
        return items;
    }
}
