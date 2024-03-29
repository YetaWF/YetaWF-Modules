/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Search.Controllers;
using YetaWF.Modules.Search.DataProvider;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Search.Modules {

    public class SearchBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchBrowseModule>, IInstallableModel { }

    [ModuleGuid("{579f8078-c443-4ca8-9f1c-189b0935303a}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SearchBrowseModule : ModuleDefinition {

        public SearchBrowseModule() {
            Title = this.__ResStr("modTitle", "Search Keywords");
            Name = this.__ResStr("modName", "Search Keywords");
            Description = this.__ResStr("modSummary", "Displays and manages search keywords that were found in the site's pages. It is accessible using Admin > Panel > Search Keywords (standard YetaWF site). This module offers a Collect Keywords action, which can be used to extract all page keywords immediately (instead of by the Scheduler at regular intervals). This task may run for a long time. Editing and removing a search keyword is available in the Search Keywords grid shown by the Search Keywords Module.");
            DefaultViewName = StandardViews.Browse;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SearchBrowseModuleDataProvider(); }

        [Category("General"), Caption("Add URL"), Description("The URL to add a new search keyword - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string? AddUrl { get; set; }
        [Category("General"), Caption("Display URL"), Description("The URL to display a search keyword - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string? DisplayUrl { get; set; }
        [Category("General"), Caption("Edit URL"), Description("The URL to edit a search keyword - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string? EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveItems",
                        this.__ResStr("roleRemItemsC", "Remove Search Items"), this.__ResStr("roleRemItems", "The role has permission to remove individual search keywords"),
                        this.__ResStr("userRemItemsC", "Remove Search Items"), this.__ResStr("userRemItems", "The user has permission to remove individual search keywords")),
                    new RoleDefinition("CollectKeywords",
                        this.__ResStr("roleCollectC", "Collect Keywords"), this.__ResStr("roleCollect", "The role has permission to collect a site's search keywords"),
                        this.__ResStr("userCollectC", "Collect Keywords"), this.__ResStr("userCollect", "The user has permission to collect a site's search keywords")),
                };
            }
        }

        public ModuleAction? GetAction_Items(string? url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Search Keywords"),
                MenuText = this.__ResStr("browseText", "Search Keywords"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage search keywords"),
                Legend = this.__ResStr("browseLegend", "Displays and manages search keywords"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction? GetAction_Remove(int searchDataId) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(SearchBrowseModuleController), "Remove"),
                NeedsModuleContext = true,
                QueryArgs = new { SearchDataId = searchDataId },
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove search keyword"),
                MenuText = this.__ResStr("removeMenu", "Remove search keyword"),
                Tooltip = this.__ResStr("removeTT", "Remove the search keyword"),
                Legend = this.__ResStr("removeLegend", "Removes the search keyword"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this keyword?"),
            };
        }
        public ModuleAction? GetAction_RemoveAll() {
            if (!IsAuthorized("RemoveItems")) return null;
            if (!SearchDataProvider.IsUsable) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(SearchBrowseModuleController), "RemoveAll"),
                NeedsModuleContext = true,
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeAllLink", "Remove All"),
                MenuText = this.__ResStr("removeAllMenu", "Remove All"),
                Tooltip = this.__ResStr("removeAllTT", "Remove all search keyword"),
                Legend = this.__ResStr("removeAllLegend", "Remove all search keyword"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("removeAllConfirm", "Are you sure you want to remove ALL keywords?"),
                PleaseWaitText = this.__ResStr("removeAllPlsWait", "Keywords are being removed..."),
            };
        }
        public async Task<ModuleAction?> GetAction_CollectKeywordsAsync() {
            if (!IsAuthorized("CollectKeywords")) return null;
            if (!SearchDataProvider.IsUsable) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(SearchBrowseModuleController), nameof(SearchBrowseModuleController.CollectKeywords)),
                NeedsModuleContext = true,
                Image = await CustomIconAsync("CollectKeywords.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("collectLink", "Collect Keywords"),
                MenuText = this.__ResStr("collectMenu", "Collect Keywords"),
                Tooltip = this.__ResStr("collectTT", "Collect the site's search keywords by examining all pages - Only considers pages modified since the last search"),
                Legend = this.__ResStr("collectLegend", "Collects a site's search keywords by examining all pages - Only considers pages modified since the last search"),
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("collectConfirm", "Are you sure you want to collect all keywords for this site?"),
                PleaseWaitText = this.__ResStr("collectPlsWait", "Keywords are being collected..."),
            };
        }
    }
}