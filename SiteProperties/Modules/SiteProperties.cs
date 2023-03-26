/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

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
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.SiteProperties.Modules;

public class SitePropertiesModuleDataProvider : ModuleDefinitionDataProvider<Guid, SitePropertiesModule>, IInstallableModel { }

[ModuleGuid("522296A0-B03B-49b7-B849-AB4149466E0D"), PublishedModuleGuid]
public class SitePropertiesModule : ModuleDefinition {

    public SitePropertiesModule() {
        Title = this.__ResStr("modTitle", "Site Settings");
        Name = this.__ResStr("modName", "Site Settings");
        Description = this.__ResStr("modSummary", "Used to edit a site's properties. This module is used by the Sites Module to edit site properties. It is accessible using Admin > Settings > Site Settings (standard YetaWF site).");
        UsePartialFormCss = false;
    }
    public override IModuleDefinitionIO GetDataProvider() { return new SitePropertiesModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_EditCurrentSite(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Edit",
            LinkText = this.__ResStr("editSiteLink", "Site Settings"),
            MenuText = this.__ResStr("editSiteText", "Site Settings"),
            Tooltip = this.__ResStr("editSiteTooltip", "Change settings for the current site"),
            Legend = this.__ResStr("editSiteLegend", "Changes settings for the current site"),
            Location = ModuleAction.ActionLocationEnum.MainMenu,
            Mode = ModuleAction.ActionModeEnum.View,
            Style = ModuleAction.ActionStyleEnum.Popup,

        };
    }

    public ModuleAction GetAction_EditSite(string url, string domain) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { Domain = domain },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Site Settings"),
            MenuText = this.__ResStr("editText", "Site Settings"),
            Tooltip = this.__ResStr("editTooltip", "Change settings for site \"{0}\"", domain),
            Legend = this.__ResStr("editLegend", "Changes settings for site \"{0}\"", domain),
            Location = ModuleAction.ActionLocationEnum.MainMenu,
            Mode = ModuleAction.ActionModeEnum.View,
            Style = ModuleAction.ActionStyleEnum.Popup,

        };
    }

    // Properties used to save initial settings from Templates
    [DontSave]
    public SiteDefinition CurrentSite {
        get {
            return Manager.CurrentSite;
        }
    }
    public void InitComplete() {
        YetaWFManager.Syncify(async () => { // Runs only during installation
            await Manager.CurrentSite.SaveAsync();
        });
    }

    public class SitePropertiesModel {
        [UIHint("PropertyList")]
        public SiteDefinition Site { get; set; } = null!;
        [UIHint("Hidden")]
        public string SiteHost { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync(string domain) {
        SiteDefinition? site;
        if (string.IsNullOrEmpty(domain)) {
            site = Manager.CurrentSite;
            domain = site.SiteDomain;
        } else
            site = await SiteDefinition.LoadSiteDefinitionAsync(domain);
        if (site == null)
            throw new Error(this.__ResStr("errNoSite", "Site \"{0}\" not found", domain));
        SitePropertiesModel model = new SitePropertiesModel {
            SiteHost = domain,
            Site = site,
        };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(SitePropertiesModel model) {
        SiteDefinition? origSite;
        if (model.SiteHost == null)
            origSite = Manager.CurrentSite;
        else
            origSite = await SiteDefinition.LoadSiteDefinitionAsync(model.SiteHost) ?? throw new InternalError($"Host {model.SiteHost} not found");
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        ObjectSupport.CopyDataFromOriginal(origSite, model.Site);
        await model.Site.SaveAsync();
        ObjectSupport.ModelDisposition modelDisp = ObjectSupport.EvaluateModelChanges(origSite, model.Site);
        switch (modelDisp) {
            default:
            case ObjectSupport.ModelDisposition.None:
                return await FormProcessedAsync(model, this.__ResStr("okSaved", "Site settings updated"));
            case ObjectSupport.ModelDisposition.PageReload:
                return await FormProcessedAsync(model, this.__ResStr("okSaved", "Site settings updated"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage, ForceReload: true);
            case ObjectSupport.ModelDisposition.SiteRestart:
                return await FormProcessedAsync(model, this.__ResStr("okSavedRestart", "Site settings updated - These settings won't take effect until the site is restarted"));
        }
    }
}