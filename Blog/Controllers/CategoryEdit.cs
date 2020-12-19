/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Controllers {

    public class CategoryEditModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CategoryEditModule> {

        public CategoryEditModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Id"), Description("The id of this blog category - used to uniquely identify this blog category internally")]
            [UIHint("IntValue"), ReadOnly]
            public int DisplayIdentity { get; set; }

            [Caption("Category"), Description("The name of this blog category")]
            [UIHint("MultiString40"), StringLength(BlogCategory.MaxCategory), Required, Trim]
            public MultiString Category { get; set; } = null!;

            [Caption("Description"), Description("The description of the blog category - the category's description is shown at the top of each blog entry to describe your blog")]
            [UIHint("MultiString80"), StringLength(BlogCategory.MaxDescription), Required, Trim]
            public MultiString Description { get; set; } = null!;

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
            public MultiString SyndicationCopyright { get; set; } = null!;

            [UIHint("Hidden")]
            public int Identity { get; set; }

            public BlogCategory GetData(BlogCategory data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(BlogCategory data) {
                ObjectSupport.CopyData(data, this);
                DisplayIdentity = Identity;
            }
            public EditModel() { }
        }

        [AllowGet]
        public async Task<ActionResult> CategoryEdit(int blogCategory) {
            using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                EditModel model = new EditModel { };
                BlogCategory? data = await dataProvider.GetItemAsync(blogCategory);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Blog category with id {0} not found."), blogCategory);
                model.SetData(data);
                Module.Title = this.__ResStr("modTitle", "Blog Category \"{0}\"", data.Category.ToString());
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> CategoryEdit_Partial(EditModel model) {
            int originalCategory = model.Identity;

            using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                // get the original item
                BlogCategory? data = await dataProvider.GetItemAsync(originalCategory);
                if (data == null)
                    throw new Error(this.__ResStr("alreadyDeletedId", "The blog category with id {0} has been removed and can no longer be updated.", originalCategory));

                if (!ModelState.IsValid)
                    return PartialView(model);

                // save updated item
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                switch (await dataProvider.UpdateItemAsync(data)) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("alreadyDeleted", "The blog category named \"{0}\" has been removed and can no longer be updated.", model.Category.ToString()));
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError(nameof(model.Category), this.__ResStr("alreadyExists", "A blog category named \"{0}\" already exists.", model.Category.ToString()));
                        return PartialView(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Blog category saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}