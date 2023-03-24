/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.PageEdit.Endpoints;

namespace YetaWF.Modules.PageEdit.Modules;

public class PageEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, PageEditModule>, IInstallableModel { }

[ModuleGuid("{FBB3C6D3-FBD2-4ab1-BF0E-8716F3D1B052}"), PublishedModuleGuid]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class PageEditModule : ModuleDefinition {

    public PageEditModule() : base() {
        Title = this.__ResStr("modTitle", "Page Edit");
        Name = this.__ResStr("modName", "Page Edit");
        Description = this.__ResStr("modSummary", "Implements page editing services.");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new PageEditModuleDataProvider(); }

    public override bool ShowModuleMenu { get { return false; } }

    public async Task<ModuleAction?> GetAction_EditAsync(string? url, Guid? pageGuid = null) {
        Guid guid;
        if (pageGuid == null) {
            if (Manager.CurrentPage == null) return null;
            if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_PageSettings))
                return null;
            guid = Manager.CurrentPage.PageGuid;
        } else {
            guid = (Guid)pageGuid;
        }
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { PageGuid = guid },
            Image = "#Edit",
            Name = "PageSettings",
            LinkText = this.__ResStr("editLink", "Page Settings"),
            MenuText = this.__ResStr("editText", "Page Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit page settings"),
            Legend = this.__ResStr("editLegend", "Edits page settings"),
            
            Style = ModuleAction.ActionStyleEnum.PopupEdit,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            Mode = ModuleAction.ActionModeEnum.Any,
            DontFollow = true,

            SaveReturnUrl = true,
        };
    }
    public async Task<ModuleAction?> GetAction_RemoveAsync(Guid? pageGuid = null) {
        Guid guid;
        PageDefinition? page;
        if (pageGuid == null) {
            page = Manager.CurrentPage;
            if (page == null) return null;
            guid = page.PageGuid;
        } else {
            guid = (Guid)pageGuid;
            page = await PageDefinition.LoadAsync(guid);
        }
        if (page == null) return null;
        if (!page.IsAuthorized_Remove()) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(PageEditModuleEndpoints), nameof(PageEditModuleEndpoints.RemovePage)),
            QueryArgs = new { PageGuid = guid },
            Image = "#Remove",
            Name = "RemovePage",
            LinkText = this.__ResStr("delLink", "Remove Current Page"),
            MenuText = this.__ResStr("delText", "Remove Current Page"),
            Tooltip = this.__ResStr("delTooltip", "Remove the current page"),
            Legend = this.__ResStr("delLegend", "Removes the current page"),
            Style = ModuleAction.ActionStyleEnum.Post,
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            Mode = ModuleAction.ActionModeEnum.Any,
            ConfirmationText = this.__ResStr("delConfirm", "Are you ABSOLUTELY sure you want to remove the currently displayed page? This action cannot be undone."),
        };
    }

    [Trim]
    public class EditablePage {

        public virtual List<string> CategoryOrder { get { return new List<string> { "Page", "Authorization", "Urls", "Skin", "References", "Addons", "Meta", "Variables" }; } }

        [Category("Page"), Caption("Url"), Description("The Url used to identify this page - local Urls start with / and do not include http:// or https://")]
        [UIHint("Text80"), StringLength(Globals.MaxUrl), UrlValidation(urlType: UrlTypeEnum.New), Required, Trim]
        public string? Url { get; set; }

        [Category("Page"), Caption("Canonical Url"), Description("The optional complete Url used to identify this page (including query string) - If not specified, the Url is used instead - The data entered is used as-is but allows variable substitution - Special characters in the query string portion must be encoded")]
        [UIHint("Text80"), StringLength(Globals.MaxUrl), Trim]
        public string? CanonicalUrl { get; set; }

        [Category("Page"), Caption("Title"), Description("The page title which will appear as title in the browser window")]
        [UIHint("MultiString80"), StringLength(PageDefinition.MaxTitle), Trim]
        public MultiString Title { get; set; }

        [Category("Page"), Caption("Description"), Description("The page description - This description is used for meta tags and when information about this page is displayed")]
        [UIHint("MultiString80"), StringLength(PageDefinition.MaxDescription), Trim]
        public MultiString Description { get; set; }

        [Category("Page"), Caption("Keywords"), Description("Defines page keywords - These keywords are used for meta tags and to generate search terms")]
        [UIHint("MultiString80"), StringLength(PageDefinition.MaxKeywords), Trim]
        public MultiString Keywords { get; set; }

        [Category("Page"), Caption("Search Keywords"), Description("Defines whether this page's contents should be added to the site's search keywords")]
        [UIHint("Boolean")]
        public bool WantSearch { get; set; }

        [Category("Variables"), Caption("Temporary"), Description("Defines whether the current page is a temporary (generated) page")]
        [UIHint("Boolean"), ReadOnly, SuppressIf(nameof(Debug), false)]
        public bool Temporary { get; set; }

        public bool Debug {
            get {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        [Category("Page"), Caption("Secure Page"), Description("Defines whether the page is accessed using a secure connection (using SSL) - The use of secure connections requires that Secure Connections in the Site Properties is also enabled, otherwise the page property defined here is ignored")]
        [UIHint("Enum")]
        public PageDefinition.PageSecurityType PageSecurity { get; set; }

        [Category("Page"), Caption("Page Language"), Description("Defines the page's language - Specify a specific language if this page is not available in multiple languages")]
        [UIHint("LanguageId"), StringLength(LanguageData.MaxId), Trim]
        public string? LanguageId { get; set; }

        [Category("Page"), Caption("Static Page"), Description("Defines whether the page is rendered as a static page (for anonymous users only) - A page whose content doesn't change can be marked as a static page, which results in faster page load for the end-user - Site Settings can be used to enable/disable the use of static pages globally using the StaticPages property - Static pages are only used with deployed sites")]
        [UIHint("Enum")]
        public PageDefinition.StaticPageEnum StaticPage { get; set; }

        [Category("Page"), Caption("IFrame Use"), Description("Defines whether the page can be used in an IFrame by this and other sites")]
        [UIHint("Enum")]
        public PageDefinition.IFrameUseEnum IFrameUse { get; set; }

        [Category("Page"), Caption("FavIcon"), Description("The icon representing this page (a 32x32 pixel PNG image) shown by the web browser used to display the page")]
        [UIHint("Image"), AdditionalMetadata("ImageType", PageDefinition.ImageType)]
        [AdditionalMetadata("Width", 40), AdditionalMetadata("Height", 40)]
        public string? FavIcon {
            get {
                if (_favIcon == null) {
                    if (FavIcon_Data != null && FavIcon_Data.Length > 0)
                        _favIcon = PageGuid.ToString();
                }
                return _favIcon;
            }
            set {
                _favIcon = value;
            }
        }
        private string? _favIcon = null;

        public byte[]? FavIcon_Data { get; set; }

        [Category("Page"), Caption("FavIcon (SVG)"), Description("The SVG icon representing this page (typically rendered as a 32x32 pixel image) shown in PageBars used to display the page - Use a complete <SVG> tag or reference a skin SVG by prefixing the icon name with # which locates the SVG in the skin's folder ./SVG/FAV_name (e.g., #UserSettings uses ./SVG/FAV_UserSettings.svg)")]
        [UIHint("TextAreaSourceOnly"), AdditionalMetadata("EmHeight", 6), StringLength(PageDefinition.MaxFav_SVG)]
        public string? Fav_SVG { get; set; }

        [Category("Page"), Caption("Copyright"), Description("Defines an optional copyright notice displayed on the page, if supported by the skin used - If not defined, a default copyright notice may be defined in Site Properties - Use <<Year>> for current year")]
        [UIHint("MultiString80"), StringLength(PageDefinition.MaxCopyright), Trim]
        public MultiString Copyright { get; set; }

        [Category("Page"), Caption("Robots - NoIndex"), Description("Prevents a page from being indexed by search engines if set")]
        [UIHint("Boolean")]
        public bool RobotNoIndex { get; set; }
        [Category("Page"), Caption("Robots - NoFollow"), Description("Prevents a page from being crawled by search engines if set")]
        [UIHint("Boolean")]
        public bool RobotNoFollow { get; set; }
        [Category("Page"), Caption("Robots - NoArchive"), Description("Instructs search engines not to store an archived copy of the page if set - not supported by all search engines")]
        [UIHint("Boolean")]
        public bool RobotNoArchive { get; set; }
        [Category("Page"), Caption("Robots - NoSnippet"), Description("Instructs search engines not include a snippet from the page along with the page's listing in search results - not supported by all search engines")]
        [UIHint("Boolean")]
        public bool RobotNoSnippet { get; set; }

        [Category("Authorization"), Caption("Permitted Roles"), Description("The roles that are permitted to access this page")]
        [UIHint("YetaWF_PageEdit_AllowedRoles")]
        public SerializableList<PageDefinition.AllowedRole> AllowedRoles { get; set; }

        [Category("Authorization"), Caption("Permitted Users"), Description("The users that are permitted to access this page")]
        [UIHint("YetaWF_PageEdit_AllowedUsers")]
        public SerializableList<PageDefinition.AllowedUser> AllowedUsers { get; set; }

        [Category("Urls"), Caption("Mobile Page Url"), Description("If this page is accessed by a mobile device, it is redirected to the Url defined here as mobile page Url - Redirection is not active in Site Edit Mode")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string? MobilePageUrl { get; set; }

        [Category("Urls"), Caption("Redirect To Page"), Description("If this page is accessed, it is redirected to the Url defined here - Redirection is not active in Site Edit Mode")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string? RedirectToPageUrl { get; set; }

        [Category("References"), Caption("Skin Modules"), Description("Defines modules which must be injected into this page")]
        [UIHint("ReferencedModules")]
        public SerializableList<ModuleDefinition.ReferencedModule> ReferencedModules { get; set; }

        [Category("Skin"), Caption("Popup Page"), Description("The popup page used for the popup window when this page is shown in a popup")]
        [UIHint("SkinNamePopup"), AdditionalMetadata("NoDefault", false), StringLength(SiteDefinition.MaxPopupPage)]
        public string? PopupPage { get; set; }
        public string PopupPage_Collection { get { return Manager.CurrentSite.Skin.Collection; } }

        [Category("Skin"), Caption("Template"), Description("The local designed page used as a template for this page - All modules from the template are copied and rendered on this page in their defined pane - Modules in panes that are not available are not shown - Any page can be used as a template")]
        [UIHint("PageSelection"), Trim]
        public Guid? TemplateGuid { get; set; }

        [Category("Skin"), Caption("CSS Class"), Description("The optional CSS classes to be added to the page's <body> tag for further customization through stylesheets")]
        [UIHint("Text40"), StringLength(PageDefinition.MaxCssClass), CssClassesValidationAttribute, Trim]
        public string? CssClass { get; set; }

        [Category("Addons"), Caption("Analytics"), Description("Add analytics Javascript code (for example, the Universal Analytics tracking code used by Google Analytics or the code used by Clicky) - Any code that should be added at the end of the HTML page can be added here including <script></script> tags - If omitted, the site defined analytics code is used (Site Settings)")]
        [TextAbove("Analytics code is only available in deployed production sites and is ignored in debug builds (not marked deployed).")]
        [UIHint("TextAreaSourceOnly"), StringLength(SiteDefinition.MaxAnalytics), Trim]
        public string? Analytics { get; set; }
        [Category("Addons"), Caption("Analytics (Content)"), Description("Add analytics Javascript code that should be executed when a new page becomes active in an active Unified Page Set - Do not include <script></script> tags - Use <<Url>> to substitute the actual Url - If omitted, the site defined analytics code is used (Site Settings)")]
        [UIHint("TextAreaSourceOnly"), StringLength(SiteDefinition.MaxAnalytics), Trim]
        public string? AnalyticsContent { get; set; }

        [Category("Meta"), Caption("Meta Tags"), Description("Defines <meta> tags that are added to the page - If specified, this replaces the site meta tags defined using the PageMetaTags property (Site Settings)")]
        [UIHint("TextAreaSourceOnly"), StringLength(SiteDefinition.MaxMeta), Trim]
        public string? PageMetaTags { get; set; }

        [Category("Addons"), Caption("<HEAD>"), Description("Any tags that should be added to the <HEAD> tag of each page can be added here")]
        [UIHint("TextAreaSourceOnly"), StringLength(SiteDefinition.MaxHead), Trim]
        public string? ExtraHead { get; set; }

        [Category("Addons"), Caption("<BODY> Top"), Description("Any tags that should be added to the top of the <BODY> tag of each page can be added here")]
        [UIHint("TextAreaSourceOnly"), StringLength(SiteDefinition.MaxBodyTop), Trim]
        public string? ExtraBodyTop { get; set; }

        [Category("Addons"), Caption("<BODY> Bottom"), Description("Any tags that should be added to the bottom of the <BODY> tag of each page can be added here")]
        [UIHint("TextAreaSourceOnly"), StringLength(SiteDefinition.MaxBodyBottom), Trim]
        public string? ExtraBodyBottom { get; set; }

        [Category("Meta"), Caption("SiteMap Priority"), Description("Defines the page priority used for the site map")]
        [UIHint("Enum")]
        public PageDefinition.SiteMapPriorityEnum SiteMapPriority { get; set; }
        [Category("Meta"), Caption("Change Frequency"), Description("Defines the page's change frequency used for the site map")]
        [UIHint("Enum")]
        public PageDefinition.ChangeFrequencyEnum ChangeFrequency { get; set; }

        [Category("Variables"), Caption("Page Guid Name"), Description("The page name encoded using its unique id")]
        [UIHint("String"), ReadOnly]
        public string? PageGuidName { get; set; }

        [Category("Variables"), Caption("Page Url"), Description("Returns the user defined Url (Url property) or if none has been defined, a system generated Url")]
        [UIHint("String"), ReadOnly]
        public string? PageUrl { get; set; }

        [Category("Variables"), Caption("Canonical Url Link"), Description("The Html used for the canonical Url")]
        [UIHint("String"), ReadOnly]
        public string? CanonicalUrlLink { get; set; }

        [Category("Variables"), Caption("Hreflang Html"), Description("The Html used for language definition (hreflang and metadata)")]
        [UIHint("String"), ReadOnly]
        public string? HrefLangHtml { get; set; }

        [Category("Variables"), Caption("Created"), Description("The date the page was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }

        [Category("Variables"), Caption("Updated"), Description("The date the page was last updated")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Updated { get; set; }

        [Category("Variables"), Caption("Panes"), Description("The panes defined by the page skin")]
        [UIHint("ListOfStrings"), ReadOnly]
        public List<string> Panes { get; set; }

        [Category("Variables"), Caption("Copyright"), Description("The Copyright property with evaluated substitutions")]
        [UIHint("String"), ReadOnly]
        public string? CopyrightEvaluated { get; set; }

        public EditablePage() {
            Title = new MultiString();
            Description = new MultiString();
            Keywords = new MultiString();
            Copyright = new MultiString();
            AllowedRoles = new SerializableList<PageDefinition.AllowedRole>();
            AllowedUsers = new SerializableList<PageDefinition.AllowedUser>();
            Panes = new List<string>();
            ReferencedModules = new SerializableList<ModuleDefinition.ReferencedModule>();
        }
        public void UpdateData(PageDefinition page) {
            this.Panes = page.GetPanes();
        }

        [UIHint("Hidden")]
        public Guid PageGuid { get; set; }
    }

    [Trim]
    public class EditModel {
        [UIHint("PropertyList")]
        public EditablePage Page { get; set; }

        [UIHint("Hidden")]
        public Guid PageGuid { get; set; }

        public EditModel() {
            Page = new EditablePage();
        }

        public PageDefinition GetData(PageDefinition page) {
            ObjectSupport.CopyData(this.Page, page);
            return page;
        }
        public void SetData(PageDefinition page) {
            ObjectSupport.CopyData(page, this.Page);
        }
        public void UpdateData(PageDefinition page) {
            Page.Panes = page.GetPanes();
        }
    }

    [ResourceAuthorize(CoreInfo.Resource_PageSettings)]
    public async Task<ActionInfo> RenderModuleAsync(Guid pageGuid) {
        PageDefinition? page = await PageDefinition.LoadAsync(pageGuid);
        if (page == null)
            throw new Error(this.__ResStr("notFound", "Page {0} doesn't exist", pageGuid));

        EditModel model = new EditModel() {
            PageGuid = pageGuid
        };
        model.SetData(page);
        model.UpdateData(page);
        Title = this.__ResStr("editTitle", "Page {0}", page.Url);
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    [ResourceAuthorize(CoreInfo.Resource_PageSettings)]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        PageDefinition? page = await PageDefinition.LoadAsync(model.PageGuid);
        if (page == null)
            throw new Error(this.__ResStr("alreadyDeleted", "This page has been removed and can no longer be updated."));

        model.UpdateData(page);
        ObjectSupport.CopyData(page, model.Page, ReadOnly: true); // update read only properties in model in case there is an error
        model.Page.FavIcon_Data = page.FavIcon_Data; // copy favicon
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        page = model.GetData(page); // merge new data into original
        model.SetData(page); // and all the data back into model for final display

        await page.SaveAsync();// this handles changing the Url automatically
        MenuList.ClearCachedMenus();// page changes may affect all menus so clear the menu cache (this only clears current session)
        // if we're in a popup and the parent page is the page we're editing, then force a reload

        //$$$$ rename with querystring doesn't work
        OnPopupCloseEnum popupClose = OnPopupCloseEnum.ReloadModule;
        if (PageDefinition.IsSamePage(Manager.QueryReturnToUrl.Url, model.Page.Url ?? string.Empty))
            popupClose = OnPopupCloseEnum.ReloadParentPage;
        return await FormProcessedAsync(model, this.__ResStr("okSaved", "Page settings saved"), OnPopupClose: popupClose);
    }
}