/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Pages.DataProvider;
using YetaWF.Modules.Pages.Modules;

namespace YetaWF.Modules.Pages.Controllers {

    public class PagesBrowseModuleController : ControllerImpl<YetaWF.Modules.Pages.Modules.PagesBrowseModule> {

        public class PageItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    actions.New(Module.GetAction_ShowPage(CompleteUrl), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(PageEditModule.GetModuleAction("Edit", null, PageGuid), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(Module.GetAction_RemoveLink(Url), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [Caption("Url"), Description("The Url used to identify this page - This is the name of the designed page and may not include necessary query string arguments to display the page")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; }

            [Caption("Canonical Url"), Description("The Canonical Url used to identify this page")]
            [UIHint("Url"), ReadOnly]
            public string CanonicalUrl { get; set; }

            [Caption("Title"), Description("The page title which will appear as title in the browser window")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; }

            [Caption("Description"), Description("The page description (not usually visible, entered by page designer, used for search keywords)")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Description { get; set; }

            [Caption("Date Created"), Description("The date the page was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }
            [Caption("Date Updated"), Description("The date the page was updated")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Updated { get; set; }

            [Category("Skin"), Caption("Page Skin"), Description("The skin used to display the page")]
            [UIHint("PageSkin"), ReadOnly]
            public SkinDefinition SelectedSkin { get; set; }

            [Category("Skin"), Caption("Popup Skin"), Description("The skin used when the page is displayed in a popup window")]
            [UIHint("PopupSkin"), ReadOnly]
            public SkinDefinition SelectedPopupSkin { get; set; }

            [Category("Skin"), Caption("jquery UI Skin"), Description("The skin for jQuery-UI elements (buttons, modal dialogs, etc.) used on this page")]
            [UIHint("jQueryUISkin"), ReadOnly]
            public string jQueryUISkin { get; set; }

            [Category("Skin"), Caption("Kendo UI Skin"), Description("The skin for Kendo UI elements (buttons, modal dialogs, etc.) used on this page")]
            [UIHint("KendoUISkin"), ReadOnly]
            public string KendoUISkin { get; set; }

            [Category("Skin"), Caption("CSS Class"), Description("The optional CSS classes to be added to the page's <body> tag for further customization through stylesheets")]
            [UIHint("String"), ReadOnly]
            public string CssClass { get; set; }

            [Category("Urls"), Caption("Mobile Page Url"), Description("If this page is accessed by a mobile device, it is redirected to the Url defined here as mobile page Url - Redirection is not active in Site Edit Mode")]
            [UIHint("Url"), ReadOnly]
            public string MobilePageUrl { get; set; }

            [Category("Urls"), Caption("Redirect To Page"), Description("If this page is accessed, it is redirected to the Url defined here - Redirection is not active in Site Edit Mode")]
            [UIHint("Url"), ReadOnly]
            public string RedirectToPageUrl { get; set; }

            public string CompleteUrl { get; set; }
            public Guid PageGuid { get; set; }
            private PagesBrowseModule Module { get; set; }
            public ModuleDefinition PageEditModule { get; set; }

            public PageItem(YetaWFManager manager, PagesBrowseModule module, PageDefinition page) {
                Module = module;
                // page editing services
                ModuleDefinition pageSettings = ModuleDefinition.Load(manager.CurrentSite.PageEditingServices, AllowNone: true);
                if (pageSettings == null)
                    throw new InternalError("No page edit services available - no module has been defined in Site Properties");
                PageEditModule = pageSettings;

                ObjectSupport.CopyData(page, this);
            }
        }

        public class PagesBrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [HttpGet]
        public ActionResult PagesBrowse() {
            PagesBrowseModel model = new PagesBrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("PagesBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(PageItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult PagesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (PageDefinitionDataProvider dataProvider = new PageDefinitionDataProvider()) {
                int total;
                List<PageDefinition> pages = dataProvider.GetItems(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in pages select new PageItem(Manager, Module, s)).ToList<object>(),
                    Total = total
                });
            }
        }

        [HttpPost]
        [Permission("RemovePages")]
        public ActionResult Remove(string pageName) {
            if (string.IsNullOrWhiteSpace(pageName))
                throw new Error(this.__ResStr("noPageName", "No page name specified"));
            PageDefinition page = PageDefinition.LoadFromUrl(pageName);
            if (page == null)
                throw new Error(this.__ResStr("noPage", "Page \"{0}\" not found", pageName));

            PageDefinition.RemovePageDefinition(page.PageGuid);
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
    }
}