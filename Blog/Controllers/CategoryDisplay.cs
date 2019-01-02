/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Controllers {

    public class CategoryDisplayModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CategoryDisplayModule> {

        public CategoryDisplayModuleController() { }

        public class DisplayModel {

            [Caption("Id"), Description("The id of this blog category - used to uniquely identify this blog category internally")]
            [UIHint("IntValue"), ReadOnly]
            public int Identity { get; set; }

            [Caption("Category"), Description("The name of this blog category")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Category { get; set; }

            [Caption("Description"), Description("The description of the blog category - the category's description is shown at the top of each blog entry to describe your blog")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Description { get; set; }

            [Caption("Date Created"), Description("The creation date of the blog category")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateCreated { get; set; }

            [Caption("Use Captcha"), Description("Defines whether anonymous users entering comments are presented with a Captcha to insure they are not automated spam scripts")]
            [UIHint("Boolean"), ReadOnly]
            public bool UseCaptcha { get; set; }

            [Caption("Comment Approval"), Description("Defines whether submitted comments must be approved before being publicly viewable")]
            [UIHint("Enum"), ReadOnly]
            public BlogCategory.ApprovalType CommentApproval { get; set; }

            [Caption("Syndicated"), Description("Defines whether the blog category can be subscribed to by news readers (entries must be published before they can be syndicated)")]
            [UIHint("Boolean"), ReadOnly]
            public bool Syndicated { get; set; }

            [Caption("Email Address"), Description("The email address used as email address responsible for the blog category")]
            [UIHint("String"), ReadOnly]
            public string SyndicationEmail { get; set; }

            [Caption("Syndication Copyright"), Description("The optional copyright information shown when the blog is accessed by news readers")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString SyndicationCopyright { get; set; }

            public void SetData(BlogCategory data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public async Task<ActionResult> CategoryDisplay(int blogCategory) {
            using (BlogCategoryDataProvider dataProvider = new BlogCategoryDataProvider()) {
                BlogCategory data = await dataProvider.GetItemAsync(blogCategory);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Blog category with id {0} not found."), blogCategory);
                DisplayModel model = new DisplayModel();
                model.SetData(data);
                Module.Title = this.__ResStr("modTitle", "Blog Category \"{0}\"", data.Category.ToString());
                return View(model);
            }
        }
    }
}