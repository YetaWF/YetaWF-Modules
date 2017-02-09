/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Controllers {

    public class CategoriesListModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CategoriesListModule> {

        public CategoriesListModuleController() { }

        public class Model {

            [Caption("Blog Category"), Description("Used to select the displayed blog category")]
            [UIHint("YetaWF_Blog_Category"), AdditionalMetadata("ShowAll", true), SubmitFormOnChange, Required]
            public int BlogCategory { get; set; }

        }

        [HttpGet]
        public ActionResult CategoriesList(int blogCategory = 0) {
            Model model = new Model() {
                BlogCategory = blogCategory,
            };
            Manager.CurrentPage.EvaluatedCanonicalUrl = BlogConfigData.GetCategoryCanonicalName(blogCategory);
            if (blogCategory != 0) {
                using (BlogCategoryDataProvider catDP = new BlogCategoryDataProvider()) {
                    BlogCategory cat = catDP.GetItem(blogCategory);
                    if (cat == null)
                        throw new Error(this.__ResStr("notFound", "Blog category id {0} not found."), blogCategory);
                    if (string.IsNullOrWhiteSpace(Manager.PageTitle))
                        Manager.PageTitle = cat.Category;
                    if (string.IsNullOrWhiteSpace(Manager.CurrentPage.Description))
                        Manager.CurrentPage.Description = cat.Category;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        public ActionResult CategoriesList_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return Redirect(BlogConfigData.GetCategoryCanonicalName(model.BlogCategory));
        }

    }
}