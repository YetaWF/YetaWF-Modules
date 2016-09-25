/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
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
        public ActionResult CategoriesList() {

            int blogCategory = Module.DefaultCategory;
            if (!Manager.TryGetUrlArg<int>("BlogCategory", out blogCategory, blogCategory))
                Manager.AddUrlArg("BlogCategory", blogCategory.ToString());
            Model model = new Model() {
                BlogCategory = blogCategory,
            };

            Manager.CurrentPage.EvaluatedCanonicalUrl = BlogConfigData.GetCategoryCanonicalName(blogCategory);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoriesList_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return Redirect(BlogConfigData.GetCategoryCanonicalName(model.BlogCategory));
        }

    }
}