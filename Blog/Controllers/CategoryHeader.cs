/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Controllers {

    public class CategoryHeaderModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CategoryHeaderModule> {

        public CategoryHeaderModuleController() { }

        public class DisplayModel {

            public int Identity { get; set; }
            public MultiString Category { get; set; } = null!;

            [UIHint("MultiString")]
            public MultiString Description { get; set; } = null!;

            public void SetData(BlogCategory data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public async Task<ActionResult> CategoryHeader(int? blogEntry) {
            int category;
            Manager.TryGetUrlArg<int>("BlogCategory", out category);
            int entry = (int) (blogEntry ?? 0);
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
                        Module.Title = data.Category.ToString();
                        return View(model);
                    }
                }
            }
            return new EmptyResult();
        }
    }
}