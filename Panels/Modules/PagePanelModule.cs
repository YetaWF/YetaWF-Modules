/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Panels.Endpoints;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Modules;

public class PagePanelModuleDataProvider : ModuleDefinitionDataProvider<Guid, PagePanelModule>, IInstallableModel { }

[ModuleGuid("{F8EF23F3-A690-47FC-ABB5-753D8BA9B9DA}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class PagePanelModule : ModuleDefinition {

    public PagePanelModule() {
        Title = this.__ResStr("modTitle", "Page Panel");
        Name = this.__ResStr("modName", "Page Panel");
        Description = this.__ResStr("modSummary", "Used to display multiple links to pages using the pages' FavIcon. A sample page is available at /Admin/Tests (standard YetaWF site).");
        PageList = new SerializableList<LocalPage>();
        DefaultImage_Data = new byte[] { };
        UsePartialFormCss = false;
        WantSearch = false;
        WantFocus = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new PagePanelModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Page List"), Description("Defines the pages and their order as they are displayed in the Page Panel using their FavIcons and page description - Pages added to the Page List are shown ahead of pages discovered using the Page Pattern")]
    [UIHint("YetaWF_Panels_ListOfLocalPages")]
    [Data_Binary]
    public SerializableList<LocalPage> PageList { get; set; }
    public string PageList_AjaxUrl { get { return Utility.UrlFor(typeof(PagePanelModule), nameof(PagePanelModule.AddPage)); } }

    [Category("General"), Caption("Page Pattern"), Description("Defines a Regex pattern - All pages matching this pattern will be included in the Page Panel - for example, ^/Admin/Config/[^/]*$ would include all pages starting with /Admin/Config/, but would not include their child pages - Pages added to the Page List are shown ahead of pages discovered using the Page Pattern")]
    [UIHint("Text40"), Trim]
    [StringLength(500)]
    public string? PagePattern { get; set; }

    [Category("General"), Caption("Use Popups"), Description("Defines whether pages added automatically using the Page Pattern field are shown as popups - otherwise full pages are shown")]
    [UIHint("Boolean")]
    public bool UsePopup { get; set; }

    public enum PanelStyleEnum {
        [EnumDescription("Default", "Displays the page FavIcon and page title in tiles, arranged horizontally, wrapping around within the available space - A large icon is used")]
        Default = 0,
        [EnumDescription("Small Vertical", "Displays the page FavIcon and page title as a list, arranged vertically, wrapping around within the available space - A small icon is used")]
        SmallVertical = 1,
        [EnumDescription("Small Table", "Displays the page FavIcon, page title and description as a table - A small icon is used")]
        SmallTable = 2,
    }
    [Category("General"), Caption("Style"), Description("Defines the appearance of page entries")]
    [UIHint("Enum")]
    [Data_NewValue]
    public PanelStyleEnum Style { get; set; }

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

    public ModuleAction? GetAction_Display(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Page Panel"),
            MenuText = this.__ResStr("displayText", "Page Panel"),
            Tooltip = this.__ResStr("displayTooltip", "Display the Page Panel"),
            Legend = this.__ResStr("displayLegend", "Displays the Page Panel"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [UIHint("YetaWF_Panels_PagePanelInfo")]
        public PagePanelInfo PanelInfo { get; set; }

        [Caption("Pages"), Description("Defines the pages and their order as they are displayed in the Page Panel using their FavIcons and page description")]
        [UIHint("YetaWF_Panels_ListOfLocalPages")]
        public SerializableList<LocalPage> PageList { get; set; }
        public string PageList_AjaxUrl { get { return Utility.UrlFor(typeof(PagePanelModuleEndpoints), nameof(PagePanelModuleEndpoints.AddPage)); } }

        [Caption("Page Pattern"), Description("Defines a Regex pattern - all pages matching this pattern will be included in the Page Panel - for example, ^/Admin/Config/[^/]*$ would include all pages starting with /Admin/Config/, but would not include their child pages")]
        [UIHint("Text40"), Trim]
        [StringLength(500)]
        public string? PagePattern { get; set; }

        [Category("General"), Caption("Use Popups"), Description("Defines whether pages added automatically using the Page Pattern field are shown as popups - otherwise full pages are shown")]
        [UIHint("Boolean")]
        public bool UsePopup { get; set; }

        [Category("General"), Caption("Style"), Description("Defines the appearance of page entries")]
        [UIHint("Enum")]
        public PagePanelModule.PanelStyleEnum Style { get; set; }

        [Category("General"), Caption("Default Image"), Description("The default image used when a page doesn't define its own FavIcon")]
        [UIHint("Image"), AdditionalMetadata("ImageType", ModuleImageSupport.ImageType)]
        [AdditionalMetadata("Width", 100), AdditionalMetadata("Height", 100)]
        [DontSave]
        public string? DefaultImage { get; set; }

        [CopyAttribute]
        public byte[]? DefaultImage_Data { get; set; }

        public Model() {
            PanelInfo = new PagePanelInfo();
            PageList = new SerializableList<LocalPage>();
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Model model;
        if (Manager.EditMode) {
            model = new Model {
                PagePattern = PagePattern,
                PageList = PageList,
                UsePopup = UsePopup,
                Style = Style,
                DefaultImage = DefaultImage,
                DefaultImage_Data = DefaultImage_Data
            };
        } else {
            model = new Model {
                PanelInfo = new Models.PagePanelInfo() {
                    UsePopups = UsePopup,
                    Style = Style,
                    Panels = await GetPanelsAsync()
                }
            };
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
        UsePopup = model.UsePopup;
        Style = model.Style;
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
        return await FormProcessedAsync(model, NextPage: Manager.ReturnToUrl, OnClose: OnCloseEnum.GotoNewPage, OnPopupClose: OnPopupCloseEnum.GotoNewPage);
    }

    private async Task<List<PagePanelInfo.PanelEntry>> GetPanelsAsync() {
        SavedCacheInfo? info = GetCache(ModuleGuid);
        if (info == null || info.UserId != Manager.UserId || info.Language != Manager.UserLanguage) {
            // We only reload the pages when the user is new (logon/logoff), otherwise we would build this too often
            List<PagePanelInfo.PanelEntry> list = new SerializableList<PagePanelInfo.PanelEntry>();
            foreach (LocalPage page in PageList) {
                AddPage(list, await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionByUrlAsync(page.Url), page.Popup);
            }
            if (!string.IsNullOrWhiteSpace(PagePattern)) {
                SerializableList<PagePanelInfo.PanelEntry> listPattern = new SerializableList<PagePanelInfo.PanelEntry>();
                Regex regPages = new Regex(PagePattern);
                foreach (PageDefinition.DesignedPage desPage in await YetaWF.Core.Pages.PageDefinition.GetDesignedPagesAsync()) {
                    Match m = regPages.Match(desPage.Url);
                    if (m.Success) {
                        if ((from p in PageList where p.Url == desPage.Url select p).FirstOrDefault() == null)
                            AddPage(listPattern, await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionAsync(desPage.PageGuid), UsePopup);
                    }
                }
                listPattern = new SerializableList<PagePanelInfo.PanelEntry>(listPattern.OrderBy((m) => m.Caption.ToString()).ToList());
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
    private void AddPage(List<PagePanelInfo.PanelEntry> list, PageDefinition? pageDef, bool popup) {
        if (pageDef != null && pageDef.IsAuthorized_View()) {
            // If a page is authorized for anonymous but not users, we suppress it if we're Editor, Admin, Superuser, etc. to avoid cases where we
            // have 2 of the same pages, one for anonymous users, the other for logged on users.
            if (Manager.HaveUser && pageDef.IsAuthorized_View_Anonymous() && !pageDef.IsAuthorized_View_AnyUser())
                return;
            list.Add(new PagePanelInfo.PanelEntry {
                Url = pageDef.EvaluatedCanonicalUrl!,
                Caption = pageDef.Title,
                ToolTip = pageDef.Description,
                ImageUrl = GetImage(pageDef),
                Popup = popup
            });
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
            case PagePanelModule.PanelStyleEnum.Default:
                size = 64;
                break;
            case PagePanelModule.PanelStyleEnum.SmallVertical:
                size = 16;
                break;
            case PagePanelModule.PanelStyleEnum.SmallTable:
                size = 16;
                break;
        }
        return ImageHTML.FormatUrl(type, null, image, size, size, Stretch: true);
    }

    // Panel Cache
    public class SavedCacheInfo {
        public List<PagePanelInfo.PanelEntry> PanelEntries { get; set; } = null!;
        public string? Language { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; }
        public SavedCacheInfo() {
            Created = DateTime.UtcNow;
        }
    }
    private string GetCacheName(Guid moduleGuid) {
        Package package = YetaWF.Core.AreaRegistration.CurrentPackage;
        return string.Format("{0}_PagePanelCache_{1}_{2}", package.AreaName, Manager.CurrentSite.Identity, moduleGuid);
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
