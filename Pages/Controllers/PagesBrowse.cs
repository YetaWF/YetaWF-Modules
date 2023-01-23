/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Pages.DataProvider;
using YetaWF.Modules.Pages.Endpoints;
using YetaWF.Modules.Pages.Modules;

namespace YetaWF.Modules.Pages.Controllers {

    public class PagesBrowseModuleController : ControllerImpl<YetaWF.Modules.Pages.Modules.PagesBrowseModule> {

        public class PageItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands { get; set; } = null!;

            public async Task<List<ModuleAction>> __GetCommandsAsync() {
                List<ModuleAction> actions = new List<ModuleAction>();

                actions.New(Module.GetAction_ShowPage(EvaluatedCanonicalUrl), ModuleAction.ActionLocationEnum.GridLinks);

                if (PageEditModule != null)
                    actions.New(await PageEditModule.GetModuleActionAsync("Edit", null, PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
//#if DEBUG
//                actions.New(await Module.GetAction_SetSuperuserAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
//                actions.New(await Module.GetAction_SetAdminAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
//                actions.New(await Module.GetAction_SetUserAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
//                actions.New(await Module.GetAction_SetAnonymousAsync(PageGuid), ModuleAction.ActionLocationEnum.GridLinks);
//#endif
                actions.New(Module.GetAction_RemoveLink(Url), ModuleAction.ActionLocationEnum.GridLinks);

                return actions;
            }

            [Caption("Url"), Description("The URL used to identify this page - This is the name of the designed page and may not include necessary query string arguments to display the page")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; } = null!;

            [Caption("Canonical URL"), Description("The Canonical URL used to identify this page")]
            [UIHint("Url"), ReadOnly]
            public string? CanonicalUrl { get; set; }

            [Caption("Title"), Description("The page title which will appear as title in the browser window")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; } = null!;

            [Caption("Description"), Description("The page description (not usually visible, entered by page designer, used for search keywords)")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Description { get; set; } = null!;

            [Caption("Static Page"), Description("Defines whether the page is rendered as a static page (for anonymous users only) - A page whose content doesn't change can be marked as a static page, which results in faster page load for the end-user - Site Settings can be used to enable/disable the use of static pages globally using the StaticPages property - Static pages are only used with deployed sites")]
            [UIHint("Enum"), ReadOnly]
            public PageDefinition.StaticPageEnum StaticPage { get; set; }

            [Caption("Anonymous"), Description("Anonymous users can view this page")]
            [UIHint("Boolean"), ReadOnly]
            public bool Anonymous { get; set; }

            [Caption("Users"), Description("Logged on users can view this page")]
            [UIHint("Boolean"), ReadOnly]
            public bool Users { get; set; }

            [Caption("Editors"), Description("Editors can view this page")]
            [UIHint("Boolean"), ReadOnly]
            public bool Editors { get; set; }

            [Caption("Administrators"), Description("Administrators can view this page")]
            [UIHint("Boolean"), ReadOnly]
            public bool Administrators { get; set; }

            [Caption("SiteMap Priority"), Description("Defines the page priority used for the site map")]
            [UIHint("Enum"), ReadOnly]
            public PageDefinition.SiteMapPriorityEnum SiteMapPriority { get; set; }

            [Caption("Date Created"), Description("The date the page was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }
            [Caption("Date Updated"), Description("The date the page was updated")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Updated { get; set; }

            [Description("The optional CSS classes to be added to the page's <body> tag for further customization through stylesheets")]
            [UIHint("String"), ReadOnly]
            public string? CssClass { get; set; }

            [Caption("Mobile Page URL"), Description("If this page is accessed by a mobile device, it is redirected to the URL defined here as mobile page URL - Redirection is not active in Site Edit Mode")]
            [UIHint("Url"), ReadOnly]
            public string? MobilePageUrl { get; set; }

            [Caption("Redirect To Page"), Description("If this page is accessed, it is redirected to the URL defined here - Redirection is not active in Site Edit Mode")]
            [UIHint("Url"), ReadOnly]
            public string? RedirectToPageUrl { get; set; }

            [Caption("Guid"), Description("The id uniquely identifying this page")]
            [UIHint("Guid"), ReadOnly]
            public Guid PageGuid { get; set; }

            public string EvaluatedCanonicalUrl { get; set; } = null!;
            private PagesBrowseModule Module { get; set; }
            public ModuleDefinition? PageEditModule { get; set; }

            public PageItem(PagesBrowseModule module, PageDefinition page, ModuleDefinition? pageSettings) {
                Module = module;
                PageEditModule = pageSettings;
                ObjectSupport.CopyData(page, this);
                Anonymous = page.IsAuthorized_View_Anonymous();
                Users = page.IsAuthorized_View_AnyUser();
                Editors = page.IsAuthorized_View_Editor();
                Administrators = page.IsAuthorized_View_Administrator();
            }
        }

        public class PagesBrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }

        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                InitialPageSize = 20,
                RecordType = typeof(PageItem),
                AjaxUrl = Utility.UrlFor(typeof(PagesBrowseModuleEndpoints), GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    // page editing services
                    ModuleDefinition? pageSettings = await ModuleDefinition.LoadAsync(Manager.CurrentSite.PageEditingServices, AllowNone: true);
                    //if (pageSettings == null)
                    //    throw new InternalError("No page edit services available - no module has been defined in Site Properties");

                    using (PageDefinitionDataProvider dataProvider = new PageDefinitionDataProvider()) {
                        DataProviderGetRecords<PageDefinition> pages = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in pages.Data select new PageItem((PagesBrowseModule)module, s, pageSettings)).ToList<object>(),
                            Total = pages.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult PagesBrowse() {
            PagesBrowseModel model = new PagesBrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}
