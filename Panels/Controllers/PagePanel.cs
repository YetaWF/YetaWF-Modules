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

            [Caption("Page Pattern"), Description("Defines a Regex pattern - all pages matching this pattern will be included in the Page Panel - for example, ^/Admin/Config/[^/]*$ would include all pages starting with /Admin/Config, but would not include their child pages")]
            [UIHint("Text40"), Trim]
            [StringLength(500)]
            public string PagePattern { get; set; }

            [Category("General"), Caption("Use Popups"), Description("Defines whether all pages are shown as popups - otherwise full pages are shown")]
            [UIHint("Boolean")]
            public bool UsePopup { get; set; }

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
                };
                Module.DefaultViewName = ModuleDefinition.StandardViews.EditApply;
                return View(model);
            } else {
                ModelDisplay model = new ModelDisplay { };
                model.PanelInfo = new Models.PagePanelInfo() {
                    UsePopups = Module.UsePopup,
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
                PageDefinition pageDef = await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionByUrlAsync(page);
                if (pageDef != null && pageDef.IsAuthorized_View()) {
                    string imageUrl = null;
                    if (pageDef.FavIcon != null)
                        imageUrl = ImageHelper.FormatUrl(PageDefinition.ImageType, null, pageDef.FavIcon, 64, 64, Stretch: true);
                    else
                        imageUrl = ImageHelper.FormatUrl(PageDefinition.ImageType, null, Guid.Empty.ToString(), 64, 64, Stretch: true);
                    list.Add(new PagePanelInfo.PanelEntry {
                        Url = pageDef.EvaluatedCanonicalUrl,
                        Caption = pageDef.Title,
                        ToolTip = pageDef.Description,
                        ImageUrl = imageUrl,
                    });

                }
            }
            if (!string.IsNullOrWhiteSpace(Module.PagePattern)) {
                SerializableList<PagePanelInfo.PanelEntry> listPattern = new SerializableList<PagePanelInfo.PanelEntry>();
                Regex regPages = new Regex(Module.PagePattern);
                foreach (PageDefinition.DesignedPage desPage in await YetaWF.Core.Pages.PageDefinition.GetDesignedPagesAsync()) {
                    if (!Module.PageList.Contains(desPage.Url)) {
                        Match m = regPages.Match(desPage.Url);
                        if (m.Success) {
                            PageDefinition pageDef = await YetaWF.Core.Pages.PageDefinition.LoadPageDefinitionAsync(desPage.PageGuid);
                            if (pageDef != null && pageDef.IsAuthorized_View()) {
                                string imageUrl = null;
                                if (pageDef.FavIcon != null)
                                    imageUrl = ImageHelper.FormatUrl(PageDefinition.ImageType, null, pageDef.FavIcon, 64, 64, Stretch: true);
                                else
                                    imageUrl = ImageHelper.FormatUrl(PageDefinition.ImageType, null, Guid.Empty.ToString(), 64, 64, Stretch: true);
                                listPattern.Add(new PagePanelInfo.PanelEntry {
                                    Url = pageDef.EvaluatedCanonicalUrl,
                                    Caption = pageDef.Title,
                                    ToolTip = pageDef.Description,
                                    ImageUrl = imageUrl,
                                });
                            }
                        }
                    }
                }
                //$$$sort is language specific
                listPattern = new SerializableList<PagePanelInfo.PanelEntry>(listPattern.OrderBy((m) => m.Caption.ToString()).ToList());
                list.AddRange(listPattern);
            }
            return list;
        }
    }
}