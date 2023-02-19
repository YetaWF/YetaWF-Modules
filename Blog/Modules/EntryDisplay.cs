/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Modules;

public class EntryDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, EntryDisplayModule>, IInstallableModel { }

[ModuleGuid("{dc5acba8-fdf9-4146-abf3-8321b4fe8c7a}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Entries")]
public class EntryDisplayModule : ModuleDefinition2 {

    public EntryDisplayModule() {
        Title = this.__ResStr("modTitle", "Blog Entry");
        Name = this.__ResStr("modName", "Display Blog Entry");
        Description = this.__ResStr("modSummary", "Displays a blog entry. Add a Category Header Module to display information for the selected category. Add a Comments Display Module to display all comments for the blog entry. Add a Comment Add Module to allow visitors to enter comments for your blog entry.");
        ShowTitleActions = true;
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new EntryDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Edit Url"), Description("The Url to edit the displayed blog entry - if omitted, a default page is generated")]
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

    public class DisplayModel {

        public int Identity { get; set; }
        public int CategoryIdentity { get; set; }

        [Caption("Author"), Description("The name of the blog author")]
        [UIHint("String"), ReadOnly, SuppressIfNot("AuthorUrl", null)]
        public string? Author { get; set; }

        [Caption("Author"), Description("The optional Url linking to the author's information")]
        [UIHint("Url"), ReadOnly, SuppressEmpty]
        public string? AuthorUrl { get; set; }
        public string? AuthorUrl_Text { get { return Author; } }

        public bool Published { get; set; }

        [Caption("Date Published"), Description("The date this entry has been published")]
        [UIHint("Date"), SuppressIf("Published", false), ReadOnly]
        public DateTime DatePublished { get; set; }

        //[Caption("Date Created"), Description("The date this entry was created")]
        //[UIHint("DateTime"), ReadOnly]
        //public DateTime DateCreated { get; set; }

        //[Caption("Date Updated"), Description("The date this entry was updated")]
        //[UIHint("DateTime"), ReadOnly]
        //public DateTime DateUpdated { get; set; }

        [Caption("Blog Text"), Description("The complete text for this blog entry")]
        [UIHint("TextArea"), AdditionalMetadata("Encode", false), ReadOnly]
        public string? Text { get; set; }

        public void SetData(BlogEntry data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!int.TryParse(Manager.RequestQueryString["BlogEntry"], out int entryNum)) entryNum = 0;
        using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
            BlogEntry? data = null;
            if (entryNum != 0)
                data = await dataProvider.GetItemAsync(entryNum);
            if (data == null) {
                MarkNotFound();
                return await RenderAsync(new { }, ViewName: "NotFound");
            }

            Manager.CurrentPage.EvaluatedCanonicalUrl = await BlogConfigData.GetEntryCanonicalNameAsync(entryNum);
            if (!string.IsNullOrWhiteSpace(data.Keywords)) {
                Manager.CurrentPage.Keywords = data.Keywords;
                Manager.MetatagsManager.AddMetatag("news_keywords", data.Keywords.ToString());
            }
            Manager.CurrentPage.Description = data.Title;
            Manager.PageTitle = data.Title;

            DisplayModel model = new DisplayModel();
            model.SetData(data);
            Title = data.Title;
            return await RenderAsync(model);
        }
    }
}