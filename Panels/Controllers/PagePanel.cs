/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core.Serializers;
using YetaWF.Modules.Panels.Models;
using YetaWF.Modules.Pages.Controllers;
using YetaWF.Core.Pages;
using YetaWF.Core.Modules;
using YetaWF.Core;
using System.Text.RegularExpressions;
using System.Linq;
using YetaWF.Core.Views.Shared;
using System;
using YetaWF.Modules.Panels.Modules;
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
            [UIHint("YetaWF_Pages_ListOfLocalPages")]
            public SerializableList<string> PageList { get; set; }
            public string PageList_AjaxUrl { get { return YetaWFManager.UrlFor(typeof(TemplateListOfLocalPagesModuleController), nameof(TemplateListOfLocalPagesModuleController.AddPage)); } }

            [Caption("Page Pattern"), Description("Defines a Regex pattern - all pages matching this pattern will be included in the Page Panel - for example, ^/Admin/Config/[^/]*$ would include all pages starting with /Admin/Config/, but would not include their child pages")]
            [UIHint("Text40"), Trim]
            [StringLength(500)]
            public string PagePattern { get; set; }

            [Category("General"), Caption("Use Popups"), Description("Defines whether all pages are shown as popups - otherwise full pages are shown")]
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
                PageList = new SerializableList<string>();
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
            Module.PageList = model.PageList ?? new SerializableList<string>();
            Module.PagePattern = model.PagePattern;
            Module.UsePopup = model.UsePopup;
            Module.Style = model.Style;
            Module.DefaultImage = model.DefaultImage;
            Module.DefaultImage_Data = Module.DefaultImage_Data;
            await Module.SaveAsync();
            model.PageList = Module.PageList;
            if (Manager.RequestForm[Globals.Link_SubmitIsApply] == null) {
                Manager.EditMode = false;
                return Redirect(model.Url, SetCurrentEditMode: true);
            }
            Module.DefaultViewName = ModuleDefinition.StandardViews.EditApply;
            return FormProcessed(model, OnClose:OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
        }

        private async Task<SerializableList<PagePanelInfo.PanelEntry>> GetPanelsAsync() {
            //$$$ add caching based on logged on user/language
            SerializableList<PagePanelInfo.PanelEntry> list = new SerializableList<PagePanelInfo.PanelEntry>();
            foreach (string page in Module.PageList) {
                AddPage(list, await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionByUrlAsync(page));
            }
            if (!string.IsNullOrWhiteSpace(Module.PagePattern)) {
                SerializableList<PagePanelInfo.PanelEntry> listPattern = new SerializableList<PagePanelInfo.PanelEntry>();
                Regex regPages = new Regex(Module.PagePattern);
                foreach (PageDefinition.DesignedPage desPage in await YetaWF.Core.Pages.PageDefinition.GetDesignedPagesAsync()) {
                    if (!Module.PageList.Contains(desPage.Url)) {
                        Match m = regPages.Match(desPage.Url);
                        if (m.Success)
                            AddPage(listPattern, await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionAsync(desPage.PageGuid));
                    }
                }
                //$$$sort is language specific
                listPattern = new SerializableList<PagePanelInfo.PanelEntry>(listPattern.OrderBy((m) => m.Caption.ToString()).ToList());
                list.AddRange(listPattern);
            }
            return list;
        }
        private void AddPage(SerializableList<PagePanelInfo.PanelEntry> list, PageDefinition pageDef) {
            if (pageDef != null && pageDef.IsAuthorized_View()) {
                list.Add(new PagePanelInfo.PanelEntry {
                    Url = pageDef.EvaluatedCanonicalUrl,
                    Caption = pageDef.Title,
                    ToolTip = pageDef.Description,
                    ImageUrl = GetImage(pageDef),
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
            return ImageHelper.FormatUrl(type, null, image, size, size, Stretch: true, CacheBuster: Module.DateUpdated.Ticks.ToString());
        }
    }
}