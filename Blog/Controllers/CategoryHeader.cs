/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Controllers {

    public class CategoryHeaderModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CategoryHeaderModule> {

        public CategoryHeaderModuleController() { }

        public class DisplayModel {

            public int Identity { get; set; }
            public MultiString Category { get; set; }

            [UIHint("MultiString")]
            public MultiString Description { get; set; }

            public void SetData(BlogCategory data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [HttpGet]
        public ActionResult CategoryHeader(int? blogEntry, int blogCategory = 0) {
            int entry = (int) (blogEntry ?? 0);
            if (entry != 0) {
                using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                    BlogEntry data = entryDP.GetItem(entry);
                    if (data != null)
                        blogCategory = data.CategoryIdentity;
                }
            }
            if (blogCategory != 0) {
                using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                    BlogCategory data = dataProvider.GetItem(blogCategory);
                    if (data != null) {
                        DisplayModel model = new DisplayModel();
                        model.SetData(data);
                        Module.Title = data.Category.ToString();
                        Manager.PageTitle = data.Category.ToString();
                        if (string.IsNullOrWhiteSpace(Manager.CurrentPage.Description))
                            Manager.CurrentPage.Description = data.Category.ToString();
                        if (string.IsNullOrWhiteSpace(Manager.CurrentPage.Keywords))
                            Manager.CurrentPage.Keywords = data.Category.ToString();
                        return View(model);
                    }
                }
            }
            return new EmptyResult();
        }
    }
}