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
        public ActionResult CategoryHeader(int? blogEntry) {
            int category;
            Manager.TryGetUrlArg<int>("BlogCategory", out category);
            int entry = (int) (blogEntry ?? 0);
            if (entry != 0) {
                using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                    BlogEntry data = entryDP.GetItem(entry);
                    if (data != null)
                        category = data.CategoryIdentity;
                }
            }
            if (category != 0) {
                using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                    BlogCategory data = dataProvider.GetItem(category);
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