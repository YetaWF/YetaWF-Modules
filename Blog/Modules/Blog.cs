/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Web.Routing;
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

namespace YetaWF.Modules.Blog.Modules {
    
    public class BlogModuleDataProvider : ModuleDefinitionDataProvider<Guid, BlogModule>, IInstallableModel { }

    [ModuleGuid("{e1954fdc-0ccb-40bd-b018-c40dc792e894}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BlogModule : ModuleDefinition {

        public BlogModule() {
            Title = this.__ResStr("modTitle", "Blog");
            Name = this.__ResStr("modName", "Blog");
            Description = this.__ResStr("modSummary", "Displays the main blog entry point with all or a single blog category");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BlogModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public override List<ModuleAction> ModuleActions {
            get {
                List<ModuleAction> actions = base.ModuleActions;
                actions.New(GetAction_RssFeed());
                return actions;
            }
        }

        public ModuleAction GetAction_Blog(string url, int blogCategory = 0, DateTime? StartDate = null, int Count = 0) {
            BlogConfigData config = BlogConfigDataProvider.GetConfig();
            if (!config.Feed) return null;
            RouteValueDictionary rvd = new RouteValueDictionary();
            if (string.IsNullOrWhiteSpace(url))
                url = BlogConfigData.GetCategoryCanonicalName(blogCategory);
            else {
                url = ModulePermanentUrl;
                rvd.Add("BlogCategory", blogCategory);
            }
            string date = null;
            if (StartDate != null) {
                rvd.Add("StartDate", StartDate);
                date = Formatting.Date_Month_YYYY((DateTime) StartDate);
            } else {
                Count = 0;// must have a date for Count to be displayed
            }
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgsRvd = rvd,
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
        public ModuleAction GetAction_RssFeed(int blogCategory = 0) {
            BlogConfigData config = BlogConfigDataProvider.GetConfig();
            if (!config.Feed) return null;
            //if (blogCategory == 0)
            //    manager.TryGetUrlArg<int>("BlogCategory", out blogCategory);
            object qargs = null, qargsHR = null;
            if (blogCategory != 0) {
                qargs = new { BlogCategory = blogCategory, };
                using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                    BlogCategory data = dataProvider.GetItem(blogCategory);
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