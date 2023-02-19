/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Modules;

public class CategoryAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, CategoryAddModule>, IInstallableModel { }

[ModuleGuid("{beeabd31-6607-461a-aa0c-717645f1be83}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Categories")]
public class CategoryAddModule : ModuleDefinition2 {

    public CategoryAddModule() {
        Title = this.__ResStr("modTitle", "Add New Blog Category");
        Name = this.__ResStr("modName", "Add Blog Category");
        Description = this.__ResStr("modSummary", "Creates a new blog category. Used by the Blog Categories Module.");
        DefaultViewName = StandardViews.Add;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new CategoryAddModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Add(string? url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add"),
            MenuText = this.__ResStr("addText", "Add"),
            Tooltip = this.__ResStr("addTooltip", "Create a new blog category"),
            Legend = this.__ResStr("addLegend", "Creates a new blog category"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class AddModel {

        [Caption("Category"), Description("The name of this blog category")]
        [UIHint("MultiString40"), StringLength(BlogCategory.MaxCategory), Required, Trim]
        public MultiString Category { get; set; }

        [Caption("Description"), Description("The description of the blog category - the category's description is shown at the top of each blog entry to describe your blog")]
        [UIHint("MultiString80"), StringLength(BlogCategory.MaxDescription), Required, Trim]
        public MultiString Description { get; set; }

        [Caption("Use Captcha"), Description("Defines whether anonymous users entering comments are presented with a Captcha to insure they are not automated spam scripts")]
        [UIHint("Boolean")]
        public bool UseCaptcha { get; set; }

        [Caption("Comment Approval"), Description("Defines whether comments submitted must be approved before being publicly viewable")]
        [UIHint("Enum"), Required]
        public BlogCategory.ApprovalType CommentApproval { get; set; }

        [Caption("Syndicated"), Description("Defines whether the blog category can be subscribed to by news readers (entries must be published before they can be syndicated)")]
        [UIHint("Boolean")]
        public bool Syndicated { get; set; }

        [Caption("Syndication Email Address"), Description("The email address used as email address responsible for the blog category")]
        [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, RequiredIf("Syndicated", true), Trim]
        public string? SyndicationEmail { get; set; }

        [Caption("Syndication Copyright"), Description("The optional copyright information shown when the blog is accessed by news readers")]
        [UIHint("MultiString80"), StringLength(BlogCategory.MaxCopyright), Trim]
        public MultiString SyndicationCopyright { get; set; }

        public AddModel() {
            Category = new MultiString();
            Description = new MultiString();
            SyndicationCopyright = new MultiString();
        }

        public BlogCategory GetData() {
            BlogCategory data = new BlogCategory();
            ObjectSupport.CopyData(this, data);
            return data;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        AddModel model = new AddModel { };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(AddModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
            if (!await dataProvider.AddItemAsync(model.GetData())) {
                ModelState.AddModelError(nameof(model.Category), this.__ResStr("alreadyExists", "A blog category named \"{0}\" already exists.", model.Category));
                return await PartialViewAsync(model);
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New blog category saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
