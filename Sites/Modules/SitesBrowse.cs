/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Sites#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Sites.Endpoints;

namespace YetaWF.Modules.Sites.Modules;

public class SitesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, SitesBrowseModule>, IInstallableModel { }

[ModuleGuid("{904ad843-1e42-4995-a600-a04b1233abfa}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SitesBrowseModule : ModuleDefinition {

    public SitesBrowseModule() {
        Title = this.__ResStr("modTitle", "Sites");
        Name = this.__ResStr("modName", "Sites");
        Description = this.__ResStr("modSummary", "Displays and manages sites. Sites can be accessed at Admin > Panel > Sites (standard YeatWF site).");
        DefaultViewName = StandardViews.Browse;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SitesBrowseModuleDataProvider(); }

    [Category("General"), Caption("Add URL"), Description("The URL to add a new web site - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? AddUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        SiteAddModule mod = new SiteAddModule();
        menuList.New(mod.GetAction_Add(AddUrl), location);
        return menuList;
    }

    public ModuleAction GetAction_Sites(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Sites"),
            MenuText = this.__ResStr("browseText", "Sites"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage sites"),
            Legend = this.__ResStr("browseLegend", "Displays and manages sites"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction GetAction_SiteDisplay(SiteDefinition site) {
        return new ModuleAction() {
            Url = site.MakeUrl(RealDomain: site.SiteDomain),
            Image = "#Display",
            LinkText = this.__ResStr("dispLink", "Display Site \"{0}\"", site.SiteDomain),
            MenuText = this.__ResStr("dispText", "Display Site \"{0}\"", site.SiteDomain),
            Tooltip = this.__ResStr("dispTooltip", "Display the site \"{0}\"", site.SiteDomain),
            Legend = this.__ResStr("dispLegend", "Displays the site \"{0}\"", site.SiteDomain),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands { get; set; } = null!;

        public async Task<List<ModuleAction>> __GetCommandsAsync() {
            List<ModuleAction> actions = new List<ModuleAction>();

            actions.New(Module.GetAction_SiteDisplay(SiteData), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(await SiteEditModule.GetModuleActionAsync("EditSite", null, SiteDomain), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(ConfirmModule.GetAction_Remove(null, SiteData), ModuleAction.ActionLocationEnum.GridLinks);
            return actions;
        }

        [Caption("Site Domain"), Description("The domain name of the site")]
        [UIHint("String"), ReadOnly]
        public string SiteDomain { get; set; } = null!;

        [Caption("Default Site"), Description("Shows whether the site is the default site for this instance of YetaWF - the default site cannot be removed")]
        [UIHint("Boolean"), ReadOnly]
        public bool IsDefaultSite { get; set; }

        [Caption("Site Name"), Description("The name associated with your site, usually your company name or your name")]
        [UIHint("String"), ReadOnly]
        public string SiteName { get; set; } = null!;

        [Caption("Site Id"), Description("The id associated with your site, generated by YetaWF when the site is created")]
        [UIHint("IntValue"), ReadOnly]
        public int Identity { get; set; }

        private SitesBrowseModule Module { get; set; }
        private ModuleDefinition SiteEditModule { get; set; }
        private SiteDefinition SiteData { get; set; }
        private ConfirmRemovalModule ConfirmModule { get; set; }

        public BrowseItem(SitesBrowseModule module, ModuleDefinition siteEditModule, ConfirmRemovalModule confirmModule, SiteDefinition site) {
            Module = module;
            SiteData = site;
            SiteEditModule = siteEditModule;
            ConfirmModule = confirmModule;
            ObjectSupport.CopyData(site, this, ForceReadOnlyFromCopy: true);
            Identity = site.Identity;
        }
    }

    public class BrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<SitesBrowseModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                ModuleDefinition siteEditModule = await ModuleDefinition.LoadAsync(new Guid("522296A0-B03B-49b7-B849-AB4149466E0D")) ?? throw new InternalError("Site Edit module not available");
                ConfirmRemovalModule confirmModule = new ConfirmRemovalModule();

                DataProviderGetRecords<SiteDefinition> info = await SiteDefinition.GetSitesAsync(skip, take, sort, filters);
                return new DataSourceResult {
                    Data = (from s in info.Data select new BrowseItem(this, siteEditModule, confirmModule, s)).ToList<object>(),
                    Total = info.Total
                };
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}