/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Panels.Models;
using YetaWF.Modules.Panels.Modules;
using YetaWF.Modules.Panels.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Panels.Controllers {

    public class PagePanelModuleController : ControllerImpl<YetaWF.Modules.Panels.Modules.PagePanelModule> {
        
        public PagePanelModuleController() { }

        [Trim]
        public class ModelDisplay {

            public PagePanelInfo PanelInfo { get; set; }

            public ModelDisplay() {
                PanelInfo = new PagePanelInfo();
            }
        }
        [Trim]
        public class ModelEdit {

            [Caption("Pages"), Description("Defines the pages and their order as they are displayed in the Page Panel using their FavIcons and page description")]
            [UIHint("YetaWF_Panels_ListOfLocalPages")]
            public SerializableList<LocalPage> PageList { get; set; }
            public string PageList_AjaxUrl { get { return YetaWFManager.UrlFor(typeof(PagePanelModuleController), nameof(PagePanelModuleController.AddPage)); } }

            [Caption("Page Pattern"), Description("Defines a Regex pattern - all pages matching this pattern will be included in the Page Panel - for example, ^/Admin/Config/[^/]*$ would include all pages starting with /Admin/Config/, but would not include their child pages")]
            [UIHint("Text40"), Trim]
            [StringLength(500)]
            public string PagePattern { get; set; }

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
            public string DefaultImage { get; set; }

            [CopyAttribute]
            public byte[] DefaultImage_Data { get; set; }

            [UIHint("Hidden")]
            public string Url { get; set; }

            public ModelEdit() {
                PageList = new SerializableList<LocalPage>();
            }
        }
        [AllowGet]
        public async Task<ActionResult> PagePanel() {
            if (Manager.EditMode) {
                ModelEdit model = new ModelEdit {
                    PagePattern = Module.PagePattern,
                    PageList = Module.PageList,
                    Url = Manager.CurrentRequestUrl,
                    UsePopup = Module.UsePopup,
                    Style = Module.Style,
                    DefaultImage = Module.DefaultImage,
                    DefaultImage_Data = Module.DefaultImage_Data
                };
                Module.DefaultViewName = ModuleDefinition.StandardViews.EditApply;
                return View(model);
            } else {
                ModelDisplay model = new ModelDisplay { };
                model.PanelInfo = new Models.PagePanelInfo() {
                    UsePopups = Module.UsePopup,
                    Style = Module.Style,
                    Panels = await GetPanelsAsync()
                };
                return View(model);
            }
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> PagePanel_Partial(ModelEdit model) {
            if (!ModelState.IsValid) {
                Module.DefaultViewName = ModuleDefinition.StandardViews.EditApply;
                return PartialView(model);
            }
            Module.PageList = model.PageList ?? new SerializableList<LocalPage>();
            Module.PagePattern = model.PagePattern;
            Module.UsePopup = model.UsePopup;
            Module.Style = model.Style;
            Module.DefaultImage = model.DefaultImage;
            Module.DefaultImage_Data = Module.DefaultImage_Data;
            await Module.SaveAsync();
            model.PageList = Module.PageList;
            ClearCache(Module.ModuleGuid);
            if (Manager.RequestForm[Globals.Link_SubmitIsApply] == null) {
                Manager.EditMode = false;
                return Redirect(model.Url, SetCurrentEditMode: true);
            }
            Module.DefaultViewName = ModuleDefinition.StandardViews.EditApply;
            return FormProcessed(model, OnClose:OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
        }

        private async Task<List<PagePanelInfo.PanelEntry>> GetPanelsAsync() {
            SavedCacheInfo info = GetCache(Module.ModuleGuid);
            if (info == null || info.UserId != Manager.UserId || info.Language != Manager.UserLanguage) {
                // We only reload the pages when the user is new logon/logoff, otherwise we would build this too often
                List<PagePanelInfo.PanelEntry> list = new SerializableList<PagePanelInfo.PanelEntry>();
                foreach (LocalPage page in Module.PageList) {
                    AddPage(list, await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionByUrlAsync(page.Url), page.Popup);
                }
                if (!string.IsNullOrWhiteSpace(Module.PagePattern)) {
                    SerializableList<PagePanelInfo.PanelEntry> listPattern = new SerializableList<PagePanelInfo.PanelEntry>();
                    Regex regPages = new Regex(Module.PagePattern);
                    foreach (PageDefinition.DesignedPage desPage in await YetaWF.Core.Pages.PageDefinition.GetDesignedPagesAsync()) {
                        Match m = regPages.Match(desPage.Url);
                        if (m.Success) {
                            if ((from p in Module.PageList where p.Url == desPage.Url select p).FirstOrDefault() == null)
                                AddPage(listPattern, await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionAsync(desPage.PageGuid), Module.UsePopup);
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
                SetCache(Module.ModuleGuid, info);
            }
            return info.PanelEntries;
        }
        private void AddPage(List<PagePanelInfo.PanelEntry> list, PageDefinition pageDef, bool popup) {
            if (pageDef != null && pageDef.IsAuthorized_View()) {
                list.Add(new PagePanelInfo.PanelEntry {
                    Url = pageDef.EvaluatedCanonicalUrl,
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
            return ImageHelper.FormatUrl(type, null, image, size, size, Stretch: true);
        }

        // Panel Cache
        public class SavedCacheInfo {
            public List<PagePanelInfo.PanelEntry> PanelEntries { get; set; }
            public string Language { get; set; }
            public int UserId { get; set; }
            public DateTime Created { get; set; }
            public SavedCacheInfo() {
                Created = DateTime.UtcNow;
            }
        }
        private string GetCacheName(Guid moduleGuid) {
            Package package = YetaWF.Core.Controllers.AreaRegistration.CurrentPackage;
            return string.Format("{0}_PagePanelCache_{1}_{2}", package.AreaName, Manager.CurrentSite.Identity, moduleGuid);
        }
        public SavedCacheInfo GetCache(Guid moduleGuid) {
            SessionStateIO<SavedCacheInfo> session = new SessionStateIO<SavedCacheInfo> {
                Key = GetCacheName(moduleGuid)
            };
            SavedCacheInfo info = session.Load();
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
        public async Task<ActionResult> AddPage(string prefix, int newRecNumber, string newValue) {
            // Validation
            UrlValidationAttribute attr = new UrlValidationAttribute(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local);
            if (!attr.IsValid(newValue))
                throw new Error(attr.ErrorMessage);
            // add new grid record
            ListOfLocalPagesHelper.GridEntryEdit entry = (ListOfLocalPagesHelper.GridEntryEdit)Activator.CreateInstance(typeof(ListOfLocalPagesHelper.GridEntryEdit));
            entry.UrlDisplay = newValue;
            return await GridPartialViewAsync(new GridDefinition.GridEntryDefinition(prefix, newRecNumber, entry));
        }
    }
}