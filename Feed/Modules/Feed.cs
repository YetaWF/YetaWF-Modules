/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feed#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
#if MVC6
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif
namespace YetaWF.Modules.Feed.Modules {

    // For documentation about Google Feed please see https://developers.google.com/feed/

    public class FeedModuleDataProvider : ModuleDefinitionDataProvider<Guid, FeedModule>, IInstallableModel { }

    [ModuleGuid("{04c32e25-f9bf-4baf-9602-3c929ce77790}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class FeedModule : ModuleDefinition {

        public FeedModule() : base() {
            Title = this.__ResStr("modTitle", "News Feed");
            Name = this.__ResStr("modName", "News Feed");
            Description = this.__ResStr("modSummary", "Displays the defined news feed. Use the module's Module Settings to define the desired news feed. Sample News Feed Modules can be accessed at Tests > News Feed (standard YetaWF site).");
            FeedUrl = "https://YetaWF.com/NewsFeed";
            Interval = 5;
            NumEntries = 10;
            WantSearch = false;
            WantFocus = false;
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new FeedModuleDataProvider(); }

        [Category("General"), Caption("News Feed Url"), Description("The Url providing the news feed")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Remote), StringLength(Globals.MaxUrl), UrlValidation, Required, Trim]
        public string FeedUrl { get; set; }

        [Category("General"), Caption("News Entries"), Description("The maximum number of news entries displayed (between 2 and 50)")]
        [UIHint("IntValue2"), Required, Range(2, 50)]
        public int NumEntries { get; set; }

        [Category("General"), Caption("Interval"), Description("The number of seconds after which the primary news item is replaced with the next item - specify 0 to disable")]
        [UIHint("IntValue2"), Required, Range(0, 30)]
        public int Interval { get; set; }

        [Category("Variables"), Caption("Cache Key"), Description("The name used to cache the news information")]
        [UIHint("String"), ReadOnly]
        public string CacheKey { get { return YetaWF.Modules.Feed.AreaRegistration.CurrentPackage.AreaName + "_" + ModuleGuidName + "_Rss_" + Manager.CurrentSite.Identity; } }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction> GetAction_NewsAsync() {
            return new ModuleAction(this) {
                Url = FeedUrl,
                Image = await CustomIconAsync("Feed.png"),
                LinkText = Title,
                MenuText = Title,
                Tooltip = this.__ResStr("displayTooltip", "Display the {0} news feed", Title),
                Legend = this.__ResStr("displayLegend", "Displays the {0} news feed", Title),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
        public override Task ModuleSavingAsync() {
            return RemoveCachedInfoAsync();// whenever the module is saved, we remove the cached information
        }
        public override Task ModuleRemovingAsync() {
            return RemoveCachedInfoAsync();// whenever the module is removed, we remove the cached information
        }
        private async Task RemoveCachedInfoAsync() {
            using (ICacheDataProvider localCacheDP = YetaWF.Core.IO.Caching.GetLocalCacheProvider()) {
                await localCacheDP.RemoveAsync<Controllers.FeedModuleController.DisplayModel>(CacheKey);
            }
        }
    }
}