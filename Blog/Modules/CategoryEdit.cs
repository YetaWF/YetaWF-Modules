/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {

    public class CategoryEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, CategoryEditModule>, IInstallableModel { }

    [ModuleGuid("{9c689112-e55b-4a2e-8570-8e116b2fb75f}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Categories")]
    public class CategoryEditModule : ModuleDefinition {

        public CategoryEditModule() {
            Title = this.__ResStr("modTitle", "Blog Category");
            Name = this.__ResStr("modName", "Edit Blog Category");
            Description = this.__ResStr("modSummary", "Edits an existing blog category. Used by the Blog Categories Module.");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CategoryEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction? GetAction_Edit(string? url, int blogCategory) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { BlogCategory = blogCategory },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit blog category"),
                Legend = this.__ResStr("editLegend", "Edits an existing blog category"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}