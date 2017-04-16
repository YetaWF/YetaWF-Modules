/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

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

    public class DisqusConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisqusConfigModule>, IInstallableModel { }

    [ModuleGuid("{71583859-baa9-43fa-895b-a6ebd47561a1}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class DisqusConfigModule : ModuleDefinition {

        public DisqusConfigModule() {
            Title = this.__ResStr("modTitle", "Disqus Settings");
            Name = this.__ResStr("modName", "Disqus Settings");
            Description = this.__ResStr("modSummary", "Edits a site's Disqus settings");
            ShowHelp = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new DisqusConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new DisqusConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Disqus Settings"),
                MenuText = this.__ResStr("editText", "Disqus Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the Disqus settings"),
                Legend = this.__ResStr("editLegend", "Edits the Disqus settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}