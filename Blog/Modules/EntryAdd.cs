/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {
    public class EntryAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, EntryAddModule>, IInstallableModel { }

    [ModuleGuid("{a08cc005-5da2-4cc6-91f9-b195879d4dab}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class EntryAddModule : ModuleDefinition {

        public EntryAddModule() {
            Title = this.__ResStr("modTitle", "Add New Blog Entry");
            Name = this.__ResStr("modName", "Add Blog Entry");
            Description = this.__ResStr("modSummary", "Creates a new blog entry");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EntryAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add(string url) {
            // if the url has a category, use it as default category to add a new entry
            int blogCategory;
            Manager.TryGetUrlArg<int>("BlogCategory", out blogCategory);
            object qs = null;
            if (blogCategory != 0)
                qs = new { BlogCategory = blogCategory };
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = qs,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Add a new blog entry"),
                Legend = this.__ResStr("addLegend", "Adds a new blog entry"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

