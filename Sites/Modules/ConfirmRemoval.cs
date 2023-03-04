/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Sites#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Sites.Modules;

public class ConfirmRemovalModuleDataProvider : ModuleDefinitionDataProvider<Guid, ConfirmRemovalModule>, IInstallableModel { }

[ModuleGuid("{a3d76eb7-f9c2-4dca-b486-797b2d7d0037}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class ConfirmRemovalModule : ModuleDefinition2 {

    public ConfirmRemovalModule() {
        Title = this.__ResStr("modTitle", "Remove Site");
        Name = this.__ResStr("modName", "Remove Site Definition - Confirmation");
        Description = this.__ResStr("modSummary", "Displays a confirmation before the site is removed. It is used by the Sites module when a site is removed.");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ConfirmRemovalModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Remove(string? url, SiteDefinition site) {
        if (site.IsDefaultSite) return null;
        string siteName = site.SiteDomain;
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { SiteDomain = site.SiteDomain },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Normal,
            LinkText = this.__ResStr("removeLink", "Remove Site"),
            MenuText = this.__ResStr("removeMenu", "Remove Site"),
            Tooltip = this.__ResStr("removeTT", "Remove the site \"{0}\"", siteName),
            Legend = this.__ResStr("removeLegend", "Removes the site \"{0}\"", siteName),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
            DontFollow = true,
        };
    }

    [Header("Are you absolutely sure you want to remove this site? This will remove all data for this site!!! Please make sure you have a backup (just in case...).")]
    [Footer("Click Confirm to delete this site and all site-specific data.")]
    [Trim]
    public class EditModel {

        [Caption("Site"), Description("The domain name of the site to remove")]
        [UIHint("String"), ReadOnly]
        public string SiteDomainDisplay { get; set; } = null!;

        [UIHint("Hidden")]
        public string SiteDomain { get; set; } = null!;

        public EditModel() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        string siteDomain = Manager.RequestQueryString["SiteDomain"] ?? throw new InternalError($"{nameof(siteDomain)} parameter mising");
        EditModel model = new EditModel { };
        model.SiteDomainDisplay = siteDomain;
        model.SiteDomain = siteDomain;
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        model.SiteDomainDisplay = model.SiteDomain;
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        SiteDefinition? site = await SiteDefinition.LoadSiteDefinitionAsync(model.SiteDomain);
        if (site == null)
            throw new InternalError($"Site {model.SiteDomain} not found");

        SiteDefinition currentSite = Manager.CurrentSite;
        Manager.CurrentSite = site;
        try {
            await Manager.CurrentSite.RemoveAsync();
        } catch (Exception) {
            throw;
        } finally {
            Manager.CurrentSite = currentSite;
        }

        string nextPage = Manager.CurrentSite.MakeUrl(RealDomain: Manager.CurrentSite.SiteDomain);

        Manager.RestartSite();

        return await FormProcessedAsync(null, this.__ResStr("okRemoved", "Site \"{0}\" has been removed(+nl)(+nl)The site is now restarting", model.SiteDomain),
            NextPage: nextPage);
    }
}