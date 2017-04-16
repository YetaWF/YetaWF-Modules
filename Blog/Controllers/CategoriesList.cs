/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Blog.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

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
        [ConditionalAntiForgeryToken]
        public ActionResult CategoriesList_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return Redirect(BlogConfigData.GetCategoryCanonicalName(model.BlogCategory));
        }

    }
}