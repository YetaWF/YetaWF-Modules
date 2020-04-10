/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.Addons;

namespace YetaWF.Modules.Blog.Modules {

    public class CommentEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, CommentEditModule>, IInstallableModel { }

    [ModuleGuid("{c4f62548-6c3f-4be2-a7c7-88a0f683c889}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Comments")]
    public class CommentEditModule : ModuleDefinition {

        public CommentEditModule() {
            Title = this.__ResStr("modTitle", "Comment");
            Name = this.__ResStr("modName", "Edit Comment");
            Description = this.__ResStr("modSummary", "Edits an existing comment entry. This is used by the Entries Browse Module and is not site visitor accessible. It is typically used by a site administrator to edit user comments.");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CommentEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction> GetAction_EditAsync(string url, int blogEntry, int comment) {
            if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(Info.Resource_AllowManageComments)) return null;
            return new ModuleAction(this) {
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
    }
}