/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {

    public class CategoryDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, CategoryDisplayModule>, IInstallableModel { }

    [ModuleGuid("{ead14c93-8fe1-4bed-9656-74c08e277723}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CategoryDisplayModule : ModuleDefinition {

        public CategoryDisplayModule() {
            Title = this.__ResStr("modTitle", "Blog Category");
            Name = this.__ResStr("modName", "Display Blog Category");
            Description = this.__ResStr("modSummary", "Displays an existing blog category");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CategoryDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, int blogCategory) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { BlogCategory = blogCategory },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the blog category"),
                Legend = this.__ResStr("displayLegend", "Displays an existing blog category"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}