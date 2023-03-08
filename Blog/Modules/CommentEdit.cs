/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints.Support;
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

namespace YetaWF.Modules.Blog.Modules;

public class CommentEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, CommentEditModule>, IInstallableModel { }

[ModuleGuid("{c4f62548-6c3f-4be2-a7c7-88a0f683c889}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Comments")]
public class CommentEditModule : ModuleDefinition2 {

    public CommentEditModule() {
        Title = this.__ResStr("modTitle", "Comment");
        Name = this.__ResStr("modName", "Edit Comment");
        Description = this.__ResStr("modSummary", "Edits an existing comment entry. This is used by the Entries Browse Module and is not site visitor accessible. It is typically used by a site administrator to edit user comments.");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new CommentEditModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public async Task<ModuleAction?> GetAction_EditAsync(string? url, int blogEntry, int comment) {
        if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowManageComments)) return null;
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { BlogEntry = blogEntry, Comment = comment },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Edit"),
            MenuText = this.__ResStr("editText", "Edit"),
            Tooltip = this.__ResStr("editTooltip", "Edit comment"),
            Legend = this.__ResStr("editLegend", "Edits an existing comment"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

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

    [ResourceAuthorize(Info.Resource_AllowManageComments)]
    public async Task<ActionInfo> RenderModuleAsync(int blogEntry, int comment) {
        int entry = blogEntry;
        using (BlogCommentDataProvider dataProvider = new BlogCommentDataProvider(entry)) {
            EditModel model = new EditModel { };
            BlogComment? data = await dataProvider.GetItemAsync(comment);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Comment entry with id {0} not found."), comment);
            await model.SetData(data);
            Title = data.Title;
            return await RenderAsync(model);
        }
    }

    [ResourceAuthorize(Info.Resource_AllowManageComments)]
    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        await model.UpdateDataAsync();
        using (BlogCommentDataProvider dataProvider = new BlogCommentDataProvider(model.EntryIdentity)) {
            BlogComment? data = await dataProvider.GetItemAsync(model.Identity);
            if (data == null)
                throw new Error(this.__ResStr("alreadyDeleted", "The comment entry with id {0} has been removed and can no longer be updated", model.Identity));
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);

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
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Comment saved"));
        }
    }
}
