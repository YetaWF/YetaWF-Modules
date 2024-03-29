/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Panels.Models;
using YetaWF.Modules.Panels.Modules;
using YetaWF.Core.Components;
using YetaWF.Modules.Panels.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Skins;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Panels.Controllers {

    public class PageBarModuleController : ControllerImpl<YetaWF.Modules.Panels.Modules.PageBarModule> {

        public PageBarModuleController() { }

        [Trim]
        public class Model {

            [UIHint("YetaWF_Panels_PageBarInfo")]
            public PageBarInfo PanelInfo { get; set; }

            [Caption("Pages"), Description("Defines the pages and their order as they are displayed in the Page Panel using their FavIcons and page description")]
            [UIHint("YetaWF_Panels_ListOfLocalPages")]
            public SerializableList<LocalPage> PageList { get; set; }
            public string PageList_AjaxUrl { get { return Utility.UrlFor(typeof(PageBarModuleController), nameof(PageBarModuleController.AddPage)); } }

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
        [AllowGet]
        public async Task<ActionResult> PageBar() {
            Model model;
            if (Manager.EditMode) {
                model = new Model {
                    PagePattern = Module.PagePattern,
                    PageList = Module.PageList,
                    Style = Module.Style,
                    ContentPane = Module.ContentPane,
                    DefaultImage = Module.DefaultImage,
                    DefaultImage_Data = Module.DefaultImage_Data
                };
            } else {

                model = new Model {
                    PanelInfo = new Models.PageBarInfo() {
                        Style = Module.Style,
                        ContentPane = Module.ContentPane,
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
            return View(model);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> PageBar_Partial(Model model) {
            if (!ModelState.IsValid) {
                Module.DefaultViewName = ModuleDefinition.StandardViews.EditApply;
                return PartialView(model);
            }
            Module.PageList = model.PageList ?? new SerializableList<LocalPage>();
            Module.PagePattern = model.PagePattern;
            Module.Style = model.Style;
            Module.ContentPane = model.ContentPane;
            Module.DefaultImage = model.DefaultImage;
            Module.DefaultImage_Data = Module.DefaultImage_Data;
            await Module.SaveAsync();
            model.PageList = Module.PageList;
            ClearCache(Module.ModuleGuid);
            if (IsApply) {
                Module.DefaultViewName = ModuleDefinition.StandardViews.EditApply;
                return FormProcessed(model, OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
            }
            Manager.EditMode = false;
            return Redirect(Manager.ReturnToUrl, SetCurrentEditMode: true);
        }

        private async Task<List<PageBarInfo.PanelEntry>> GetPanelsAsync() {
            SavedCacheInfo? info = GetCache(Module.ModuleGuid);
            if (info == null || info.UserId != Manager.UserId || info.Language != Manager.UserLanguage) {
                // We only reload the pages when the user is new (logon/logoff), otherwise we would build this too often
                List<PageBarInfo.PanelEntry> list = new SerializableList<PageBarInfo.PanelEntry>();
                foreach (LocalPage page in Module.PageList) {
                    AddPage(list, await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionByUrlAsync(page.Url), page.Popup);
                }
                if (!string.IsNullOrWhiteSpace(Module.PagePattern)) {
                    SerializableList<PageBarInfo.PanelEntry> listPattern = new SerializableList<PageBarInfo.PanelEntry>();
                    Regex regPages = new Regex(Module.PagePattern);
                    foreach (PageDefinition.DesignedPage desPage in await YetaWF.Core.Pages.PageDefinition.GetDesignedPagesAsync()) {
                        Match m = regPages.Match(desPage.Url);
                        if (m.Success) {
                            if ((from p in Module.PageList where p.Url == desPage.Url select p).FirstOrDefault() == null)
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
                SetCache(Module.ModuleGuid, info);
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
            } else if (Module.DefaultImage != null) {
                type = ModuleImageSupport.ImageType;
                image = Module.DefaultImage;
            } else {
                type = PageDefinition.ImageType;
                image = Guid.Empty.ToString();
            }
            int size;
            switch (Module.Style) {
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
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddPage(string data, string fieldPrefix, string newUrl) {
            // Validation
            UrlValidationAttribute attr = new UrlValidationAttribute(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local);
            if (!attr.IsValid(newUrl))
                throw new Error(attr.ErrorMessage!);
            List<ListOfLocalPagesEditComponent.Entry> list = Utility.JsonDeserialize<List<ListOfLocalPagesEditComponent.Entry>>(data);
            if ((from l in list where l.Url.ToLower() == newUrl.ToLower() select l).FirstOrDefault() != null)
                throw new Error(this.__ResStr("dupUrl", "Page {0} has already been added", newUrl));
            // add new grid record
            ListOfLocalPagesEditComponent.Entry entry = new ListOfLocalPagesEditComponent.Entry {
                Url = newUrl,
            };
            return await GridRecordViewAsync(await ListOfLocalPagesEditComponent.GridRecordAsync(fieldPrefix, entry));
        }
    }
}