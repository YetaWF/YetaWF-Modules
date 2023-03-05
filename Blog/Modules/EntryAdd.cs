/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Endpoints.Support;
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

public class EntryAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, EntryAddModule>, IInstallableModel { }

[ModuleGuid("{a08cc005-5da2-4cc6-91f9-b195879d4dab}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Entries")]
public class EntryAddModule : ModuleDefinition2 {

    public EntryAddModule() {
        Title = this.__ResStr("modTitle", "Add New Blog Entry");
        Name = this.__ResStr("modName", "Add Blog Entry");
        Description = this.__ResStr("modSummary", "Creates a new blog entry.");
        DefaultViewName = StandardViews.Add;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new EntryAddModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Add(string? url) {
        // if the url has a category, use it as default category to add a new entry
        int blogCategory;
        Manager.TryGetUrlArg<int>("BlogCategory", out blogCategory);
        object? qs = null;
        if (blogCategory != 0)
            qs = new { BlogCategory = blogCategory };
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = qs,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add"),
            MenuText = this.__ResStr("addText", "Add"),
            Tooltip = this.__ResStr("addTooltip", "Add a new blog entry"),
            Legend = this.__ResStr("addLegend", "Adds a new blog entry"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class AddModel {

        [Caption("Category"), Description("The category for this blog entry")]
        [UIHint("YetaWF_Blog_Category"), AdditionalMetadata("ShowSelect", true), Required]
        public int CategoryIdentity { get; set; }

        [Caption("Title"), Description("The title for this blog entry")]
        [UIHint("MultiString"), StringLength(BlogEntry.MaxTitle), Required, Trim]
        public MultiString Title { get; set; }

        [Caption("Keywords"), Description("The keywords for this blog entry")]
        [UIHint("MultiString"), StringLength(BlogEntry.MaxKwds), Trim]
        public MultiString Keywords { get; set; }

        [Caption("Author"), Description("The name of the blog author")]
        [UIHint("Text40"), StringLength(BlogEntry.MaxAuthor), Required, Trim]
        public string? Author { get; set; }

        [Caption("Author's Url"), Description("The optional Url linking to the author's information")]
        [UIHint("Url"), StringLength(Globals.MaxUrl), Trim]
        [AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        public string? AuthorUrl { get; set; }

        [Caption("Allow Comments"), Description("Defines whether comments can be entered for this blog entry")]
        [UIHint("Boolean")]
        public bool OpenForComments { get; set; }

        [Caption("Published"), Description("Defines whether this entry has been published and is viewable by everyone")]
        [UIHint("Boolean")]
        public bool Published { get; set; }
        [Caption("Date Published"), Description("The date this entry has been published")]
        [UIHint("Date"), RequiredIf("Published", true)]
        public DateTime DatePublished { get; set; }

        [Caption("Summary"), Description("The summary for this blog entry - If no summary is entered, the entire blog text is shown instead of the summary")]
        [UIHint("TextArea"), AdditionalMetadata("EmHeight", 10), StringLength(BlogEntry.MaxSummary)]
        [AdditionalMetadata("TextAreaSave", false)]
        public string? Summary { get; set; }

        [Caption("Blog Text"), Description("The complete text for this blog entry")]
        [UIHint("TextArea"), AdditionalMetadata("EmHeight", 20), StringLength(BlogEntry.MaxText)]
        [AdditionalMetadata("TextAreaSave", false)]
        [RequiredIf("Published", true)]
        public string? Text { get; set; }

        public AddModel() {
            Title = new MultiString();
            Keywords = new MultiString();
            DatePublished = DateTime.UtcNow;
        }

        public BlogEntry GetData() {
            BlogEntry data = new BlogEntry();
            ObjectSupport.CopyData(this, data);
            return data;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!int.TryParse(Manager.RequestQueryString["BlogCategory"], out int category)) category = 0;
        AddModel model = new AddModel {
            CategoryIdentity = category
        };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(AddModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
            if (!await dataProvider.AddItemAsync(model.GetData()))
                throw new Error(this.__ResStr("alreadyExists", "An error occurred adding this new blog entry"));
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New blog entry saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
