/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace YetaWF.Modules.Blog.Controllers {

    public class CommentsDisplayModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.CommentsDisplayModule> {

        public CommentsDisplayModuleController() { }

        public class CommentData {

            public int Identity { get; set; }
            public int CategoryIdentity { get; set; }
            public int EntryIdentity { get; set; }

            public string? Name { get; set; }

            public bool ShowGravatar { get; set; }
            [UIHint("YetaWF_Blog_Gravatar")]
            public string? Email { get; set; }
            public string? Website { get; set; }

            public string? Title { get; set; }
            public string? Comment { get; set; }

            public bool Approved { get; set; }
            public bool Deleted { get; set; }
            public DateTime DateCreated { get; set; }

            [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.IconsOnly)]
            public List<ModuleAction> Actions { get; set; }

            public CommentData(BlogComment data, ModuleAction? editAction, ModuleAction? approveAction, ModuleAction? removeAction) {
                ObjectSupport.CopyData(data, this);
                Actions = new List<ModuleAction>();
                Actions.New(editAction);
                if (!data.Deleted && !data.Approved)
                    Actions.New(approveAction);
                Actions.New(removeAction);
            }
        }

        public class DisplayModel {
            public List<CommentData> Comments { get; set; } = null!;
            public int PendingApproval { get; set; }
            public bool CanApprove { get; set; }
            public bool CanRemove { get; set; }
            public bool ShowGravatars { get; set; }
            public bool OpenForComments { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> CommentsDisplay(int? blogEntry) {

            int entryNum = blogEntry ?? 0;
            BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();

            DisplayModel model = new DisplayModel() { };

            CommentEditModule editMod = new CommentEditModule();
            using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                BlogEntry? entry = null;
                if (entryNum != 0)
                    entry = await entryDP.GetItemAsync(entryNum);
                if (entry == null)
                    return new EmptyResult();
                model.OpenForComments = entry.OpenForComments;
            }
            using (BlogCommentDataProvider commentDP = new BlogCommentDataProvider(entryNum)) {
                DataProviderGetRecords<BlogComment> comments = await commentDP.GetItemsAsync(0, 0, null, null);

                if (!model.OpenForComments && comments.Total == 0)
                    return new EmptyResult();

                model.ShowGravatars = config.ShowGravatar;
                model.CanApprove = await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowManageComments);
                model.CanRemove = await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowManageComments);

                List<CommentData> list = new List<CommentData>();
                foreach (BlogComment d in comments.Data) {
                    ModuleAction? editAction = await editMod.GetAction_EditAsync(Module.EditUrl, d.EntryIdentity, d.Identity);
                    ModuleAction? approveAction = await Module.GetAction_ApproveAsync(d.CategoryIdentity, d.Identity);
                    ModuleAction? removeAction = await Module.GetAction_RemoveAsync(d.CategoryIdentity, d.Identity);
                    if (model.CanApprove || model.CanRemove)
                        list.Add(new CommentData(d, editAction, approveAction, removeAction));
                    else {
                        if (!d.Deleted && d.Approved)
                            list.Add(new CommentData(d, editAction, approveAction, removeAction));
                    }

                }
                model.Comments = list;

                int pending = (from d in comments.Data where !d.Deleted && !d.Approved select d).Count();
                model.PendingApproval = pending;
                int commentsTotal = (from c in comments.Data where !c.Deleted select c).Count();

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
        [AllowPost]
        [ResourceAuthorize(Info.Resource_AllowManageComments)]
        [ExcludeDemoMode]
        public async Task<ActionResult> Approve(int blogEntry, int comment) {
            using (BlogCommentDataProvider dataProvider = new BlogCommentDataProvider(blogEntry)) {
                BlogComment? cmt = await dataProvider.GetItemAsync(comment);
                if (cmt == null)
                    throw new InternalError("Can't find comment entry {0}", comment);
                cmt.Approved = true;
                UpdateStatusEnum status = await dataProvider.UpdateItemAsync(cmt);
                if (status != UpdateStatusEnum.OK)
                    throw new InternalError("Can't update comment entry - {0}", status);
                return Reload(null, Reload: ReloadEnum.Page);
            }
        }
        [AllowPost]
        [ResourceAuthorize(Info.Resource_AllowManageComments)]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(int blogEntry, int comment) {
            using (BlogCommentDataProvider dataProvider = new BlogCommentDataProvider(blogEntry)) {
                BlogComment? cmt = await dataProvider.GetItemAsync(comment);
                if (cmt == null)
                    throw new InternalError("Can't find comment entry {0}", comment);
                if (!await dataProvider.RemoveItemAsync(comment))
                    throw new InternalError("Can't remove comment entry");
                return Reload(null, Reload: ReloadEnum.Page);
            }
        }
    }
}