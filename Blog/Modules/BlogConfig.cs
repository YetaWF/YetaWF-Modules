/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Modules {

    public class BlogConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, BlogConfigModule>, IInstallableModel { }

    [ModuleGuid("{8e6986c9-3d25-4479-bce6-a54062efdb15}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class BlogConfigModule : ModuleDefinition {

        public BlogConfigModule() {
            Title = this.__ResStr("modTitle", "Blog Settings");
            Name = this.__ResStr("modName", "Blog Settings");
            Description = this.__ResStr("modSummary", "Edits a site's blog settings");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BlogConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new BlogConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Blog Settings"),
                MenuText = this.__ResStr("editText", "Blog Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the blog settings"),
                Legend = this.__ResStr("editLegend", "Edits the blog settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}