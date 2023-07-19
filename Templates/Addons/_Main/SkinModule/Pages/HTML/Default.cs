using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Skins;
using $companynamespace$.Modules.$projectnamespace$;

namespace $companynamespace$.Modules.$projectnamespace$.Pages {

    /// <summary>
    /// Implements the Default page.
    /// </summary>
    public class DefaultPage : YetaWFPageBase, IYetaWFPage {

        /// <summary>
        /// Returns the name of the page.
        /// </summary>
        /// <returns>Returns the name of the page.</returns>
        public override string GetPageName() { return SkinAccess.PAGE_VIEW_DEFAULT; }

        /// <summary>
        /// Returns the package implementing the page.
        /// </summary>
        /// <returns>Returns the package implementing the page.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }

        /// <summary>
        /// Returns the names of all panes available in this page.
        /// </summary>
        /// <returns>Returns a collection of pane names available in this page.</returns>
        public List<string> GetPanes() {
            return new List<string> {
                "Header",
                Globals.MainPane,
                "PaneSetL2_LeftPane", "PaneSetL2_MainPane",
                "PaneSet3_LeftPane", "PaneSet3_MainPane", "PaneSet3_RightPane",
                "PaneSetR2_MainPane", "PaneSetR2_RightPane",
                "Footer"
            };
        }

    /// <summary>
    /// Renders the page header (everything before &lt;body&gt;).
    /// </summary>
    /// <returns>The HTML representing the page header.</returns>
    public Task<string> RenderPageHeaderAsync() {

        HtmlBuilder hb = new HtmlBuilder();

        string? favIcon = Manager.CurrentPage.FavIconLink;
        if (string.IsNullOrEmpty(favIcon))
            favIcon = Manager.CurrentSite.FavIconLink;

        hb.Append($@"
<!DOCTYPE html>
<html lang='{Manager.CurrentPage.GetPageLanguageId()}'>
<head>
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    {Manager.MetatagsHtml}
    {Manager.PageTitleHtml}
    {favIcon}
    {Manager.CurrentPage.CanonicalUrlLink}
    {Manager.CurrentPage.HrefLangHtml}
</head>");

        return Task.FromResult(hb.ToString());
    }

    /// <summary>
    /// Renders the page body (&lt;body&gt;, contents and &lt;/body&gt;).
    /// </summary>
    /// <returns>The HTML representing the page body.</returns>
    public async Task<string> RenderPageBodyAsync() {

        HtmlBuilder hb = new HtmlBuilder();

            string? copyright = Manager.CurrentPage.CopyrightEvaluated;
            if (string.IsNullOrEmpty(copyright))
                copyright = Manager.CurrentSite.CopyrightEvaluated;

            hb.Append($@"
<body class='{Manager.PageCss()}'>

    {await HtmlHelper.RenderPageStatus()}

    <!-- Page Header -->
    <div class='container d-print-none'>
        <div class='row'>
            <div class='col-lg-5 mt-0'>
                {await HtmlHelper.RenderModuleAsync<YetaWF.Modules.Text.Modules.TextModule>(new Guid("{AFA55B5E-1CA5-43c7-B9B3-245C172E0819}"), m => {
                    m.Name = "Page Logo (Skin)";
                    m.ShowTitle = false;
                    m.EditOnPage = false;
                    m.CssClass = "skinLogo";
                })}
            </div>
            <div class='col-lg-3 mt-2'>");

            if (Manager.CurrentSite.Localization) {
                hb.Append($@"
                    {await HtmlHelper.RenderModuleAsync<YetaWF.Modules.TinyLanguage.Modules.TinyLanguageModule>(new Guid("{E24356D7-9F90-42e6-A370-ECDB36FD34F5}"), m => {
                        m.Name = "Tiny Language (Skin)";
                        m.CssClass = "skinTinyLanguage";
                    })}");
            }

            hb.Append($@"
            </div>
            <div class='col-lg-4 mt-2 d-flex justify-content-end'>
                {await HtmlHelper.RenderUniqueModuleAsync<YetaWF.Modules.TinyLogin.Modules.TinyLoginModule>(m => {
                    m.CssClass = "skinTinyLogin";
                })}
            </div>
        </div>
    </div>

    <!-- Main Menu Navbar -->
    <nav class='navbar navbar-expand-lg navbar-dark bg-dark d-print-none mt-3 mt-lg-0'>
        <div class='container'>
            <a class='navbar-brand' href='#'></a>
            <div class='navbar-header'>
                <button type='button' class='navbar-toggler' data-toggle='collapse' data-target='.yPageMenu'>
                    <span class='navbar-toggler-icon'></span>
                </button>
            </div>
            {await HtmlHelper.RenderUniqueModuleAsync<YetaWF.Modules.Menus.Modules.MainMenuModule>(m => {
                m.Name = "Main Menu Module (Skin)";
            })}
        </div>
    </nav>

    <!-- Jumbotron -->
    <div class='jumbotron'>
        <div class='container'>
            <div class='row'>
                {await RenderPaneAsync("Header", "col-lg-12 HeaderPane AnyPane", Unified: true)}
            </div>
        </div>
    </div>

    <!-- Panes -->
    <div class='container maincontainer'>
        <div class='row'>
            {await RenderPaneAsync("", "col-lg-12 MainPane AnyPane", Unified: true)}
        </div>
        <div class='row'>
            {await RenderPaneAsync("PaneSetL2_LeftPane", "col-lg-4 PaneSetL2_LeftPane AnyPane", Unified: true)}
            {await RenderPaneAsync("PaneSetL2_MainPane", "col-lg-8 PaneSetL2_MainPane AnyPane", Unified: true)}
        </div>
        <div class='row'>
            {await RenderPaneAsync("PaneSet3_LeftPane", "col-lg-3 PaneSet3_LeftPane AnyPane", Unified: true)}
            {await RenderPaneAsync("PaneSet3_MainPane", "col-lg-6 PaneSet3_MainPane AnyPane", Unified: true)}
            {await RenderPaneAsync("PaneSet3_RightPane", "col-lg-3 PaneSet3_RightPane AnyPane", Unified: true)}
        </div>
        <div class='row'>
            {await RenderPaneAsync("PaneSetR2_MainPane", "col-lg-8 PaneSetR2_MainPane AnyPane", Unified: true)}
            {await RenderPaneAsync("PaneSetR2_RightPane", "col-lg-4 PaneSetR2_RightPane AnyPane", Unified: true)}
        </div>
    </div>

    <div class='PaneSetFooter'>
        <div class='container'>
            <div class='row col-lg-12'>
                {await RenderPaneAsync("Footer", "FooterPane AnyPane", Unified: true)}
            </div>
        </div>
    </div>

    <footer>
        <p>{HE(copyright)}</p>
    </footer>

    {await HtmlHelper.RenderUniqueModuleAddOnsAsync()}
</body>");

            return hb.ToString();
        }

        /// <summary>
        /// Called by the framework for additional processing to be performed.
        /// </summary>
        /// <remarks>A possible use for this method is to add fonts to a page.</remarks>
        public Task AdditionalProcessingAsync() { return Task.CompletedTask; }
    }
}
