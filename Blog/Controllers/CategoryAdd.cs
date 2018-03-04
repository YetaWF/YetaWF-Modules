/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Blog.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Controllers {
    public class CategoryAddModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CategoryAddModule> {

        public CategoryAddModuleController() { }

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
            public string SyndicationEmail { get; set; }

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

        [AllowGet]
        public ActionResult CategoryAdd() {
            AddModel model = new AddModel { };
            return View(model);
        }


        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> CategoryAdd_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                if (!await dataProvider.AddItemAsync(model.GetData())) {
                    ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "A blog category named \"{0}\" already exists.", model.Category));
                    return PartialView(model);
                }
                return FormProcessed(model, this.__ResStr("okSaved", "New blog category saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}
