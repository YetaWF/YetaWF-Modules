/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Search.Controllers;
using YetaWF.Modules.Search.DataProvider;
using YetaWF.Modules.Search.Modules;

namespace YetaWF.Modules.Search.Views {

    public class SearchResultsView : YetaWFView, IYetaWFView<SearchResultsModule, SearchResultsModuleController.Model> {

        public const string ViewName = "SearchResults";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(SearchResultsModule module, SearchResultsModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model.SearchResults.Count == 0) {
                if (Manager.EditMode) {
                    hb.Append($@"
<div class='t_summary t_none'>
    {HE(this.__ResStr("noResultsAdmin", "No search results available."))}
</div>");
                } else {
                    hb.Append($@"
<div class='t_summary t_none'>
    {HE(this.__ResStr("noResults", "Your search for \"{0}\" did not match any documents.", model.SearchTerms))}
</div>");
                }
            } else {

                if (model.MoreResults) {
                    if (model.SearchResults.Count > 1) {

                        hb.Append($@"
<div class='t_summary t_results'>
    {HE(this.__ResStr("resultsMore", "Your search for \"{0}\" found {1} matches - more are available.", model.SearchTerms, model.SearchResults.Count))}
</div>");
                    } else {
                        hb.Append($@"
<div class='t_summary t_results'>
    {HE(this.__ResStr("resultsMore1", "Your search for \"{0}\" found {1} match - more are available.", model.SearchTerms, model.SearchResults.Count))}
</div>");
                    }
                } else {
                    if (model.SearchResults.Count > 1) {
                        hb.Append($@"
<div class='t_summary t_results'>
    {HE(this.__ResStr("results", "Your search for \"{0}\" found {1} matches.", model.SearchTerms, model.SearchResults.Count))}
</div>");
                    } else {
                        hb.Append($@"
<div class='t_summary t_results'>
    {HE(this.__ResStr("results1", "Your search for \"{0}\" found {1} match.", model.SearchTerms, model.SearchResults.Count))}
</div>");
                    }
                }

                hb.Append($@"
<div class='t_list'>
    {await HtmlHelper.ForDisplayAsync(model, nameof(model.SearchTerms))}");

                int count = 0;
                foreach (SearchResult data in model.SearchResults) {
                    string row = string.Format("t_r{0}", count);
                    string url = Manager.CurrentSite.MakeUrl(data.PageUrl, data.PageSecurity);
                    string urlKwds = QueryHelper.ToUrl(url, model.QSArgs);// add search kwds

                    hb.Append($@"
    <div class='t_entry {row}'>
        <div class='t_desc {row}'>");

                    if (!string.IsNullOrWhiteSpace(data.Description)) {

                        hb.Append($@"
            <a href='{HAE(urlKwds)}' target='{(model.NewWindow ? "_blank": "_self")}' class='yaction-link' rel='noopener noreferrer'>
                {HE(data.Description)}
            </a>");

                    } else {
                        hb.Append($@"
            <a href='{HAE(urlKwds)}' target='{(model.NewWindow ? "_blank" : "_self")}' class='yaction-link' rel='noopener noreferrer'>
                {HE(this.__ResStr("noDesc", "(No Description)"))}
            </a>");
                    }

                    hb.Append($@"
        </div>");

                    if (model.ShowSummary && !string.IsNullOrWhiteSpace(data.PageSummary)) {
                        hb.Append($@"
        <div class='t_symmary {row}'>
            {HE(data.PageSummary)}
        </div>");
                    }

                    if (model.ShowUrl) {
                        hb.Append($@"
        <div class='t_url {row}'>
            {HE(url)}
        </div>");
                    }

                    DateTime modified = data.DateCreated;
                    if (data.DateUpdated != null) modified = (DateTime)data.DateUpdated;
                    if (modified != DateTime.MinValue) {
                        hb.Append($@"
        <div class='t_updated {row}'>
            {HE(this.__ResStr("lastUpd", "Updated {0}", Formatting.FormatDateTime(modified)))}
        </div>");
                    }

                    hb.Append($@"
    </div>");
                    ++count;
                }
                hb.Append($@"
</div>");

            }
            return hb.ToString();
        }
    }
}
