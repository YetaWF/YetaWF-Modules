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
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Dashboard.Endpoints;

namespace YetaWF.Modules.Dashboard.Modules;

public class AddonsBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddonsBrowseModule>, IInstallableModel { }

[ModuleGuid("{2d30fa39-622d-45eb-b52c-530699b2c9ae}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class AddonsBrowseModule : ModuleDefinition {

    public AddonsBrowseModule() {
        Title = this.__ResStr("modTitle", "AddOn Info");
        Name = this.__ResStr("modName", "AddOn Info");
        Description = this.__ResStr("modSummary", "Displays information about all JavaScript AddOns, CSS AddOns that are installed in the current YetaWF instance. AddOn information can be accessed using Admin > Dashboard > AddOn Info (standard YetaWF site).");
        DefaultViewName = StandardViews.PropertyListEdit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AddonsBrowseModuleDataProvider(); }

    [Category("General"), Caption("Display Url"), Description("The Url to display a AddOn info - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Items(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "AddOn Info"),
            MenuText = this.__ResStr("browseText", "AddOn Info"),
            Tooltip = this.__ResStr("browseTooltip", "Display information about all JavaScript AddOns, Css AddOns that are installed in the current YetaWF instance"),
            Legend = this.__ResStr("browseLegend", "Displays information about all JavaScript AddOns, Css AddOns that are installed in the current YetaWF instance"),
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

                AddonDisplayModule dispMod = new AddonDisplayModule();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, AddonKey), ModuleAction.ActionLocationEnum.GridLinks);

                return actions;
            }
        }

        [Caption("Type"), Description("The AddOn type")]
        [UIHint("Enum"), ReadOnly]
        public Package.AddOnType Type { get; set; }
        [Caption("Domain"), Description("The domain owning this AddOn")]
        [UIHint("String"), ReadOnly]
        public string Domain { get; set; } = null!;
        [Caption("Product"), Description("The AddOn's product name")]
        [UIHint("String"), ReadOnly]
        public string Product { get; set; } = null!;
        [Caption("Version"), Description("The AddOn's version")]
        [UIHint("String"), ReadOnly]
        public string Version { get; set; } = null!;
        [Caption("Name"), Description("The AddOn's internal name")]
        [UIHint("String"), ReadOnly]
        public string Name { get; set; } = null!;
        [Caption("Url"), Description("The AddOn's Url where its files are located")]
        [UIHint("String"), ReadOnly]
        public string Url { get; set; } = null!;

        public AddonsBrowseModule Module { get; set; }

        [UIHint("Hidden"), ReadOnly]
        public string AddonKey { get; set; } = null!;

        public BrowseItem(AddonsBrowseModule module, Package.AddOnProduct data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }

    public class BrowseModel {

        [Caption("AddOns Url"), Description("The Url containing all AddOns")]
        [UIHint("String"), ReadOnly]
        public string AddOnsUrl { get; set; } = null!;

        [Caption("Custom AddOns Url"), Description("The Url containing all customized AddOns (if any)")]
        [UIHint("String"), ReadOnly]
        public string AddOnsCustomUrl { get; set; } = null!;

        [Caption("NPM Url"), Description("The Url containing npm modules")]
        [UIHint("String"), ReadOnly]
        public string NodeModulesUrl { get; set; } = null!;

        [Caption("Installed AddOns"), Description("Displays all installed AddOns")]
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            InitialPageSize = 20,
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor(typeof(AddonsBrowseModuleEndpoints), GridSupport.BrowseGridData),
            SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                foreach (BrowseItem r in recs.Data)
                    r.Module = this;
                return new DataSourceResult {
                    Data = recs.Data.ToList<object>(),
                    Total = recs.Total,
                };
            },
            DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                List<BrowseItem> list = (from a in Package.GetAvailableAddOns() select new BrowseItem(this, a)).ToList();
                DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(list, skip, take, sorts, filters);
                foreach (BrowseItem r in recs.Data)
                    r.Module = this;
                DataSourceResult data = new DataSourceResult {
                    Data = recs.Data.ToList<object>(),
                    Total = list.Count,
                };
                return Task.FromResult(data);
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            AddOnsUrl = Package.AddOnsUrl,
            AddOnsCustomUrl = Package.AddOnsCustomUrl,
            NodeModulesUrl = Globals.NodeModulesUrl,
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}