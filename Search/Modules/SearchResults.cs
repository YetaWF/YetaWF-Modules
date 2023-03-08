/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Search.DataProvider;

namespace YetaWF.Modules.Search.Modules;

public class SearchResultsModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchResultsModule>, IInstallableModel { }

[ModuleGuid("{5f786472-884b-47db-9704-d50690003dc9}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SearchResultsModule : ModuleDefinition {

    public SearchResultsModule() {
        Title = this.__ResStr("modTitle", "Search Results");
        Name = this.__ResStr("modName", "Search Results");
        Description = this.__ResStr("modSummary", "Displays search results. The Search Input Module uses the Search Results Module.");
        WantFocus = false;
        WantSearch = false;
        MaxResults = 20;
        ShowUrl = true;
        ShowSummary = true;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SearchResultsModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Max. Results"), Description("The maximum number of search results to display")]
    [UIHint("IntValue4"), Range(10, 1000)]
    [Data_NewValue]
    public int MaxResults { get; set; }

    [Category("General"), Caption("Show Url"), Description("Defines whether the URL is shown in search results")]
    [UIHint("Boolean")]
    [Data_NewValue]
    public bool ShowUrl { get; set; }

    [Category("General"), Caption("New Window"), Description("Defines whether a new window is opened when a search result URL is clicked")]
    [UIHint("Boolean")]
    [Data_NewValue]
    public bool NewWindow { get; set; }

    [Category("General"), Caption("Show Summary"), Description("Defines whether a page summary is shown in search results")]
    [UIHint("Boolean")]
    [Data_NewValue]
    public bool ShowSummary { get; set; }

    public async Task<ModuleAction> GetAction_GetResultsAsync(string url, string searchTerms) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { SearchTerms = searchTerms },
            Image = await CustomIconAsync("SearchResults.png"),
            LinkText = this.__ResStr("resultsLink", "Search Results"),
            MenuText = this.__ResStr("resultsText", "Search Results"),
            Tooltip = this.__ResStr("resultsTooltip", "Display the search results for \"{0}\"", searchTerms),
            Legend = this.__ResStr("resultsLegend", "Displays the search results for \"{0}\"", searchTerms),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }

    public class Model {

        [UIHint("Hidden")]
        public string SearchTerms { get; set; } = null!;
        public bool MoreResults { get; set; }
        public int MaxResults { get; set; }
        public List<SearchResult> SearchResults { get; set; } = null!;
        public bool ShowUrl { get; set; }
        public bool ShowSummary { get; set; }
        public bool NewWindow { get; set; }
        public string QSArgs { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync(string searchTerms) {
        if (string.IsNullOrWhiteSpace(searchTerms)) return ActionInfo.Empty;

        if (!SearchDataProvider.IsUsable)
            return await RenderAsync(new { }, ViewName: "SearchUnavailable_Results");

        using (SearchResultDataProvider searchResDP = new SearchResultDataProvider()) {
            SearchResultDataProvider.SearchResultsInfo list = await searchResDP.GetSearchResultsAsync(searchTerms, MaxResults, MultiString.ActiveLanguage, Manager.HaveUser);
            Model model = new Model() {
                SearchTerms = searchTerms,
                SearchResults = list.Data,
                MoreResults = list.HaveMore,
                NewWindow = NewWindow,
                MaxResults = MaxResults,
                ShowUrl = ShowUrl,
                ShowSummary = ShowSummary,
                QSArgs = searchResDP.GetQueryArgsFromKeywords(searchTerms),
            };
            return await RenderAsync(model);
        }
    }
}