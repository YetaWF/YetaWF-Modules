/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.SendEmail;
using YetaWF.Core.Support;
using YetaWF.Core.Components;
using YetaWF.Modules.Blog.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Controllers {
    public class CommentAddModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CommentAddModule> {

        public CommentAddModuleController() { }

        [Trim]
        [Header("Complete this simple form to add a comment to this blog entry.")]
        public class AddModel {

            [UIHint("Hidden")]
            public int CategoryIdentity { get; set; }
            [UIHint("Hidden")]
            public int EntryIdentity { get; set; }

            public bool GravatarsEnabled { get; set; }

            [Caption("Your Name"), Description("Enter your name - Once your comment is published, your name will appear as part of your comment")]
            [UIHint("Text40"), StringLength(BlogComment.MaxName), Required, Trim]
            public string Name { get; set; }

            [Caption("Your Email Address"), Description("Enter your email address - Once your comment is published, your email address will not be shown, it is not publicly viewable")]
            [UIHint("Email"), EmailValidation, StringLength(Globals.ChEmail), Required, Trim]
            public string Email { get; set; }

            [Caption("Show Gravatar"), Description("Defines whether your optional Gravatar is shown, which is a publicly viewable image, that you can define using the free service at http://gravatar.com so all your comments will appear with your image")]
            [HelpLink("http://www.gravatar.com")]
            [UIHint("Boolean"), SuppressIf("GravatarsEnabled", false)]
            public bool ShowGravatar { get; set; }

            [Caption("Your Website"), Description("Enter your optional website, so readers of your comment may visit your website. Your website is completely optional - Once your comment is published, your website address will appear as part of your comment")]
            [UIHint("Url"), StringLength(Globals.MaxUrl), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Remote), Trim]
            public string Website { get; set; }

            [Caption("Title"), Description("Enter a title for your comment - Once your comment is published, the title will appear as part of your comment")]
            [UIHint("Text80"), StringLength(BlogComment.MaxTitle), Required, Trim]
            public string Title { get; set; }

            [Caption("Comment"), Description("Enter your comment about this blog entry for others to view")]
            [UIHint("TextArea"), AdditionalMetadata("EmHeight", 10), StringLength(BlogComment.MaxComment)]
            [AdditionalMetadata("TextAreaSave", false), AdditionalMetadata("RestrictedHtml", true)]
            public string Comment { get; set; }

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIf("ShowCaptcha", false)]
            public RecaptchaV2Data Captcha { get; set; }

            [UIHint("Hidden")]
            public bool ShowCaptcha { get; set; }

            public bool OpenForComments { get; set; }

            public AddModel() {
                ObjectSupport.CopyData(new BlogComment(), this);
                Captcha = new RecaptchaV2Data() { };
            }

            public BlogComment GetData() {
                BlogComment data = new BlogComment();
                ObjectSupport.CopyData(this, data);
                return data;
            }

            internal async Task UpdateDataAsync() {
                BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
                GravatarsEnabled = config.ShowGravatar;

                using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                    BlogCategory cat = await categoryDP.GetItemAsync(CategoryIdentity);
                    if (cat == null) throw new InternalError("Category with id {0} not found", CategoryIdentity);
                    ShowCaptcha = cat.UseCaptcha;
                    ShowGravatar = GravatarsEnabled;
                }
                OpenForComments = await TestOpenForCommentsAsync(EntryIdentity);
            }
            private async Task<bool> TestOpenForCommentsAsync(int blogEntry) {
                using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                    BlogEntry ent = await entryDP.GetItemAsync(blogEntry);
                    if (ent == null) throw new InternalError("Entry with id {0} not found", blogEntry);
                    return ent.OpenForComments;
                }
            }
        }

        [AllowGet]
        public async Task<ActionResult> CommentAdd(int? blogEntry) {
            int entryNum = blogEntry ?? 0;
            int blogCategory;
            using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                BlogEntry data = null;
                if (entryNum != 0)
                    data = await entryDP.GetItemAsync(entryNum);
                if (data == null)
                    return new EmptyResult();
                blogCategory = data.CategoryIdentity;
                if (!data.OpenForComments) {
                    if (data.Comments == 0)
                        return new EmptyResult();
                }
            }
            AddModel model = new AddModel {
                CategoryIdentity = blogCategory,
                EntryIdentity = entryNum,
                Captcha = new RecaptchaV2Data(),
            };
            await model.UpdateDataAsync();

            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> CommentAdd_Partial(AddModel model) {
            await model.UpdateDataAsync();
            using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                BlogEntry blogEntry = await entryDP.GetItemAsync(model.EntryIdentity);
                if (blogEntry == null)
                    throw new Error(this.__ResStr("notFound", "Blog entry with id {0} not found."), model.EntryIdentity);
                if (!blogEntry.OpenForComments)
                    throw new InternalError("Can't add comments to this blog entry");
                if (!ModelState.IsValid)
                    return PartialView(model);
                using (BlogCommentDataProvider blogCommentDP = new BlogCommentDataProvider(model.EntryIdentity)) {
                    BlogComment blogComment = model.GetData();
                    if (!await blogCommentDP.AddItemAsync(blogComment)) {
                        ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "An error occurred adding this new comment"));
                        return PartialView(model);
                    }
                    // send notification email
                    BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
                    if (config.NotifyNewComment) {
                        SendEmail sendEmail = new SendEmail();
                        object parms = new {
                            Description = !blogComment.Approved ? this.__ResStr("needApproval", "This comment requires your approval.") : this.__ResStr("autoApproval", "This comment has been automatically approved."),
                            Category = (await blogEntry.GetCategoryAsync()).ToString(),
                            Title = blogEntry.Title.ToString(),
                            Url = Manager.CurrentSite.MakeUrl(await BlogConfigData.GetEntryCanonicalNameAsync(blogEntry.Identity)),
                            Comment = Utility.HtmlDecode(model.Comment),
                            UserName = model.Name,
                            UserEmail = model.Email,
                            UserWebsite = model.Website,
                        };
                        string subject = this.__ResStr("newComment", "New Blog Comment ({0} - {1})", blogEntry.Title.ToString(), Manager.CurrentSite.SiteDomain);
                        await sendEmail.PrepareEmailMessageAsync(config.NotifyEmail, subject, await sendEmail.GetEmailFileAsync(Package.GetCurrentPackage(this), "New Comment.txt"), parameters: parms);
                        await sendEmail.SendAsync(true);
                    }

                    if (!blogComment.Approved)
                        return FormProcessed(model, this.__ResStr("okSavedReview", "New comment saved - It will be reviewed before becoming publicly viewable"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
                    else
                        return FormProcessed(model, this.__ResStr("okSaved", "New comment added"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
                }
            }
        }
    }
}
