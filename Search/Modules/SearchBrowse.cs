/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

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
using YetaWF.Modules.Search.DataProvider;
using YetaWF.Modules.Search.Endpoints;

namespace YetaWF.Modules.Search.Modules;

public class SearchBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchBrowseModule>, IInstallableModel { }

[ModuleGuid("{579f8078-c443-4ca8-9f1c-189b0935303a}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SearchBrowseModule : ModuleDefinition2 {

    public SearchBrowseModule() {
        Title = this.__ResStr("modTitle", "Search Keywords");
        Name = this.__ResStr("modName", "Search Keywords");
        Description = this.__ResStr("modSummary", "Displays and manages search keywords that were found in the site's pages. It is accessible using Admin > Panel > Search Keywords (standard YetaWF site). This module offers a Collect Keywords action, which can be used to extract all page keywords immediately (instead of by the Scheduler at regular intervals). This task may run for a long time. Editing and removing a search keyword is available in the Search Keywords grid shown by the Search Keywords Module.");
        DefaultViewName = StandardViews.Browse;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SearchBrowseModuleDataProvider(); }

    [Category("General"), Caption("Add Url"), Description("The Url to add a new search keyword - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? AddUrl { get; set; }
    [Category("General"), Caption("Display Url"), Description("The Url to display a search keyword - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }
    [Category("General"), Caption("Edit Url"), Description("The Url to edit a search keyword - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? EditUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveItems",
                    this.__ResStr("roleRemItemsC", "Remove Search Items"), this.__ResStr("roleRemItems", "The role has permission to remove individual search keywords"),
                    this.__ResStr("userRemItemsC", "Remove Search Items"), this.__ResStr("userRemItems", "The user has permission to remove individual search keywords")),
                new RoleDefinition("CollectKeywords",
                    this.__ResStr("roleCollectC", "Collect Keywords"), this.__ResStr("roleCollect", "The role has permission to collect a site's search keywords"),
                    this.__ResStr("userCollectC", "Collect Keywords"), this.__ResStr("userCollect", "The user has permission to collect a site's search keywords")),
            };
        }
    }

    public ModuleAction? GetAction_Items(string? url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Search Keywords"),
            MenuText = this.__ResStr("browseText", "Search Keywords"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage search keywords"),
            Legend = this.__ResStr("browseLegend", "Displays and manages search keywords"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_Remove(int searchDataId) {
        if (!IsAuthorized("RemoveItems")) return null;
        return new ModuleAction(this) {
            Url = $"{Utility.UrlFor(typeof(SearchBrowseModuleEndpoints), SearchBrowseModuleEndpoints.Remove)}/{searchDataId}",
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove search keyword"),
            MenuText = this.__ResStr("removeMenu", "Remove search keyword"),
            Tooltip = this.__ResStr("removeTT", "Remove the search keyword"),
            Legend = this.__ResStr("removeLegend", "Removes the search keyword"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this keyword?"),
        };
    }
    public ModuleAction? GetAction_RemoveAll() {
        if (!IsAuthorized("RemoveItems")) return null;
        if (!SearchDataProvider.IsUsable) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(SearchBrowseModuleEndpoints), SearchBrowseModuleEndpoints.RemoveAll),
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeAllLink", "Remove All"),
            MenuText = this.__ResStr("removeAllMenu", "Remove All"),
            Tooltip = this.__ResStr("removeAllTT", "Remove all search keyword"),
            Legend = this.__ResStr("removeAllLegend", "Remove all search keyword"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = this.__ResStr("removeAllConfirm", "Are you sure you want to remove ALL keywords?"),
            PleaseWaitText = this.__ResStr("removeAllPlsWait", "Keywords are being removed..."),
        };
    }
    public async Task<ModuleAction?> GetAction_CollectKeywordsAsync() {
        if (!IsAuthorized("CollectKeywords")) return null;
        if (!SearchDataProvider.IsUsable) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(SearchBrowseModuleEndpoints), SearchBrowseModuleEndpoints.CollectKeywords),
            Image = await CustomIconAsync("CollectKeywords.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("collectLink", "Collect Keywords"),
            MenuText = this.__ResStr("collectMenu", "Collect Keywords"),
            Tooltip = this.__ResStr("collectTT", "Collect the site's search keywords by examining all pages - Only considers pages modified since the last search"),
            Legend = this.__ResStr("collectLegend", "Collects a site's search keywords by examining all pages - Only considers pages modified since the last search"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = this.__ResStr("collectConfirm", "Are you sure you want to collect all keywords for this site?"),
            PleaseWaitText = this.__ResStr("collectPlsWait", "Keywords are being collected..."),
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();

                SearchEditModule editMod = new SearchEditModule();
                actions.New(editMod.GetAction_Edit(Module.EditUrl, SearchDataId), ModuleAction.ActionLocationEnum.GridLinks);

                actions.New(Module.GetAction_Remove(SearchDataId), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }
        }

        [Caption("Search Keyword"), Description("")]
        [UIHint("String"), ReadOnly]
        public string SearchTerm { get; set; } = null!;

        [Caption("Url"), Description("The page where this keyword was found")]
        [UIHint("Url"), ReadOnly]
        public string PageUrl { get; set; } = null!;

        [Caption("Title"), Description("The page title")]
        [UIHint("String"), ReadOnly]
        public string? PageTitle { get; set; }

        [Caption("Summary"), Description("The page summary")]
        [UIHint("String"), ReadOnly]
        public string? PageSummary { get; set; }

        [Caption("Created"), Description("The date/time the page was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime DatePageCreated { get; set; }
        [Caption("Updated"), Description("The date/time the page was last updated")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime DatePageUpdated { get; set; }

        [Caption("Added"), Description("The date/time this keyword was added")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime DateAdded { get; set; }

        [Caption("Language"), Description("The page language where this keyword was found")]
        [UIHint("String"), ReadOnly]
        public string Language { get; set; } = null!;

        [Caption("Count"), Description("The number of times this keyword was found on the page")]
        [UIHint("IntValue"), ReadOnly]
        public int Count { get; set; }

        [Caption("Anonymous Users"), Description("Whether anonymous users can view the page")]
        [UIHint("Boolean"), ReadOnly]
        public bool AllowAnonymous { get; set; }

        [Caption("Any Users"), Description("Whether any logged on user can view the page")]
        [UIHint("Boolean"), ReadOnly]
        public bool AllowAnyUser { get; set; }

        [Caption("Id"), Description("The internal id this keyword")]
        [UIHint("IntValue"), ReadOnly]
        public int SearchDataId { get; set; }

        private SearchBrowseModule Module { get; set; }

        public BrowseItem(SearchBrowseModule module, SearchData data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }

    public class BrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            InitialPageSize = 20,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<SearchBrowseModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (SearchDataProvider searchDP = new SearchDataProvider()) {
                    DataProviderGetRecords<SearchData> browseItems = await searchDP.GetItemsWithUrlAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!SearchDataProvider.IsUsable)
            return await RenderAsync(new { }, ViewName: "SearchUnavailable_Browse");
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}