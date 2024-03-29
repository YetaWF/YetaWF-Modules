/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Sites#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.DataProvider;
#if MVC6
#else
using System.Web.Routing;
#endif

namespace YetaWF.Modules.Sites.Modules {

    public class ConfirmRemovalModuleDataProvider : ModuleDefinitionDataProvider<Guid, ConfirmRemovalModule>, IInstallableModel { }

    [ModuleGuid("{a3d76eb7-f9c2-4dca-b486-797b2d7d0037}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class ConfirmRemovalModule : ModuleDefinition {

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
                NeedsModuleContext = true,
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
    }
}