/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Search.Modules {

    public class SearchResultsModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchResultsModule>, IInstallableModel { }

    [ModuleGuid("{5f786472-884b-47db-9704-d50690003dc9}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SearchResultsModule : ModuleDefinition {

        public SearchResultsModule() {
            Title = this.__ResStr("modTitle", "Search Results");
            Name = this.__ResStr("modName", "Search Results");
            Description = this.__ResStr("modSummary", "Displays search results");
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
            return new ModuleAction(this) {
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
    }
}