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
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Modules;

public class CategoryHeaderModuleDataProvider : ModuleDefinitionDataProvider<Guid, CategoryHeaderModule>, IInstallableModel { }

[ModuleGuid("{7c3d3c99-78a0-4661-bbc7-77c71978463c}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Categories")]
public class CategoryHeaderModule : ModuleDefinition2 {

    public CategoryHeaderModule() {
        Title = this.__ResStr("modTitle", "Blog Category Header");
        Name = this.__ResStr("modName", "Blog Category Header");
        Description = this.__ResStr("modSummary", "Displays a blog category header. Add this to a page with a Blog Module or an Entry Display Module to show the category information for the current blog category.");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new CategoryHeaderModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public class DisplayModel {

        public int Identity { get; set; }
        public MultiString Category { get; set; } = null!;

        [UIHint("MultiString")]
        public MultiString Description { get; set; } = null!;

        public void SetData(BlogCategory data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(int blogCategory, int blogEntry) {
        int category = blogCategory;
        int entry = blogEntry;
        if (entry != 0) {
            using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                BlogEntry? data = await entryDP.GetItemAsync(entry);
                if (data != null)
                    category = data.CategoryIdentity;
            }
        }
        if (category != 0) {
            using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                BlogCategory? data = await dataProvider.GetItemAsync(category);
                if (data != null) {
                    DisplayModel model = new DisplayModel();
                    model.SetData(data);
                    Title = data.Category.ToString();
                    return await RenderAsync(model);
                }
            }
        }
        return ActionInfo.Empty;
    }
}
