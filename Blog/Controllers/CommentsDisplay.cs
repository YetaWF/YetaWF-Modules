/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Addons;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Controllers {

    public class CommentsDisplayModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CommentsDisplayModule> {

        public CommentsDisplayModuleController() { }

        public class CommentData {

            public int Identity { get; set; }
            public int CategoryIdentity { get; set; }
            public int EntryIdentity { get; set; }

            public string Name { get; set; }

            public bool ShowGravatar { get; set; }
            [UIHint("YetaWF_Blog_Gravatar")]
            public string Email { get; set; }
            public string Website { get; set; }

            public string Title { get; set; }
            public string Comment { get; set; }

            public bool Approved { get; set; }
            public bool Deleted { get; set; }
            public DateTime DateCreated { get; set; }

            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.IconsOnly)]
            public List<ModuleAction> Actions { get; set; }

            public CommentData(BlogComment data, CommentsDisplayModule dispMod, CommentEditModule editMod) {
                ObjectSupport.CopyData(data, this);
                Actions = new List<ModuleAction>();
                Actions.New(editMod.GetAction_Edit(dispMod.EditUrl, EntryIdentity, Identity));
                if (!data.Deleted && !data.Approved)
                    Actions.New(dispMod.GetAction_Approve(CategoryIdentity, Identity));
                Actions.New(dispMod.GetAction_Remove(CategoryIdentity, Identity));
            }
        }

        public class DisplayModel {
            public List<CommentData> Comments { get; set; }
            public int PendingApproval { get; set; }
            public bool CanApprove { get; set; }
            public bool CanRemove { get; set; }
            public bool ShowGravatars { get; set; }
            public bool OpenForComments { get; set; }
        }

        [HttpGet]
        public ActionResult CommentsDisplay(int blogEntry) {

            BlogConfigData config = BlogConfigDataProvider.GetConfig();

            DisplayModel model = new DisplayModel() { };

            CommentEditModule editMod = new CommentEditModule();
            using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                BlogEntry entry = entryDP.GetItem(blogEntry);
                if (entry == null)
                    return new EmptyResult();
                model.OpenForComments = entry.OpenForComments;
            }
            using (BlogCommentDataProvider commentDP = new BlogCommentDataProvider(blogEntry)) {
                int total;
                List<BlogComment> comments = commentDP.GetItems(0, 0, null, null, out total);

                if (!model.OpenForComments && total == 0)
                    return new EmptyResult();

                model.ShowGravatars = config.ShowGravatar;
                model.CanApprove = Resource.ResourceAccess.IsResourceAuthorized(Info.Resource_AllowManageComments);
                model.CanRemove = Resource.ResourceAccess.IsResourceAuthorized(Info.Resource_AllowManageComments);

                if (model.CanApprove || model.CanRemove)
                    model.Comments = (from d in comments select new CommentData(d, Module, editMod)).ToList();
                else
                    model.Comments = (from d in comments where !d.Deleted && d.Approved select new CommentData(d, Module, editMod)).ToList();

                int pending = (from d in comments where !d.Deleted && !d.Approved select d).Count();
                model.PendingApproval = pending;
                int commentsTotal = (from c in comments where !c.Deleted select c).Count();

                // set a module title
                string title;
                if (commentsTotal > 0 && pending > 0) {
                    if (commentsTotal > 1) {
                        if (pending > 1)
                            title = this.__ResStr("commentsPs", "{0} Comments - {1} Comments Pending Approval", commentsTotal, pending);
                        else
                            title = this.__ResStr("commentsP", "{0} Comments - 1 Comment Pending Approval", commentsTotal);
                    } else
                        title = this.__ResStr("commentP", "1 Comment Pending Approval");
                } else {
                    if (commentsTotal > 1)
                        title = this.__ResStr("comments", "{0} Comments", commentsTotal);
                    else if (commentsTotal == 1)
                        title = this.__ResStr("comment1", "1 Comment");
                    else
                        title = this.__ResStr("comment0", "No Comments");
                }
                Module.Title = title;

                return View(model);
            }
        }
        [HttpPost]
        [ResourceAuthorize(Info.Resource_AllowManageComments)]
        [ExcludeDemoMode]
        public ActionResult Approve(int blogEntry, int comment) {
            using (BlogCommentDataProvider dataProvider = new BlogCommentDataProvider(blogEntry)) {
                BlogComment cmt = dataProvider.GetItem(comment);
                if (cmt == null)
                    throw new InternalError("Can't find comment entry {0}", comment);
                cmt.Approved = true;
                UpdateStatusEnum status = dataProvider.UpdateItem(cmt);
                if (status != UpdateStatusEnum.OK)
                    throw new InternalError("Can't update comment entry - {0}", status);
                return Reload(null, Reload: ReloadEnum.Page);
            }
        }
        [HttpPost]
        [ResourceAuthorize(Info.Resource_AllowManageComments)]
        [ExcludeDemoMode]
        public ActionResult Remove(int blogEntry, int comment) {
            using (BlogCommentDataProvider dataProvider = new BlogCommentDataProvider(blogEntry)) {
                BlogComment cmt = dataProvider.GetItem(comment);
                if (cmt == null)
                    throw new InternalError("Can't find comment entry {0}", comment);
                if (!dataProvider.RemoveItem(comment))
                    throw new InternalError("Can't remove comment entry");
                return Reload(null, Reload: ReloadEnum.Page);
            }
        }
    }
}