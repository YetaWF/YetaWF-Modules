/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
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
using YetaWF.Core.Support.StaticPages;
using YetaWF.DataProvider;
using YetaWF.Modules.Dashboard.Endpoints;

namespace YetaWF.Modules.Dashboard.Modules;

public class StaticPagesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, StaticPagesBrowseModule>, IInstallableModel { }

[ModuleGuid("{21b15c5c-d999-424e-8bff-17d9919a9ce8}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class StaticPagesBrowseModule : ModuleDefinition2 {

    public StaticPagesBrowseModule() {
        Title = this.__ResStr("modTitle", "Static Pages");
        Name = this.__ResStr("modName", "Static Pages");
        Description = this.__ResStr("modSummary", "Displays and manages information about static pages. This can be accessed using Admin > Dashboard > Static Pages (standard YetaWF site).");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new StaticPagesBrowseModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        return menuList;
    }

    public ModuleAction GetAction_Items(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Static Pages"),
            MenuText = this.__ResStr("browseText", "Static Pages"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage static pages"),
            Legend = this.__ResStr("browseLegend", "Displays and manages static pages"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction GetAction_Remove(string localUrl) {
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(StaticPagesBrowseModuleEndpoints), StaticPagesBrowseModuleEndpoints.Remove),
            NeedsModuleContext = true,
            QueryArgs = new { LocalUrl = localUrl },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Unload Static Page"),
            MenuText = this.__ResStr("removeMenu", "Unload Loaded Static Page"),
            Tooltip = this.__ResStr("removeTT", "Unload the static page so it is regenerated the next time it's requested"),
            Legend = this.__ResStr("removeLegend", "Unloads the static page so it is regenerated the next time it's requested"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to unload static page \"{0}\"?", localUrl),
        };
    }
    public ModuleAction GetAction_RemoveAll() {
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(StaticPagesBrowseModuleEndpoints), StaticPagesBrowseModuleEndpoints.RemoveAll),
            NeedsModuleContext = true,
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeAllLink", "Unload All"),
            MenuText = this.__ResStr("removeAllMenu", "Unload All"),
            Tooltip = this.__ResStr("removeAllTT", "Unload all static pages and remove the saved data - Pages will be regenerated when they are accessed"),
            Legend = this.__ResStr("removeAllLegend", "Unloads all static pages and removes the saved data - Pages will be regenerated when they are accessed"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = this.__ResStr("toggleremoveAll", "Are you sure you want to unload all static pages?"),
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();
                actions.New(Module.GetAction_Remove(LocalUrl), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }
        }

        [Caption("Local Url"), Description("The local Url of the static page")]
        [UIHint("Url"), ReadOnly]
        public string LocalUrl { get; set; } = null!;
        [Caption("Type"), Description("The type of storage used for the static page")]
        [UIHint("Enum"), ReadOnly]
        public StaticPageManager.PageEntryEnum StorageType { get; set; }
        [Caption("Local Files"), Description("The local file(s) containing the contents of the static page")]
        [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br/>"), ReadOnly]
        public List<string> FileNames { get; set; }

        public StaticPagesBrowseModule Module { get; set; }

        public BrowseItem(StaticPagesBrowseModule module, StaticPageManager.PageEntry data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
            FileNames = new List<string> {
                data.FileName ?? "-",
                data.FileNameHttps ?? "-",
                data.FileNamePopup ?? "-",
                data.FileNamePopupHttps ?? "-",
            };
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
            AjaxUrl = Utility.UrlFor<StaticPagesBrowseModuleEndpoints>(GridSupport.BrowseGridData),
            SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                foreach (BrowseItem r in recs.Data)
                    r.Module = this;
                return new DataSourceResult {
                    Data = recs.Data.ToList<object>(),
                    Total = recs.Total,
                };
            },
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                List<BrowseItem> items = (from k in await Manager.StaticPageManager.GetSiteStaticPagesAsync() select new BrowseItem(this, k)).ToList();
                int total = items.Count;
                DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(items, skip, take, sort, filters);
                DataSourceResult data = new DataSourceResult {
                    Data = recs.Data.ToList<object>(),
                    Total = total
                };
                return data;
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
