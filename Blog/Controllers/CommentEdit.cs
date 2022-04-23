/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Addons;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Controllers {

    public class CommentEditModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CommentEditModule> {

        public CommentEditModuleController() { }

        [Trim]
        public class EditModel {

            [UIHint("Hidden")]
            public int Identity { get; set; }
            [UIHint("Hidden")]
            public int CategoryIdentity { get; set; }
            [UIHint("Hidden")]
            public int EntryIdentity { get; set; }

            public bool GravatarsEnabled { get; set; }

            [Caption("Name"), Description("The author's name")]
            [UIHint("Text40"), StringLength(BlogComment.MaxName), Required, Trim]
            public string? Name { get; set; }

            [Caption("Email Address"), Description("The author's email address")]
            [UIHint("Email"), EmailValidation, StringLength(Globals.ChEmail), Required, Trim]
            public string? Email { get; set; }

            [Caption("Show Gravatar"), Description("Defines whether the optional Gravatar is shown")]
            [HelpLink("http://www.gravatar.com")]
            [UIHint("Boolean"), SuppressIf("GravatarsEnabled", false)]
            public bool ShowGravatar { get; set; }

            [Caption("Website"), Description("The author's optional website")]
            [UIHint("Url"), StringLength(Globals.MaxUrl), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Remote), Trim]
            public string? Website { get; set; }

            [Caption("Approved"), Description("Defines whether the comment has been approved for public display")]
            [UIHint("Boolean")]
            public bool Approved { get; set; }
            [Caption("Deleted"), Description("Defines whether the comment has been deleted")]
            [UIHint("Boolean")]
            public bool Deleted { get; set; }
            [Caption("Date Created"), Description("The date the comment was created")]
            [UIHint("DateTime"), Required]
            public DateTime DateCreated { get; set; }

            [Caption("Title"), Description("The comment title")]
            [UIHint("Text80"), StringLength(BlogComment.MaxTitle), Required, Trim]
            public string? Title { get; set; }

            [Caption("Comment"), Description("The comment")]
            [UIHint("TextArea"), AdditionalMetadata("EmHeight", 10), StringLength(BlogComment.MaxComment), Required]
            [AdditionalMetadata("TextAreaSave", false), AdditionalMetadata("RestrictedHtml", true)]
            public string? Comment { get; set; }

            internal BlogComment GetData(BlogComment data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }

            internal Task SetData(BlogComment data) {
                ObjectSupport.CopyData(data, this);
                return UpdateDataAsync();
            }
            internal async Task UpdateDataAsync() {
                BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
                GravatarsEnabled = config.ShowGravatar;
            }
            public EditModel() { }
        }

        [AllowGet]
        [ResourceAuthorize(Info.Resource_AllowManageComments)]
        public async Task<ActionResult> CommentEdit(int blogEntry, int comment) {
            using (BlogCommentDataProvider dataProvider = new BlogCommentDataProvider(blogEntry)) {
                EditModel model = new EditModel { };
                BlogComment? data = await dataProvider.GetItemAsync(comment);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Comment entry with id {0} not found."), comment);
                await model.SetData(data);
                Module.Title = data.Title;
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowManageComments)]
        [ExcludeDemoMode]
        public async Task<ActionResult> CommentEdit_Partial(EditModel model) {
            await model.UpdateDataAsync();
            using (BlogCommentDataProvider dataProvider = new BlogCommentDataProvider(model.EntryIdentity)) {
                BlogComment? data = await dataProvider.GetItemAsync(model.Identity);
                if (data == null)
                    throw new Error(this.__ResStr("alreadyDeleted", "The comment entry with id {0} has been removed and can no longer be updated", model.Identity));
                if (!ModelState.IsValid)
                    return PartialView(model);

                // save updated item
                data = model.GetData(data); // merge new data into original
                await model.SetData(data); // and all the data back into model for final display

                switch (await dataProvider.UpdateItemAsync(data)) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("alreadyDeleted", "The comment entry with id {0} has been removed and can no longer be updated", model.Identity));
                    case UpdateStatusEnum.NewKeyExists:
                        throw new Error(this.__ResStr("alreadyExists", "A comment with id {0} already exists.", model.Identity));
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Comment saved"));
            }
        }
    }
}