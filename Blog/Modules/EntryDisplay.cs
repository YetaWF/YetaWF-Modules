/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Modules {

    public class EntryDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, EntryDisplayModule>, IInstallableModel { }

    [ModuleGuid("{dc5acba8-fdf9-4146-abf3-8321b4fe8c7a}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Entries")]
    public class EntryDisplayModule : ModuleDefinition {

        public EntryDisplayModule() {
            Title = this.__ResStr("modTitle", "Blog Entry");
            Name = this.__ResStr("modName", "Display Blog Entry");
            Description = this.__ResStr("modSummary", "Displays a blog entry. Add a Category Header Module to display information for the selected category. Add a Comments Display Module to display all comments for the blog entry. Add a Comment Add Module to allow visitors to enter comments for your blog entry.");
            ShowTitleActions = true;
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EntryDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Edit URL"), Description("The URL to edit the displayed blog entry - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string? EditUrl { get; set; }

        public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
            EntryEditModule mod = new EntryEditModule();
            int blogEntry;
            if (Manager.TryGetUrlArg<int>("BlogEntry", out blogEntry))
                menuList.New(await mod.GetAction_EditAsync(EditUrl, blogEntry), location);
            BlogModule blogMod = new BlogModule();
            menuList.New(await blogMod.GetAction_RssFeedAsync(), location);
            return menuList;
        }

        public async Task<ModuleAction> GetAction_DisplayAsync(int blogEntry, bool ReadMore = false) {
            string url = await BlogConfigData.GetEntryCanonicalNameAsync(blogEntry);
            return new ModuleAction(this) {
                Url = url,
                Image = "#Display",
                LinkText = ReadMore ? this.__ResStr("moreLink", "Read More") : this.__ResStr("displayLink", "Display"),
                MenuText = ReadMore ? this.__ResStr("moreText", "Display") : this.__ResStr("displayText", "Display"),
                Tooltip = ReadMore ? this.__ResStr("moreTooltip", "Read the entire blog entry") : this.__ResStr("displayTooltip", "Display the blog entry"),
                Legend = ReadMore ? this.__ResStr("moreLegend", "Displays the entire blog entry") : this.__ResStr("displayLegend", "Displays an existing blog entry"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}