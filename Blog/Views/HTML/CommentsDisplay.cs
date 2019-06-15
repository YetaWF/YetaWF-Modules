/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Controllers;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Views {

    public class CommentsDisplayView : YetaWFView, IYetaWFView<CommentsDisplayModule, CommentsDisplayModuleController.DisplayModel> {

        public const string ViewName = "CommentsDisplay";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(CommentsDisplayModule module, CommentsDisplayModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync()}");

            if (model.Comments.Count == 0) {
                hb.Append($@"
    <div class='t_nocomments'>
        {Utility.HtmlEncode(this.__ResStr("nocommentsAdd", "Be the first to add a comment!"))}
    </div>");
            }

            hb.Append($@"
    <div class='t_comments'>");

            int count = 0;
            foreach (YetaWF.Modules.Blog.Controllers.CommentsDisplayModuleController.CommentData comment in model.Comments) {
                string commentClass = "t_comment" + count.ToString();
                if (model.CanApprove || model.CanRemove) {
                    if (comment.Deleted) {
                        commentClass += " t_deleted";
                    } else {
                        commentClass += " t_notdeleted";
                        if (comment.Approved) {
                            commentClass += " t_approved";
                        } else {
                            commentClass += " t_notapproved";
                        }
                    }
                }

                hb.Append($@"
        <div class='{commentClass}'>");

                if (model.CanApprove || model.CanRemove) {
                    if (comment.Deleted) {
                        hb.Append($@"
            <div class='t_boxdeleted'>
                {Utility.HtmlEncode(this.__ResStr("deleted", "This comment has been deleted"))}
            </div>");
                    } else if (!comment.Approved) {
                        hb.Append($@"
            <div class='t_boxnotapproved'>
                {Utility.HtmlEncode(this.__ResStr("notApproved", "This comment has not yet been approved"))}
            </div>");
                    }
                }

                if (model.ShowGravatars && comment.ShowGravatar) {
                    hb.Append($@"
            <div class='t_gravatar'>
                {await HtmlHelper.ForDisplayAsync(comment, nameof(comment.Email))}
            </div>");
                }

                hb.Append($@"
            <div class='t_title'>{Utility.HtmlEncode(comment.Title)}</div>
            <div class='t_comment'>{comment.Comment}</div>
            <div class='t_boxinfo'>
                <div class='t_cmtby'>{Utility.HtmlEncode(this.__ResStr("by", "By:"))}</div>
                <div class='t_cmtauthor'>");

                if (string.IsNullOrEmpty(comment.Website)) {
                    hb.Append(Utility.HtmlEncode(comment.Name));
                } else {
                    hb.Append($"<a href='{Utility.HtmlAttributeEncode(comment.Website)}' class='linkpreview-show' target='_blank' rel='nofollow noopener noreferrer'>{Utility.HtmlEncode(comment.Name)}</a>");
                }

                hb.Append($@"
                </div>
                <div class='t_cmton'>{Utility.HtmlEncode(this.__ResStr("on", "On:"))}</div>
                <div class='t_cmtdate'>{Utility.HtmlEncode(Formatting.FormatDate(comment.DateCreated))}</div>
            </div>
            <div class='t_boxactions'>
                {await HtmlHelper.ForDisplayAsync(comment, nameof(comment.Actions))}
            </div >
            <div class='t_cmtend'></div>
        </div>");
            }

            hb.Append($@"
    </div>
{await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, Text=this.__ResStr("btnCancel", "Return") },
    })}
{await RenderEndFormAsync()}");

            return hb.ToString();
        }
    }
}
