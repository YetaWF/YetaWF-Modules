/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Threading.Tasks;
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

public class CategoryDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, CategoryDisplayModule>, IInstallableModel { }

[ModuleGuid("{ead14c93-8fe1-4bed-9656-74c08e277723}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Categories")]
public class CategoryDisplayModule : ModuleDefinition {

    public CategoryDisplayModule() {
        Title = this.__ResStr("modTitle", "Blog Category");
        Name = this.__ResStr("modName", "Display Blog Category");
        Description = this.__ResStr("modSummary", "Displays details for an existing blog category. Used by the Blog Categories Module.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new CategoryDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Display(string? url, int blogCategory) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { BlogCategory = blogCategory },
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Display"),
            MenuText = this.__ResStr("displayText", "Display"),
            Tooltip = this.__ResStr("displayTooltip", "Display the blog category"),
            Legend = this.__ResStr("displayLegend", "Displays an existing blog category"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    public class DisplayModel {

        [Caption("Id"), Description("The id of this blog category - used to uniquely identify this blog category internally")]
        [UIHint("IntValue"), ReadOnly]
        public int Identity { get; set; }

        [Caption("Category"), Description("The name of this blog category")]
        [UIHint("MultiString"), ReadOnly]
        public MultiString Category { get; set; } = null!;

        [Caption("Description"), Description("The description of the blog category - the category's description is shown at the top of each blog entry to describe your blog")]
        [UIHint("MultiString"), ReadOnly]
        public MultiString Description { get; set; } = null!;

        [Caption("Date Created"), Description("The creation date of the blog category")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime DateCreated { get; set; }

        [Caption("Use Captcha"), Description("Defines whether anonymous users entering comments are presented with a Captcha to insure they are not automated spam scripts")]
        [UIHint("Boolean"), ReadOnly]
        public bool UseCaptcha { get; set; }

        [Caption("Comment Approval"), Description("Defines whether submitted comments must be approved before being publicly viewable")]
        [UIHint("Enum"), ReadOnly]
        public BlogCategory.ApprovalType CommentApproval { get; set; }

        [Caption("Syndicated"), Description("Defines whether the blog category can be subscribed to by news readers (entries must be published before they can be syndicated)")]
        [UIHint("Boolean"), ReadOnly]
        public bool Syndicated { get; set; }

        [Caption("Email Address"), Description("The email address used as email address responsible for the blog category")]
        [UIHint("String"), ReadOnly]
        public string? SyndicationEmail { get; set; }

        [Caption("Syndication Copyright"), Description("The optional copyright information shown when the blog is accessed by news readers")]
        [UIHint("MultiString"), ReadOnly]
        public MultiString SyndicationCopyright { get; set; } = null!;

        public void SetData(BlogCategory data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(int blogCategory) {
        int category = blogCategory;
        using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
            BlogCategory? data = await dataProvider.GetItemAsync(category);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Blog category with id {0} not found."), category);
            DisplayModel model = new DisplayModel();
            model.SetData(data);
            Title = this.__ResStr("title", "Blog Category \"{0}\"", data.Category.ToString());
            return await RenderAsync(model);
        }
    }
}
