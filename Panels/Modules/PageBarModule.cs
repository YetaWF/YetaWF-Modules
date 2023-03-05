/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Panels.Endpoints;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Modules;

public class PageBarModuleDataProvider : ModuleDefinitionDataProvider<Guid, PageBarModule>, IInstallableModel { }

[ModuleGuid("{AF54719E-BEB6-4dda-B724-E0399EB57733}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class PageBarModule : ModuleDefinition2 {

    public PageBarModule() {
        Title = this.__ResStr("modTitle", "Page Bar");
        Name = this.__ResStr("modName", "Page Bar");
        Description = this.__ResStr("modSummary", "Used to display multiple links to pages using the pages' FavIcon, displaying the page contents within the Page Bar Module. A sample page is available at Admin > Settings (standard YetaWF site).");
        PageList = new SerializableList<LocalPage>();
        DefaultImage_Data = new byte[] { };
        ShowTitle = false;
        UsePartialFormCss = false;
        WantSearch = false;
        WantFocus = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new PageBarModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Page List"), Description("Defines the pages and their order as they are displayed in the Page Bar using their FavIcons and page description - Pages added to the Page List are shown ahead of pages discovered using the Page Pattern")]
    [UIHint("YetaWF_Panels_ListOfLocalPages")]
    [Data_Binary]
    public SerializableList<LocalPage> PageList { get; set; }
    public string PageList_AjaxUrl { get { return Utility.UrlFor(typeof(PageBarModuleEndpoints), PageBarModuleEndpoints.AddPage); } }

    [Category("General"), Caption("Page Pattern"), Description("Defines a Regex pattern - All pages matching this pattern will be included in the Page Bar - for example, ^/Admin/Config/[^/]*$ would include all pages starting with /Admin/Config/, but would not include their child pages - Pages added to the Page List are shown ahead of pages discovered using the Page Pattern")]
    [UIHint("Text40"), Trim]
    [StringLength(500)]
    public string? PagePattern { get; set; }

    public enum PanelStyleEnum {
        [EnumDescription("Vertical", "Displays the page FavIcon and page title arranged vertically")]
        Vertical = 0,
        [EnumDescription("Horizontal", "Displays the page FavIcon, page title arranged horizontally")]
        Horizontal = 1,
    }
    [Category("General"), Caption("Style"), Description("Defines the appearance of page entries")]
    [UIHint("Enum")]
    public PanelStyleEnum Style { get; set; }

    [Category("General"), Caption("Content Pane"), Description("Defines the pane used to extract page content - leave blank to use the main pane as page content")]
    [UIHint("Text20"), StringLength(20)]
    [Data_NewValue]
    public string? ContentPane { get; set; }

    [Category("General"), Caption("Default Image"), Description("The default image used when a page doesn't define its own FavIcon")]
    [UIHint("Image"), AdditionalMetadata("ImageType", ModuleImageSupport.ImageType)]
    [AdditionalMetadata("Width", 100), AdditionalMetadata("Height", 100)]
    [DontSave]
    public string? DefaultImage {
        get {
            if (_defaultImage == null) {
                if (DefaultImage_Data != null && DefaultImage_Data.Length > 0)
                    _defaultImage = ModuleGuid.ToString() + ",DefaultImage_Data";
            }
            return _defaultImage;
        }
        set {
            _defaultImage = value;
        }
    }
    private string? _defaultImage = null;

    [Data_Binary, CopyAttribute]
    public byte[]? DefaultImage_Data { get; set; }

    public ModuleAction GetAction_Display(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Page Bar"),
            MenuText = this.__ResStr("displayText", "Page Bar"),
            Tooltip = this.__ResStr("displayTooltip", "Display the Page Bar"),
            Legend = this.__ResStr("displayLegend", "Displays the Page Bar"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [UIHint("YetaWF_Panels_PageBarInfo")]
        public PageBarInfo PanelInfo { get; set; }

        [Caption("Pages"), Description("Defines the pages and their order as they are displayed in the Page Panel using their FavIcons and page description")]
        [UIHint("YetaWF_Panels_ListOfLocalPages")]
        public SerializableList<LocalPage> PageList { get; set; }
        public string PageList_AjaxUrl { get { return Utility.UrlFor(typeof(PageBarModuleEndpoints), PageBarModuleEndpoints.AddPage); } }

        [Caption("Page Pattern"), Description("Defines a Regex pattern - all pages matching this pattern will be included in the Page Panel - for example, ^/Admin/Config/[^/]*$ would include all pages starting with /Admin/Config/, but would not include their child pages")]
        [UIHint("Text40"), Trim]
        [StringLength(500)]
        public string? PagePattern { get; set; }

        [Category("General"), Caption("Style"), Description("Defines the appearance of page entries")]
        [UIHint("Enum")]
        public PageBarModule.PanelStyleEnum Style { get; set; }

        [Category("General"), Caption("Content Pane"), Description("Defines the pane used to extract page content - leave blank to use the main pane as page content")]
        [UIHint("Text20"), StringLength(20)]
        public string? ContentPane { get; set; }

        [Category("General"), Caption("Default Image"), Description("The default image used when a page doesn't define its own FavIcon")]
        [UIHint("Image"), AdditionalMetadata("ImageType", ModuleImageSupport.ImageType)]
        [AdditionalMetadata("Width", 100), AdditionalMetadata("Height", 100)]
        [DontSave]
        public string? DefaultImage { get; set; }

        [CopyAttribute]
        public byte[]? DefaultImage_Data { get; set; }

        public Model() {
            PanelInfo = new PageBarInfo();
            PageList = new SerializableList<LocalPage>();
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Model model;
        if (Manager.EditMode) {
            model = new Model {
                PagePattern = PagePattern,
                PageList = PageList,
                Style = Style,
                ContentPane = ContentPane,
                DefaultImage = DefaultImage,
                DefaultImage_Data = DefaultImage_Data
            };
        } else {

            model = new Model {
                PanelInfo = new Models.PageBarInfo() {
                    Style = Style,
                    ContentPane = ContentPane,
                    Panels = await GetPanelsAsync()
                }
            };

            // Check whether current page contents are accessible and get pane contents
            Uri? contentUri = null;
            Manager.TryGetUrlArg<string>("!ContentUrl", out string? contentUrl);
            if (!string.IsNullOrWhiteSpace(contentUrl)) {
                if (contentUrl.StartsWith("/"))
                    contentUrl = Manager.CurrentSite.MakeUrl(contentUrl);
                contentUri = new Uri(contentUrl);
            } else {
                if (model.PanelInfo.Panels.Count > 0)
                    contentUri = new Uri(model.PanelInfo.Panels[0].Url);
            }
            if (contentUri != null) {
                PageDefinition? page = await PageDefinition.LoadFromUrlAsync(contentUri.AbsolutePath);
                if (page != null) {
                    if (page.IsAuthorized_View()) {
                        model.PanelInfo.ContentUri = contentUri;
                        model.PanelInfo.ContentPage = page;
                    } else {
                        if (!Manager.HaveUser)
                            return RedirectToUrl(Manager.CurrentSite.LoginUrl);
                    }
                }
            }

        }
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        if (!ModelState.IsValid) {
            DefaultViewName = ModuleDefinition.StandardViews.EditApply;
            return await PartialViewAsync(model);
        }
        PageList = model.PageList ?? new SerializableList<LocalPage>();
        PagePattern = model.PagePattern;
        Style = model.Style;
        ContentPane = model.ContentPane;
        DefaultImage = model.DefaultImage;
        DefaultImage_Data = DefaultImage_Data;
        await SaveAsync();
        model.PageList = PageList;
        ClearCache(ModuleGuid);
        if (IsApply) {
            DefaultViewName = ModuleDefinition.StandardViews.EditApply;
            return await FormProcessedAsync(model, OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
        }
        Manager.EditMode = false;
        string url = YetaWFEndpoints.AddUrlPayload(Manager.ReturnToUrl, true, null);
        return await FormProcessedAsync(model, NextPage: url, OnClose: OnCloseEnum.GotoNewPage, OnPopupClose: OnPopupCloseEnum.GotoNewPage);
    }

    private async Task<List<PageBarInfo.PanelEntry>> GetPanelsAsync() {
        SavedCacheInfo? info = GetCache(ModuleGuid);
        if (info == null || info.UserId != Manager.UserId || info.Language != Manager.UserLanguage) {
            // We only reload the pages when the user is new (logon/logoff), otherwise we would build this too often
            List<PageBarInfo.PanelEntry> list = new SerializableList<PageBarInfo.PanelEntry>();
            foreach (LocalPage page in PageList) {
                AddPage(list, await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionByUrlAsync(page.Url), page.Popup);
            }
            if (!string.IsNullOrWhiteSpace(PagePattern)) {
                SerializableList<PageBarInfo.PanelEntry> listPattern = new SerializableList<PageBarInfo.PanelEntry>();
                Regex regPages = new Regex(PagePattern);
                foreach (PageDefinition.DesignedPage desPage in await YetaWF.Core.Pages.PageDefinition.GetDesignedPagesAsync()) {
                    Match m = regPages.Match(desPage.Url);
                    if (m.Success) {
                        if ((from p in PageList where p.Url == desPage.Url select p).FirstOrDefault() == null)
                            AddPage(listPattern, await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionAsync(desPage.PageGuid), false);
                    }
                }
                listPattern = new SerializableList<PageBarInfo.PanelEntry>(listPattern.OrderBy((m) => m.Caption.ToString()).ToList());
                list.AddRange(listPattern);
            }
            info = new SavedCacheInfo {
                PanelEntries = list,
                Language = Manager.UserLanguage,
                UserId = Manager.UserId,
            };
            SetCache(ModuleGuid, info);
        }
        return info.PanelEntries;
    }
    private void AddPage(List<PageBarInfo.PanelEntry> list, PageDefinition? pageDef, bool popup) {
        if (pageDef != null && pageDef.IsAuthorized_View()) {
            // If a page is authorized for anonymous but not users, we suppress it if we're Editor, Admin, Superuser, etc. to avoid cases where we
            // have 2 of the same pages, one for anonymous users, the other for logged on users.
            if (Manager.HaveUser && pageDef.IsAuthorized_View_Anonymous() && !pageDef.IsAuthorized_View_AnyUser())
                return;

            string? svg = GetSVG(pageDef.Fav_SVG);
            string? imageUrl = null;
            if (svg == null)
                imageUrl = GetImage(pageDef);

            list.Add(new PageBarInfo.PanelEntry {
                Url = pageDef.EvaluatedCanonicalUrl!,
                Caption = pageDef.Title,
                ToolTip = pageDef.Description,
                ImageUrl = imageUrl,
                ImageSVG = svg,
                Popup = popup
            });
        }
    }
    private string? GetSVG(string? svg) {
        if (string.IsNullOrWhiteSpace(svg)) return null;
        if (svg.StartsWith('#')) {
            if (svg.Length <= 1) return null;
            svg = svg.Substring(1);
            if (string.IsNullOrWhiteSpace(svg)) return null;
            return SkinSVGs.GetSkin($"FAV_{svg}");
        } else {
            return svg;
        }
    }

    private string GetImage(PageDefinition pageDef) {
        string image;
        string type;
        if (pageDef.FavIcon != null) {
            type = PageDefinition.ImageType;
            image = pageDef.FavIcon;
        } else if (DefaultImage != null) {
            type = ModuleImageSupport.ImageType;
            image = DefaultImage;
        } else {
            type = PageDefinition.ImageType;
            image = Guid.Empty.ToString();
        }
        int size;
        switch (Style) {
            default:
            case PageBarModule.PanelStyleEnum.Vertical:
                size = 32;
                break;
            case PageBarModule.PanelStyleEnum.Horizontal:
                size = 32;
                break;
        }
        return ImageHTML.FormatUrl(type, null, image, size, size, Stretch: true);
    }

    // Panel Cache
    public class SavedCacheInfo {
        public List<PageBarInfo.PanelEntry> PanelEntries { get; set; } = null!;
        public string? Language { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; }
        public SavedCacheInfo() {
            Created = DateTime.UtcNow;
        }
    }
    private string GetCacheName(Guid moduleGuid) {
        Package package = YetaWF.Core.AreaRegistration.CurrentPackage;
        return string.Format("{0}_PageBarCache_{1}_{2}", package.AreaName, Manager.CurrentSite.Identity, moduleGuid);
    }
    public SavedCacheInfo? GetCache(Guid moduleGuid) {
        SessionStateIO<SavedCacheInfo> session = new SessionStateIO<SavedCacheInfo> {
            Key = GetCacheName(moduleGuid)
        };
        SavedCacheInfo? info = session.Load();
        if (info != null && info.Created < DateTime.UtcNow.AddMinutes(-5)) return null;
        return info;
    }
    public void SetCache(Guid moduleGuid, SavedCacheInfo cacheInfo) {
        SessionStateIO<SavedCacheInfo> session = new SessionStateIO<SavedCacheInfo> {
            Key = GetCacheName(moduleGuid),
            Data = cacheInfo,
        };
        session.Save();
    }
    public void ClearCache(Guid moduleGuid) {
        SessionStateIO<SavedCacheInfo> session = new SessionStateIO<SavedCacheInfo> {
            Key = GetCacheName(moduleGuid),
        };
        session.Remove();
    }
}
