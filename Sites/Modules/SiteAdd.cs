/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Sites#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.DataProvider;
using YetaWF.Modules.Packages.DataProvider;

namespace YetaWF.Modules.Sites.Modules;

public class SiteAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, SiteAddModule>, IInstallableModel { }

[ModuleGuid("{c4505071-bc76-4c88-8b01-fe40a8bc273d}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SiteAddModule : ModuleDefinition {

    public SiteAddModule() {
        Title = this.__ResStr("modTitle", "Add New Site");
        Name = this.__ResStr("modName", "Add New Site");
        Description = this.__ResStr("modSummary", "Creates a new web site. It is used by the Sites Module. It is recommended to add the new site domain to your hosts file BEFORE adding a new site, so IIS can resolve the site when it is requested the first time.");
        DefaultViewName = StandardViews.Add;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SiteAddModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Add(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add New Site"),
            MenuText = this.__ResStr("addText", "Add New Site"),
            Tooltip = this.__ResStr("addTooltip", "Create a new web site"),
            Legend = this.__ResStr("addLegend", "Creates a new web site"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,

        };
    }

    [Trim]
    public class AddModel {

        [Caption("Site Domain"), Description("The domain name of the site")]
        [UIHint("Text80"), StringLength(SiteDefinition.MaxSiteDomain), SiteDomainValidation, Required, Trim]
        public string SiteDomain { get; set; } = null!;

        [Caption("Site Name"), Description("The name associated with your site, usually your company name or your name")]
        [UIHint("Text80"), StringLength(SiteDefinition.MaxSiteName), Required, Trim]
        public string SiteName { get; set; } = null!;

        public AddModel() { }

        public SiteDefinition GetData() {
            SiteDefinition data = new SiteDefinition();
            ObjectSupport.CopyData(this, data);
            return data;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        AddModel model = new AddModel { };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(AddModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        SiteDefinition currentSite = Manager.CurrentSite;

        SiteDefinition newSite = model.GetData();
        await newSite.AddNewAsync();

        Manager.CurrentSite = newSite;
        try {
            PackagesDataProvider packagesDP = new PackagesDataProvider();
            await packagesDP.InitNewAsync(true);
        } catch (Exception) {
            throw;
        } finally {
            Manager.CurrentSite = currentSite;
        }

        string nextPage = Manager.CurrentSite.MakeUrl(RealDomain: Manager.CurrentSite.SiteDomain);

        Manager.RestartSite();

        return await FormProcessedAsync(model, this.__ResStr("okSaved", "New site \"{0}\" created - Click OK to populate the new site with the current site template.(+nl)(+nl)IMPORTANT: This site is not accessible by its Url until the domain \"{0}\" is defined in IIS and in the hosts file.", newSite.SiteDomain),
            NextPage: nextPage);
    }
}

