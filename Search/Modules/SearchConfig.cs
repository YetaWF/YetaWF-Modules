/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Search.DataProvider;

namespace YetaWF.Modules.Search.Modules {

    public class SearchConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchConfigModule>, IInstallableModel { }

    [ModuleGuid("{f27ed8f6-e844-4668-a9fe-1dda07bd7277}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SearchConfigModule : ModuleDefinition {

        public SearchConfigModule() {
            Title = this.__ResStr("modTitle", "Search Settings");
            Name = this.__ResStr("modName", "Search Settings");
            Description = this.__ResStr("modSummary", "Edits a site's search settings");
            SameAsPage = false;
            ShowHelp = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SearchConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new SearchConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Search Settings"),
                MenuText = this.__ResStr("editText", "Search Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the site's search settings"),
                Legend = this.__ResStr("editLegend", "Edits the search settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}