/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Modules;

public class TinyVisitorsModuleDataProvider : ModuleDefinitionDataProvider<Guid, TinyVisitorsModule>, IInstallableModel { }

[ModuleGuid("{0180db70-e251-494e-87e9-e97d3796c1ce}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class TinyVisitorsModule : ModuleDefinition {

    public TinyVisitorsModule() {
        Title = this.__ResStr("modTitle", "Visitor Stats");
        Name = this.__ResStr("modName", "Visitor Stats");
        Description = this.__ResStr("modSummary", "Displays a small icon. When an authorized user hovers over the icon, today's and yesterday's totals for anonymous and logged on visitors are shown. By clicking on the icon, the Visitors Module is displayed, showing all visitor activity (assuming that the URL for a page with the Visitors Module has been defined in the TinyVisitor Module's settings). This module is typically added to every page (as a skin module) so the Administrator has access to visitor activity.");
        WantFocus = false;
        WantSearch = false;
        ShowTitle = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new TinyVisitorsModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Visitors Url"), Description("The page shown when the icon is clicked")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
    [StringLength(Globals.MaxUrl), Trim]
    public string? VisitorsUrl { get; set; }

    public class DisplayModel {

        public int TodaysAnonymous { get; set; }
        public int TodaysUsers { get; set; }
        public int YesterdaysAnonymous { get; set; }
        public int YesterdaysUsers { get; set; }

        public string? VisitorsUrl { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Tooltip { get; set; } = null!;

        public void SetData(VisitorEntry data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
            if (visitorDP.Usable) {
                string addonUrl = Package.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName);
                VisitorEntryDataProvider.Info info = await visitorDP.GetStatsAsync();
                DisplayModel model = new DisplayModel { };
                model.TodaysAnonymous = info.TodaysAnonymous;
                model.TodaysUsers = info.TodaysUsers;
                model.YesterdaysAnonymous = info.YesterdaysAnonymous;
                model.YesterdaysUsers = info.YesterdaysUsers;
                model.Tooltip = this.__ResStr("tooltip", "Today: {0} Users, {1} Anonymous - Yesterday: {2}/{3}", model.TodaysUsers, model.TodaysAnonymous, model.YesterdaysUsers, model.YesterdaysAnonymous);
                model.ImageUrl = addonUrl + "Icons/People.png";
                model.VisitorsUrl = VisitorsUrl;
                return await RenderAsync(model);
            }
            return ActionInfo.Empty;
        }
    }
}