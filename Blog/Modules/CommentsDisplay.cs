/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.Addons;
using YetaWF.Modules.Blog.Controllers;
using System.Threading.Tasks;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Modules {

    public class CommentsDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, CommentsDisplayModule>, IInstallableModel { }

    [ModuleGuid("{2539dab0-c210-4578-a615-3e732b65bcec}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CommentsDisplayModule : ModuleDefinition {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Uhm yeah?")]
        public CommentsDisplayModule() {
            Title = this.__ResStr("modTitle", "Comment Entries");
            Name = this.__ResStr("modName", "Comment Entries");
            Description = this.__ResStr("modSummary", "Displays all comment entries for a blog entry");
            AnchorId = "Comments";
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CommentsDisplayModuleDataProvider(); }

        [Category("General"), Caption("Edit URL"), Description("The URL to edit a blog comment - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction> GetAction_ApproveAsync(int blogEntry, int comment) {
            if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowManageComments)) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(CommentsDisplayModuleController), nameof(CommentsDisplayModuleController.Approve)),
                NeedsModuleContext = true,
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
        public async Task<ModuleAction> GetAction_RemoveAsync(int blogEntry, int comment) {
            if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowManageComments)) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(CommentsDisplayModuleController), "Remove"),
                NeedsModuleContext = true,
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
    }
}