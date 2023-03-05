/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
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

public class DisposableTrackerBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisposableTrackerBrowseModule>, IInstallableModel { }

[ModuleGuid("{56e429a9-2c8d-49dd-8e78-fb1202aefb93}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class DisposableTrackerBrowseModule : ModuleDefinition2 {

    public DisposableTrackerBrowseModule() {
        Title = this.__ResStr("modTitle", "Disposable Objects");
        Name = this.__ResStr("modName", "Disposable Objects");
        Description = this.__ResStr("modSummary", "Displays (debug) information about Disposable objects. Used to find leaks where objects aren't properly Dispose()'d. Disposable Objects information can be accessed using Admin > Dashboard > Disposable Objects (standard YetaWF site).");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new DisposableTrackerBrowseModuleDataProvider(); }

    [Category("General"), Caption("Display Url"), Description("The Url to display a disposable object - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Items(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Disposable Objects"),
            MenuText = this.__ResStr("browseText", "Disposable Objects"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage disposable objects"),
            Legend = this.__ResStr("browseLegend", "Displays and manages disposable objects"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();
                return actions;
            }
        }

        [Caption("Type"), Description("The object type of the disposable object that is currently in use")]
        [UIHint("String"), ReadOnly]
        public string DisposableObject { get; set; } = null!;
        [Caption("Time"), Description("The time the object was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }
        [Caption("Callstack"), Description("The callstack at the time the object was created")]
        [UIHint("String"), ReadOnly]
        public string? CallStack { get; set; }

        public DisposableTrackerBrowseModule Module { get; set; }

        public BrowseItem(DisposableTrackerBrowseModule module, TrackedEntry data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
            DisposableObject = data.DisposableObject.GetType().FullName!;
        }
    }
    public class BrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            InitialPageSize = 20,
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<DisposableTrackerBrowseModuleEndpoints>(GridSupport.BrowseGridData),
            SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                foreach (BrowseItem r in recs.Data)
                    r.Module = this;
                return new DataSourceResult {
                    Data = recs.Data.ToList<object>(),
                    Total = recs.Total,
                };
            },
            DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                List<BrowseItem> items = (from k in DisposableTracker.GetDisposableObjects() select new BrowseItem(this, k)).ToList();
                DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(items, skip, take, sort, filters);
                DataSourceResult data = new DataSourceResult {
                    Data = recs.Data.ToList<object>(),
                    Total = recs.Total
                };
                return Task.FromResult(data);
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
