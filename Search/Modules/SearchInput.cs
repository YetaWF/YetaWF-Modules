/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Search.DataProvider;

namespace YetaWF.Modules.Search.Modules;

public class SearchInputModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchInputModule>, IInstallableModel { }

[ModuleGuid("{c7991e91-c691-449a-a911-e5feacfba8a4}")] // Published
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SearchInputModule : ModuleDefinition {

    public SearchInputModule() {
        Title = this.__ResStr("modTitle", "Search");
        Name = this.__ResStr("modName", "Search Input");
        Description = this.__ResStr("modSummary", "Allow search term input.");
        WantFocus = false;
        WantSearch = false;
        Print = false;
        SearchButtonText = this.__ResStr("searchButton", "Search");
        SearchButtonTT = this.__ResStr("searchButtonTT", "Enter search keywords and click the Search button to search this site - You can use AND and OR to combine multiple terms");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SearchInputModuleDataProvider(); }

    [Category("General"), Caption("Search Button"), Description("The text shown on the search button")]
    [UIHint("MultiString20"), StringLength(20), Required]
    public MultiString SearchButtonText { get; set; }

    [Category("General"), Caption("Search Button Tooltip"), Description("The tooltip shown for the search button")]
    [UIHint("MultiString80"), StringLength(500)]
    public MultiString SearchButtonTT { get; set; }

    [Category("General"), Caption("Results Url"), Description("The URL where the search results are displayed")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
    [StringLength(Globals.MaxUrl), Required, Trim]
    [Data_NewValue]
    public string? ResultsUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public async Task<ModuleAction> GetAction_SearchAsync(string url, string searchTerms) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { SearchTerms = searchTerms },
            Image = await CustomIconAsync("SearchInput.png"),
            ImageSVG = SkinSVGs.GetSkin("FAV_Search"),
            LinkText = this.__ResStr("editLink", "Search"),
            MenuText = this.__ResStr("editText", "Search"),
            Tooltip = this.__ResStr("editTooltip", "Search for keywords on this site"),
            Legend = this.__ResStr("editLegend", "Allows you to search this site for keywords"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }

    [Trim]
    public class Model {

        [TextAbove("Enter the search term(s). Use AND or OR to create search queries. When multiple terms (without AND or OR) are used, all terms must be found in a page to be listed in the search results. Use * at the end of a search term to search for all occurrences that start with the specified term (wildcard searching).")]
        [Caption("Search Terms"), Description("Enter the search term(s) - Use AND or OR to create search queries - When multiple terms (without AND or OR) are used, all terms must be found in a page to be listed in the search results - Use * at the end of a search term to search for all occurrences that start with the specified term (wildcard searching)")]
        [UIHint("Text40"), Trim, StringLength(SearchData.MaxSearchTerm), Required]
        public string? SearchTerms { get; set; }

        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync(string searchTerms) {
        if (!SearchDataProvider.IsUsable)
            return await RenderAsync(new { }, ViewName: "SearchUnavailable_Input");
        if (!Manager.EditMode && string.IsNullOrWhiteSpace(ResultsUrl)) // if no search result url is available, don't show the module
            throw new InternalError($"No {nameof(ResultsUrl)} defined for search results");
        Model model = new Model { SearchTerms = searchTerms };
        return await RenderAsync(model);
    }

    public async Task<IResult> UpdateModuleAsync(Model model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        QueryHelper query = new QueryHelper();
        query["SearchTerms"] = model.SearchTerms;
        string url = query.ToUrl(ResultsUrl ?? Manager.CurrentSite.HomePageUrl);
        return await FormProcessedAsync(model, NextPage: url);
    }
}