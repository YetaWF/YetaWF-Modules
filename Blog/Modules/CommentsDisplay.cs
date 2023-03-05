/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.Addons;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Endpoints;

namespace YetaWF.Modules.Blog.Modules;

public class CommentsDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, CommentsDisplayModule>, IInstallableModel { }

[ModuleGuid("{2539dab0-c210-4578-a615-3e732b65bcec}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Comments")]
public class CommentsDisplayModule : ModuleDefinition2 {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Uhm yeah?")]
    public CommentsDisplayModule() {
        Title = this.__ResStr("modTitle", "Comment Entries");
        Name = this.__ResStr("modName", "Comment Entries");
        Description = this.__ResStr("modSummary", "Displays all comments for a blog entry. This module is typically used on the same page as the Entry Display Module. Authorized users can approve and remove comments.");
        AnchorId = "Comments";
    }

    public override IModuleDefinitionIO GetDataProvider() { return new CommentsDisplayModuleDataProvider(); }

    [Category("General"), Caption("Edit Url"), Description("The Url to edit a blog comment - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? EditUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public async Task<ModuleAction?> GetAction_ApproveAsync(int blogEntry, int comment) {
        if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowManageComments)) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(CommentsDisplayModuleEndpoints), nameof(CommentsDisplayModuleEndpoints.Approve)),
            QueryArgs = new { BlogEntry = blogEntry, Comment = comment },
            Image = await CustomIconAsync("CommentEntryApprove.png"),
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("approveLink", "Approve"),
            MenuText = this.__ResStr("approveMenu", "Approve"),
            Tooltip = this.__ResStr("approveTT", "Approve the comment"),
            Legend = this.__ResStr("approveLegend", "Approves a comment"),
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("approveConfirm", "Are you sure you want to approve this comment?"),
        };
    }
    public async Task<ModuleAction?> GetAction_RemoveAsync(int blogEntry, int comment) {
        if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowManageComments)) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(CommentsDisplayModuleEndpoints), nameof(CommentsDisplayModuleEndpoints.Remove)),
            QueryArgs = new { BlogEntry = blogEntry, Comment = comment },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove"),
            MenuText = this.__ResStr("removeMenu", "Remove"),
            Tooltip = this.__ResStr("removeTT", "Remove the comment"),
            Legend = this.__ResStr("removeLegend", "Removes a comment"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this comment?"),
        };
    }

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

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!int.TryParse(Manager.RequestQueryString["BlogEntry"], out int entryNum)) entryNum = 0;
        BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();

        DisplayModel model = new DisplayModel() { };

        CommentEditModule editMod = new CommentEditModule();
        using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
            BlogEntry? entry = null;
            if (entryNum != 0)
                entry = await entryDP.GetItemAsync(entryNum);
            if (entry == null)
                return ActionInfo.Empty;
            model.OpenForComments = entry.OpenForComments;
        }
        using (BlogCommentDataProvider commentDP = new BlogCommentDataProvider(entryNum)) {
            DataProviderGetRecords<BlogComment> comments = await commentDP.GetItemsAsync(0, 0, null, null);

            if (!model.OpenForComments && comments.Total == 0)
                return ActionInfo.Empty;

            model.ShowGravatars = config.ShowGravatar;
            model.CanApprove = await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowManageComments);
            model.CanRemove = await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowManageComments);

            List<CommentData> list = new List<CommentData>();
            foreach (BlogComment d in comments.Data) {
                ModuleAction? editAction = await editMod.GetAction_EditAsync(EditUrl, d.EntryIdentity, d.Identity);
                ModuleAction? approveAction = await GetAction_ApproveAsync(d.CategoryIdentity, d.Identity);
                ModuleAction? removeAction = await GetAction_RemoveAsync(d.CategoryIdentity, d.Identity);
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
            Title = title;
            return await RenderAsync(model);
        }
    }
}
