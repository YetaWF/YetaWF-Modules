/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {
    public class CategoryAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, CategoryAddModule>, IInstallableModel { }

    [ModuleGuid("{beeabd31-6607-461a-aa0c-717645f1be83}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Categories")]
    public class CategoryAddModule : ModuleDefinition {

        public CategoryAddModule() {
            Title = this.__ResStr("modTitle", "Add New Blog Category");
            Name = this.__ResStr("modName", "Add Blog Category");
            Description = this.__ResStr("modSummary", "Creates a new blog category. Used by the Blog Categories Module.");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CategoryAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction? GetAction_Add(string? url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add"),
                MenuText = this.__ResStr("addText", "Add"),
                Tooltip = this.__ResStr("addTooltip", "Create a new blog category"),
                Legend = this.__ResStr("addLegend", "Creates a new blog category"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

