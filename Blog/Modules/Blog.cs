/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.Controllers;
using YetaWF.Modules.Blog.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Routing;
#else
using System.Web.Routing;
#endif

namespace YetaWF.Modules.Blog.Modules {

    public class BlogModuleDataProvider : ModuleDefinitionDataProvider<Guid, BlogModule>, IInstallableModel { }

    [ModuleGuid("{e1954fdc-0ccb-40bd-b018-c40dc792e894}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BlogModule : ModuleDefinition {

        public BlogModule() {
            Title = this.__ResStr("modTitle", "Blog");
            Name = this.__ResStr("modName", "Blog");
            Description = this.__ResStr("modSummary", "Displays the main blog entry point with all or a single blog category");
            ShowTitleActions = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BlogModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public override List<ModuleAction> ModuleActions {
            get {
                List<ModuleAction> actions = base.ModuleActions;
                actions.New(GetAction_RssFeedAsync().Result);//$$$$$
                return actions;
            }
        }

        public async Task<ModuleAction> GetAction_BlogAsync(string url, int blogCategory = 0, DateTime? StartDate = null, int Count = 0) { //$$$$$
            BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
            QueryHelper query = new QueryHelper();
            if (string.IsNullOrWhiteSpace(url))
                url = await BlogConfigData.GetCategoryCanonicalNameAsync(blogCategory);
            else {
                url = ModulePermanentUrl;
                query.Add("BlogCategory", blogCategory.ToString());
            }
            string date = null;
            if (StartDate != null) {
                query.Add("StartDate", StartDate.ToString());
                date = Formatting.Date_Month_YYYY((DateTime) StartDate);
                if (StartDate >= DateTime.UtcNow)
                    date = this.__ResStr("latest", "{0} - Latest", date);
            } else {
                Count = 0;// must have a date for Count to be displayed
            }
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgsDict = query,
                Image = "#Display",
                LinkText = Count != 0 ? this.__ResStr("countLink", "{0} ({1})", date, Count) : this.__ResStr("displayLink", "Blog"),
                MenuText = Count != 0 ? this.__ResStr("countMenu", "{0} ({1})", date, Count) : this.__ResStr("displayText", "Blog"),
                Tooltip = Count != 0 ? this.__ResStr("countTooltip", "Display blog entries starting {0}", date, Count) : this.__ResStr("displayTooltip", "Display the blog"),
                Legend = Count != 0 ? this.__ResStr("countLegend", "Displays the blog entries starting {0}", date, Count) : this.__ResStr("displayLegend", "Displays the blog"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
        public async Task<ModuleAction> GetAction_RssFeedAsync(int blogCategory = 0) {   //?$$$$$$
            BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
            if (!config.Feed) return null;
            //if (blogCategory == 0)
            //    manager.TryGetUrlArg<int>("BlogCategory", out blogCategory);
            object qargs = null, qargsHR = null;
            if (blogCategory != 0) {
                qargs = new { BlogCategory = blogCategory, };
                using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                    BlogCategory data = await dataProvider.GetItemAsync(blogCategory);
                    if (data != null)
                        qargsHR = new { Title = data.Category.ToString().Truncate(80) };
                }
            }
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(RssController), "RssFeed"),
                QueryArgs = qargs,
                QueryArgsHR = qargsHR,
                Image = "RssFeed.png",
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("rssLink", "RSS Feed"),
                MenuText = this.__ResStr("rssMenu", "RSS Feed"),
                Tooltip = this.__ResStr("rssTT", "Display the blog's RSS Feed"),
                Legend = this.__ResStr("rssLegend", "Displays the blog's RSS Feed"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            };
        }
    }
}